# DiscordBotTicTacToe
> Play Tic Tac Toe in your Discord guild!
## General info

- Bot uses prefix . however it can be changed in config.json
- Your bot key must be put in config.json
- Use .help to see all available commands

## Features

- Duel a user or open a game for any user to join!
- User stats: Wins/Losses/Abandons on a guild or globally on overy guild where they have played Tic Tac Toe
- Admins can restrict a user from playing Tic Tac Toe and using the bot without banning them from guild
- Can support any size of square board

## Installation

> Packages required for DiscordBotTicTacToe.Bot
- DSharpPlus
- DSharpPlus.CommandsNext
- DSharpPlus.Interactivity
- Microsoft.AspNetCore.Hosting
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.Extensions.Hosting

> Packages required for DiscordBotTicTacToe.DAL
- Microsoft.EntityFrameworkCore
- System.ComponentModel.Annotations

> Packages required for DiscordBotTicTacToe.DAL.Migrations
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore.SqlServer

> Add reference to projects DiscordBotTicTacToe.DAL and DiscordBotTicTacToe.DAL.Migrations from DiscordBotTicTacToe.Bot

> Build SQL database table using:
- dotnet tool install --global dotnet-ef  
- Open git bash in core project and use command: dotnet add package Microsoft.EntityFrameworkCore.Design   
- dotnet-ef migrations add InitialCreate -p (path to DiscordBotTicTacToe.DAL.Migrations.csproj) --context DiscordBotTicTacToe.DAL.TicTacToeContext
- dotnet-ef database update -p (path to iscordBotTicTacToe.DAL.Migrations.csproj)--context DiscordBotTicTacToe.DAL.TicTacToeContext

## Usage 

> Not all commands are listed here!
-  .start - Start a search for a user to play Tic Tac Toe
-  .duel @username - Challenge a user to play Tic Tac Toe
-  .help - See all other commands

---
