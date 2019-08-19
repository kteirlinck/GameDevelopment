using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.Models
{
    // De Animation klasse maakt het afspelen van animations mogelijk, waar de Spritesheet klasse eigenschappen voorziet wijst de Animation klasse gedrag toe.
    struct Animation
    {
        Spritesheet _spritesheet;
        Rectangle _drawRectangle;
        public Spritesheet Spritesheet
        {
            get { return _spritesheet; }
        }

        int frameIndex;
        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        private float _timer;
        public Vector2 Origin
        {
            get { return new Vector2(_spritesheet.FrameWidth / 2, _spritesheet.FrameHeight); }
        }

        public void PlayAnimation(Spritesheet newAnimation)
        {
            if (_spritesheet == newAnimation)
                return;

            _spritesheet = newAnimation;
            frameIndex = 0;
            _timer = 0;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (Spritesheet == null)
                throw new NotSupportedException("No animation selected");
            if (_drawRectangle == null)
                new Rectangle(frameIndex * Spritesheet.FrameWidth, 0, Spritesheet.FrameWidth, Spritesheet.FrameHeight);

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_timer >= _spritesheet.FrameTime)
            {
                _timer -= _spritesheet.FrameTime;

                if (_spritesheet.IsLooping)
                    frameIndex = (frameIndex + 1) % _spritesheet.FrameCount;
                else frameIndex = Math.Min(frameIndex + 1, _spritesheet.FrameCount - 1);
            }

            _drawRectangle.X = frameIndex * Spritesheet.FrameWidth;
            _drawRectangle.Y = 0;
            _drawRectangle.Width = Spritesheet.FrameWidth;
            _drawRectangle.Height = Spritesheet.FrameHeight;

            spriteBatch.Draw(Spritesheet.Texture, position, _drawRectangle, Color.White, 0f, Origin, 1f, spriteEffects, 0f);
        }
    }
}
