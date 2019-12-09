using System;
using Microsoft.Xna.Framework;

namespace Super_Mario
{
    static class Camera
    {
        private static Vector2 myPosition;

        public static Vector2 Position
        {
            get => myPosition;
            set => myPosition = value;
        }
        public static Matrix TranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-myPosition, 0));
            }
        }

        public static void Reset()
        {
            myPosition = Vector2.Zero;
        }

        public static void MoveCamera(GameWindow aWindow, GameTime aGameTime, float aNewPosition)
        {
            if (SnapToMap(aWindow, aNewPosition))
            {
                Background.MoveBackground(aGameTime, -(aNewPosition / 12));
                myPosition.X += aNewPosition;
            }
        }

        public static void FollowObject(GameWindow aWindow, GameTime aGameTime, DynamicObject aObject)
        {
            if (aObject.Position.X - (aWindow.ClientBounds.Width / 8) < myPosition.X)
            {
                MoveCamera(aWindow, aGameTime, -Math.Abs(aObject.CurrentVelocity.X));
            }
            if (aObject.Position.X + aObject.Size.X + (aWindow.ClientBounds.Width / 4) > myPosition.X + aWindow.ClientBounds.Width)
            {
                MoveCamera(aWindow, aGameTime, Math.Abs(aObject.CurrentVelocity.X));
            }

            if (aObject.Position.X < myPosition.X)
            {
                MoveCamera(aWindow, aGameTime, -aObject.Velocity.X * 3);
            }
            if (aObject.Position.X + aObject.Size.X > myPosition.X + aWindow.ClientBounds.Width)
            {
                MoveCamera(aWindow, aGameTime, aObject.Velocity.X * 3);
            }
        }

        private static bool SnapToMap(GameWindow aWindow, float aNewPosition)
        {
            if (myPosition.X + aNewPosition + aWindow.ClientBounds.Width >= Level.MapSize.X)
            {
                myPosition.X = Level.MapSize.X - aWindow.ClientBounds.Width;
                return false;
            }
            if (myPosition.X + aNewPosition <= 0)
            {
                myPosition.X = 0;
                return false;
            }
            return true;
        }
    }
}
