
using Konquer.Classes.InterfaceControls;
using Konquer.Classes.Models;
using Konquer.Classes.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.Sprites
{
    // Coin klasse: animated collectible die alleen in level 1 geplaatsd wordt.
    public class Coin : Sprite
    {
        Animation _animationPlayer;
        Spritesheet _animation;

        Rectangle _collisionRectangle;

        private SpriteBatch _spriteBatch;

        private Random _rand = new Random();

        private int _coinWidth = 32;
        private int _coinHeight = 40;

        public bool IsCollected { get; set; }

        public void Load(ContentManager Content)
        {
            _animation = new Spritesheet(Content.Load<Texture2D>("coin"), _coinWidth, 0.2f, true);
            _animationPlayer.PlayAnimation(_animation);
            
            
            _collisionRectangle = new Rectangle((int)Position.X, (int)Position.Y - _animation.FrameHeight, _animation.FrameWidth, _animation.FrameHeight);
        }

        public Coin(Texture2D texture, Vector2 position, SpriteBatch spriteBatch)
            : base(texture, position, spriteBatch)
        {
            _spriteBatch = spriteBatch;
            IsCollected = false;
        }

        public void Update(GameTime gameTime, Player player)
        {

            if (_collisionRectangle.Intersects(player.Bounds) && !IsCollected) {
                IsCollected = true;
                GameController.Instance.ScoreCount++;
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            if (!IsCollected)
                _animationPlayer.Draw(gameTime, _spriteBatch, Position, SpriteEffects.None);
        }
    }
}
