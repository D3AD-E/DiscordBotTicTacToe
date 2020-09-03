using DiscordBotTicTacToe.Core.Services;
using DiscordBotTicTacToe.DAL.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace DiscordBotTicTacToe.Commands
{
    public class ContextCommands
    {
        private readonly IPlayerService _playerService;

        public ContextCommands(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [Description("Ban a player from playing Tic Tac Toe and using the bot")]
        [Command("ban")]
        public async Task BanPlayer(CommandContext ctx, [Description("Specify a user via @username")] DiscordMember user)
        {
            var roles = ctx.Member.Roles;
            foreach (var role in roles)
            {
                if (DSharpPlus.PermissionLevel.Allowed == role.CheckPermission(DSharpPlus.Permissions.BanMembers))
                {
                    Player toBeBanned = await _playerService.GetPlayer(user.Id, ctx.Guild.Id);

                    if (toBeBanned == null)
                    {
                        return;
                    }

                    toBeBanned.isBanned = true;

                    await _playerService.UpdatePlayer(toBeBanned);

                    await ctx.Channel.SendMessageAsync("Say bye-bye to " + user.Username + " :cry:");
                }
            }
        }

        [Description("Feel sorry for thigs you have done and unban a player")]
        [Command("unban")]
        public async Task UnbanPlayer(CommandContext ctx, [Description("Specify a user via @username")] DiscordMember user)
        {
            var roles = ctx.Member.Roles;
            foreach (var role in roles)
            {
                if (DSharpPlus.PermissionLevel.Allowed == role.CheckPermission(DSharpPlus.Permissions.BanMembers))
                {
                    Player toBeUnbanned = await _playerService.GetPlayer(user.Id, ctx.Guild.Id);

                    if (toBeUnbanned == null)
                    {
                        return;
                    }

                    toBeUnbanned.isBanned = false;

                    await _playerService.UpdatePlayer(toBeUnbanned);

                    await ctx.Channel.SendMessageAsync("Welcome back " + user.Username + " :v:");
                }
            }
        }

        [Description("See wins/losses/abbandons ratio for a player in this guild")]
        [Command("stats")]
        public async Task StatsForPlayer(CommandContext ctx, [Description("Specify a user via @username")] DiscordMember user)
        {
            Player gotPlayer = await _playerService.GetPlayer(user.Id, ctx.Guild.Id);

            if (gotPlayer == null)
            {
                await ctx.Channel.SendMessageAsync("This player has not played TicTacToe yet!");
                return;
            }

            var playerEmbed = new DiscordEmbedBuilder
            {
                Title = "Stats for " + user.Username + " at " + ctx.Guild.Name,
                ThumbnailUrl = user.AvatarUrl,
                Description = ":trophy:Wins: " + gotPlayer.Wins + "\n:skull:Losses: " + gotPlayer.Losses + "\n:man_running:Abandons: " + gotPlayer.Abandons,
                Color = DiscordColor.CornflowerBlue
            };

            await ctx.Channel.SendMessageAsync(embed: playerEmbed);
        }

        [Description("See wins/losses/abbandons ratio for a player in all guilds")]
        [Command("gstats")]
        public async Task GStatsForPlayer(CommandContext ctx, [Description("Specify a user via @username")] DiscordMember user)
        {
            var players = _playerService.GetPlayersByUserId(user.Id);

            if (players == null)
            {
                await ctx.Channel.SendMessageAsync("This player has not played Tic Tac Toe yet!");
                return;
            }

            int wins = 0;
            int losses = 0;
            int abandons = 0;

            foreach (Player player in players)
            {
                wins += player.Wins;
                losses += player.Losses;
                abandons += player.Abandons;
            }

            var playerEmbed = new DiscordEmbedBuilder
            {
                Title = "Global stats for " + user.Username,
                ThumbnailUrl = user.AvatarUrl,
                Description = ":trophy:Wins: " + wins + "\n:skull:Losses: " + losses + "\n:man_running:Abandons: " + abandons,
                Color = DiscordColor.CornflowerBlue
            };

            await ctx.Channel.SendMessageAsync(embed: playerEmbed);
        }
    }
}
