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
        private Point
            myBigSize,
            mySaveSize;
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
            myIsBigTimer,
            myIsBigDelay,
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

        public Player(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold, float aGravity, float aJumpHeight, float aInvincibleDelay, float aIsBigDelay, int someLives) : base(aPosition, aSize, aVelocity, aVelocityThreshold, aGravity)
        {
            this.myLives = someLives;
            this.myJumpHeight = aJumpHeight;
            this.myInvincibleDelay = aInvincibleDelay;
            this.myIsBigDelay = aIsBigDelay;

            this.myWalkingAnimation = new AnimationManager(new Point(3, 1), 0.1f, true);
            this.myClimbingAnimation = new AnimationManager(new Point(2, 1), 0.3f, true);
            this.myPosition += new Vector2(0, Level.TileSize.Y - mySize.Y);
            this.myBigSize = new Point(mySize.X * 2, mySize.Y * 2);
            this.mySaveSize = mySize;
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
                    Movement(aWindow, aGameTime, myVelocity.X);
                    Idle();
                    Jump();
                    Collisions();
                    break;
                case PlayerState.isClimbing:
                    Movement(aWindow, aGameTime, myVelocity.X / 2);
                    Climbing(aGameTime);
                    Idle();
                    Collisions();
                    break;
                case PlayerState.isFalling:
                    Movement(aWindow, aGameTime, myVelocity.X);
                    Idle();
                    Gravity(aGameTime);
                    Collisions();
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

        private void Collisions()
        {
            IsFalling();
            CollisionCoinBlock();
            CollisionItemBlock();
            CollisionBlock();
            CollisionLadder();
            CollisionEnemy();
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
        private void CollisionCoinBlock()
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.TileType == '^')
                {
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && myCurrentVelocity.Y < 0)
                    {
                        tile.TileType = '(';
                        tile.IsBlock = true;
                        tile.SetTexture();

                        GameInfo.AddScore(tile.GetCenter(), 100);
                    }
                }
            }
        }
        private void CollisionItemBlock()
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.TileType == '/')
                {
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && myCurrentVelocity.Y < 0)
                    {
                        tile.TileType = '(';
                        tile.IsBlock = true;
                        tile.SetTexture();

                        Tile tempTile = Level.TileAtPos(new Vector2(tile.GetCenter().X, tile.GetCenter().Y - Level.TileSize.Y)).Item1;
                        if (tempTile.TileType == '-')
                        {
                            switch (StaticRandom.RandomNumber(0, 2))
                            {
                                case 0:
                                    tempTile.TileType = '=';
                                    break;
                                case 1:
                                    tempTile.TileType = ')';
                                    break;
                            }
                            tempTile.SetTexture();
                        }
                    }
                }
            }
        }
        private void CollisionBlock()
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && myCurrentVelocity.Y < 0)
                    {
                        myCurrentVelocity.Y = 0.0f;
                        myPosition.Y = tile.Position.Y + tile.Size.Y;
                    }
                    if (CollisionManager.CheckBelow(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && myCurrentVelocity.Y > 0)
                    {
                        myCurrentVelocity.Y = 0.0f;
                        myPosition.Y = tile.Position.Y - mySize.Y;

                        myPlayerState = PlayerState.isWalking;
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
                        myCurrentVelocity.Y = 0.0f;
                    }
                    if (KeyMouseReader.KeyHold(Keys.Down) && (!CollisionManager.Collision(tempColRectBelow, tile.BoundingBox) || tile.TileType != '#'))
                    {
                        myPlayerState = PlayerState.isClimbing;
                        myCurrentVelocity.Y = 0.0f;
                    }
                }
                else if (myPlayerState == PlayerState.isClimbing)
                {
                    myPlayerState = PlayerState.isFalling;
                }
            }
        }
        private void CollisionEnemy()
        {
            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                if (!enemy.HasCollided)
                {
                    if (mySize != myBigSize)
                    {
                        if (CollisionManager.CheckBelow(myBoundingBox, enemy.BoundingBox, myCurrentVelocity) && myCurrentVelocity.Y > 0)
                        {
                            enemy.IsAlive = false;
                            enemy.HasCollided = true;
                            myCurrentVelocity.Y = myJumpHeight;

                            GameInfo.AddScore(enemy.BoundingBox.Center.ToVector2(), 100);

                            break;
                        }
                        else
                        {
                            if (!myIsInvincible)
                            {
                                if (myBoundingBox.Bottom + myCurrentVelocity.Y > enemy.BoundingBox.Top)
                                {
                                    if (CollisionManager.CheckAbove(myBoundingBox, enemy.BoundingBox, myCurrentVelocity) && myCurrentVelocity.Y < 0)
                                    {
                                        ReduceHealth(1);
                                    }
                                    else if (CollisionManager.CheckLeft(myBoundingBox, enemy.BoundingBox, myCurrentVelocity))
                                    {
                                        ReduceHealth(1);
                                    }
                                    else if (CollisionManager.CheckRight(myBoundingBox, enemy.BoundingBox, myCurrentVelocity))
                                    {
                                        ReduceHealth(1);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (CollisionManager.Collision(myBoundingBox, enemy.BoundingBox))
                        {
                            enemy.IsAlive = false;
                            enemy.HasCollided = true;

                            GameInfo.AddScore(enemy.BoundingBox.Center.ToVector2(), 100);
                        }
                    }
                }
            }
        }
        private void CollisionItem()
        {
            foreach (Tile tile in Level.TilesOnAndAround(this))
            {
                if (CollisionManager.Collision(myBoundingBox, tile.BoundingBox))
                {
                    if (tile.TileType == '=' && mySize != myBigSize)
                    {
                        myPosition.Y -= mySize.Y;

                        mySize = myBigSize;
                        myIsBigTimer = myIsBigDelay;

                        tile.TileType = '-';
                        tile.SetTexture();
                    }
                    if (tile.TileType == ')')
                    {
                        myLives++;

                        tile.TileType = '-';
                        tile.SetTexture();
                    }
                }
            }
        }
        public bool CollisionFlag()
        {
            foreach (Tile tile in Level.TilesOnAndAround(this))
            {
                if (CollisionManager.Collision(myBoundingBox, tile.BoundingBox))
                {
                    if (tile.TileType == '*')
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void Movement(GameWindow aWindow, GameTime aGameTime, float aSpeed)
        {
            if (KeyMouseReader.KeyHold(Keys.Left))
            {
                myCurrentVelocity.X = -(aSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                if (!OutsideBounds() && CanMoveHorizontal())
                {
                    myPosition.X += myCurrentVelocity.X;

                    myIsMoving = true;
                    myIsClimbing = true;
                    myFlipSprite = SpriteEffects.FlipHorizontally;
                }
            }
            if (KeyMouseReader.KeyHold(Keys.Right))
            {
                myCurrentVelocity.X = (aSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                if (!OutsideBounds() && CanMoveHorizontal())
                {
                    myPosition.X += myCurrentVelocity.X;

                    myIsMoving = true;
                    myIsClimbing = true;
                    myFlipSprite = SpriteEffects.None;
                }
            }

            if (myPosition.Y > aWindow.ClientBounds.Height)
            {
                ReduceHealth(1);
            }
        }
        private void Climbing(GameTime aGameTime)
        {
            if (KeyMouseReader.KeyHold(Keys.Up) && CanMoveVertical())
            {
                myCurrentVelocity.Y = -(myVelocity.Y * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                myPosition.Y += myCurrentVelocity.Y;

                myIsClimbing = true;
            }
            if (KeyMouseReader.KeyHold(Keys.Down) && CanMoveVertical())
            {
                myCurrentVelocity.Y = (myVelocity.Y * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                myPosition.Y += myCurrentVelocity.Y;

                myIsClimbing = true;
            }
        }
        private void Idle()
        {
            if (!KeyMouseReader.KeyHold(Keys.Left) && !KeyMouseReader.KeyHold(Keys.Right) || KeyMouseReader.KeyHold(Keys.Left) && KeyMouseReader.KeyHold(Keys.Right))
            {
                myIsMoving = false;
                myCurrentVelocity.X = 0;
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
                myCurrentVelocity.Y = myJumpHeight;
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

                    myCurrentVelocity.Y = myJumpHeight * 0.8f;
                    myPlayerState = PlayerState.isDead;
                }
            }
        }

        private bool OutsideBounds()
        {
            if (myPosition.X + myCurrentVelocity.X < 0)
            {
                myPosition.X = 0;
                return true;
            }
            if (myPosition.X + myCurrentVelocity.X + mySize.X > Level.MapSize.X)
            {
                myPosition.X = Level.MapSize.X - mySize.X;
                return true;
            }
            return false;
        }
        private bool CanMoveHorizontal()
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckLeft(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && KeyMouseReader.KeyHold(Keys.Left))
                    {
                        myPosition.X = tile.BoundingBox.X + tile.Size.X;
                        return false;
                    }
                    if (CollisionManager.CheckRight(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && KeyMouseReader.KeyHold(Keys.Right))
                    {
                        myPosition.X = tile.BoundingBox.X - mySize.X;
                        return false;
                    }
                }
            }
            return true;
        }
        private bool CanMoveVertical()
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && KeyMouseReader.KeyHold(Keys.Up))
                    {
                        myPosition.Y = tile.Position.Y + tile.Size.Y;
                        return false;
                    }
                    if (CollisionManager.CheckBelow(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && KeyMouseReader.KeyHold(Keys.Down))
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
            else if (myIsInvincible)
            {
                myInvincibleTimer = 0;
                myIsInvincible = false;
                myDrawSprite = true;
            }

            if (myIsBigTimer > 0)
            {
                myIsBigTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
                if (myIsBigTimer < (myIsBigDelay / 4))
                {
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
            }
            else if (mySize != mySaveSize)
            {
                myIsBigTimer = 0;
                mySize = mySaveSize;
                myDrawSprite = true;

                myPosition.Y += mySize.Y;
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
