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
    // In de Board klasse wordt de wereld aangemaakt (factory pattern) en ingevuld. Tevens wordt hier toegestane beweging/collision bepaald.
    public class Board
    {
        private int _columns, _rows;
        private Random _rnd = new Random();

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

        // Hier wordt gecontroleerd op de aanwezigheid van een Tile element welk beweging blokkeert.
        public bool HasRoomForRectangle(Rectangle rectangleToCheck)
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

        public Board(SpriteBatch spriteBatch, Texture2D tileTexture, int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
            TileTexture = tileTexture;
            SpriteBatch = spriteBatch;
            Tiles = new Tile[Columns, Rows];
        }

        // Creatie van Level 1: de plaatsing van de elementen is willekeurig. Deze functie activeert het nieuwe Board ook.
        public void CreateNewRandomBoard() {
            CreateRandomBoard();
            SetBorderTilesBlocked();
            UnrestrictedPlayerSpawn();
            CurrentBoard = this;
        }

        // Creatie van Level 2: het verschil met de bovenstaande functie is de afwezigheid van willekeur en aanwezigheid van statische platformen.
        public void CreateNewBossBoard()
        {
            CreateBoard();
            SetBorderTilesBlocked();
            UnrestrictedPlayerSpawn();
            SetBossEncounterPlatforms();
            CurrentBoard = this;
        }

        // Toewijzing statische platformen Level 2
        public void SetBossEncounterPlatforms()
        {
            //upper platforms
            Tiles[19, 6].IsBlocked = true;
            Tiles[20, 6].IsBlocked = true;
            Tiles[21, 6].IsBlocked = true;

            Tiles[35, 6].IsBlocked = true;
            Tiles[36, 6].IsBlocked = true;
            Tiles[37, 6].IsBlocked = true;

            //middle platforms
            Tiles[12, 12].IsBlocked = true;
            Tiles[13, 12].IsBlocked = true;
            Tiles[14, 12].IsBlocked = true;

            Tiles[44, 12].IsBlocked = true;
            Tiles[45, 12].IsBlocked = true;
            Tiles[46, 12].IsBlocked = true;

            //lower platforms
            Tiles[19, 18].IsBlocked = true;
            Tiles[20, 18].IsBlocked = true;
            Tiles[21, 18].IsBlocked = true;
                       
            Tiles[35, 18].IsBlocked = true;
            Tiles[36, 18].IsBlocked = true;
            Tiles[37, 18].IsBlocked = true;
        }

        // De onderstaande functie controleert of de beweging van de speler mogelijk is binnen de context van de Board. (collision met tiles)
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

        // De onderstaande functie zorgt ervoor dat wanneer er vanuit een diagonale beweging over wordt gegaan in een non-diagonale beweging het momentum
        // niet spontaan verloren gaat, maar omgezet wordt in een resterende beweging bij blokkade van de speler op de X-as of Y-as.
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

        // De onderstaande rectangle dient wordt gebruikt ter controle of de afmetingen van de speler daadwerkelijk binnen de omgeving past bij beweging.
        private Rectangle CreateRectangleAtPosition(Vector2 positionToTry, int width, int height)
        {
            return new Rectangle((int)positionToTry.X, (int)positionToTry.Y, width, height);
        }

        // Ongeblokkeerde spawnpoint linksboven.
        private void UnrestrictedPlayerSpawn()
        {
            Tiles[1, 1].IsBlocked = false;
            Tiles[1, 2].IsBlocked = false;
            Tiles[2, 1].IsBlocked = false;
            Tiles[2, 2].IsBlocked = false;
        }

        // Willekeurige toewijzing van tiles binnen Level 1.
        private void CreateRandomBoard()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    Vector2 tilePosition = new Vector2(x * TileTexture.Width, y * TileTexture.Height);
                    Tiles[x, y] = new Tile(TileTexture, tilePosition, SpriteBatch, _rnd.Next(16) == 0);
                }
            }
        }

        // Creatie 'kaal' board.
        private void CreateBoard() {
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
                    Vector2 tilePosition = new Vector2(x * TileTexture.Width, y * TileTexture.Height);
                    Tiles[x, y] = new Tile(TileTexture, tilePosition, SpriteBatch, false);
                }
            }
        }

        // Toewijzen van van de grenzen van de speelwereld.
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
