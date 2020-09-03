using DiscordBotTicTacToe.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBotTicTacToe.DAL
{
    public class TicTacToeContext : DbContext
    {
        public TicTacToeContext(DbContextOptions<TicTacToeContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
    }
}