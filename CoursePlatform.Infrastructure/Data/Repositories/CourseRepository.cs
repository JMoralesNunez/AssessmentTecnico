using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Data.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CourseDbContext _context;

    public CourseRepository(CourseDbContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task<Course?> GetByIdWithLessonsAsync(Guid id)
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(IEnumerable<Course> Items, int TotalCount)> SearchAsync(
        string? searchTerm, 
        CourseStatus? status, 
        int page, 
        int pageSize)
    {
        var query = _context.Courses
            .Include(c => c.Lessons)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Title.Contains(searchTerm));
        }

        // Apply status filter
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .OrderByDescending(c => c.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public void Add(Course course)
    {
        _context.Courses.Add(course);
    }

    public void Update(Course course)
    {
        _context.Courses.Update(course);
    }

    public void Delete(Course course)
    {
        // Soft delete
        course.IsDeleted = true;
        course.UpdatedAt = DateTime.UtcNow;
        Update(course);
    }
}
