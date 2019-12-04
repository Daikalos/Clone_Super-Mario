using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    class Player : DynamicObject
    {
        enum PlayerState
        {
            isWalking,
            isClimbing,
            isFalling,
            isDead
        }

        private AnimationManager 
            myWalkingAnimation,
            myClimbingAnimation;
        private PlayerState myPlayerState;
        private SpriteEffects myFlipSprite;
        private bool
            myIsAlive,
            myIsMoving,
            myIsClimbing,
            myIsInvincible,
            myDrawSprite;
        private float 
            myJumpHeight,
            myInvincibleTimer,
            myInvincibleDelay,
            myDrawPlayerTimer,
            myDrawPlayerDelay;
        private int myLives;

        public int Lives
        {
            get => myLives;
        }
        public bool IsAlive
        {
            get => myIsAlive;
        }

        public Player(Vector2 aPosition, Point aSize, float aSpeed, float aGravity, float aVelocityThreshold, int someLives, float aJumpHeight, float aInvincibleDelay) : base(aPosition, aSize, aSpeed, aGravity, aVelocityThreshold)
        {
            this.myLives = someLives;
            this.myJumpHeight = aJumpHeight;
            this.myInvincibleDelay = aInvincibleDelay;

            this.myWalkingAnimation = new AnimationManager(new Point(3, 1), 0.1f, true);
            this.myClimbingAnimation = new AnimationManager(new Point(2, 1), 0.3f, true);
            this.myPosition += new Vector2(0, Level.TileSize.Y - mySize.Y);
            this.myPlayerState = PlayerState.isWalking;
            this.myFlipSprite = SpriteEffects.None;
            this.myIsAlive = true;
            this.myDrawSprite = true;
            this.myDrawPlayerDelay = 0.10f; //Fixed value
        }

        public void Update(GameWindow aWindow, GameTime aGameTime)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    Movement(aWindow, aGameTime, mySpeed);
                    Idle();
                    Jump();
                    Collisions(aGameTime);
                    break;
                case PlayerState.isClimbing:
                    Movement(aWindow, aGameTime, mySpeed / 2);
                    Climbing(aGameTime, mySpeed / 2);
                    Idle();
                    Collisions(aGameTime);
                    break;
                case PlayerState.isFalling:
                    Movement(aWindow, aGameTime, mySpeed);
                    Gravity(aGameTime);
                    Collisions(aGameTime);
                    break;
                case PlayerState.isDead:
                    Gravity(aGameTime);
                    if (myPosition.Y - mySize.Y > aWindow.ClientBounds.Height)
                    {
                        myIsAlive = false;
                    }
                    break;
            }

            Timer(aGameTime);
            TextureState(); //Create post-update instead?
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            if (myDrawSprite)
            {
                switch (myPlayerState)
                {
                    case PlayerState.isWalking:
                        if (myIsMoving)
                        {
                            myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myBoundingBox, new Point(32), Color.White, 0.0f, myOrigin, myFlipSprite);
                        }
                        else
                        {
                            aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
                        }
                        break;
                    case PlayerState.isClimbing:
                        if (myIsClimbing)
                        {
                            myClimbingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myBoundingBox, new Point(32), Color.White, 0.0f, myOrigin, SpriteEffects.None);
                        }
                        else
                        {
                            aSpriteBatch.Draw(myTexture, myBoundingBox, new Rectangle(0, 0, 32, 32), Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
                        }
                        break;
                    case PlayerState.isFalling:
                        aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
                        break;
                    case PlayerState.isDead:
                        aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
                        break;
                }
            }
        }

        private void Collisions(GameTime aGameTime)
        {
            IsFalling();
            CollisionItemBlock(aGameTime);
            CollisionBlock(aGameTime);
            CollisionLadder();
            CollisionEnemy(aGameTime);
            CollisionItem();
        }
        private void IsFalling()
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
            if (tempFall && myPlayerState != PlayerState.isClimbing)
            {
                myPlayerState = PlayerState.isFalling;
            }
        }
        private void CollisionBlock(GameTime aGameTime)
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                float tempVelocity = (myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds);

                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, tempVelocity) && myVelocity < 0)
                    {
                        myVelocity = 0.0f;
                        myPosition.Y = tile.Position.Y + tile.Size.Y;
                    }
                    if (CollisionManager.CheckBelow(myBoundingBox, tile.BoundingBox, tempVelocity) && myVelocity > 0)
                    {
                        myVelocity = 0.0f;
                        myPosition.Y = tile.Position.Y - mySize.Y;

                        myPlayerState = PlayerState.isWalking;
                    }
                }
            }
        }
        private void CollisionItemBlock(GameTime aGameTime)
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                float tempVelocity = (myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds);

                if (tile.TileType == '/')
                {
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, tempVelocity) && myVelocity < 0)
                    {
                        tile.TileType = '(';
                        tile.IsBlock = true;

                        tile.SetTexture();
                    }
                }
            }
        }
        private void CollisionLadder()
        {
            Tile tempTile = null;
            foreach (Tile tile in Level.TilesOn(this))
            {
                if (tile.TileType == '%')
                {
                    tempTile = tile;
                }
            }

            Rectangle tempColRectBelow = new Rectangle(myBoundingBox.X, myBoundingBox.Y, myBoundingBox.Width, myBoundingBox.Height + (mySize.Y / 8));
            Rectangle tempColRectAbove = new Rectangle(myBoundingBox.X, myBoundingBox.Y - (mySize.Y / 8), myBoundingBox.Width, myBoundingBox.Height + (mySize.Y / 8));
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tempTile != null)
                {
                    if (KeyMouseReader.KeyHold(Keys.Up) && (!CollisionManager.Collision(tempColRectAbove, tile.BoundingBox) || tile.TileType != '#'))
                    {
                        myPlayerState = PlayerState.isClimbing;
                        myVelocity = 0.0f;
                    }
                    if (KeyMouseReader.KeyHold(Keys.Down) && (!CollisionManager.Collision(tempColRectBelow, tile.BoundingBox) || tile.TileType != '#'))
                    {
                        myPlayerState = PlayerState.isClimbing;
                        myVelocity = 0.0f;
                    }
                }
                else if (myPlayerState == PlayerState.isClimbing)
                {
                    myPlayerState = PlayerState.isFalling;
                }
            }
        }
        private void CollisionEnemy(GameTime aGameTime)
        {
            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                float tempVelocity = myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;

                if (CollisionManager.CheckBelow(myBoundingBox, enemy.BoundingBox, tempVelocity) && tempVelocity > 0)
                {
                    enemy.IsAlive = false;
                    enemy.HasCollided = true;
                    myVelocity = myJumpHeight;

                    GameInfo.AddScore(enemy.BoundingBox.Center.ToVector2(), 100);

                    break;
                }
                else if (!enemy.HasCollided)
                {
                    if (!myIsInvincible)
                    {
                        if (CollisionManager.CheckAbove(myBoundingBox, enemy.BoundingBox, tempVelocity) && tempVelocity < 0)
                        {
                            ReduceHealth(1);
                        }
                        else if (CollisionManager.CheckLeft(myBoundingBox, enemy.BoundingBox, new Vector2(0, -tempVelocity)))
                        {
                            ReduceHealth(1);
                        }
                        else if (CollisionManager.CheckRight(myBoundingBox, enemy.BoundingBox, new Vector2(0, -tempVelocity)))
                        {
                            ReduceHealth(1);
                        }
                    }
                }
            }
        }
        private void CollisionItem()
        {

        }
        public bool CollisionFlag()
        {
            foreach (Tile tile in Level.TilesOn(this))
            {
                if (tile.TileType == '*')
                {
                    return true;
                }
            }
            return false;
        }

        private void Movement(GameWindow aWindow, GameTime aGameTime, float aSpeed)
        {
            if (KeyMouseReader.KeyHold(Keys.Left) && !OutsideBounds(new Vector2(-mySpeed, 0)) && CanMoveHorizontal(aGameTime))
            {
                myPosition.X -= aSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myCurrentSpeed = aSpeed;

                myIsMoving = true;
                myIsClimbing = true;
                myFlipSprite = SpriteEffects.FlipHorizontally;
            }
            if (KeyMouseReader.KeyHold(Keys.Right) && !OutsideBounds(new Vector2(mySpeed, 0)) && CanMoveHorizontal(aGameTime))
            {
                myPosition.X += aSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myCurrentSpeed = aSpeed;

                myIsMoving = true;
                myIsClimbing = true;
                myFlipSprite = SpriteEffects.None;
            }

            if (myPosition.Y > aWindow.ClientBounds.Height)
            {
                ReduceHealth(1);
            }
        }
        private void Climbing(GameTime aGameTime, float aSpeed)
        {
            if (KeyMouseReader.KeyHold(Keys.Up) && CanMoveVertical(aGameTime))
            {
                myPosition.Y -= aSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myCurrentSpeed = aSpeed;

                myIsClimbing = true;
            }
            if (KeyMouseReader.KeyHold(Keys.Down) && CanMoveVertical(aGameTime))
            {
                myPosition.Y += aSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myCurrentSpeed = aSpeed;

                myIsClimbing = true;
            }
        }
        private void Idle()
        {
            if (!KeyMouseReader.KeyHold(Keys.Left) && !KeyMouseReader.KeyHold(Keys.Right) || KeyMouseReader.KeyHold(Keys.Left) && KeyMouseReader.KeyHold(Keys.Right))
            {
                myIsMoving = false;
                if (!KeyMouseReader.KeyHold(Keys.Up) && !KeyMouseReader.KeyHold(Keys.Down) || KeyMouseReader.KeyHold(Keys.Up) && KeyMouseReader.KeyHold(Keys.Down))
                {
                    myIsClimbing = false;
                }
            }
        }
        private void Jump()
        {
            if (KeyMouseReader.KeyPressed(Keys.Space))
            {
                myVelocity = myJumpHeight;
                myPlayerState = PlayerState.isFalling;
            }
        }
        public void ReduceHealth(int aValue)
        {
            if (myInvincibleTimer <= 0)
            {
                myLives -= aValue;

                if (myLives > 0)
                {
                    myIsInvincible = true;
                    myInvincibleTimer = myInvincibleDelay;

                    myPosition = Level.PlayerSpawn;
                    myPosition += new Vector2(0, -mySize.Y);
                }
                else
                {
                    myLives = 0;

                    myVelocity = myJumpHeight * 0.8f;
                    myPlayerState = PlayerState.isDead;
                }
            }
        }

        private bool OutsideBounds(Vector2 aDirection)
        {
            if (myPosition.X + aDirection.X < 0)
            {
                myPosition.X = 0;
                return true;
            }
            if (myPosition.X + aDirection.X + mySize.X > Level.MapSize.X)
            {
                myPosition.X = Level.MapSize.X - mySize.X;
                return true;
            }
            return false;
        }
        private bool CanMoveHorizontal(GameTime aGameTime)
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    float tempVelocity = Math.Abs((myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds));
                    float tempSpeed = (mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);

                    if (CollisionManager.CheckLeft(myBoundingBox, tile.BoundingBox, new Vector2(tempSpeed, tempVelocity)) && KeyMouseReader.KeyHold(Keys.Left))
                    {
                        myPosition.X = tile.BoundingBox.X + tile.Size.X;
                        return false;
                    }
                    if (CollisionManager.CheckRight(myBoundingBox, tile.BoundingBox, new Vector2(tempSpeed, tempVelocity)) && KeyMouseReader.KeyHold(Keys.Right))
                    {
                        myPosition.X = tile.BoundingBox.X - mySize.X;
                        return false;
                    }
                }
            }
            return true;
        }
        private bool CanMoveVertical(GameTime aGameTime)
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    float tempSpeed = ((mySpeed / 2) * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);

                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, -tempSpeed) && KeyMouseReader.KeyHold(Keys.Up))
                    {
                        myPosition.Y = tile.Position.Y + tile.Size.Y;
                        return false;
                    }
                    if (CollisionManager.CheckBelow(myBoundingBox, tile.BoundingBox, tempSpeed) && KeyMouseReader.KeyHold(Keys.Down))
                    {
                        myPosition.Y = tile.Position.Y - mySize.Y;
                        return false;
                    }
                }
            }
            return true;
        }

        private void Timer(GameTime aGameTime)
        {
            if (myInvincibleTimer > 0)
            {
                myInvincibleTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
                if (myDrawPlayerTimer > 0)
                {
                    myDrawPlayerTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    myDrawPlayerTimer = myDrawPlayerDelay;
                    myDrawSprite = !myDrawSprite;
                }
            }
            else
            {
                myInvincibleTimer = 0;
                myIsInvincible = false;
                myDrawSprite = true;
            }
        }

        private void TextureState()
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (myIsMoving)
                    {
                        myTexture = ResourceManager.RequestTexture("Mario_Walking");
                    }
                    else
                    {
                        myTexture = ResourceManager.RequestTexture("Mario_Idle");
                    }
                    break;
                case PlayerState.isClimbing:
                    myTexture = ResourceManager.RequestTexture("Mario_Climbing");
                    break;
                case PlayerState.isFalling:
                    myTexture = ResourceManager.RequestTexture("Mario_Jumping");
                    break;
                case PlayerState.isDead:
                    myTexture = ResourceManager.RequestTexture("Mario_Death");
                    break;
            }
        }
    }
}
