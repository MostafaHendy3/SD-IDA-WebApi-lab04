using Lab04.Models;

namespace Lab04.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UniDbContext _context;

        public UnitOfWork(UniDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
