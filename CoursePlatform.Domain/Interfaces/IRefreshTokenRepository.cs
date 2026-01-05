using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    void Update(RefreshToken refreshToken);
    IUnitOfWork UnitOfWork { get; }
}
