
using Konquer.Classes.Models;
using Konquer.Classes.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.Sprites
{
    // In de Player klasse wordt gameplay input verwerkt en positie/beweging berekend.
    public class Player : Sprite
    {
        Animation animationPlayer;
        Spritesheet runAnimation;
        Spritesheet idleAnimation;

        private SpriteBatch playerSpriteBatch;
        private Vector2 pastPosition;
        private long lastJumpTimeMillis { get; set; }
        private bool doubleJumpActivatable { get; set; }
        public Vector2 Movement { get; set; }

        public void Load(ContentManager Content)
        {
            idleAnimation = new Spritesheet(Content.Load<Texture2D>("hero/gothic-hero-idle"), 38, 0.3f, true);
            runAnimation = new Spritesheet(Content.Load<Texture2D>("hero/gothic-hero-run"), 66, 0.1f, true);
            animationPlayer.PlayAnimation(idleAnimation);
        }

        public bool IsGrounded()
        {
            Rectangle onePixelBelow = Bounds;
            onePixelBelow.Offset(0, 1);
            return !Board.CurrentBoard.HasRoomForRectangle(onePixelBelow);
        }

        public bool CanDoubleJump()
        {
            return ((DateTime.Now.Ticks / 1000) - lastJumpTimeMillis > 800) && doubleJumpActivatable;
        }

        public Player(Texture2D texture, Vector2 position, SpriteBatch spriteBatch)
            : base(texture, position, spriteBatch)
        {
            this.playerSpriteBatch = spriteBatch;
        }

        public void Update(GameTime gameTime)
        {
            CaptureInputUpdateMovement();
            ApplyNewton();
            SimulateFriction();
            MoveIfOk(gameTime);
            StopWhenBlocked();
        }

        private void ApplyNewton()
        {
            Movement += Vector2.UnitY * .9f;
        }

        private void MoveIfOk(GameTime gameTime)
        {
            pastPosition = Position;
            UpdatePosition(gameTime);
            Position = Board.CurrentBoard.AllowedMovement(pastPosition, Position, Bounds);
        }

        private void StopWhenBlocked()
        {
            Vector2 lastMovement = Position - pastPosition;
            if(lastMovement.X == 0) { Movement *= Vector2.UnitY; }
            if (lastMovement.Y == 0) { Movement *= Vector2.UnitX; }
        }

        private void CaptureInputUpdateMovement()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left)) { Movement += new Vector2(-1, 0); }
            if (keyboardState.IsKeyDown(Keys.Right)) { Movement += new Vector2(1, 0); }
            if (keyboardState.IsKeyDown(Keys.Space) && CanDoubleJump()) { Movement = -Vector2.UnitY * 55; doubleJumpActivatable = false; }
            if (keyboardState.IsKeyDown(Keys.Space) && IsGrounded()) { lastJumpTimeMillis = DateTime.Now.Ticks / 1000; Movement = -Vector2.UnitY * 55; doubleJumpActivatable = true; }

            if (Movement.X == 0)
                animationPlayer.PlayAnimation(idleAnimation);
            else if (Movement.X != 0)
                animationPlayer.PlayAnimation(runAnimation);

        }

        private void SimulateFriction()
        {
            if (IsGrounded()) { Movement -= Movement * Vector2.One * .3f; }
            else { Movement -= Movement * Vector2.One * .05f; }
        }

        private void UpdatePosition(GameTime gameTime)
        {
            Position += Movement * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 15;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteEffects flip = SpriteEffects.None;

            if (Movement.X >= 0)
                flip = SpriteEffects.None;
            else if (Movement.X < 0)
                flip = SpriteEffects.FlipHorizontally;

            Vector2 tempPos = Position;

            tempPos.Y += 40;
            tempPos.X += 16;

            animationPlayer.Draw(gameTime, playerSpriteBatch, tempPos, flip);
        }
    }
}
