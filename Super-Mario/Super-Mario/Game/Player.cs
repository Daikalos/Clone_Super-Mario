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
            isJumping,
            isDead
        }

        private PlayerState myPlayerState;

        private AnimationManager myWalkingAnimation;

        public Player(Vector2 aPosition, Point aSize, float aSpeed) : base(aPosition, aSize, aSpeed)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;

            this.myWalkingAnimation = new AnimationManager(new Point(3, 1), 0.1f, true);
            this.myPlayerState = PlayerState.isWalking;
        }

        public void Update(GameTime aGameTime)
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (KeyMouseReader.KeyHold(Keys.Left) && !OutsideBounds(new Vector2(-mySpeed, 0)))
                    {
                        myPosition.X -= mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    }
                    if (KeyMouseReader.KeyHold(Keys.Right) && !OutsideBounds(new Vector2(mySpeed, 0)))
                    {
                        myPosition.X += mySpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    }
                    break;
                case PlayerState.isJumping:

                    break;
                case PlayerState.isDead:

                    break;
            }
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, myOrigin, new Point(32, 34), mySize, Color.White, 0.0f);
                    break;
                case PlayerState.isJumping:

                    break;
                case PlayerState.isDead:

                    break;
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
    }
}
