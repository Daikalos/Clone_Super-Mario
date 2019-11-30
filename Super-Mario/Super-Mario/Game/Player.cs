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

        private PlayerState 
            myPlayerState;
        private SpriteEffects 
            myFlipSprite;

        float myJumpHeight;
        bool 
            myIsMoving,
            myIsClimbing;

        public Player(Vector2 aPosition, Point aSize, float aSpeed, float aGravity, float aJumpHeight) : base(aPosition, aSize, aSpeed, aGravity)
        {
            this.myJumpHeight = aJumpHeight;

            this.myWalkingAnimation = new AnimationManager(new Point(3, 1), 0.1f, true);
            this.myClimbingAnimation = new AnimationManager(new Point(2, 1), 0.3f, true);
            this.myPosition += new Vector2(0, Level.TileSize.Y - mySize.Y);
            this.myPlayerState = PlayerState.isWalking;
            this.myFlipSprite = SpriteEffects.None;
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
                    break;
                case PlayerState.isClimbing:
                    Movement(aWindow, aGameTime, mySpeed / 2);
                    Climbing(aGameTime, mySpeed / 2);
                    Idle();
                    break;
                case PlayerState.isFalling:
                    Movement(aWindow, aGameTime, mySpeed);
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case PlayerState.isDead:

                    break;
            }

            Collisions(aGameTime);
            TextureState(); //Create post-update instead?
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (myIsMoving)
                    {
                        myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, mySize, new Point(32), Color.White, 0.0f, myOrigin, myFlipSprite);
                    }
                    else
                    {
                        aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
                    }
                    break;
                case PlayerState.isClimbing:
                    if (myIsClimbing)
                    {
                        myClimbingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, mySize, new Point(32), Color.White, 0.0f, myOrigin, SpriteEffects.None);
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

                    break;
            }
        }

        private void Collisions(GameTime aGameTime)
        {
            IsFalling();
            CollisionBlock(aGameTime);
            CollisionLadder();
            CollisionEnemy();
        }
        private void IsFalling()
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

                if (tile.TileType == '#')
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
        private void CollisionEnemy()
        {
            foreach (Enemy enemy in EnemyManager.Enemies)
            {

            }
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
                myPosition = Level.PlayerSpawn;
                myPosition += new Vector2(0, -mySize.Y);
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
                if (tile.TileType == '#')
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
                if (tile.TileType == '#')
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
            }
        }
    }
}
