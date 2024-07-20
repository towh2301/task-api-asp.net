using MyApp.Models;
namespace Interface.ITaskService
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId);
        Task<TaskItem> GetTaskByIdAsync(int taskId, int userId);
        Task<TaskItem> CreateTaskAsync(TaskItem task, int userId);
        Task UpdateTaskAsync(TaskItem task, int userId);
        Task DeleteTaskAsync(int taskId, int userId);
    }
}