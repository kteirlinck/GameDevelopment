using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.InterfaceControls
{
    // De Button klasse voorziet de texture/font van de knoppen en mogelijkheden tot implementatie ervan.
    public class Button : Control
    {
        public Button(ContentManager Content, string newText, Rectangle newRectangle)
        {
            texture = Content.Load<Texture2D>("SoloButton");
            font = Content.Load<SpriteFont>("DebugFont");

            text = newText;

            Rectangle = newRectangle;

            IsVisible = true;
            IsEnabled = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                spriteBatch.Draw(texture, Rectangle, Color.White);

                if (text.Length > 0 && font != null)
                {
                    float x = font.MeasureString(text).X / 2;
                    float y = font.MeasureString(text).Y / 2;

                    Vector2 fPosition = new Vector2((X + (Width / 2)) - x, (Y + (Height / 2)) - y);

                    spriteBatch.DrawString(font, text, fPosition, Color.White);
                }
            }
        }
    }
}
