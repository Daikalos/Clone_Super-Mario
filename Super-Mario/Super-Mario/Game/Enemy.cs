using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class Enemy : DynamicObject
    {
        public Enemy(Vector2 aPosition, Point aSize, float aSpeed, float aGravity) : base(aPosition, aSize, aSpeed, aGravity)
        {

        }
    }
}
