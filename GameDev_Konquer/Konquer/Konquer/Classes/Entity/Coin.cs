
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
    public class Coin : Sprite
    {
        Animation animationPlayer;
        Spritesheet animation;

        Rectangle collisionRectangle;

        private SpriteBatch spriteBatch;

        private Random rand = new Random();

        private int coinWidth = 32;
        private int coinHeight = 40;

        public bool IsCollected { get; set; }

        public void Load(ContentManager Content)
        {
            animation = new Spritesheet(Content.Load<Texture2D>("coin"), coinWidth, 0.2f, true);
            animationPlayer.PlayAnimation(animation);
            
            
            Console.WriteLine("Spawning coin at " + Position.ToString());
            collisionRectangle = new Rectangle((int)Position.X, (int)Position.Y - animation.FrameHeight, animation.FrameWidth, animation.FrameHeight);
        }

        public Coin(Texture2D texture, Vector2 position, SpriteBatch spriteBatch)
            : base(texture, position, spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            IsCollected = false;
        }

        public void Update(GameTime gameTime, Player player)
        {

            if (collisionRectangle.Intersects(player.Bounds) && !IsCollected) {
                IsCollected = true;
                GameController.Instance.ScoreCount++;
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            if (!IsCollected)
                animationPlayer.Draw(gameTime, spriteBatch, Position, SpriteEffects.None);
        }
    }
}
