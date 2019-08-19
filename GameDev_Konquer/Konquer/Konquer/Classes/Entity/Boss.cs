using Konquer.Classes.InterfaceControls;
using Konquer.Classes.Models;
using Konquer.Classes.Sprites;
using Konquer.Classes.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.Entity
{
    public class Boss : Enemy
    {
        Animation animationPlayer;
        Spritesheet geryonAnimation;

        private SpriteBatch bossSpriteBatch;


        public int Health;

        public override void Load(ContentManager Content)
        {
            geryonAnimation = new Spritesheet(Content.Load<Texture2D>("nightmare-galloping"), 144, 0.2f, true);
            animationPlayer.PlayAnimation(geryonAnimation);
        }

        public Boss(Texture2D texture, Vector2 position, SpriteBatch spriteBatch, float newDistance, int newHealth) : base(texture, position, spriteBatch, newDistance)
        {
            bossSpriteBatch = spriteBatch;
            EnemyPosition = position;
            Distance = newDistance;
            OldDistance = Distance;
            Health = newHealth;
        }

        public override void Update(GameTime gameTime, Player player)
        {
            EnemyPosition += Movement;
            Origin = new Vector2(geryonAnimation.FrameWidth / 2, geryonAnimation.FrameHeight / 2);
            if (Distance <= 0)
            {
                Right = true;
                Movement.X = 1f;
            }
            else if (Distance >= OldDistance)
            {
                Right = false;
                Movement.X = -1f;
            }

            if (Right) Distance += 1; else Distance -= 1;

            PlayerDistanceX = player.Position.X - EnemyPosition.X + player.Bounds.Width;

            if (PlayerDistanceX >= -2000 && PlayerDistanceX <= 2000)
            {
                if (PlayerDistanceX < -1)
                    Movement.X = -2.5f;
                else if (PlayerDistanceX > 1)
                    Movement.X = 2.5f;
                else if (PlayerDistanceX == 0)
                    Movement.X = 0f;
            }

            PlayerDistanceY = player.Position.Y - EnemyPosition.Y + player.Bounds.Height;

            if (PlayerDistanceY >= -2000 && PlayerDistanceY <= 2000)
            {
                if (PlayerDistanceY < -1)
                    Movement.Y = -2.5f;
                else if (PlayerDistanceY > 1)
                    Movement.Y = 2.5f;
                else if (PlayerDistanceY == 0)
                    Movement.Y = 0f;
            }
            if(EnemyPosition.Y < Konquer.ScreenHeight - player.Texture.Height && EnemyPosition.Y > Konquer.ScreenHeight - player.Texture.Height * 4)
            {
                Movement.X = Movement.X * 2;
            }
        }

        public override void Draw(GameTime gameTime)
        {

            SpriteEffects flip = SpriteEffects.None;

            if (Movement.X >= 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Movement.X < 0)
                flip = SpriteEffects.None;

            animationPlayer.Draw(gameTime, bossSpriteBatch, EnemyPosition, flip);
        }
    }
}
