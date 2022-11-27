using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(this IApplicationBuilder builder,bool isProduction)
        {
            using (var serviceScope = builder.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext ctx, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("-->Attempting to apply Migrations...");
                try
                {
                    ctx.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"--> Could not run migrations: {e.Message}");
                }
                
            }
            if (!ctx.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");
                ctx.Platforms.AddRange(
                    new Platform {Name = "DotNet", Publisher = "Micrsoft", Cost = "Free"},
                    new Platform {Name = "SqlServer", Publisher = "Micrsoft", Cost = "Free"},
                    new Platform {Name = "Kubernets", Publisher = "Cloud Native Computing Foundation", Cost = "Free"}
                );
                
                ctx.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}