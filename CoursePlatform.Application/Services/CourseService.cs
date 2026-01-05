using CoursePlatform.Application.DTOs.Course;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Domain.Interfaces;

namespace CoursePlatform.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseSearchResponse> SearchCoursesAsync(CourseSearchRequest request)
    {
        var (items, totalCount) = await _courseRepository.SearchAsync(
            request.Query,
            request.Status,
            request.Page,
            request.PageSize
        );

        var courseDtos = items.Select(c => new CourseSummaryDto(
            c.Id,
            c.Title,
            c.Status,
            c.Lessons.Count,
            c.UpdatedAt
        )).ToList();

        return new CourseSearchResponse(
            courseDtos,
            totalCount,
            request.Page,
            request.PageSize
        );
    }

    public async Task<CourseDetailDto?> GetCourseSummaryAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(courseId);
        
        if (course == null)
        {
            return null;
        }

        return new CourseDetailDto(
            course.Id,
            course.Title,
            course.Status,
            course.Lessons.Count,
            course.CreatedAt,
            course.UpdatedAt
        );
    }

    public async Task<CourseSummaryDto> CreateCourseAsync(CreateCourseRequest request)
    {
        var course = new CoursePlatform.Domain.Entities.Course
        {
            Title = request.Title,
            Status = CourseStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _courseRepository.Add(course);
        await _courseRepository.UnitOfWork.SaveChangesAsync();

        return new CourseSummaryDto(
            course.Id,
            course.Title,
            course.Status,
            0,
            course.UpdatedAt
        );
    }

    public async Task<CourseSummaryDto> UpdateCourseAsync(Guid courseId, UpdateCourseRequest request)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(courseId);
        if (course == null)
        {
            throw new Exception("Course not found");
        }

        course.Title = request.Title;
        course.UpdatedAt = DateTime.UtcNow;

        _courseRepository.Update(course);
        await _courseRepository.UnitOfWork.SaveChangesAsync();

        return new CourseSummaryDto(
            course.Id,
            course.Title,
            course.Status,
            course.Lessons.Count,
            course.UpdatedAt
        );
    }

    public async Task DeleteCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new Exception("Course not found");
        }

        _courseRepository.Delete(course); // This performs soft delete
        await _courseRepository.UnitOfWork.SaveChangesAsync();
    }

    public async Task PublishCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(courseId);
        
        if (course == null)
        {
            throw new Exception("Course not found");
        }

        // Business Rule: A course can only be published if it has at least one active lesson
        var activeLessons = course.Lessons.Where(l => !l.IsDeleted).ToList();
        if (activeLessons.Count == 0)
        {
            throw new Exception("Cannot publish a course without active lessons");
        }

        course.Status = CourseStatus.Published;
        course.UpdatedAt = DateTime.UtcNow;
        
        _courseRepository.Update(course);
        await _courseRepository.UnitOfWork.SaveChangesAsync();
    }

    public async Task UnpublishCourseAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        
        if (course == null)
        {
            throw new Exception("Course not found");
        }

        course.Status = CourseStatus.Draft;
        course.UpdatedAt = DateTime.UtcNow;
        
        _courseRepository.Update(course);
        await _courseRepository.UnitOfWork.SaveChangesAsync();
    }
}
