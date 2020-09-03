
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using DiscordBotTicTacToe.DAL;
using DiscordBotTicTacToe.Core.Services;

namespace DiscordBotTicTacToe
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<TicTacToeContext>(options =>
            {
                options.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true",
                    x => x.MigrationsAssembly("DiscordBotTicTacToe.DAL.Migrations"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddScoped<IPlayerService, PlayerService>();

            var serviceProvider = services.BuildServiceProvider();

            var Bot = new Bot(serviceProvider);

            services.AddSingleton(Bot);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
