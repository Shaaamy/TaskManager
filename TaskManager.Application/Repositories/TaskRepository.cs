using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repositories;
using TaskManager.Core.Entities;
using TaskManager.Repository.Data;

namespace TaskManager.Repository.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagerDbContext _dbContext;

        public TaskRepository(TaskManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(TaskItem task)
        {
            await _dbContext.TaskItems.AddAsync(task);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(TaskItem task)
        {
            _dbContext.TaskItems.Remove(task);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _dbContext.TaskItems.ToListAsync();
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await _dbContext.TaskItems.FindAsync(id);
        }
    }
}
