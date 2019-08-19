using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konquer.Classes.InterfaceControls;
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
        private Random rand;
        private Rectangle CollisionRectangle;

        public Vector2 Movement;
        public Vector2 Origin;
        public Vector2 EnemyPosition;

        public float Rotation = 0f;

        public bool Right;
        public float Distance;
        public float OldDistance;
        public float PlayerDistanceX;
        public float PlayerDistanceY;

        public long ExpiredTimeSeconds;
        public long StartTimeSeconds;
        public long ExpireThreshold;
        public bool HasExpired;
        public int RespawnTime;



        public virtual void Load(ContentManager Content)
        {
            demonAnimation = new Spritesheet(Content.Load<Texture2D>("demon-idle"), 160, 0.3f, true);
            animationPlayer.PlayAnimation(demonAnimation);

            CollisionRectangle = new Rectangle((int)Position.X, (int)Position.Y, demonAnimation.FrameWidth, demonAnimation.FrameHeight);
        }

        public Enemy(Texture2D texture, Vector2 position, SpriteBatch spriteBatch, float newDistance) : base(texture, position, spriteBatch)
        {
            enemySpriteBatch = spriteBatch;
            EnemyPosition = position;
            Distance = newDistance;
            OldDistance = Distance;
            
            rand = new Random();

            ExpiredTimeSeconds = 0;
            StartTimeSeconds = DateTime.Now.Ticks / 1000 / 10000;
            ExpireThreshold = 10;
            RespawnTime = 5;
            HasExpired = false;
        }

        public virtual void Update(GameTime gameTime, Player player)
        {
            CheckExpireThresholdReached();

            if (HasExpired)
                return;

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

            PlayerDistanceX = player.Position.X - EnemyPosition.X + player.Bounds.Width;

            if (PlayerDistanceX >= -2000 && PlayerDistanceX <= 2000)
            {
                if (PlayerDistanceX < -1)
                    Movement.X = -0.5f;
                else if (PlayerDistanceX > 1)
                    Movement.X = 0.5f;
                else if (PlayerDistanceX == 0)
                    Movement.X = 0f;
            }

            PlayerDistanceY = player.Position.Y - EnemyPosition.Y + player.Bounds.Height;

            if (PlayerDistanceY >= -2000 && PlayerDistanceY <= 2000)
            {
                if (PlayerDistanceY < -1)
                    Movement.Y = -0.5f;
                else if (PlayerDistanceY > 1)
                    Movement.Y = 0.5f;
                else if (PlayerDistanceY == 0)
                    Movement.Y = 0f;
            }
            //checkPlayerCollision(player);
        }

        public void CheckExpireThresholdReached() {
            ExpiredTimeSeconds = DateTime.Now.Ticks / 1000 / 10000;
            if (ExpiredTimeSeconds - StartTimeSeconds > ExpireThreshold) {
                HasExpired = true;
            }

            if (HasExpired && ExpiredTimeSeconds - StartTimeSeconds > (ExpireThreshold + RespawnTime)) {
                //float posX = Position.X < Konquer.ScreenWidth / 2 ? Konquer.ScreenWidth - Position.X : Position.X - Konquer.ScreenWidth;
                //float posY = Position.Y < Konquer.ScreenHeight / 2 ? Konquer.ScreenHeight - Position.Y : Position.Y - Konquer.ScreenHeight;
                //Position = new Vector2(100, 100);
                
                StartTimeSeconds = ExpiredTimeSeconds;
                HasExpired = false;
            }
        }

        //private void checkPlayerCollision(Player player) {
        //    CollisionRectangle.X = (int)Position.X;
        //    CollisionRectangle.Y = (int)Position.Y;

        //    if (player.Bounds.Intersects(CollisionRectangle)) {
        //        GameController.Instance.FinishLevel();
        //    }
        //}

        public void UpdatePosition(GameTime gameTime)
        {
            Position += Movement * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 15;
        }

        public override void Draw(GameTime gameTime)
        {
            if (HasExpired)
                return;

            SpriteEffects flip = SpriteEffects.None;

            if (Movement.X >= 0)
                flip = SpriteEffects.None;
            else if (Movement.X < 0)
                flip = SpriteEffects.FlipHorizontally;

            Vector2 tempPos = EnemyPosition;

            tempPos.Y += 40;
            tempPos.X += 16;

            animationPlayer.Draw(gameTime, enemySpriteBatch, tempPos, flip);
        }
    }
}
