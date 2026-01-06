using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Data;

public class CourseDbContext : IdentityDbContext<IdentityUser>, IUnitOfWork
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        return new EfDbTransaction(transaction);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseDbContext).Assembly);

        // Global query filter for soft delete
        modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);
    }

    private class EfDbTransaction : IDbTransaction
    {
        private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction _transaction;

        public EfDbTransaction(Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default) => _transaction.CommitAsync(cancellationToken);
        public Task RollbackAsync(CancellationToken cancellationToken = default) => _transaction.RollbackAsync(cancellationToken);

        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
