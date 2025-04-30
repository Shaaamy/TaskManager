using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Repositories;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;

namespace TaskManager.Service
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task AddTaskAsync(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                throw new ArgumentException("Task title cannot be empty.");
            }
            await _taskRepository.AddAsync(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            var Task = await _taskRepository.GetTaskByIdAsync(id);
            if (Task == null)
            {
                // Handle the case where the task is not found, maybe throw an exception or return a specific result
                throw new ArgumentException($"Task with id {id} not found.");
            }
            await _taskRepository.Delete(Task);
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetTaskByIdAsync(id);
        }
    }
}
