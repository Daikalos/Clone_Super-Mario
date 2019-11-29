using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    static class Background
    {
        private static Texture2D 
            myTexture;
        private static Rectangle 
            myBoundingBox;
        private static Vector2
            myPosition,
            myScrollPosition;
        private static Point 
            mySize;

        public static void Update()
        {
            myBoundingBox = new Rectangle((int)(myPosition.X + myScrollPosition.X), (int)myPosition.Y, mySize.X, mySize.Y);
        }

        public static void Reset()
        {
            myScrollPosition = Vector2.Zero;
            myBoundingBox.Location = Point.Zero;
        }

        public static void Draw(SpriteBatch aSpriteBatch)
        {
            for (int i = -2; i < (Level.MapSize.X / mySize.X) + 2; i++)
            {
                aSpriteBatch.Draw(myTexture, 
                    new Rectangle(myBoundingBox.X + mySize.X * i, myBoundingBox.Y, myBoundingBox.Width, myBoundingBox.Height), 
                    null, Color.White);
            }
        }

        public static void Scrolling(GameTime aGameTime, float aSpeed)
        {
            myScrollPosition.X += aSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;

            if (myScrollPosition.X > mySize.X)
            {
                myScrollPosition.X = 0;
            }
            if (myScrollPosition.X < -mySize.X)
            {
                myScrollPosition.X = 0;
            }
        }

        public static void MoveBackground(GameWindow aWindow, GameTime aGameTime, float aNewPosition)
        {
            myPosition.X += aNewPosition * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
        }

        public static void SetTexture(string aName)
        {
            myTexture = ResourceManager.RequestTexture(aName);

            myPosition = Vector2.Zero;
            mySize = new Point(myTexture.Width, myTexture.Height);
        }
    }
}
