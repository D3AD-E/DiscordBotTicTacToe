using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBotTicTacToe.Core.Game
{
    class PlayerController
    {
        public PlayerPair Players { get; private set; }
        public int CurrentPlayer { get; private set; }

        public PlayerController(ulong playerID1, ulong playerID2)
        {
            Players = new PlayerPair(playerID1, playerID2);
            CurrentPlayer = 0;
        }

        public bool IsReady()
        {
            return Players.PlayerID2 != 0;
        }
        public void NextMove()
        {
            CurrentPlayer++;
            if (CurrentPlayer > 1)
                CurrentPlayer = 0;
            //REDO
        }

        public ulong CurrentPlayerID()
        {
            if (CurrentPlayer == 0)
            {
                return Players.PlayerID1;
            }
            else if (CurrentPlayer == 1)
            {
                return Players.PlayerID2;
            }
            else
                throw new IndexOutOfRangeException("Only 2 players allowed");
        }
    }
}
