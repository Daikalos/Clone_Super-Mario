using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    abstract class Button : StaticObject
    {
        protected SpriteFont my8bitFont;
        protected Rectangle myOffset;

        public Button(Vector2 aPosition, Point aSize) : base(aPosition, aSize)
        {
            myOffset = new Rectangle(
                (int)aPosition.X - (aSize.X / 96),
                (int)aPosition.Y - (aSize.Y / 96),
                aSize.X + (aSize.X / 48),
                aSize.Y + (aSize.Y / 48));
        }

        public abstract void Update(MainGame aGame, GameWindow aWindow);

        public abstract override void Draw(SpriteBatch aSpriteBatch);

        protected bool IsClicked()
        {
            return 
                KeyMouseReader.LeftClick() && 
                myBoundingBox.Contains(KeyMouseReader.GetCurrentMouseState.Position);
        }

        protected bool IsHold()
        {
            return myBoundingBox.Contains(KeyMouseReader.GetCurrentMouseState.Position);
        }

        public void LoadContent()
        {
            myTexture = ResourceManager.RequestTexture("Border");
            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
