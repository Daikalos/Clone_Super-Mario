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

        public Vector2 Velocity
        {
            get => myVelocity;
        }
        public Vector2 CurrentVelocity
        {
            get => myCurrentVelocity;
        }
        
        public DynamicObject(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold) : base(aPosition, aSize)
        {
            this.myVelocity = aVelocity; //Speed of object in x, y-axis
            this.myVelocityThreshold = aVelocityThreshold; //Maximum speed in x, y-axis
        }

        protected void Gravity(GameTime aGameTime)
        {
            if (myCurrentVelocity.Y + myVelocity.Y < myVelocityThreshold.Y)
            {
                myCurrentVelocity.Y += myVelocity.Y;
            }
            else
            {
                myCurrentVelocity.Y = myVelocityThreshold.Y;
            }

             myPosition.Y += myCurrentVelocity.Y * (float)aGameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
