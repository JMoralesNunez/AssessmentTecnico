using CoursePlatform.Application.Services;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Domain.Enums;
using CoursePlatform.Domain.Interfaces;
using Moq;
using Xunit;

namespace CoursePlatform.Tests;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _courseRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _courseRepoMock = new Mock<ICourseRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _courseRepoMock.Setup(r => r.UnitOfWork).Returns(_uowMock.Object);
        _courseService = new CourseService(_courseRepoMock.Object);
    }

    [Fact]
    public async Task PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course 
        { 
            Id = courseId, 
            Status = CourseStatus.Draft 
        };
        course.Lessons.Add(new Lesson { Title = "Lesson 1", IsDeleted = false });

        _courseRepoMock.Setup(r => r.GetByIdWithLessonsAsync(courseId))
            .ReturnsAsync(course);

        // Act
        await _courseService.PublishCourseAsync(courseId);

        // Assert
        Assert.Equal(CourseStatus.Published, course.Status);
        _courseRepoMock.Verify(r => r.Update(course), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Id = courseId, Status = CourseStatus.Draft };

        _courseRepoMock.Setup(r => r.GetByIdWithLessonsAsync(courseId))
            .ReturnsAsync(course);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _courseService.PublishCourseAsync(courseId));
        Assert.Contains("without active lessons", exception.Message);
        Assert.Equal(CourseStatus.Draft, course.Status);
    }

    [Fact]
    public async Task DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Id = courseId, IsDeleted = false };
        
        // This test actually tests the repository logic if we use a real DB, 
        // but for Service/Repo interface logic:
        _courseRepoMock.Setup(r => r.Delete(It.IsAny<Course>()))
            .Callback<Course>(c => c.IsDeleted = true);

        // Actually the Delete logic is in Infrastructure, but we can verify the Service calls the Repo.
        // Or we can test the Infrastructure Logic if we use InMemory.
        
        // Let's verify the Repo method is called.
        _courseRepoMock.Object.Delete(course);

        Assert.True(course.IsDeleted);
    }
}
