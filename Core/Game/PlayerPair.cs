using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotTicTacToe.Core.Game
{
    class PlayerPair
    {
        public ulong PlayerID1 { get; private set; }
        public ulong PlayerID2 { get; private set; }

        public PlayerPair(ulong p1, ulong p2)
        {
            PlayerID1 = p1;
            PlayerID2 = p2;
        }
        public override bool Equals(object obj)
        {
            if(obj is PlayerPair pair)
            {
                return (this.PlayerID1 == pair.PlayerID1) || (this.PlayerID1 == pair.PlayerID2) || (this.PlayerID2 == pair.PlayerID1) || (this.PlayerID2 == pair.PlayerID2);
            }
            else if (obj is ulong id)
            {
                return (this.PlayerID1 == id) || (this.PlayerID2 == id);
            }   
            else
                return false;
        }
        public override int GetHashCode()
        {
            return PlayerID1.GetHashCode() ^ PlayerID2.GetHashCode();
        }
        public static bool operator ==(PlayerPair pair, PlayerPair other)
        {
            return pair.Equals(other);
        }
        public static bool operator !=(PlayerPair pair, PlayerPair other)
        {
            return !pair.Equals(other);
        }

        public static bool operator ==(PlayerPair pair, ulong id)
        {
            return pair.Equals(id);
        }

        public static bool operator !=(PlayerPair pair, ulong id)
        {
            return !pair.Equals(id);
        }
    }
}
