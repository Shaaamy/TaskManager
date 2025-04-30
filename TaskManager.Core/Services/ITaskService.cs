using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Services
{
    public interface ITaskService
    {
        Task AddTaskAsync(TaskItem task);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task DeleteTaskAsync(int id);
    }
}
