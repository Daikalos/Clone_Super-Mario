using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    internal class AnimationManager
    {
        //Animation-Info
        private Point myCurrentFramePos;
        private float myTimer;
        private int myCurrentFrame;
        private bool myIsFinished;
        //Sprite-Info
        private Point myFrameAmount;
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

        public void DrawSpriteSheet(SpriteBatch aSpriteBatch, GameTime aGameTime, Texture2D aTexture, Rectangle aBoundingBox, Point aFrameSize, Color aColor, float aRotation, Vector2 aOrigin, SpriteEffects aSE)
        {
            if (myIsFinished) return;

            if (!GameInfo.IsPaused)
            {
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
            }

            aSpriteBatch.Draw(aTexture,
                aBoundingBox, new Rectangle(aFrameSize.X * myCurrentFramePos.X, aFrameSize.Y * myCurrentFramePos.Y, aFrameSize.X, aFrameSize.Y),
                aColor, aRotation, aOrigin, aSE, 0.0f);
        }
    }
}