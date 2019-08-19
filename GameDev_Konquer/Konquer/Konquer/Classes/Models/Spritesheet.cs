using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;


namespace Konquer.Classes.Models
{
    // De Spritesheet klasse biedt de basis voor de beschrijving van specifieke animations. Dit omvat het toewijzen van  het (aantal)frames/afmetingen/loops
        class Spritesheet
        {
            Texture2D _texture;
            public Texture2D Texture
            {
                get { return _texture; }
            }

            public int FrameWidth;

            public int FrameHeight
            {
                get { return _texture.Height; }
            }

            float frameTime;
            public float FrameTime
            {
                get { return frameTime; }
            }

            public int FrameCount;

            bool isLooping;
            public bool IsLooping
            {
                get { return isLooping; }
            }

            public Spritesheet(Texture2D newTexture, int newFrameWidth, float newFrameTime, bool newIsLooping)
            {
                _texture = newTexture;
                FrameWidth = newFrameWidth;
                frameTime = newFrameTime;
                isLooping = newIsLooping;
                FrameCount = _texture.Width / FrameWidth;
            }
        }
    
}
