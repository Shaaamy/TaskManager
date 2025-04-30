using AutoMapper;
using TaskManager.APIs.DTOs;
using TaskManager.Core.Entities;

namespace TaskManager.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<TaskItem, TaskItemDTO>()
                .ReverseMap();
        }
    }
}
