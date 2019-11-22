using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    static class Background
    {
        private static Texture2D myTexture;
        private static Rectangle myBoundingBox;
        private static Vector2 myPosition;
        private static Point mySize;

        public static void Update()
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);
        }

        public static void Parallax(DynamicObject aObject)
        {

        }

        public static void Draw(SpriteBatch aSpriteBatch)
        {
            for (int i = 0; i < (Level.MapSize.X / mySize.X) + 1; i++)
            {
                aSpriteBatch.Draw(myTexture, 
                    new Rectangle((int)myPosition.X + mySize.X * i, (int)myPosition.Y, mySize.X, mySize.Y), 
                    null, Color.White);
            }
        }

        public static void SetTexture(string aName)
        {
            myTexture = ResourceManager.RequestTexture(aName);

            myPosition = Vector2.Zero;
            mySize = new Point(myTexture.Width, myTexture.Height);
        }
    }
}
