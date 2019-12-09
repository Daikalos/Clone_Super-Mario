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

        protected void ChangeState(EnemyState aNewState)
        {
            if (myEnemyState != EnemyState.isDead)
            {
                myEnemyState = aNewState;
            }
        }

        protected EnemyState myEnemyState;
        protected SpriteEffects
            myFlipVertical,
            myFlipHorizontal,
            myFlipSprite;
        protected bool
            myIsAlive,
            myHasCollided;
        protected float myIsDeadDelay;
        protected string myEnemyType;

        public bool IsAlive
        {
            get => myIsAlive;
            set
            {
                myEnemyState = EnemyState.isDead;
                myTexture = ResourceManager.RequestTexture(myEnemyType + "_Death");
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
            base.Update();

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

            IsFalling();
            CollisionBlock();
            OutsideBounds();

            FlipState();
        }

        public abstract void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime);

        protected abstract void Behaviour(GameTime aGameTime);

        protected void IsFalling()
        {
            bool tempFall = true;
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (GameInfo.Gravity > 0)
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
                else
                {
                    Rectangle tempColRectAbove = new Rectangle(myBoundingBox.X, myBoundingBox.Y - (mySize.Y / 4), myBoundingBox.Width, myBoundingBox.Height);
                    if (CollisionManager.Collision(tempColRectAbove, tile.BoundingBox))
                    {
                        if (tile.IsBlock)
                        {
                            tempFall = false;
                        }
                    }
                }
            }

            if (tempFall)
            {
                ChangeState(EnemyState.isFalling);
            }
        }
        protected void CollisionBlock()
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (myBotCollision(this, tile, myCurrentVelocity))
                    {
                        myCurrentVelocity.Y = 0.0f;
                        SnapBotCollision(tile);

                        ChangeState(EnemyState.isActing);
                    }
                }
            }
        }

        private void OutsideBounds()
        {
            if (myPosition.Y > Level.MapSize.Y || myPosition.Y + mySize.Y < -Level.MapSize.Y)
            {
                myIsAlive = false;
            }
        }

        protected virtual void FlipState()
        {
            if (GameInfo.Gravity > 0)
            {
                myFlipVertical = SpriteEffects.None;
            }
            else
            {
                myFlipVertical = SpriteEffects.FlipVertically;
            }

            myFlipSprite = myFlipVertical;
        }
    }

    class Chase : Enemy
    {
        private AnimationManager myKoopaAnimation;
        private bool myDirection;

        public Chase(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold) : base(aPosition, aSize, aVelocity, aVelocityThreshold)
        {
            this.myKoopaAnimation = new AnimationManager(new Point(2, 1), 0.2f, true);
            this.myEnemyType = "Koopa";
        }

        protected override void Behaviour(GameTime aGameTime)
        {
            switch (myDirection)
            {
                case true:
                    myCurrentVelocity.X = -(myVelocity.X * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                    break;
                case false:
                    myCurrentVelocity.X = (myVelocity.X * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                    break;
            }

            if (CanMoveHorizontally())
            {
                myPosition.X += myCurrentVelocity.X;
            }

            SwitchDirection();
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myEnemyState)
            {
                case EnemyState.isDead:
                    aSpriteBatch.Draw(myTexture, DestRect, null, Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
                    break;
                default:
                    myKoopaAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, DestRect, new Point(32, 64), Color.White, 0.0f, myOrigin, myFlipSprite);
                    break;
            }
        }

        private void SwitchDirection()
        {
            bool tempSwitchDirection = false;
            foreach (Tile tile in Level.TilesAround(this))
            {
                Rectangle tempColRect = Rectangle.Empty;
                if (GameInfo.Gravity > 0)
                {
                    switch (myDirection)
                    {
                        case true:
                            tempColRect = new Rectangle((int)myPosition.X, (int)myPosition.Y + mySize.Y, 1, (mySize.Y / 8));
                            break;
                        case false:
                            tempColRect = new Rectangle((int)myPosition.X + (mySize.X) - 1, (int)myPosition.Y + mySize.Y, 1, (mySize.Y / 8));
                            break;
                    }
                }
                else
                {
                    switch (myDirection)
                    {
                        case true:
                            tempColRect = new Rectangle((int)myPosition.X, (int)myPosition.Y - (mySize.Y / 8), 1, (mySize.Y / 8));
                            break;
                        case false:
                            tempColRect = new Rectangle((int)myPosition.X + (mySize.X) - 1, (int)myPosition.Y - (mySize.Y / 8), 1, (mySize.Y / 8));
                            break;
                    }
                }

                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckLeft(this, tile, myCurrentVelocity) && myDirection)
                    {
                        tempSwitchDirection = true;
                    }
                    if (CollisionManager.CheckRight(this, tile, myCurrentVelocity) && !myDirection)
                    {
                        tempSwitchDirection = true;
                    }
                }
                else
                {
                    if (CollisionManager.Collision(tempColRect, tile.BoundingBox))
                    {
                        tempSwitchDirection = true;
                    }
                }

                if (myPosition.X - myCurrentVelocity.X < 0 && myDirection)
                {
                    tempSwitchDirection = true;
                }
                if (myPosition.X + mySize.X + myCurrentVelocity.X > Level.MapSize.X && !myDirection)
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

        private bool CanMoveHorizontally()
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckLeft(this, tile, myCurrentVelocity))
                    {
                        myPosition.X = tile.BoundingBox.X + tile.Size.X;
                        return false;
                    }
                    if (CollisionManager.CheckRight(this, tile, myCurrentVelocity))
                    {
                        myPosition.X = tile.BoundingBox.X - mySize.X;
                        return false;
                    }
                }
            }
            return true;
        }

        protected override void FlipState()
        {
            if (!myDirection)
            {
                myFlipHorizontal = SpriteEffects.FlipHorizontally;
            }
            if (myDirection)
            {
                myFlipHorizontal = SpriteEffects.None;
            }

            if (GameInfo.Gravity > 0)
            {
                myFlipVertical = SpriteEffects.None;
            }
            else
            {
                myFlipVertical = SpriteEffects.FlipVertically;
            }

            myFlipSprite = myFlipVertical | myFlipHorizontal;
        }
    }

    class Patrol : Enemy
    {
        private AnimationManager myGoombaAnimation;
        private bool myDirection;

        public Patrol(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold) : base(aPosition, aSize, aVelocity, aVelocityThreshold)
        {
            this.myGoombaAnimation = new AnimationManager(new Point(2, 1), 0.2f, true);
            this.myEnemyType = "Goomba";
        }

        protected override void Behaviour(GameTime aGameTime)
        {
            switch (myDirection)
            {
                case true:
                    myCurrentVelocity.X = -(myVelocity.X * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                    break;
                case false:
                    myCurrentVelocity.X = (myVelocity.X * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                    break;
            }

            if (CanMoveHorizontally())
            {
                myPosition.X += myCurrentVelocity.X;
            }

            SwitchDirection();
        }

        public override void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myEnemyState)
            {
                case EnemyState.isDead:
                    aSpriteBatch.Draw(myTexture, DestRect, null, Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
                    break;
                default:
                    myGoombaAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, DestRect, new Point(32), Color.White, 0.0f, myOrigin, myFlipSprite);
                    break;
            }
        }

        private void SwitchDirection()
        {
            bool tempSwitchDirection = false;
            foreach (Tile tile in Level.TilesAround(this))
            {
                Rectangle tempColRect = Rectangle.Empty;
                if (GameInfo.Gravity > 0)
                {
                    switch (myDirection)
                    {
                        case true:
                            tempColRect = new Rectangle((int)myPosition.X, (int)myPosition.Y + mySize.Y, 1, (mySize.Y / 8));
                            break;
                        case false:
                            tempColRect = new Rectangle((int)myPosition.X + (mySize.X) - 1, (int)myPosition.Y + mySize.Y, 1, (mySize.Y / 8));
                            break;
                    }
                }
                else
                {
                    switch (myDirection)
                    {
                        case true:
                            tempColRect = new Rectangle((int)myPosition.X, (int)myPosition.Y - (mySize.Y / 8), 1, (mySize.Y / 8));
                            break;
                        case false:
                            tempColRect = new Rectangle((int)myPosition.X + (mySize.X) - 1, (int)myPosition.Y - (mySize.Y / 8), 1, (mySize.Y / 8));
                            break;
                    }
                }

                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckLeft(this, tile, myCurrentVelocity) && myDirection)
                    {
                        tempSwitchDirection = true;
                    }
                    if (CollisionManager.CheckRight(this, tile, myCurrentVelocity) && !myDirection)
                    {
                        tempSwitchDirection = true;
                    }
                }
                else
                {
                    if (CollisionManager.Collision(tempColRect, tile.BoundingBox))
                    {
                        tempSwitchDirection = true;
                    }
                }

                if (myPosition.X - myCurrentVelocity.X < 0 && myDirection)
                {
                    tempSwitchDirection = true;
                }
                if (myPosition.X + mySize.X + myCurrentVelocity.X > Level.MapSize.X && !myDirection)
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

        private bool CanMoveHorizontally()
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckLeft(this, tile, myCurrentVelocity))
                    {
                        myPosition.X = tile.BoundingBox.X + tile.Size.X;
                        return false;
                    }
                    if (CollisionManager.CheckRight(this, tile, myCurrentVelocity))
                    {
                        myPosition.X = tile.BoundingBox.X - mySize.X;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
