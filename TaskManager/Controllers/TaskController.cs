using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManager.APIs.DTOs;
using TaskManager.Application.Services;
using TaskManager.Core.Entities;

namespace TaskManager.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService,IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles ="AppUser" , Policy ="DepartmentPolicy")]
        public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetAllTasksAsync()
        {
            var Tasks = await _taskService.GetAllTasksAsync();
            var MappedTasks = _mapper.Map<IEnumerable<TaskItemDTO>>(Tasks);

            return Ok(MappedTasks);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDTO>> GetTaskByIdAsync(int id)
        {
            var Task = await _taskService.GetTaskByIdAsync(id);
            var MappedTask =  _mapper.Map<TaskItemDTO>(Task);
            if(Task  == null)
            {
                return NotFound();
            }
            return Ok(MappedTask);
        }
        [HttpPost]
        [Authorize(Roles ="AppUser")]
        public async Task<ActionResult> CreateTask(TaskItem task)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _taskService.AddTaskAsync(task);
            return Ok(task);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveTask(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return Ok();
        }
    }
}
