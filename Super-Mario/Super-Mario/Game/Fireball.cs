using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class Fireball : DynamicObject
    {
        private Rectangle myDrawBox;
        private bool
            myIsAlive,
            myDirection;
        private float
            myRotation,
            myRotationSpeed;

        public bool IsAlive
        {
            get => myIsAlive;
            set => myIsAlive = value;
        }

        public Fireball(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold, bool aDirection) : base(aPosition, aSize, aVelocity, aVelocityThreshold)
        {
            this.myDirection = aDirection;

            this.myIsAlive = true;
            this.myRotationSpeed = 0.25f; //Fixed value
        }

        public void Update(GameTime aGameTime)
        {
            base.Update();
            myDrawBox = new Rectangle(DestRect.X + (int)myOrigin.X, DestRect.Y + (int)myOrigin.Y, DestRect.Width, DestRect.Height);

            switch (myDirection)
            {
                case true:
                    myCurrentVelocity.X = myVelocity.X * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    myPosition.X += myCurrentVelocity.X;

                    myRotation += myRotationSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case false:
                    myCurrentVelocity.X = -(myVelocity.X * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds);
                    myPosition.X += myCurrentVelocity.X;

                    myRotation -= myRotationSpeed * 60 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
            }

            Gravity(aGameTime);
            Collisions();
        }

        private void Collisions()
        {
            CollisionBlock();
            CollisionEnemy();
        }
        private void CollisionEnemy()
        {
            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                if (!enemy.HasCollided)
                {
                    if (CollisionManager.PixelCollision(this, enemy))
                    {
                        enemy.IsAlive = false;
                        enemy.HasCollided = true;
                        myIsAlive = false;

                        GameInfo.AddScore(enemy.BoundingBox.Center.ToVector2(), 100);

                        break;
                    }
                }
            }
        }
        private void CollisionBlock()
        {
            foreach (Tile tile in Level.TilesOnAndAround(this))
            {
                if (tile.IsBlock)
                {
                    if (CollisionManager.PixelCollision(this, tile))
                    {
                        myIsAlive = false;
                    }
                }
            }

            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (myTopCollision(this, tile, myCurrentVelocity))
                    {
                        myCurrentVelocity.Y = 0;
                        SnapTopCollision(tile);
                    }
                    if (myBotCollision(this, tile, myCurrentVelocity))
                    {
                        myCurrentVelocity.Y = -myVelocityThreshold.Y * Extensions.Signum(GameInfo.Gravity);
                        SnapBotCollision(tile);
                    }
                }
            }

            if (myPosition.X + mySize.X < 0 || myPosition.X > Level.MapSize.X || myPosition.Y > Level.MapSize.Y)
            {
                myIsAlive = false;
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch)
        {
            aSpriteBatch.Draw(myTexture, myDrawBox, null, Color.White, myRotation, myOrigin, SpriteEffects.None, 0.0f);
        }

        public override void SetTexture(string aName)
        {
            myTexture = ResourceManager.RequestTexture(aName);
            SetColorData();

            SetOrigin(new Point(1, 1));
        }
    }
}
