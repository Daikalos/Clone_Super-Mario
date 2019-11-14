using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class GameObject
    {
        protected Texture2D myTexture;
        protected Vector2
            myPosition,
            myOrigin;
        protected Rectangle myBoundingBox;
        protected Point mySize;

        protected GameObject(Vector2 aPosition, Point aSize)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;

            this.myOrigin = Vector2.Zero;
        }

        public virtual void Draw(SpriteBatch aSpriteBatch) //Override if needed
        {
            aSpriteBatch.Draw(myTexture, myPosition, null, Color.White);
        }

        public virtual void SetTexture(string aName)
        {
            myTexture = ResourceManager.RequestTexture(aName);
        }

        public void SetOrigin(Point aFrameSize)
        {
            myOrigin = new Vector2(myTexture.Width / 2 / aFrameSize.X, myTexture.Height / 2 / aFrameSize.Y);
        }
    }
}
