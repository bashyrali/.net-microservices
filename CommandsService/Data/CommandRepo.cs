using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _ctx;

        public CommandRepo(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public bool SaveChanges()
        {
            return _ctx.SaveChanges() >= 0;
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _ctx.Platforms.ToList();
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _ctx.Platforms.Add(platform);
        }

        public bool PlatformExist(int platformId)
        {
            return _ctx.Platforms.Any(p => p.Id == platformId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _ctx.Commands
                .Where(c => c.PlatformId == platformId)
                .OrderBy(c => c.Platform.Name);
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _ctx.Commands
                .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;
            _ctx.Commands.Add(command);
        }
    }
}