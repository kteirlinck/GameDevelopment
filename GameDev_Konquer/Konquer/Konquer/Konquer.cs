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
        private Random _rand = new Random();
        private Texture2D _healthTexture;
        private Rectangle _healthRectangle;

        private GameController gc;
        private bool _pauseGame { get; set; }
        private bool _winGame { get; set; }
        private bool _loseGame { get; set; }


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

            // Onderstaande 4 textures worden gebruikt voor collision bepalingen.
            _tileTexture = Content.Load<Texture2D>("Tiles/L2/BlockA1");
            _playerTexture = Content.Load<Texture2D>("Tiles/L2/BlockA1");
            _enemyTexture = Content.Load<Texture2D>("Tiles/L2/BlockA1");
            _bossTexture = Content.Load<Texture2D>("Tiles/L2/BlockA1");

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

            // Hier worden de ingestelde hoeveelheid (gc.MaxScoreCount) Coins in de speelwereld op een willekeurige plaats gezet, met controle dat deze niet
            // per ongeluk worden ingebouwd door random geplaatste blocked tiles.
            _coins = new List<Coin>();
            for (int i = 0; i < gc.MaxScoreCount; i++) {
                Vector2 randPos = new Vector2((_rand.Next(0, Konquer.ScreenWidth / TileWidth) * TileWidth) + 24, _rand.Next(0, Konquer.ScreenHeight / TileHeight) * TileHeight);
                Rectangle coinRect = new Rectangle((int)randPos.X, (int)randPos.Y - TileHeight, TileWidth, TileHeight);

                if(!_board.HasRoomForRectangle(coinRect)) {
                    randPos = new Vector2(_rand.Next(TileWidth, (ScreenWidth - (TileWidth * 2))), _rand.Next(0, Konquer.ScreenHeight / TileHeight) * TileHeight);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState pauseButton = Keyboard.GetState();
            if (pauseButton.IsKeyDown(Keys.P)) { _pauseGame = false; }
            CheckLivingEntityCollision(_player);
            if (!_pauseGame)
            {
                if (gc.CurrentLevel == 1 && gc.LevelFinished)
                {
                    gc.LevelFinished = false;
                    gc.CurrentLevel = 2;
                    _board = new Board(_spriteBatch, _tileTexture, 59, 25);
                    _board.CreateNewBossBoard();

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
                    CheckWinCondition();
                }
            }
        }

        // Deze functie maakt demons aan een verwijdert deze wanneer Level 1 klaar is.
        public void LoadEnemies()
        {
            int randX = _rand.Next(_tileTexture.Width * 2, ScreenWidth - _tileTexture.Width * 2);
            int randY = _rand.Next(_tileTexture.Height * 2, ScreenHeight - _tileTexture.Height * 2);

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

        // Onderstaande functie omvat de speler-enemy/boss interacties die plaatsvinden, om dit werkend te krijgen was er een kleine toevoeging m.b.t.
        // collision-rectangles nodig die alleen hier van toepassing zijn.
        public void CheckLivingEntityCollision(Player _player)
        {
            Rectangle playerCollRect = new Rectangle((int)_player.Position.X, (int)_player.Position.Y, _playerTexture.Width, _playerTexture.Height);
            Rectangle bossCollRect = new Rectangle((int)_geryon.EnemyPosition.X - _bossTexture.Width, (int)_geryon.EnemyPosition.Y - _bossTexture.Height, _bossTexture.Width * 2, _bossTexture.Height * 2);
            foreach (Enemy enemy in _demonHorde)
            {
                Rectangle enemyCollRect = new Rectangle((int)enemy.EnemyPosition.X - _enemyTexture.Width, (int)enemy.EnemyPosition.Y - _enemyTexture.Height, _enemyTexture.Width, _enemyTexture.Height - 20);
                if (playerCollRect.Intersects(enemyCollRect)){
                    gc.PlayerDeath = true;
                    _loseGame = true;
                }
            }
            if (playerCollRect.Intersects(bossCollRect) && playerCollRect.Y >= bossCollRect.Y - 39 && gc.CurrentLevel == 2 && bossCollRect.Y > _playerTexture.Height * 2 && !_player.IsGrounded() && _player.Position.Y < (ScreenHeight - _playerTexture.Height * 5))
            {
                _player.Movement = -Vector2.UnitY * 55;
                _player.Movement = -Vector2.UnitX * 100;
                _geryon.Health -= 3;
            }
            else if (playerCollRect.Intersects(bossCollRect) && playerCollRect.Y < bossCollRect.Y && gc.CurrentLevel == 2)
            {
                gc.PlayerDeath = true;
                _loseGame = true;
            }
        }

        public void CheckWinCondition()
        {
            if(gc.CurrentLevel == 2 && _geryon.Health < 1)
            {
                gc.FinishLevel();
                _winGame = true;
            }
        }




        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
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
            DrawPauseGame();
            _spriteBatch.End();
        }

        // Onderstaande functie voorziet een simplistische versie voor startscherm/eindschermen.
        public void DrawPauseGame()
        {
            if (_pauseGame)
            {
                _spriteBatch.Draw(_pauseMenuTexture, new Rectangle(0,0, ScreenWidth, ScreenHeight), Color.DarkSlateBlue);
                _spriteBatch.DrawString(_myFont, "K O N Q U E R => Press 'P' to play...", new Vector2((ScreenWidth / 2), ScreenHeight / 5), Color.IndianRed);
            }
            if (_loseGame)
            {
                _spriteBatch.Draw(_pauseMenuTexture, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.DarkSlateBlue);
                _spriteBatch.DrawString(_myFont, "Y O U  D I E D...", new Vector2((ScreenWidth / 2), ScreenHeight / 5), Color.IndianRed);
            }
            if (_winGame)
            {
                _spriteBatch.Draw(_pauseMenuTexture, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.DarkSlateBlue);
                _spriteBatch.DrawString(_myFont, "Y O U  A R E  V I C T O R I O U S...", new Vector2((ScreenWidth / 2), ScreenHeight / 5), Color.IndianRed);
            }
        }

        // Deze functie voorkomt dat het spel gelijk begint bij het starten van het programma en maakt de weergave van het startscherm dus mogelijk.
        public void TriggerPauseGame()
        {
            _pauseGame = true;
        }


        // Onderstaande functie is zowel gebruikt voor debugging tijdens development alsook uiteindelijke weergaven van gameplay objectives in UI-vorm. 
        private void WriteInfo()
        {
            //string positionInText =
            //    string.Format("Position of Player: ({0:0.0}, {1:0.0})", _player.Position.X, _player.Position.Y);
            //string movementInText =
            //    string.Format("Current movement: ({0:0.0}, {1:0.0})", _player.Movement.X, _player.Movement.Y);
            //string isGroundedText =
            //    string.Format("Grounded? : {0}", _player.IsGrounded());
            string scoreCountText = string.Format("Score: {0} / {1}", gc.ScoreCount, gc.MaxScoreCount);
            string bossText = string.Format("Geryon ({0}%)", _geryon.Health);

            //_spriteBatch.DrawString(_myFont, positionInText, new Vector2(10, 0), Color.White);
            //_spriteBatch.DrawString(_myFont, movementInText, new Vector2(10, 20), Color.White);
            //_spriteBatch.DrawString(_myFont, isGroundedText, new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(_myFont, scoreCountText, new Vector2(35, 40), Color.White);
            if(gc.CurrentLevel == 2) { _spriteBatch.DrawString(_myFont, bossText, new Vector2((ScreenWidth / 3) + (ScreenWidth / 10), ScreenHeight / 10), Color.White); }
        }
    }
}
