using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class DynamicObject : GameObject
    {
        protected float mySpeed;

        public float Speed
        {
            get => mySpeed;
        }
        
        public DynamicObject(Vector2 aPosition, Point aSize, float aSpeed) : base(aPosition, aSize)
        {
            this.mySpeed = aSpeed;
        }
    }
}
