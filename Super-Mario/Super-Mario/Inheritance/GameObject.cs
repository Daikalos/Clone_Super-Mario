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

        public Texture2D Texture
        {
            get => myTexture;
        }
        public Vector2 Position
        {
            get => myPosition;
            set => myPosition = value;
        }
        public Rectangle BoundingBox
        {
            get => myBoundingBox;
            set => myBoundingBox = value;
        }
        public Point Size
        {
            get => mySize;
            set => mySize = value;
        }

        protected GameObject(Vector2 aPosition, Point aSize)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;

            this.myOrigin = Vector2.Zero;
        }

        public virtual void Update()
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);
        }

        public virtual void Draw(SpriteBatch aSpriteBatch) //Override if needed
        {
            aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White);
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
