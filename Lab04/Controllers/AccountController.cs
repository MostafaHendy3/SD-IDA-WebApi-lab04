using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

using Lab04.Models;

using Lab04.Models.DTOs;

using Lab04.Repositories;

using Lab04.Security;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;

using Microsoft.IdentityModel.Tokens;



namespace Lab04.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    public class AccountController : ControllerBase

    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IConfiguration _configuration;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IRefreshTokenRepository _refreshTokens;



        public AccountController(

            UserManager<ApplicationUser> userManager,

            IConfiguration configuration,

            IUnitOfWork unitOfWork,

            IRefreshTokenRepository refreshTokens

        )

        {

            _userManager = userManager;

            _configuration = configuration;

            _unitOfWork = unitOfWork;

            _refreshTokens = refreshTokens;

        }



        [HttpPost("login")]

        public async Task<IActionResult> Login(

            [FromBody] LoginDTO request,

            CancellationToken cancellationToken

        )

        {

            if (!ModelState.IsValid)

            {

                return BadRequest(ModelState);

            }



            var user = await _userManager.FindByNameAsync(request.Username);

            if (user is null)

            {

                return Unauthorized("Invalid username or password.");

            }



            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)

            {

                return Unauthorized("Invalid username or password.");

            }



            var roles = await _userManager.GetRolesAsync(user);

            var (accessToken, accessExpiresAt) = CreateAccessToken(user, roles);

            var (rawRefresh, refreshExpiresAt) = await PersistNewRefreshTokenAsync(

                user,

                cancellationToken

            );



            return Ok(

                new

                {

                    message = "Login successful.",

                    accessToken,

                    token = accessToken,

                    refreshToken = rawRefresh,

                    expiresAt = accessExpiresAt,

                    refreshExpiresAt,

                    roles,

                }

            );

        }



        [HttpPost("refresh")]

        [AllowAnonymous]

        public async Task<IActionResult> Refresh(

            [FromBody] RefreshTokenRequestDto request,

            CancellationToken cancellationToken

        )

        {

            if (!ModelState.IsValid)

            {

                return BadRequest(ModelState);

            }



            var hash = RefreshTokenHasher.Hash(request.RefreshToken);

            var stored = await _refreshTokens.FindActiveByHashAsync(hash, cancellationToken);

            if (stored is null)

            {

                return Unauthorized("Invalid or expired refresh token.");

            }



            var user = await _userManager.FindByIdAsync(stored.UserId);

            if (user is null)

            {

                return Unauthorized("Invalid or expired refresh token.");

            }



            var roles = await _userManager.GetRolesAsync(user);

            var (accessToken, accessExpiresAt) = CreateAccessToken(user, roles);



            var now = DateTime.UtcNow;

            stored.RevokedAt = now;

            var newRaw = RefreshTokenHasher.GenerateRawToken();

            var newHash = RefreshTokenHasher.Hash(newRaw);

            stored.ReplacedByHash = newHash;



            var refreshDays = double.Parse(

                _configuration["Jwt:RefreshTokenExpireDays"] ?? "14"

            );

            var newEntity = new RefreshToken

            {

                UserId = user.Id,

                TokenHash = newHash,

                ExpiresAt = now.AddDays(refreshDays),

                CreatedAt = now,

            };

            await _refreshTokens.AddAsync(newEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);



            return Ok(

                new

                {

                    accessToken,

                    token = accessToken,

                    refreshToken = newRaw,

                    expiresAt = accessExpiresAt,

                    refreshExpiresAt = newEntity.ExpiresAt,

                    roles,

                }

            );

        }



        [HttpPost("revoke")]

        [AllowAnonymous]

        public async Task<IActionResult> Revoke(

            [FromBody] RefreshTokenRequestDto request,

            CancellationToken cancellationToken

        )

        {

            if (!ModelState.IsValid)

            {

                return BadRequest(ModelState);

            }



            var hash = RefreshTokenHasher.Hash(request.RefreshToken);

            var stored = await _refreshTokens.FindActiveByHashAsync(hash, cancellationToken);

            if (stored is not null)

            {

                stored.RevokedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

            }



            return Ok(new { message = "Refresh token revoked." });

        }



        private async Task<(string rawRefresh, DateTime refreshExpiresAt)> PersistNewRefreshTokenAsync(

            ApplicationUser user,

            CancellationToken cancellationToken

        )

        {

            var raw = RefreshTokenHasher.GenerateRawToken();

            var hash = RefreshTokenHasher.Hash(raw);

            var refreshDays = double.Parse(

                _configuration["Jwt:RefreshTokenExpireDays"] ?? "14"

            );

            var now = DateTime.UtcNow;

            var entity = new RefreshToken

            {

                UserId = user.Id,

                TokenHash = hash,

                ExpiresAt = now.AddDays(refreshDays),

                CreatedAt = now,

            };

            await _refreshTokens.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return (raw, entity.ExpiresAt);

        }



        private (string accessToken, DateTime expiresAt) CreateAccessToken(

            ApplicationUser user,

            IList<string> roles

        )

        {

            var jwtSection = _configuration.GetSection("Jwt");

            var key = jwtSection["Key"]

                ?? throw new InvalidOperationException("Jwt:Key is not configured.");

            var issuer = jwtSection["Issuer"];

            var audience = jwtSection["Audience"];

            var expireMinutes = double.Parse(jwtSection["ExpireMinutes"] ?? "60");



            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expireMinutes);



            var claims = new List<Claim>

            {

                new Claim(JwtRegisteredClaimNames.Sub, user.Id),

                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),

                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            foreach (var role in roles)

            {

                claims.Add(new Claim(ClaimTypes.Role, role));

            }



            var token = new JwtSecurityToken(

                issuer: issuer,

                audience: audience,

                claims: claims,

                expires: expires,

                signingCredentials: credentials

            );



            var written = new JwtSecurityTokenHandler().WriteToken(token);

            return (written, expires);

        }



        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] RegisterDTO request)

        {

            if (!ModelState.IsValid)

            {

                return BadRequest(ModelState);

            }



            var user = new ApplicationUser

            {

                UserName = request.Username,

                Email = request.Email,

                Address = request.Address,

            };



            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)

            {

                return BadRequest(createResult.Errors);

            }



            await _userManager.AddToRoleAsync(user, AppRoles.Student);



            return Ok(new { message = "User registered successfully." });

        }

    }

}

