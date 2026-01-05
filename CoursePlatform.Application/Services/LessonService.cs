using CoursePlatform.Application.DTOs.Lesson;
using CoursePlatform.Application.Interfaces;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Interfaces;

namespace CoursePlatform.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseRepository _courseRepository;

    public LessonService(ILessonRepository lessonRepository, ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _courseRepository = courseRepository;
    }

    public async Task<LessonDto> CreateLessonAsync(Guid courseId, CreateLessonRequest request)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(courseId);
        if (course == null) throw new Exception("Course not found");

        // Business Rule: Order must be unique within the same course
        if (course.Lessons.Any(l => l.Order == request.Order && !l.IsDeleted))
        {
            throw new Exception($"A lesson with order {request.Order} already exists in this course.");
        }

        var lesson = new Lesson
        {
            CourseId = courseId,
            Title = request.Title,
            Order = request.Order
        };

        _lessonRepository.Add(lesson);
        await _lessonRepository.UnitOfWork.SaveChangesAsync();

        return MapToDto(lesson);
    }

    public async Task<LessonDto> UpdateLessonAsync(Guid lessonId, UpdateLessonRequest request)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null) throw new Exception("Lesson not found");

        // Business Rule: Order uniqueness if order is changing
        if (lesson.Order != request.Order)
        {
            var course = await _courseRepository.GetByIdWithLessonsAsync(lesson.CourseId);
            if (course != null && course.Lessons.Any(l => l.Order == request.Order && l.Id != lessonId && !l.IsDeleted))
            {
                throw new Exception($"A lesson with order {request.Order} already exists in this course.");
            }
        }

        lesson.Title = request.Title;
        lesson.Order = request.Order;
        lesson.UpdatedAt = DateTime.UtcNow;

        _lessonRepository.Update(lesson);
        await _lessonRepository.UnitOfWork.SaveChangesAsync();

        return MapToDto(lesson);
    }

    public async Task DeleteLessonAsync(Guid lessonId)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null) throw new Exception("Lesson not found");

        _lessonRepository.Delete(lesson); // This does soft delete
        await _lessonRepository.UnitOfWork.SaveChangesAsync();
    }

    public async Task ReorderLessonsAsync(Guid courseId, ReorderLessonsRequest request)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(courseId);
        if (course == null) throw new Exception("Course not found");

        // Business Rule: Reordering must not generate duplicate orders
        var requestedOrders = request.Orders.Select(o => o.NewOrder).ToList();
        if (requestedOrders.Distinct().Count() != requestedOrders.Count())
        {
            throw new Exception("Duplicate orders found in reorder request.");
        }

        foreach (var orderUpdate in request.Orders)
        {
            var lesson = course.Lessons.FirstOrDefault(l => l.Id == orderUpdate.LessonId);
            if (lesson != null)
            {
                lesson.Order = orderUpdate.NewOrder;
                lesson.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _courseRepository.UnitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdWithLessonsAsync(courseId);
        if (course == null) return Enumerable.Empty<LessonDto>();

        return course.Lessons.OrderBy(l => l.Order).Select(MapToDto);
    }

    private static LessonDto MapToDto(Lesson lesson)
    {
        return new LessonDto(
            lesson.Id,
            lesson.CourseId,
            lesson.Title,
            lesson.Order,
            lesson.UpdatedAt
        );
    }
}
