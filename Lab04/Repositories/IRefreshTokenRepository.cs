using Lab04.Models;

namespace Lab04.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default);
        Task<RefreshToken?> FindActiveByHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default
        );
    }
}
