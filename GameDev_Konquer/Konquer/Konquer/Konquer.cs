﻿using Konquer.Classes.InterfaceControls;
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

        private Texture2D _tileTexture, _playerTexture, _enemyTexture;
        private Player _player;

        //private Enemy _demon;
        private List<Enemy> _demonHorde;
        private float _spawn = 0;
        private float _mobDistance;

        private List<Coin> _coins;

        private Vortex _vortex;

        private Board _board;
        private Background background1;

        private SpriteFont _debugFont;

        public static int ScreenWidth = 1888, ScreenHeight = 1000;
        public static int TileWidth = 32, TileHeight = 40;

        private GameController gc;
        private Random rand = new Random();




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

            //_demon = new Enemy(_enemyTexture, new Vector2(250, 250), _spriteBatch, _mobDistance);
            //_demon.Load(Content);
            _demonHorde = new List<Enemy>();

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


            _debugFont = Content.Load<SpriteFont>("DebugFont");
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (gc.CurrentLevel == 1 && gc.LevelFinished) {
                gc.LevelFinished = false;
                gc.CurrentLevel = 2;
                _board = new Board(_spriteBatch, _tileTexture, 59, 25);
                _board.CreateNewBoard();

                _player.Position = new Vector2(50, ScreenHeight - (TileHeight * 2));
                //_demon.Position = new Vector2(250, 250);
            }

            base.Update(gameTime);
            background1.Update(gameTime);
            _player.Update(gameTime);
            _vortex.Update(gameTime, _player);

            _spawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Enemy demon in _demonHorde)
                demon.Update(gameTime, _player);
            if(gc.CurrentLevel == 1)LoadEnemies();

            for (int i = 0; i < gc.MaxScoreCount; i++) {
                _coins[i].Update(gameTime, _player);
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

            //_demon.Draw(gameTime);

            foreach (Enemy demon in _demonHorde)
                demon.Draw(gameTime);

            _player.Draw(gameTime);
            WriteDebugInfo();
            _spriteBatch.End();
        }

        private void WriteDebugInfo()
        {
            string positionInText =
                string.Format("Position of Player: ({0:0.0}, {1:0.0})", _player.Position.X, _player.Position.Y);
            string movementInText =
                string.Format("Current movement: ({0:0.0}, {1:0.0})", _player.Movement.X, _player.Movement.Y);
            string isGroundedText =
                string.Format("Grounded? : {0}", _player.IsGrounded());
            string scoreCountText = string.Format("Score: {0} / {1}", gc.ScoreCount, gc.MaxScoreCount);

            _spriteBatch.DrawString(_debugFont, positionInText, new Vector2(10, 0), Color.White);
            _spriteBatch.DrawString(_debugFont, movementInText, new Vector2(10, 20), Color.White);
            _spriteBatch.DrawString(_debugFont, isGroundedText, new Vector2(10, 40), Color.White);
            _spriteBatch.DrawString(_debugFont, scoreCountText, new Vector2(10, 60), Color.White);
        }
    }
}