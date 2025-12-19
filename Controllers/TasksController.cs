using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pomodoro_api.Data;
using pomodoro_api.Models;
using pomodoro_api.DTOs;
using System.Collections;
using System.Security.Claims;

namespace pomodoro_api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TasksController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User ID claim is missing from token.");
        }

        return int.Parse(userIdClaim);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable>> GetTasks()
    {
        var userId = GetUserId();
        return await _context.Tasks
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTask(int id)
    {
        var userId = GetUserId();
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult> CreateTask(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            UserId = GetUserId(),
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Category = dto.Category
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTask(int id, UpdateTaskDto dto)
    {
        var userId = GetUserId();

        var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (existingTask == null)
            return NotFound();

        existingTask.Title = dto.Title;
        existingTask.Description = dto.Description;
        existingTask.Priority = dto.Priority;
        existingTask.Category = dto.Category;
        existingTask.IsCompleted = dto.IsCompleted;

        if (dto.IsCompleted && existingTask.CompletedAt == null)
            existingTask.CompletedAt = DateTime.UtcNow;
        else if (!dto.IsCompleted)
            existingTask.CompletedAt = null;

        await _context.SaveChangesAsync();

        return Ok(existingTask);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        var userId = GetUserId();
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
