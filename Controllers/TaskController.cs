using System.Security.Claims;
using Interface.ITaskService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;

namespace Controllers.TaskController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ApplicationException("User not found"));
            return Ok(await _taskService.GetUserTasksAsync(userId));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ApplicationException("User not found"));
            return Ok(await _taskService.GetTaskByIdAsync(id, userId));
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTask(TaskItem taskItem)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ApplicationException("User not found"));
            return Ok(await _taskService.CreateTaskAsync(taskItem, userId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, TaskItem taskItem)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ApplicationException("User not found"));
            if (id != taskItem.Id)
            {
                return BadRequest();
            }

            try
            {
                await _taskService.UpdateTaskAsync(taskItem, userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new ApplicationException("User not found"));
            try
            {
                await _taskService.DeleteTaskAsync(id, userId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Task not found");
            }

            return NoContent();
        }
    }
}