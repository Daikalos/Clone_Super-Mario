using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class Enemy : DynamicObject
    {
        protected enum EnemyState
        {
            isActing,
            isFalling,
            isDead
        }

        protected EnemyState myEnemyState;
        protected bool myIsAlive;

        public bool IsAlive
        {
            get => myIsAlive;
            set => myIsAlive = value;
        }

        public Enemy(Vector2 aPosition, Point aSize, float aSpeed, float aGravity) : base(aPosition, aSize, aSpeed, aGravity)
        {

        }

        public void Update(GameTime aGameTime)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            switch (myEnemyState)
            {
                case EnemyState.isActing:
                    Behaviour(aGameTime);
                    break;
                case EnemyState.isFalling:
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
            }

            CollisionBlock(aGameTime);
            IsFalling();
        }

        public virtual void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {

        }

        protected virtual void Behaviour(GameTime aGameTime)
        {

        }
        protected void CollisionBlock(GameTime aGameTime)
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                float tempVelocity = (myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds);

                if (tile.TileType == '#')
                {
                    if (CollisionManager.CheckBelow(myBoundingBox, tile.BoundingBox, tempVelocity) && myVelocity > 0)
                    {
                        myVelocity = 0.0f;
                        myPosition.Y = tile.Position.Y - mySize.Y;

                        myEnemyState = EnemyState.isActing;
                    }
                }
            }
        }
        protected void IsFalling()
        {
            bool tempFall = true;
            foreach (Tile tile in Level.TilesAround(this))
            {
                Rectangle tempColRectBelow = new Rectangle(myBoundingBox.X, myBoundingBox.Y, myBoundingBox.Width, myBoundingBox.Height + (mySize.Y / 4));
                if (CollisionManager.Collision(tempColRectBelow, tile.BoundingBox))
                {
                    if (tile.TileType == '#')
                    {
                        tempFall = false;
                    }
                }
            }
            if (tempFall)
            {
                myEnemyState = EnemyState.isFalling;
            }
        }
    }

    class Chase : Enemy
    {
        public Chase(Vector2 aPosition, Point aSize, float aSpeed, float aGravity) : base(aPosition, aSize, aSpeed, aGravity)
        {

        }

        protected override void Behaviour(GameTime aGameTime)
        {

        }

        public override void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {

        }
    }

    class Patrol : Enemy
    {
        private AnimationManager myGoombaAnimation;
        private bool myDirection;

        public Patrol(Vector2 aPosition, Point aSize, float aSpeed, float aGravity) : base(aPosition, aSize, aSpeed, aGravity)
        {
            myGoombaAnimation = new AnimationManager(new Point(2, 1), 0.2f, true);
        }

        protected override void Behaviour(GameTime aGameTime)
        {
            switch (myDirection)
            {
                case true:
                    myPosition.X += mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case false:
                    myPosition.X -= mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
            }

            SwitchDirection(aGameTime);
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            myGoombaAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, mySize, new Point(32), Color.White, 0.0f, myOrigin, SpriteEffects.None);
        }

        private void SwitchDirection(GameTime aGameTime)
        {
            bool tempSwitchDirection = false;
            foreach (Tile tile in Level.TilesAround(this))
            {
                float tempSpeed = (mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                Rectangle tempColRectBot = new Rectangle((int)myPosition.X, (int)myPosition.Y + mySize.Y, mySize.X, (mySize.Y / 8));

                if (tile.TileType == '#')
                {
                    if (CollisionManager.CheckLeft(myBoundingBox, tile.BoundingBox, new Vector2(tempSpeed, 0)) && !myDirection)
                    {
                        myPosition.X = tile.BoundingBox.X + tile.Size.X;
                        tempSwitchDirection = true;
                    }
                    if (CollisionManager.CheckRight(myBoundingBox, tile.BoundingBox, new Vector2(tempSpeed, 0)) && myDirection)
                    {
                        myPosition.X = tile.BoundingBox.X - mySize.X;
                        tempSwitchDirection = true;
                    }
                }
                if (tile.TileType == '-')
                {
                    if (CollisionManager.Collision(tempColRectBot, tile.BoundingBox))
                    {
                        tempSwitchDirection = true;
                    }
                }
            }
            if (tempSwitchDirection)
            {
                myDirection = !myDirection;
            }
        }
    }
}
