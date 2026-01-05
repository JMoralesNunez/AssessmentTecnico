using CoursePlatform.Application.DTOs.Course;

namespace CoursePlatform.Application.Interfaces;

public interface ICourseService
{
    Task<CourseSearchResponse> SearchCoursesAsync(CourseSearchRequest request);
    Task<CourseDetailDto?> GetCourseSummaryAsync(Guid courseId);
    Task<CourseSummaryDto> CreateCourseAsync(CreateCourseRequest request);
    Task<CourseSummaryDto> UpdateCourseAsync(Guid courseId, UpdateCourseRequest request);
    Task DeleteCourseAsync(Guid courseId);
    Task PublishCourseAsync(Guid courseId);
    Task UnpublishCourseAsync(Guid courseId);
}
