using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    internal class AnimationManager
    {
        //Animation-Info
        private int myCurrentFrame;

        private Point myCurrentFramePos;
        private bool myIsFinished;
        private float myTimer;

        //Texture-Info
        private Point
            myFrameAmount;

        private float myAnimationSpeed;
        private bool myIsLoop;

        public bool IsFinished
        {
            get => myIsFinished;
            set => myIsFinished = value;
        }

        public AnimationManager(Point aFrameAmount, float aAnimationSpeed, bool aIsLoop)
        {
            this.myCurrentFrame = 0;
            this.myIsFinished = false;

            this.myFrameAmount = aFrameAmount;
            this.myAnimationSpeed = aAnimationSpeed;
            this.myIsLoop = aIsLoop;
        }

        public void DrawSpriteSheet(SpriteBatch aSpriteBatch, GameTime aGameTime, Texture2D aTexture, Vector2 aPos, Vector2 aOrigin, Point aFrameSize, Point aDestSize, Color aColor, float aRotation)
        {
            if (myIsFinished) return;

            myTimer += (float)aGameTime.ElapsedGameTime.TotalSeconds;
            if (myTimer > myAnimationSpeed)
            {
                myCurrentFrame++;
                myCurrentFramePos.X++;
                if (myCurrentFrame >= (myFrameAmount.X * myFrameAmount.Y))
                {
                    if (myIsLoop)
                    {
                        myCurrentFrame = 0;
                        myCurrentFramePos = new Point(0, 0);
                    }
                    else
                    {
                        myCurrentFrame = (myFrameAmount.X * myFrameAmount.Y) - 1;
                        myIsFinished = true;
                    }
                }
                if (myCurrentFramePos.X >= myFrameAmount.X) //Animation
                {
                    myCurrentFramePos.Y++;
                    myCurrentFramePos.X = 0;
                }
                myTimer = 0;
            }

            aSpriteBatch.Draw(aTexture,
                new Rectangle((int)aPos.X, (int)aPos.Y, aDestSize.X, aDestSize.Y),
                new Rectangle(aFrameSize.X * myCurrentFramePos.X, aFrameSize.Y * myCurrentFramePos.Y, aFrameSize.X, aFrameSize.Y),
                aColor, aRotation, aOrigin, SpriteEffects.None, 0.0f);
        }
    }
}