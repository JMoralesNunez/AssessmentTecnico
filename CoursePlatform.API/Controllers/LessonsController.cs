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

    [HttpGet("course/{courseId}")]
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

    [HttpPost("course/{courseId}")]
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

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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

    [HttpPost("course/{courseId}/reorder")]
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
