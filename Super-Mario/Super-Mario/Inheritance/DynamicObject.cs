using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class DynamicObject : GameObject
    {
        protected static CollisionFlip //Used to flip collision checking when gravity is reversed
            myTopCollision,
            myBotCollision;
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

            this.myPosition += new Vector2(0, Level.TileSize.Y - mySize.Y); //Adjust spawn position after size
        }

        protected void SnapTopCollision(GameObject aObject)
        {
            if (GameInfo.Gravity > 0)
            {
                myPosition.Y = aObject.Position.Y + aObject.Size.Y;
            }
            else
            {
                myPosition.Y = aObject.Position.Y - mySize.Y;
            }
        }
        protected void SnapBotCollision(GameObject aObject)
        {
            if (GameInfo.Gravity > 0)
            {
                myPosition.Y = aObject.Position.Y - mySize.Y;
            }
            else
            {
                myPosition.Y = aObject.Position.Y + aObject.Size.Y;
            }
        }

        protected void Gravity(GameTime aGameTime)
        {
            myCurrentVelocity.Y += GameInfo.Gravity * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
            MathHelper.Clamp(myCurrentVelocity.Y, -myVelocityThreshold.Y, myVelocityThreshold.Y);

            myPosition.Y += myCurrentVelocity.Y * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
        }

        protected delegate bool CollisionFlip(Rectangle aRectangle1, Rectangle aRectangle2, Vector2 aVelocity);
    }
}
