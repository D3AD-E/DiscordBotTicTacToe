using DiscordBotTicTacToe.DAL;
using DiscordBotTicTacToe.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotTicTacToe.Core.Services
{
    public interface IPlayerService
    {
        List<Player> GetPlayersByUserId(ulong id);

        Task<Player> GetOrCreatePlayer(ulong discordId, ulong guildId);

        Task<Player> GetPlayer(ulong discordId, ulong guildId);

        Task CreateNewPlayerAsync(Player player);

        Task UpdatePlayer(Player player);
    }

    public class PlayerService : IPlayerService
    {
        private readonly DbContextOptions<TicTacToeContext> _options;

        public PlayerService(DbContextOptions<TicTacToeContext> options)
        {
            _options = options;
        }

        public List<Player> GetPlayersByUserId(ulong id)
        {
            using var context = new TicTacToeContext(_options);
            var players = context.Players.Where(x => x.UserId == id);

            List<Player> toret = new List<Player>();

            foreach (var player in players)
            {
                toret.Add(player);
            }
            return toret;
        }

        public async Task UpdatePlayer(Player player)
        {
            using var context = new TicTacToeContext(_options);

            //Player gotPlayer =  await context.Players.FirstOrDefaultAsync(x => x.Id == player.Id);

            //gotPlayer = player;

            context.Update(player);

            await context.SaveChangesAsync();
        }

        public async Task CreateNewPlayerAsync(Player player)
        {
            using var context = new TicTacToeContext(_options);

            context.Add(player);

            await context.SaveChangesAsync();
        }

        public async Task<Player> GetOrCreatePlayer(ulong discordId, ulong guildId)
        {
            using var context = new TicTacToeContext(_options);

            var profile = await context.Players
                .Where(x => x.GuildId == guildId).FirstOrDefaultAsync(x => x.UserId == discordId);

            if (profile != null)
            {
                return profile;
            }

            profile = new Player
            {
                UserId = discordId,
                GuildId = guildId,
                isBanned = false,
                Wins = 0,
                Losses = 0,
                Abandons = 0
            };

            context.Add(profile);

            await context.SaveChangesAsync();

            return profile;
        }

        public async Task<Player> GetPlayer(ulong discordId, ulong guildId)
        {
            using var context = new TicTacToeContext(_options);

            return await context.Players
                .Where(x => x.GuildId == guildId).FirstOrDefaultAsync(x => x.UserId == discordId);
        }
    }
}