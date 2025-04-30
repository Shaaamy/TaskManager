using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Repositories
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task);
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem> GetTaskByIdAsync(int id);
        Task Delete(TaskItem task);
    }
}
