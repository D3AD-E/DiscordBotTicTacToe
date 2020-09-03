using DiscordBotTicTacToe.Core.Game;
using DiscordBotTicTacToe.Core.Services;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBotTicTacToe.Commands
{
    internal class GameCommands
    {
        private Dictionary<PlayerPair, GameLogic> games = new Dictionary<PlayerPair, GameLogic>();

        private readonly IPlayerService _playerService;

        public GameCommands(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [Description("Start a search for a user to play Tic Tac Toe")]
        [Command("start")]
        public async Task Start(CommandContext ctx)
        {
            var player = await _playerService.GetOrCreatePlayer(ctx.User.Id, ctx.Guild.Id);
            if (player.isBanned)
            {
                await ctx.Channel.SendMessageAsync(":clown:");
                return;
            }
            var startEmbed = new DiscordEmbedBuilder
            {
                Title = "New Game",
                ThumbnailUrl = ctx.Client.CurrentUser.AvatarUrl,
                Description = "Waiting for player 2\n React to this message with :wave: to join!",
                Color = DiscordColor.Cyan
            };

            var message = await ctx.Channel.SendMessageAsync(embed: startEmbed);

            var waveEmoji = DiscordEmoji.FromName(ctx.Client, ":wave:");

            await message.CreateReactionAsync(waveEmoji);

            var interactivity = ctx.Client.GetInteractivityModule();

            bool gotReaction = false;

            while (!gotReaction)
            {
                var reactionResult = await interactivity.WaitForReactionAsync(x => x == waveEmoji);
                if (reactionResult == null)
                {
                    await ctx.Channel.SendMessageAsync("No one responded, closing the game");
                    return;
                }
                else if (reactionResult.Emoji == waveEmoji && reactionResult.User != ctx.Client.CurrentUser && reactionResult.User != ctx.User)
                {
                    gotReaction = true;
                    var pair = new PlayerPair(ctx.User.Id, reactionResult.User.Id);
                    if (games.ContainsKey(pair))
                    {
                        var errorMessage = await ctx.Channel.SendMessageAsync("Either user is already playing a game, get him to finish that one first!");
                        return;
                    }
                    await PlayGame(ctx, reactionResult.User, pair);
                }
            }
        }

        [Description("Challenge a user to play Tic Tac Toe")]
        [Command("duel")]
        public async Task Duel(CommandContext ctx, [Description("Specify a user via @username")] DiscordMember opponent)
        {
            var player = await _playerService.GetOrCreatePlayer(ctx.User.Id, ctx.Guild.Id);
            if (player.isBanned)
            {
                await ctx.Channel.SendMessageAsync(":clown:");
                return;
            }

            var pair = new PlayerPair(ctx.User.Id, opponent.Id);
            if (games.ContainsKey(pair))
            {
                var message = await ctx.Channel.SendMessageAsync("Either user is already playing a game, get him to finish that one first!");
                return;
            }
            else if (opponent == ctx.User)
            {
                var message = await ctx.Channel.SendMessageAsync(ctx.User.Username + " tried to duel himself and destroyed the universe:exploding_head: ");
                return;
            }
            //Fix later just testing
            if (opponent != null)
            {
                if (opponent == ctx.Client.CurrentUser)
                {
                    await ctx.Channel.SendMessageAsync("You cannot duel me, mortal!");
                }
                else
                {
                    var startEmbed = new DiscordEmbedBuilder
                    {
                        Title = "New Game",
                        ThumbnailUrl = opponent.AvatarUrl,
                        Description = "Waiting for " + opponent.Username + "\n Answer y to accept",
                        Color = DiscordColor.Cyan
                    };

                    var message = await ctx.Channel.SendMessageAsync(embed: startEmbed);

                    var interactivity = ctx.Client.GetInteractivityModule();

                    var messageResult = await interactivity.WaitForMessageAsync(x => x.Author == opponent, TimeSpan.FromMinutes(2));

                    if (messageResult != null && messageResult.Message != null && messageResult.Message.Content.ToLower() == "y")
                    {
                        await PlayGame(ctx, opponent, pair);
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync(opponent.Username + " has declined a duel");
                    }
                }
            }
            else
            {
                await ctx.Channel.SendMessageAsync("User with this nickname does not exist");
            }
        }

        private async Task PlayGame(CommandContext ctx, DiscordUser opponent, PlayerPair pair)
        {
            GameLogic game;
            if (games.ContainsKey(pair))
            {
                game = games[pair];
            }
            else
            {
                game = new GameLogic(pair.PlayerID1, pair.PlayerID2);
                games.Add(pair, game);
            }

            var interactivity = ctx.Client.GetInteractivityModule();
            string info = string.Empty;

            var currentState = GameState.MoveSuccessfull;

            var player1 = await _playerService.GetOrCreatePlayer(ctx.User.Id, ctx.Guild.Id);
            if (player1.isBanned)
            {
                currentState = GameState.Owin;
            }
            var player2 = await _playerService.GetOrCreatePlayer(opponent.Id, ctx.Guild.Id);
            if (player2.isBanned)
            {
                currentState = GameState.Xwin;
            }
            while (currentState == GameState.MoveSuccessfull || currentState == GameState.MoveFailed)
            {
                int currentPlayer = game.PController.CurrentPlayer;
                if (currentPlayer == 0)
                {
                    info = "Current turn: " + ctx.Member.Username + ", playing as :x:" + "\n Write a position in format X Y";
                }
                else if (currentPlayer == 1)
                {
                    info = "Current turn: " + opponent.Username + ", playing as :o:" + "\n Write a position in format X Y";
                }
                else
                    throw new IndexOutOfRangeException();

                var boardEmbed = new DiscordEmbedBuilder
                {
                    Title = ctx.Member.Username + " vs " + opponent.Username,
                    Description = game.TransformBoardToDiscord() + info,
                    Color = DiscordColor.Gold
                };

                var gameMessage = await ctx.Channel.SendMessageAsync(embed: boardEmbed);

                var messageResult = await interactivity.WaitForMessageAsync(x => x.Author.Id == game.PController.CurrentPlayerID()
                && x.Content.Length == 3
                && char.IsDigit(x.Content, 0)
                && char.IsWhiteSpace(x.Content, 1)
                && char.IsDigit(x.Content, 2),
                TimeSpan.FromMinutes(2));

                if (messageResult == null || messageResult.Message == null)
                {
                    if (game.PController.CurrentPlayer == 0)
                        currentState = GameState.OwinByDc;
                    else if (game.PController.CurrentPlayer == 1)
                        currentState = GameState.XwinByDc;
                    else
                        throw new ArgumentOutOfRangeException("Unknown player");
                    break;
                }

                currentState = game.Place(Convert.ToInt32(messageResult.Message.Content[0].ToString()), Convert.ToInt32(messageResult.Message.Content[2].ToString()));
                if (currentState == GameState.MoveFailed)
                {
                    await ctx.Channel.SendMessageAsync("Move failed, try again");
                }
                Console.WriteLine(currentState);
            }
            string winnerUrl = string.Empty;
            switch (currentState)
            {
                case GameState.Xwin:
                    info = "GG EZ for " + ctx.Member.Username + ":crown:";
                    winnerUrl = ctx.Member.AvatarUrl;
                    player1.Wins++;
                    await _playerService.UpdatePlayer(player1);
                    player2.Losses++;
                    await _playerService.UpdatePlayer(player2);
                    break;

                case GameState.Owin:
                    info = "GG EZ for " + opponent.Username + ":crown:";
                    winnerUrl = opponent.AvatarUrl;
                    player2.Wins++;
                    await _playerService.UpdatePlayer(player2);
                    player1.Losses++;
                    await _playerService.UpdatePlayer(player1);
                    break;

                case GameState.XwinByDc:
                    info = "GG EZ for " + ctx.Member.Username + ":crown: \n" + opponent.Username + " flew :man_running:";
                    winnerUrl = ctx.Member.AvatarUrl;
                    player1.Wins++;
                    await _playerService.UpdatePlayer(player1);
                    player2.Abandons++;
                    await _playerService.UpdatePlayer(player2);
                    break;

                case GameState.OwinByDc:
                    info = "GG EZ for " + opponent.Username + ":crown: \n" + ctx.User.Username + " flew :man_running:";
                    winnerUrl = opponent.AvatarUrl;
                    player2.Wins++;
                    await _playerService.UpdatePlayer(player2);
                    player1.Abandons++;
                    await _playerService.UpdatePlayer(player1);
                    break;

                case GameState.Tie:
                    info = "It's a tie this time :necktie:";
                    break;
            }

            games.Remove(pair);

            var finishEmbed = new DiscordEmbedBuilder
            {
                Title = ctx.Member.Username + " vs " + opponent.Username,
                Description = game.TransformBoardToDiscord() + info,
                ThumbnailUrl = winnerUrl,
                Color = DiscordColor.Green
            };

            var endMessage = await ctx.Channel.SendMessageAsync(embed: finishEmbed);
        }
    }
}