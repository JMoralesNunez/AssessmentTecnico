using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Data.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly CourseDbContext _context;

    public LessonRepository(CourseDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Lesson?> GetByIdAsync(Guid id)
    {
        return await _context.Lessons.FindAsync(id);
    }

    public void Add(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
    }

    public void Update(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
    }

    public void Delete(Lesson lesson)
    {
        // Soft delete
        lesson.IsDeleted = true;
        lesson.UpdatedAt = DateTime.UtcNow;
        Update(lesson);
    }
}
