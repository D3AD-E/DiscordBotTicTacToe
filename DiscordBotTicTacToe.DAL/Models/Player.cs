namespace DiscordBotTicTacToe.DAL.Models
{
    public class Player : Entity
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Abandons { get; set; }
        public bool isBanned { get; set; }
    }
}