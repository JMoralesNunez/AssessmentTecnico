using CoursePlatform.Application.DTOs.Lesson;

namespace CoursePlatform.Application.Interfaces;

public interface ILessonService
{
    Task<LessonDto> CreateLessonAsync(Guid courseId, CreateLessonRequest request);
    Task<LessonDto> UpdateLessonAsync(Guid lessonId, UpdateLessonRequest request);
    Task DeleteLessonAsync(Guid lessonId);
    Task ReorderLessonsAsync(Guid courseId, ReorderLessonsRequest request);
    Task<IEnumerable<LessonDto>> GetLessonsByCourseIdAsync(Guid courseId);
}
