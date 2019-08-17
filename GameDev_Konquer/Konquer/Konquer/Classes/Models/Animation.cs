using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.Models
{
    // De AnimationPlayer klasse maakt het afspelen van animations mogelijk, waar de Animation een object is wijst de AnimationPlayer gedrag toe aan animations.
    struct Animation
    {
        Spritesheet spritesheet;
        Rectangle drawRectangle;
        public Spritesheet Spritesheet
        {
            get { return spritesheet; }
        }

        int frameIndex;
        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        private float timer;
        public Vector2 Origin
        {
            get { return new Vector2(spritesheet.FrameWidth / 2, spritesheet.FrameHeight); }
        }

        public void PlayAnimation(Spritesheet newAnimation)
        {
            if (spritesheet == newAnimation)
                return;

            spritesheet = newAnimation;
            frameIndex = 0;
            timer = 0;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (Spritesheet == null)
                throw new NotSupportedException("No animation selected");
            if (drawRectangle == null)
                new Rectangle(frameIndex * Spritesheet.FrameWidth, 0, Spritesheet.FrameWidth, Spritesheet.FrameHeight);

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (timer >= spritesheet.FrameTime)
            {
                timer -= spritesheet.FrameTime;

                if (spritesheet.IsLooping)
                    frameIndex = (frameIndex + 1) % spritesheet.FrameCount;
                else frameIndex = Math.Min(frameIndex + 1, spritesheet.FrameCount - 1);
            }

            drawRectangle.X = frameIndex * Spritesheet.FrameWidth;
            drawRectangle.Y = 0;
            drawRectangle.Width = Spritesheet.FrameWidth;
            drawRectangle.Height = Spritesheet.FrameHeight;

            spriteBatch.Draw(Spritesheet.Texture, position, drawRectangle, Color.White, 0f, Origin, 1f, spriteEffects, 0f);
        }
    }
}
