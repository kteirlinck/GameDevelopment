using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konquer.Classes.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Konquer.Classes.Sprites
{
    public class Enemy : Sprite
    {
        Animation animationPlayer;
        Spritesheet demonAnimation;

        private SpriteBatch enemySpriteBatch;
        public Vector2 Movement;
        public Vector2 Origin;
        public Vector2 EnemyPosition;

        public float Rotation = 0f;

        public bool Right;
        public float Distance;
        public float OldDistance;
        public float PlayerDistanceX;
        public float PlayerDistanceY;

        public void Load(ContentManager Content)
        {
            demonAnimation = new Spritesheet(Content.Load<Texture2D>("demon-idle"), 160, 0.3f, true);
            animationPlayer.PlayAnimation(demonAnimation);
        }

        public Enemy(Texture2D texture, Vector2 position, SpriteBatch spriteBatch, float newDistance) : base(texture, position, spriteBatch)
        {
            enemySpriteBatch = spriteBatch;
            EnemyPosition = position;
            Distance = newDistance;
            OldDistance = Distance;
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            EnemyPosition += Movement;
            Origin = new Vector2(demonAnimation.FrameWidth / 2, demonAnimation.FrameHeight / 2);

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

            PlayerDistanceX = playerPosition.X - EnemyPosition.X;

            if (PlayerDistanceX >= -2000 && PlayerDistanceX <= 2000)
            {
                if (PlayerDistanceX < -1)
                    Movement.X = -1f;
                else if (PlayerDistanceX > 1)
                    Movement.X = 1f;
                else if (PlayerDistanceX == 0)
                    Movement.X = 0f;
            }

            PlayerDistanceY = playerPosition.Y - EnemyPosition.Y;

            if (PlayerDistanceY >= -2000 && PlayerDistanceY <= 2000)
            {
                if (PlayerDistanceY < -1)
                    Movement.Y = -1f;
                else if (PlayerDistanceY > 1)
                    Movement.Y = 1f;
                else if (PlayerDistanceY == 0)
                    Movement.Y = 0f;
            }
        }

        private void UpdatePosition(GameTime gameTime)
        {
            Position += Movement * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 15;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteEffects flip = SpriteEffects.None;

            if (Movement.X >= 0)
                flip = SpriteEffects.None;
            else if (Movement.X < 0)
                flip = SpriteEffects.FlipHorizontally;

            animationPlayer.Draw(gameTime, enemySpriteBatch, EnemyPosition, flip);
        }
    }
}
