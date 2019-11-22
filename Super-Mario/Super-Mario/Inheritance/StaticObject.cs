using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class StaticObject : GameObject
    {
        public StaticObject(Vector2 aPosition, Point aSize) : base(aPosition, aSize)
        {
            myBoundingBox = new Rectangle((int)aPosition.X, (int)aPosition.Y, aSize.X, aSize.Y);
        }
    }
}
