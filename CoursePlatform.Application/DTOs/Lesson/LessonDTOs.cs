namespace CoursePlatform.Application.DTOs.Lesson;

public record CreateLessonRequest(
    string Title,
    int Order
);

public record UpdateLessonRequest(
    string Title,
    int Order
);

public record LessonDto(
    Guid Id,
    Guid CourseId,
    string Title,
    int Order,
    DateTime UpdatedAt
);

public record ReorderLessonsRequest(
    List<LessonOrderDto> Orders
);

public record LessonOrderDto(
    Guid LessonId,
    int NewOrder
);
