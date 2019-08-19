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
    // De Background klasse voorziet de basis voor een geanimeerde achtergrond. Deze wordt in dit project geanimeerd door over de verschillende frames 
    // (aparte bestanden in het geval van de achtergrond) te itereren wanneer deze in de lijst zijn ingeladen.
    public class Background
    {
        private Texture2D _activetexture;
        private double _x = 0;
        List<Texture2D> Backgrounds;
        int counter = 0;

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

            _x += temp;
            if (_x >= _activetexture.Width / 8)
            {
                _x = 0;
                counter++;
                if (counter >= Backgrounds.Count)
                    counter = 0;

                _activetexture = Backgrounds[counter];

            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_activetexture, new Rectangle(0, 0, 1888, 1000), Color.White);
        }
    }
}
