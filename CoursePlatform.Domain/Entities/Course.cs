using CoursePlatform.Domain.Enums;

namespace CoursePlatform.Domain.Entities;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
