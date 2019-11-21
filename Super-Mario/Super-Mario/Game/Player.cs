using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private PlayerState myPlayerState;

        private AnimationManager myWalkingAnimation;

        float myJumpHeight;
        bool 
            myCanJump,
            myIsMoving;

        public Player(Vector2 aPosition, Point aSize, float aSpeed, float aGravity, float aJumpHeight) : base(aPosition, aSize, aSpeed, aGravity)
        {
            this.myJumpHeight = aJumpHeight;

            this.myWalkingAnimation = new AnimationManager(new Point(3, 1), 0.1f, true);
            this.myPosition += new Vector2(0, Level.TileSize.Y - mySize.Y);
            this.myPlayerState = PlayerState.isWalking;
            this.myCanJump = true;
        }

        public void Update(GameTime aGameTime)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    Movement(aGameTime);
                    Jump();
                    break;
                case PlayerState.isFalling:
                    Movement(aGameTime);
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case PlayerState.isDead:

                    break;
            }

            Level.GetTilesAroundObject(this);
            Collisions(aGameTime);
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, myOrigin, new Point(32, 34), mySize, Color.White, 0.0f);
                    break;
                case PlayerState.isFalling:
                    myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, myOrigin, new Point(32, 34), mySize, Color.White, 0.0f);
                    break;
                case PlayerState.isDead:

                    break;
            }
        }

        private void Collisions(GameTime aGameTime)
        {
            IsFalling(aGameTime);
            CollisionBlock(aGameTime);
        }
        private void IsFalling(GameTime aGameTime)
        {
            bool tempFall = true;
            foreach (Tile tile in Level.GetTilesAroundObject(this))
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
            foreach (Tile tile in Level.GetTilesAroundObject(this))
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
                        myPlayerState = PlayerState.isWalking;
                        myVelocity = 0.0f;

                        myPosition.Y = tile.Position.Y - mySize.Y;
                        myCanJump = true;
                    }          
                }
            }
        }

        private void Movement(GameTime aGameTime)
        {
            if (KeyMouseReader.KeyHold(Keys.Left) && !OutsideBounds(new Vector2(-mySpeed, 0)) && CanMove(aGameTime))
            {
                myPosition.X -= mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myIsMoving = true;
            }
            if (KeyMouseReader.KeyHold(Keys.Right) && !OutsideBounds(new Vector2(mySpeed, 0)) && CanMove(aGameTime))
            {
                myPosition.X += mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myIsMoving = true;
            }

            if (!KeyMouseReader.KeyHold(Keys.Left) && !KeyMouseReader.KeyHold(Keys.Right))
            {
                myIsMoving = false;
            }
        }
        private void Jump()
        {
            if (KeyMouseReader.KeyPressed(Keys.Space) && myCanJump)
            {
                myPlayerState = PlayerState.isFalling;
                myVelocity = myJumpHeight;
                myCanJump = false;
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
        private bool CanMove(GameTime aGameTime)
        {
            foreach (Tile tile in Level.GetTilesAroundObject(this))
            {
                if (tile.TileType == '#' && myIsMoving)
                {
                    float tempSpeed = (mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);

                    if (CollisionManager.CheckRight(myBoundingBox, tile.BoundingBox, tempSpeed) && KeyMouseReader.KeyHold(Keys.Right))
                    {
                        return false;
                    }
                    if (CollisionManager.CheckLeft(myBoundingBox, tile.BoundingBox, tempSpeed) && KeyMouseReader.KeyHold(Keys.Left))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
