using Konquer.Classes.InterfaceControls;
using Konquer.Classes.Sprites;
using Konquer.Classes.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.Screens
{
    // De GameScreen klasse dient elementen in te laden en de game te tekenen tijdens speelfase.
    //public class GameScreen : BaseScreen
    //{
    //    private SpriteBatch _spriteBatch;
    //    private Texture2D _tileTexture, _playerTexture;
    //    private Player _player;
    //    private Board _board;
    //    private SpriteFont _debugFont;

    //    public GameScreen(Game1 game)
    //    {
    //        _spriteBatch = new SpriteBatch(GraphicsDevice);
    //        _tileTexture = Content.Load<Texture2D>("Tiles/L2/BlockA1");
    //        _playerTexture = Content.Load<Texture2D>("Tiles/L2/Platform");

    //        _player = new Player(_playerTexture, new Vector2(50, 50), _spriteBatch);
    //        _board = new Board(_spriteBatch, _tileTexture, 59, 25);

    //        _debugFont = Content.Load<SpriteFont>("DebugFont");
    //    }

    //    public override void Update(Game1 game)
    //    {
    //        _player.Update();
    //    }

    //    public override void Draw(SpriteBatch spriteBatch)
    //    {
    //        spriteBatch.Begin();
    //        _player.Draw(spriteBatch);
    //        spriteBatch.End();
    //    }
    //}
}
