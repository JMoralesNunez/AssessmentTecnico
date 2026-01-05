using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Interfaces;

public interface ICourseRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task<Course?> GetByIdAsync(Guid id);
    Task<Course?> GetByIdWithLessonsAsync(Guid id);
    Task<(IEnumerable<Course> Items, int TotalCount)> SearchAsync(string? searchTerm, CourseStatus? status, int page, int pageSize);
    
    void Add(Course course);
    void Update(Course course);
    
    void Delete(Course course);
}
