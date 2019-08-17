using Konquer.Classes.Models;
using Konquer.Classes.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.World
{
    // In de Board klasse wordt de wereld aangemaakt en ingevuld. Tevens wordt hier toegestane beweging/collision bepaald.
    public class Board
    {
        public Tile[,] Tiles { get; set; }
        public Texture2D TileTexture { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public int Columns
        {
            set { if (value >= 0) { _columns = value; } }
            get { return _columns; }
        }
        public int Rows
        {
            set { if (value >= 0) { _rows = value; } }
            get { return _rows; }
        }

        public bool HasRoomForRectangle(Rectangle rectangleToCheck)// Collision
        {
            foreach (var tile in Tiles)
            {
                if (tile.IsBlocked && tile.Bounds.Intersects(rectangleToCheck))
                {
                    return false;
                }
            }
            return true;
        }
        
        public static Board CurrentBoard { get; private set; }

        private int _columns, _rows;
        private Random _rnd = new Random();

        // opslaan params, instantiëren double array columns/rows, instantiëren Tile objects voor Tiles[,]
        public Board(SpriteBatch spriteBatch, Texture2D tileTexture, int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
            TileTexture = tileTexture;
            SpriteBatch = spriteBatch;
            Tiles = new Tile[Columns, Rows];
        }

        public void CreateNewRandomBoard() {
            CreateRandomBoard();
            SetBorderTilesBlocked();
            UnrestrictedPlayerSpawn();

            CurrentBoard = this;
        }

        public void CreateNewBoard()
        {
            CreateBoard();
            SetBorderTilesBlocked();
            UnrestrictedPlayerSpawn();

            CurrentBoard = this;
        }

        public Vector2 AllowedMovement(Vector2 originalPosition, Vector2 destination, Rectangle boundingRectangle)
        {
            MovementWrapper move = new MovementWrapper(originalPosition, destination, boundingRectangle);

            for (int i = 1; i <= move.NumberOfStepsToBreakMovementInto; i++)
            {
                Vector2 positionToTry = originalPosition + move.OneStep * i;
                Rectangle newBoundary =
                    CreateRectangleAtPosition(positionToTry, boundingRectangle.Width, boundingRectangle.Height);
                if (HasRoomForRectangle(newBoundary)) { move.PlausibleCurrentPosition = positionToTry; }
                else if (move.IsDiagonalMove) {
                    move.PlausibleCurrentPosition = CheckForNonDiagonalMovement(move, i);
                }
                break;
            }
            return move.PlausibleCurrentPosition;
        }

        private Vector2 CheckForNonDiagonalMovement(MovementWrapper wrapper, int i)
        {
            if (wrapper.IsDiagonalMove)
            {
                int stepsLeft = wrapper.NumberOfStepsToBreakMovementInto - (i - 1);

                Vector2 remainingHorizontalMovement = wrapper.OneStep.X * Vector2.UnitX * stepsLeft;
                Vector2 finalPositionIfMovingHorizontally = wrapper.PlausibleCurrentPosition + remainingHorizontalMovement;
                wrapper.PlausibleCurrentPosition =
                    AllowedMovement(wrapper.PlausibleCurrentPosition, wrapper.PlausibleCurrentPosition + remainingHorizontalMovement, wrapper.BoundingRectangle);

                Vector2 remainingVerticalMovement = wrapper.OneStep.Y * Vector2.UnitY * stepsLeft;
                Vector2 finalPositionIfMovingVertically = wrapper.PlausibleCurrentPosition + remainingVerticalMovement;
                wrapper.PlausibleCurrentPosition =
                    AllowedMovement(wrapper.PlausibleCurrentPosition, wrapper.PlausibleCurrentPosition + remainingVerticalMovement, wrapper.BoundingRectangle);
            }
            return wrapper.PlausibleCurrentPosition;
        }

        private Rectangle CreateRectangleAtPosition(Vector2 positionToTry, int width, int height)
        {
            return new Rectangle((int)positionToTry.X, (int)positionToTry.Y, width, height);
        }

        private void UnrestrictedPlayerSpawn()
        {
            //TODO: OPSCHONEN
            Tiles[1, 1].IsBlocked = false;
            Tiles[1, 2].IsBlocked = false;
            Tiles[2, 1].IsBlocked = false;
            Tiles[2, 2].IsBlocked = false;
        }

        private void CreateRandomBoard()
        {
            //Tiles = new Tile[Columns, Rows];
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Vector2 tilePosition = new Vector2(x * TileTexture.Width, y * TileTexture.Height);
                    Tiles[x, y] = new Tile(TileTexture, tilePosition, SpriteBatch, _rnd.Next(16) == 0);
                }
            }
        }

        private void CreateBoard() {
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
                    Vector2 tilePosition = new Vector2(x * TileTexture.Width, y * TileTexture.Height);
                    Tiles[x, y] = new Tile(TileTexture, tilePosition, SpriteBatch, false);
                }
            }
        }

        private void SetBorderTilesBlocked()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (x == 0 || x == Columns - 1 || y == 0 || y == Rows - 1)
                    { Tiles[x, y].IsBlocked = true; }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var tile in Tiles)
            {
                tile.Draw(gameTime);
            }
        }
    }
}
