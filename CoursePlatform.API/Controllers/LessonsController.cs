using CoursePlatform.Application.DTOs.Lesson;
using CoursePlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursePlatform.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    /// <summary>
    /// Retrieves all active lessons for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <returns>A list of lessons.</returns>
    [HttpGet("course/{courseId}")]
    [ProducesResponseType(typeof(IEnumerable<LessonDto>), 200)]
    public async Task<IActionResult> GetByCourse(Guid courseId)
    {
        try
        {
            var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
            return Ok(lessons);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new lesson for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="request">Lesson creation details including Title and Order.</param>
    /// <returns>The created lesson details.</returns>
    [HttpPost("course/{courseId}")]
    [ProducesResponseType(typeof(LessonDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create(Guid courseId, [FromBody] CreateLessonRequest request)
    {
        try
        {
            var result = await _lessonService.CreateLessonAsync(courseId, request);
            return CreatedAtAction(nameof(GetByCourse), new { courseId }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing lesson's details.
    /// </summary>
    /// <param name="id">The unique identifier of the lesson.</param>
    /// <param name="request">Updated lesson details.</param>
    /// <returns>The updated lesson details.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(LessonDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLessonRequest request)
    {
        try
        {
            var result = await _lessonService.UpdateLessonAsync(id, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Soft deletes a lesson.
    /// </summary>
    /// <param name="id">The unique identifier of the lesson.</param>
    /// <returns>Success message.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _lessonService.DeleteLessonAsync(id);
            return Ok(new { message = "Lesson deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Reorders multiple lessons within a course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="request">List of lesson IDs and their new absolute orders.</param>
    /// <returns>Success message.</returns>
    [HttpPost("course/{courseId}/reorder")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Reorder(Guid courseId, [FromBody] ReorderLessonsRequest request)
    {
        try
        {
            await _lessonService.ReorderLessonsAsync(courseId, request);
            return Ok(new { message = "Lessons reordered successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
