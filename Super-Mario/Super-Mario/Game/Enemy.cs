using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    abstract class Enemy : DynamicObject
    {
        protected enum EnemyState
        {
            isActing,
            isFalling,
            isDead
        }

        protected EnemyState myEnemyState;
        protected bool 
            myIsAlive,
            myHasCollided;
        protected float myIsDeadDelay;

        public bool IsAlive
        {
            get => myIsAlive;
            set
            {
                myEnemyState = EnemyState.isDead;
                myTexture = ResourceManager.RequestTexture("Goomba_Death");
            }
        }
        public bool HasCollided
        {
            get => myHasCollided;
            set => myHasCollided = value;
        }

        public Enemy(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold) : base(aPosition, aSize, aVelocity, aVelocityThreshold)
        {
            this.myIsAlive = true;
            this.myHasCollided = false;
            this.myIsDeadDelay = 0.25f; //Fixed value
            this.myEnemyState = EnemyState.isFalling;
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
                    Gravity(aGameTime);
                    break;
                case EnemyState.isDead:
                    if (myIsDeadDelay > 0)
                    {
                        myIsDeadDelay -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        myIsAlive = false;
                    }
                    break;
            }

            CollisionBlock(aGameTime);
            IsFalling();
        }

        public abstract void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime);

        protected abstract void Behaviour(GameTime aGameTime);

        protected void CollisionBlock(GameTime aGameTime)
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckBelow(myBoundingBox, tile.BoundingBox, myVelocity.Y) && myVelocity.Y > 0)
                    {
                        myVelocity.Y = 0.0f;
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
                    if (tile.IsBlock)
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
        public Chase(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold) : base(aPosition, aSize, aVelocity, aVelocityThreshold)
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

        public Patrol(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold) : base(aPosition, aSize, aVelocity, aVelocityThreshold)
        {
            myGoombaAnimation = new AnimationManager(new Point(2, 1), 0.2f, true);
        }

        protected override void Behaviour(GameTime aGameTime)
        {
            myCurrentVelocity.X = myVelocity.X * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
            switch (myDirection)
            {
                case true:
                    myPosition.X += myCurrentVelocity.X;
                    break;
                case false:
                    myPosition.X -= myCurrentVelocity.X;
                    break;
            }

            SwitchDirection();
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myEnemyState)
            {
                case EnemyState.isDead:
                    aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White);
                    break;
                default:
                    myGoombaAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myBoundingBox, new Point(32), Color.White, 0.0f, myOrigin, SpriteEffects.None);
                    break;
            }
        }

        private void SwitchDirection()
        {
            bool tempSwitchDirection = false;
            foreach (Tile tile in Level.TilesAround(this))
            {
                Rectangle tempColRectBot = new Rectangle((int)myPosition.X, (int)myPosition.Y + mySize.Y, mySize.X, (mySize.Y / 8));

                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckLeft(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && !myDirection)
                    {
                        myPosition.X = tile.BoundingBox.X + tile.Size.X;
                        tempSwitchDirection = true;
                    }
                    if (CollisionManager.CheckRight(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && myDirection)
                    {
                        myPosition.X = tile.BoundingBox.X - mySize.X;
                        tempSwitchDirection = true;
                    }
                }
                else
                {
                    if (CollisionManager.Collision(tempColRectBot, tile.BoundingBox))
                    {
                        tempSwitchDirection = true;
                    }
                }
                if (myPosition.X - myCurrentVelocity.X < 0)
                {
                    tempSwitchDirection = true;
                }
                if (myPosition.X + mySize.X + myCurrentVelocity.X > Level.MapSize.X)
                {
                    tempSwitchDirection = true;
                }

                if (tempSwitchDirection)
                {
                    break;
                }
            }
            if (tempSwitchDirection)
            {
                myDirection = !myDirection;
            }
        }
    }
}
