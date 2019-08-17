using Konquer.Classes.InterfaceControls;
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
    // De MenuScreen klasse voorziet de implementatie van Play/Quit knoppen en tekent deze.
    //public class MenuScreen : BaseScreen
    //{
    //    Button btnPlay;
    //    Button btnQuit;

    //    List<Control> Controls = new List<Control>();

    //    MouseState presentMouse;

    //    public MenuScreen(Game1 game)
    //    {
    //        int centre = game.ScreenWidth / 2;

    //        btnPlay = new Button(game.Content, "Play", new Rectangle(centre - 50, 100, 100, 35));
    //        btnQuit = new Button(game.Content, "Quit", new Rectangle(centre - 50, 150, 100, 35));

    //        Controls.Add(btnPlay);
    //        Controls.Add(btnQuit);
    //    }

    //    public override void Update(Game1 game)
    //    {
    //        presentMouse = Mouse.GetState();

    //        foreach (Control control in Controls)
    //        {
    //            control.Update(presentMouse);
    //        }

    //        if (btnPlay.IsLeftClicked)
    //        {
    //            this.IsActive = false;
    //            game.GameScreen.IsActive = true;
    //        }

    //        if (btnQuit.IsLeftClicked)
    //        {
    //            game.Exit();
    //        }
    //    }

    //    public override void Draw(SpriteBatch spriteBatch)
    //    {
    //        spriteBatch.Begin();

    //        foreach (Control control in Controls)
    //        {
    //            control.Draw(spriteBatch);
    //        }

    //        spriteBatch.End();
    //    }
    //}
}
