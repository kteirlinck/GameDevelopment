
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
    // In de Player klasse wordt gameplay input verwerkt en positie/beweging berekend.
    public class Vortex : Sprite
    {
        Animation animationPlayer;
        Spritesheet animation;

        Rectangle collisionRectangle;

        private SpriteBatch spriteBatch;

        private GameController gc;

        public void Load(ContentManager Content)
        {
            animation = new Spritesheet(Content.Load<Texture2D>("whirlwind"), 192, 0.2f, true);
            animationPlayer.PlayAnimation(animation);
         
            Position = new Vector2((Game1.ScreenWidth / 2) - (animation.FrameWidth / 2), (Game1.ScreenHeight / 2) + (animation.FrameHeight / 2));
            collisionRectangle = new Rectangle((int)Position.X - animation.FrameWidth, (int)Position.Y - animation.FrameHeight, animation.FrameWidth, animation.FrameHeight);
        }

        public Vortex(Texture2D texture, Vector2 position, SpriteBatch spriteBatch)
            : base(texture, position, spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            gc = GameController.Instance;
        }

        private bool ShouldSpawn() {
            return gc.ScoreCount >= gc.MaxScoreCount && gc.CurrentLevel == 1;
        }

        public void Update(GameTime gameTime, Player player)
        {
            if (collisionRectangle.Intersects(player.Bounds) && ShouldSpawn())
                gc.FinishLevel();
            
        }
        
        public override void Draw(GameTime gameTime)
        {
            if (ShouldSpawn())
                animationPlayer.Draw(gameTime, spriteBatch, Position, SpriteEffects.None);
        }
    }
}
