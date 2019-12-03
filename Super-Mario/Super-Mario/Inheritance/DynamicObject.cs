using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class DynamicObject : GameObject
    {
        protected float 
            mySpeed,
            myCurrentSpeed,
            myVelocity,
            myVelocityThreshold,
            myGravity;

        public float CurrentSpeed
        {
            get => myCurrentSpeed;
        }
        
        public DynamicObject(Vector2 aPosition, Point aSize, float aSpeed, float aGravity, float aVelocityThreshold) : base(aPosition, aSize)
        {
            this.mySpeed = aSpeed;
            this.myGravity = aGravity;
            this.myVelocityThreshold = aVelocityThreshold;
        }
    }
}
