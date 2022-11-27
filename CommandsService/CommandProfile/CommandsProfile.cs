using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.CommandProfile
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            //Source --> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
        }
    }
}