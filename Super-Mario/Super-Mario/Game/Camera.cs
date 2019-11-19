﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public static void MoveCamera(GameWindow aWindow, float aNewPosition)
        {
            if (SnapToMap(aWindow, aNewPosition))
            {
                float tempNewPosition = myPosition.X + aNewPosition;
                myPosition.X = tempNewPosition;
            }
        }

        public static void FollowObject(GameWindow aWindow, DynamicObject aObject)
        {
            if (aObject.Position.X - (aWindow.ClientBounds.Width / 8) < myPosition.X)
            {
                MoveCamera(aWindow, -aObject.Speed);
            }
            if (aObject.Position.X + aObject.Size.X + (aWindow.ClientBounds.Width / 4) > myPosition.X + aWindow.ClientBounds.Width)
            {
                MoveCamera(aWindow, aObject.Speed);
            }
        }

        private static bool SnapToMap(GameWindow aWindow, float aNewPosition)
        {
            if (myPosition.X + aNewPosition + aWindow.ClientBounds.Width > Level.MapSize.X)
            {
                myPosition.X = Level.MapSize.X - aWindow.ClientBounds.Width;
                return false;
            }
            if (myPosition.X + aNewPosition < 0)
            {
                myPosition.X = 0;
                return false;
            }
            return true;
        }
    }
}