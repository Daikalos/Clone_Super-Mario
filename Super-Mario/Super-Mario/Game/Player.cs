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
            isFalling,
            isDead
        }

        private AnimationManager 
            myWalkingAnimation;

        private PlayerState 
            myPlayerState;
        private SpriteEffects 
            myFlipSprite;

        float myJumpHeight;
        bool myIsMoving;

        public Player(Vector2 aPosition, Point aSize, float aSpeed, float aGravity, float aJumpHeight) : base(aPosition, aSize, aSpeed, aGravity)
        {
            this.myJumpHeight = aJumpHeight;

            this.myWalkingAnimation = new AnimationManager(new Point(3, 1), 0.1f, true);
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
                    Movement(aWindow, aGameTime);
                    Idle();
                    Jump();
                    break;
                case PlayerState.isFalling:
                    Movement(aWindow, aGameTime);
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case PlayerState.isDead:

                    break;
            }

            Collisions(aGameTime);
            TextureState(aGameTime); //Create post-update instead?
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (myIsMoving)
                    {
                        myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, mySize, new Point(32, 34), Color.White, 0.0f, myOrigin, myFlipSprite);
                    }
                    else
                    {
                        aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
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
        }
        private void IsFalling()
        {
            bool tempFall = true;
            foreach (Tile tile in Level.TilesAround(this))
            {
                Rectangle tempColRectBelow = new Rectangle(myBoundingBox.X, myBoundingBox.Y, myBoundingBox.Width, myBoundingBox.Height + mySize.Y / 4);
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
        public bool CollisionFlag()
        {
            foreach (Tile tile in Level.TilesOn(this))
            {
                if (tile.TileType == '*')
                {
                    if (CollisionManager.Collision(myBoundingBox, tile.BoundingBox))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void Movement(GameWindow aWindow, GameTime aGameTime)
        {
            if (KeyMouseReader.KeyHold(Keys.Left) && !OutsideBounds(aWindow, new Vector2(-mySpeed, 0)) && CanMove(aGameTime))
            {
                myPosition.X -= mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myIsMoving = true;

                myFlipSprite = SpriteEffects.FlipHorizontally;
            }
            if (KeyMouseReader.KeyHold(Keys.Right) && !OutsideBounds(aWindow, new Vector2(mySpeed, 0)) && CanMove(aGameTime))
            {
                myPosition.X += mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myIsMoving = true;

                myFlipSprite = SpriteEffects.None;
            }

            if (myPosition.Y > aWindow.ClientBounds.Height)
            {
                myPosition = Level.PlayerSpawn;
                myPosition += new Vector2(0, -mySize.Y);
            }
        }
        private void Idle()
        {
            if (!KeyMouseReader.KeyHold(Keys.Left) && !KeyMouseReader.KeyHold(Keys.Right) || KeyMouseReader.KeyHold(Keys.Left) && KeyMouseReader.KeyHold(Keys.Right))
            {
                myIsMoving = false;
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
        private bool OutsideBounds(GameWindow aWindow, Vector2 aDirection)
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
        private bool CanMove(GameTime aGameTime)
        {
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.TileType == '#')
                {
                    float tempSpeed = (mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);

                    if (CollisionManager.CheckRight(myBoundingBox, tile.BoundingBox, tempSpeed) && KeyMouseReader.KeyHold(Keys.Right))
                    {
                        myPosition.X = tile.BoundingBox.X - mySize.X;
                        return false;
                    }
                    if (CollisionManager.CheckLeft(myBoundingBox, tile.BoundingBox, tempSpeed) && KeyMouseReader.KeyHold(Keys.Left))
                    {
                        myPosition.X = tile.BoundingBox.X + tile.Size.X;
                        return false;
                    }
                }
            }
            return true;
        }

        private void TextureState(GameTime aGameTime)
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
                case PlayerState.isFalling:
                    myTexture = ResourceManager.RequestTexture("Mario_Jumping");
                    break;
            }
        }
    }
}
