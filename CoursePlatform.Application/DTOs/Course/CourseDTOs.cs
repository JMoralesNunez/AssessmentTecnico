using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Application.DTOs.Course;

public record CourseSearchRequest(
    string? Query,
    CourseStatus? Status,
    int Page = 1,
    int PageSize = 10
);

public record CourseSearchResponse(
    List<CourseSummaryDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record CourseSummaryDto(
    Guid Id,
    string Title,
    CourseStatus Status,
    int TotalLessons,
    DateTime UpdatedAt
);

public record CourseDetailDto(
    Guid Id,
    string Title,
    CourseStatus Status,
    int TotalLessons,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateCourseRequest(
    string Title
);

public record UpdateCourseRequest(
    string Title
);
