using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konquer.Classes.Models
{
    // De MovementWrapper struct legt de basis waarmee de variabelen voor beweging/collision geïnitialiseerd kunnen worden
    // en ermee gerekend kan worden.
    public struct MovementWrapper
    {
        public Vector2 MovementAttempt { get; set; }
        public Vector2 PlausibleCurrentPosition { get; set; }
        public int NumberOfStepsToBreakMovementInto { get; private set; }
        public bool IsDiagonalMove { get; private set; }
        public Vector2 OneStep { get; private set; }
        public Rectangle BoundingRectangle { get; set; }

        public MovementWrapper(Vector2 originalPosition, Vector2 destination, Rectangle boundingRectangle) : this()
        {
            MovementAttempt = destination - originalPosition;
            PlausibleCurrentPosition = originalPosition;
            NumberOfStepsToBreakMovementInto = (int)(MovementAttempt.Length() * 0.1) + 1;
            OneStep = MovementAttempt / NumberOfStepsToBreakMovementInto;
            IsDiagonalMove = MovementAttempt.X != 0 && MovementAttempt.Y != 0;
            OneStep = MovementAttempt / NumberOfStepsToBreakMovementInto;
            BoundingRectangle = boundingRectangle;
        }
    }
}
