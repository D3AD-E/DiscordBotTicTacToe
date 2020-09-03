using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DiscordBotTicTacToe.Core.Game
{
    class GameLogic
    {
        
        private const int width = 3;//assume height = width
        private const int height = 3;

        private enum Tile
        {
            X,
            O,
            Empty
        }

        private Tile[,] Board = new Tile[height, width];
        public PlayerController PController { get; private set; }
        
        public GameLogic(ulong playerID1, ulong playerID2 = 0)
        {
            PController = new PlayerController(playerID1, playerID2);
            for(int i = 0;i<height;i++)
                for(int j = 0; j<width;j++)
                {
                    Board[i, j] = Tile.Empty;
                }

        }

        public GameState Place(int x, int y)
        {
            if (!PController.IsReady())
                return GameState.AwaitingPlayer;
            x--;
            y--;
            if (x >= width || y >= height || x < 0 || y < 0||Board[x,y]!=Tile.Empty)
                return GameState.MoveFailed;

            Board[x, y] = (Tile)PController.CurrentPlayer;           

            PController.NextMove();
            return HasGameEnded();
        }

        private GameState HasGameEnded()                //Redo
        {
            //Check rows
            for(int i = 0; i<height; i++)
            {
                Tile firstTile = Board[0, i];
                if (firstTile == Tile.Empty)
                {
                    continue;
                }    
                for(int j = 1; j<width;j++)
                {
                    if(Board[j,i] == Tile.Empty)
                    {
                        break;
                    }
                    if(firstTile!=Board[j,i])
                    {
                        break;
                    }
                    if (j == width - 1)
                        return (GameState)((int)Board[j, i] + 3);
                }
            }

            //Check columns
            for (int i = 0; i < width; i++)
            {
                Tile firstTile = Board[i, 0];
                if (firstTile == Tile.Empty)
                    continue;
                for (int j = 1; j < width; j++)
                {
                    if (firstTile != Board[i, j])
                    {
                        break;
                    }
                    if (j == height - 1)
                        return (GameState)((int)Board[i, j] + 3);
                }
            }
            //Check diagonals
            for (int j =0 ,i = 0; i < height; i++, j++)
            {
                Tile firstTile = Board[0, 0];
                if (firstTile == Tile.Empty)
                    break;
                if (firstTile != Board[i, j])
                {
                    break;
                }
                if (i == height - 1)
                    return (GameState)((int)Board[i, j] + 3);
            }
            for (int j = width-1,i = 0; i < height; i++,j--)
            {
                Tile firstTile = Board[width-1, 0];
                if (firstTile == Tile.Empty)
                    break;
                if (firstTile != Board[i, j])
                {
                    break;
                }
                if (i == height - 1)
                    return (GameState)((int)Board[i, j] + 3);
            }
            //Check tie
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    if(Board[i, j] == Tile.Empty)
                    {
                        return GameState.MoveSuccessfull;
                    }    
                }
            return GameState.Tie;
        }

        public string TransformBoardToDiscord()
        {
            var sb = new StringBuilder();
            for(int i = 0; i<height;i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sb.Append(EmojiConvert(Board[j, i]));
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private string EmojiConvert(Tile current)
        {
            switch(current)
            {
                case Tile.X: return ":x:";
                case Tile.O: return ":o:";
                case Tile.Empty: return ":white_large_square:";
            }
            return string.Empty;
        }
    }
}
