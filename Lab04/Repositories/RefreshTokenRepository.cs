using Lab04.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab04.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly UniDbContext _context;

        public RefreshTokenRepository(UniDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            RefreshToken entity,
            CancellationToken cancellationToken = default
        )
        {
            await _context.RefreshTokens.AddAsync(entity, cancellationToken);
        }

        public Task<RefreshToken?> FindActiveByHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default
        )
        {
            var now = DateTime.UtcNow;
            return _context.RefreshTokens.FirstOrDefaultAsync(
                t =>
                    t.TokenHash == tokenHash
                    && t.RevokedAt == null
                    && t.ExpiresAt > now,
                cancellationToken
            );
        }
    }
}
