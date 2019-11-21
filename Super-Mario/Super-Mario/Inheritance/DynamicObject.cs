﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class DynamicObject : GameObject
    {
        protected float 
            mySpeed,
            myVelocity,
            myGravity;

        public float Speed
        {
            get => mySpeed;
        }
        
        public DynamicObject(Vector2 aPosition, Point aSize, float aSpeed, float aGravity) : base(aPosition, aSize)
        {
            this.mySpeed = aSpeed;
            this.myGravity = aGravity;
        }
    }
}
