using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.Sprites
{
    // In de Tile klasse wordt de voorwaarde gemaakt waarmee een solide blok element kan worden geplaatst.
    public class Tile : Sprite
    {
        public bool IsBlocked { get; set; }

        public Tile(Texture2D texture, Vector2 position, SpriteBatch batch, bool isBlocked)
            : base(texture, position, batch)
        {
            IsBlocked = isBlocked;
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsBlocked) { base.Draw(gameTime); }
        }
    }
}
