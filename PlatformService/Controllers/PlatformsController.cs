using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo platformRepo, IMapper mapper, ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var platforms = _platformRepo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id:int}", Name = "GetPlatformById")]
        public IActionResult GetPlatformById(int id)
        {
            var platformEntity = _platformRepo.GetPlatformById(id);
            if (platformEntity != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformEntity));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlatform([FromBody] PlatformCreateDto platformCreateDto)
        {
            var platformEntity = _mapper.Map<Platform>(platformCreateDto);
            _platformRepo.CreatePlatform(platformEntity);
            _platformRepo.SaveChange();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformEntity);
            //Send sync message 
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send synchronously: {e.Message}");
            }

            //Send async message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send asynchronously: {e.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);
        }
    }
}