
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
    // De Vortex klasse beschrijft het portaal dat tevoorschijn komt wanneer de benodigde collectible(s) voltooid is/zijn.
    public class Vortex : Sprite
    {
        Animation _animationPlayer;
        Spritesheet _animation;

        Rectangle _collisionRectangle;

        private SpriteBatch _spriteBatch;

        private GameController _gc;

        public void Load(ContentManager Content)
        {
            _animation = new Spritesheet(Content.Load<Texture2D>("whirlwind"), 192, 0.2f, true);
            _animationPlayer.PlayAnimation(_animation);
         
            Position = new Vector2((Konquer.ScreenWidth / 2) - (_animation.FrameWidth / 2), (Konquer.ScreenHeight / 2) + (_animation.FrameHeight / 2));
            _collisionRectangle = new Rectangle((int)Position.X - (_animation.FrameWidth / 3), (int)Position.Y - (_animation.FrameHeight), _animation.FrameWidth, _animation.FrameHeight);
        }

        public Vortex(Texture2D texture, Vector2 position, SpriteBatch spriteBatch)
            : base(texture, position, spriteBatch)
        {
            this._spriteBatch = spriteBatch;

            _gc = GameController.Instance;
        }

        private bool ShouldSpawn() {
            return _gc.ScoreCount >= _gc.MaxScoreCount && _gc.CurrentLevel == 1;
        }

        public void Update(GameTime gameTime, Player player)
        {
            if (_collisionRectangle.Intersects(player.Bounds) && ShouldSpawn())
                _gc.FinishLevel();
            
        }
        
        public override void Draw(GameTime gameTime)
        {
            if (ShouldSpawn())
                _animationPlayer.Draw(gameTime, _spriteBatch, Position, SpriteEffects.None);
        }
    }
}
