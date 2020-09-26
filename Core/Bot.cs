using DiscordBotTicTacToe.Commands;
using DiscordBotTicTacToe.Core.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotTicTacToe
{
    internal class Bot
    {
        public DiscordClient Client { get; private set; }

        public CommandsNextModule Commands { get; private set; }
        private IServiceProvider _provider;
        private IPlayerService _service;

        public Bot(IServiceProvider provider)
        {
            _provider = provider;
            _service = (IPlayerService)provider.GetService(typeof(IPlayerService));

            var json = String.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, false))
                json = sr.ReadToEnd();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            Client.Ready += OnReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var deps = BuildDeps();

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefix = configJson.Prefix,
                EnableMentionPrefix = true,
                EnableDms = false,
                Dependencies = deps
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            //Commands.RegisterCommands<TestCommands>();
            Commands.RegisterCommands<GameCommands>();
            Commands.RegisterCommands<ContextCommands>();
            Client.ConnectAsync();
        }

        private DependencyCollection BuildDeps()
        {
            using var deps = new DependencyCollectionBuilder();

            deps.AddInstance(_provider).AddInstance(_service);                     

            return deps.Build();
        }

        private Task OnReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
