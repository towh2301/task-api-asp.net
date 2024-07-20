
using Interface.ITaskService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;

public class TaskService : ITaskService
{

    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task, int userId)
    {
        task.UserId = userId;
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public Task DeleteTaskAsync(int taskId, int userId)
    {
        var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.UserId == userId) ?? throw new KeyNotFoundException("Task not found");
        _context.Tasks.Remove(task);
        return _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId)
    {
        return await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<TaskItem> GetTaskByIdAsync(int taskId, int userId)
    {
        try
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            return task ?? throw new KeyNotFoundException($"Task not found with id {taskId} and userId {userId}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error when finding task with id {taskId} and userId {userId}", ex);
        }

    }

    public async Task UpdateTaskAsync(TaskItem task, int userId)
    {
        try
        {
            var currentTasks = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id && t.UserId == userId);
            if (currentTasks == null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            currentTasks.Title = task.Title;
            currentTasks.Description = task.Description;
            currentTasks.IsCompleted = task.IsCompleted;
            currentTasks.DueDate = task.DueDate;
            currentTasks.UpdatedAt = DateTime.UtcNow;

        }
        catch (DbUpdateConcurrencyException)
        {
            throw new KeyNotFoundException("Task not found");
        }
        await _context.SaveChangesAsync();
    }


}