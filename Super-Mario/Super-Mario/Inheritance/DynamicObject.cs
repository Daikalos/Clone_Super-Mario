using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class DynamicObject : GameObject
    {
        protected Vector2 
            myVelocity,
            myCurrentVelocity,
            myVelocityThreshold;
        protected float myGravity;

        public Vector2 Velocity
        {
            get => myVelocity;
        }
        public Vector2 CurrentVelocity
        {
            get => myCurrentVelocity;
        }
        
        public DynamicObject(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold, float aGravity) : base(aPosition, aSize)
        {
            this.myVelocity = aVelocity; //Speed of object in x, y-axis
            this.myVelocityThreshold = aVelocityThreshold; //Maximum speed in x, y-axis
            this.myGravity = aGravity; //Fall speed
        }

        protected void Gravity(GameTime aGameTime)
        {
            myCurrentVelocity.Y += myGravity;
            myCurrentVelocity.Y = Math.Min(Math.Max(myCurrentVelocity.Y, -myVelocityThreshold.Y), myVelocityThreshold.Y);
            
            myPosition.Y += myCurrentVelocity.Y * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
