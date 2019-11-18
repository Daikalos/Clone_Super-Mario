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
    class Player : GameObject
    {
        enum PlayerState
        {
            isWalking,
            isJumping,
            isDead
        }

        private PlayerState myPlayerState;

        private AnimationManager myWalkingAnimation;

        public Player(Vector2 aPosition, Point aSize) : base(aPosition, aSize)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;

            this.myWalkingAnimation = new AnimationManager(new Point(3, 1), 0.1f, true);
            this.myPlayerState = PlayerState.isWalking;
        }

        public void Update()
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (KeyMouseReader.KeyHold(Keys.Left))
                    {
                        myPosition.X -= 2;
                    }
                    if (KeyMouseReader.KeyHold(Keys.Right))
                    {
                        myPosition.X += 2;
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
    }
}
