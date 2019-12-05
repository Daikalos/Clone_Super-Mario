using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class Fireball : DynamicObject
    {
        public Fireball(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold, float aGravity) : base(aPosition, aSize, aVelocity, aVelocityThreshold, aGravity)
        {

        }
    }
}
