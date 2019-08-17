using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.World
{
    public class Background
    {
        List<Texture2D> Backgrounds;


        private Texture2D _activetexture;
        int counter = 0;
        private double x = 0;

        public Background()
        {

        }

        public void SetBG(List<Texture2D> BG)
        {
            Backgrounds = BG;
            _activetexture = Backgrounds[0];
        }

        public void Update(GameTime gameTime)
        {
            double temp = _activetexture.Width * ((double)gameTime.ElapsedGameTime.Milliseconds / 1000);

            x += temp;
            if (x >= _activetexture.Width / 8)
            {
                x = 0;
                counter++;
                if (counter >= Backgrounds.Count)
                    counter = 0;

                _activetexture = Backgrounds[counter];

            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_activetexture, new Rectangle(0, 0, 1888, 1000), Color.White);
            //spriteBatch.Draw(_activetexture, new Rectangle(1280, 0, 1280, 720), Color.White);
        }

    }
}
