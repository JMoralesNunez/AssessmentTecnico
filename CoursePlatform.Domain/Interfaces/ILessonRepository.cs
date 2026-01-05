using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Domain.Interfaces;

public interface ILessonRepository
{
    IUnitOfWork UnitOfWork { get; }
    
    Task<Lesson?> GetByIdAsync(Guid id);
    void Add(Lesson lesson);
    void Update(Lesson lesson);
    void Delete(Lesson lesson);
}
