using CoursePlatform.Application.DTOs.Course;
using CoursePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Searches for courses based on query and status.
    /// </summary>
    /// <param name="q">Search query for title.</param>
    /// <param name="status">Filter by course status (Draft, Published).</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Items per page.</param>
    /// <returns>Paginated search results.</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(CourseSearchResponse), 200)]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            Domain.Enums.CourseStatus? courseStatus = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Enums.CourseStatus>(status, true, out var parsedStatus))
            {
                courseStatus = parsedStatus;
            }

            var request = new CourseSearchRequest(q, courseStatus, page, pageSize);
            var response = await _courseService.SearchCoursesAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a summary of a specific course.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <returns>Course summary details.</returns>
    [HttpGet("{id}/summary")]
    [ProducesResponseType(typeof(CourseDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        try
        {
            var summary = await _courseService.GetCourseSummaryAsync(id);
            if (summary == null)
            {
                return NotFound(new { error = "Course not found" });
            }
            return Ok(summary);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Publishes a course if it has at least one active lesson.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <returns>Success message.</returns>
    [HttpPatch("{id}/publish")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            await _courseService.PublishCourseAsync(id);
            return Ok(new { message = "Course published successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Reverts a published course back to draft status.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <returns>Success message.</returns>
    [HttpPatch("{id}/unpublish")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        try
        {
            await _courseService.UnpublishCourseAsync(id);
            return Ok(new { message = "Course unpublished successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new course in Draft status.
    /// </summary>
    /// <param name="request">Course creation details.</param>
    /// <returns>The created course summary.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CourseSummaryDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        try
        {
            var result = await _courseService.CreateCourseAsync(request);
            return CreatedAtAction(nameof(GetSummary), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates the details of an existing course.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <param name="request">Updated course details.</param>
    /// <returns>The updated course summary.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CourseSummaryDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseRequest request)
    {
        try
        {
            var result = await _courseService.UpdateCourseAsync(id, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Soft deletes a course by marking it as deleted.
    /// </summary>
    /// <param name="id">The unique identifier of the course.</param>
    /// <returns>Success message.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _courseService.DeleteCourseAsync(id);
            return Ok(new { message = "Course deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
