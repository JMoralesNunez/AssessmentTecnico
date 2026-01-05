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

    [HttpGet("search")]
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

    [HttpGet("{id}/summary")]
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

    [HttpPatch("{id}/publish")]
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

    [HttpPatch("{id}/unpublish")]
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

    [HttpPost]
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

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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
