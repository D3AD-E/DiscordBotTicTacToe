using System.ComponentModel.DataAnnotations;

namespace DiscordBotTicTacToe.DAL
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}