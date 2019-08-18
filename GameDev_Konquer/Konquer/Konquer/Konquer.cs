using Konquer.Classes.Entity;
using Konquer.Classes.InterfaceControls;
using Konquer.Classes.Sprites;
using Konquer.Classes.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Konquer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Konquer : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _tileTexture, _playerTexture, _enemyTexture, _bossTexture, _pauseMenuTexture;
        private Player _player;

        private Boss _geryon;
        private List<Enemy> _demonHorde;
        private float _spawn = 0;
        private float _mobDistance;

        private List<Coin> _coins;

        private Vortex _vortex;

        private Board _board;
        private Background background1;

        private SpriteFont _myFont;

        private GameController gc;
        private bool _pauseGame { get; set; }
        private Random rand = new Random();

        // Boss Health bar
        private Texture2D _healthTexture;
        private Rectangle _healthRectangle;
        public int GeryonHealth = 100;

        public static int ScreenWidth = 1888, ScreenHeight = 1000;
        public static int TileWidth = 32, TileHeight = 40;

        public Konquer()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;

            gc = GameController.Instance;
            gc.CurrentLevel = 1;
            gc.ScoreCount = 0;
            gc.MaxScoreCount = 1;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _tileTexture = Content.Load<Texture2D>("Tiles/L2/BlockA1");
            _playerTexture = Content.Load<Texture2D>("Tiles/L2/Platform");

            _player = new Player(_playerTexture, new Vector2(50, 50), _spriteBatch);
            _vortex = new Vortex(null, new Vector2(0, 0), _spriteBatch);
            _demonHorde = new List<Enemy>();

            _geryon = new Boss(_bossTexture, new Vector2(250, 250), _spriteBatch, _mobDistance, GeryonHealth);
            _geryon.Load(Content);
            _healthTexture = Content.Load<Texture2D>("18");

            _player.Load(Content);
            _vortex.Load(Content);
            _board = new Board(_spriteBatch, _tileTexture, 59, 25);
            _board.CreateNewRandomBoard();

            _coins = new List<Coin>();
            for (int i = 0; i < gc.MaxScoreCount; i++) {
                Vector2 randPos = new Vector2((rand.Next(0, Konquer.ScreenWidth / TileWidth) * TileWidth) + 48, rand.Next(0, Konquer.ScreenHeight / TileHeight) * TileHeight);
                Rectangle coinRect = new Rectangle((int)randPos.X, (int)randPos.Y - TileHeight, TileWidth, TileHeight);

                while(!_board.HasRoomForRectangle(coinRect)) {
                    randPos = new Vector2((rand.Next(0, Konquer.ScreenWidth / TileWidth) * TileWidth) + TileHeight, rand.Next(0, Konquer.ScreenHeight / TileHeight) * TileHeight);
                    coinRect = new Rectangle((int)randPos.X, (int)randPos.Y, TileWidth, TileHeight);
                }

                _coins.Add(new Coin(null, randPos, _spriteBatch));
                _coins[i].Load(Content);
            }

            //Animated background
            List<Texture2D> _BG = new List<Texture2D>();
            for (int i = 0; i < 8; i++) {
                _BG.Add(Content.Load<Texture2D>("BG/BG" + i));
            }
            background1 = new Background();
            background1.SetBG(_BG);

            //Font
            _myFont = Content.Load<SpriteFont>("DebugFont");

            //Pause menu
            _pauseMenuTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pauseMenuTexture.SetData(new[] { Color.DarkSlateBlue });
            TriggerPauseGame();
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState pauseButton = Keyboard.GetState();
            if (pauseButton.IsKeyDown(Keys.P)) { _pauseGame = false; }
            if (!_pauseGame)
            {
                if (gc.CurrentLevel == 1 && gc.LevelFinished)
                {
                    gc.LevelFinished = false;
                    gc.CurrentLevel = 2;
                    _board = new Board(_spriteBatch, _tileTexture, 59, 25);
                    _board.CreateNewBoard();

                    _player.Position = new Vector2(50, ScreenHeight - (TileHeight * 2));
                    _geryon.Position = new Vector2(ScreenWidth - (TileWidth * 2), ScreenHeight - (TileHeight * 2));
                }

                base.Update(gameTime);
                background1.Update(gameTime);
                _player.Update(gameTime);
                _vortex.Update(gameTime, _player);


                _spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
                foreach (Enemy demon in _demonHorde)
                    demon.Update(gameTime, _player);
                if (gc.CurrentLevel == 1) LoadEnemies();

                for (int i = 0; i < gc.MaxScoreCount; i++)
                {
                    _coins[i].Update(gameTime, _player);
                }
                if (gc.CurrentLevel == 2)
                {
                    _geryon.Update(gameTime, _player);
                    _healthRectangle = new Rectangle(ScreenWidth / 2, ScreenHeight / 10, _geryon.Health, 20);
                }
            }
        }

        public void LoadEnemies()
        {
            int randX = rand.Next(100, 1788);
            int randY = rand.Next(100, 900);

            if (_spawn >= 1)
            {
                _spawn = 0;
                if (_demonHorde.Count < 3) {
                    _demonHorde.Add(new Enemy(_enemyTexture, new Vector2(randX, randY), _spriteBatch, _mobDistance));
                    for (int i = 0; i < _demonHorde.Count; i++)
                    {
                        _demonHorde[i].Load(Content);
                    }
                }
            }

            for (int i = 0; i < _demonHorde.Count; i++)
            {
                if (gc.CurrentLevel == 1 && gc.LevelFinished)
                {
                    _demonHorde.RemoveAt(i);
                    i--;
                }
            }
        }

        //public void CheckBossCollision(Player _player)
        //{
        //    _geryon.BossCollisionRectangle.X = (int)_geryon.Position.X;
        //    _geryon.BossCollisionRectangle.Y = (int)_geryon.Position.Y;

        //    if (_player.Bounds.Bottom.Equals(_geryon.Bounds.Top))
        //    {
        //        _geryon.Health -= 20;
        //    }
        //}




        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            DrawPauseGame();
            base.Draw(gameTime);
            background1.Draw(_spriteBatch);
            _board.Draw(gameTime);

            _vortex.Draw(gameTime);

            for (int i = 0; i < gc.MaxScoreCount; i++) {
                _coins[i].Draw(gameTime);
            }


            foreach (Enemy demon in _demonHorde)
                demon.Draw(gameTime);
            if (gc.CurrentLevel == 2)
            {
                _geryon.Draw(gameTime);
                _spriteBatch.Draw(_healthTexture, _healthRectangle, Color.White);
            }
            _player.Draw(gameTime);
            WriteInfo();
            _spriteBatch.End();
        }

        public void DrawPauseGame()
        {
            if (_pauseGame)
            {
                _spriteBatch.Draw(_pauseMenuTexture, new Rectangle(0,0, ScreenWidth, ScreenHeight), Color.DarkSlateBlue);
                _spriteBatch.DrawString(_myFont, "K O N Q U E R => Press P To Play", new Vector2((ScreenWidth / 2), ScreenHeight / 5), Color.IndianRed);
            }
        }

        public void TriggerPauseGame()
        {
            _pauseGame = true;
        }

        private void WriteInfo()
        {
            string positionInText =
                string.Format("Position of Player: ({0:0.0}, {1:0.0})", _player.Position.X, _player.Position.Y);
            string movementInText =
                string.Format("Current movement: ({0:0.0}, {1:0.0})", _player.Movement.X, _player.Movement.Y);
            string isGroundedText =
                string.Format("Grounded? : {0}", _player.IsGrounded());
            string scoreCountText = string.Format("Score: {0} / {1}", gc.ScoreCount, gc.MaxScoreCount);
            string bossText = string.Format("Geryon ({0}%)", _geryon.Health);

            _spriteBatch.DrawString(_myFont, positionInText, new Vector2(10, 0), Color.White);
            _spriteBatch.DrawString(_myFont, movementInText, new Vector2(10, 20), Color.White);
            _spriteBatch.DrawString(_myFont, isGroundedText, new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(_myFont, scoreCountText, new Vector2(10, 60), Color.White);
            if(gc.CurrentLevel == 2) { _spriteBatch.DrawString(_myFont, bossText, new Vector2((ScreenWidth / 3) + (ScreenWidth / 10), ScreenHeight / 10), Color.White); }
        }
    }
}
