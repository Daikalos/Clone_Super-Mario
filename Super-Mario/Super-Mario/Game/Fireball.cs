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
            myDrawBox = new Rectangle(myBoundingBox.X + (int)myOrigin.X, myBoundingBox.Y + (int)myOrigin.Y, myBoundingBox.Width, myBoundingBox.Height);

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
                    if (CollisionManager.Collision(myBoundingBox, enemy.BoundingBox))
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
            foreach (Tile tile in Level.TilesAround(this))
            {
                if (tile.IsBlock)
                {
                    if (myTopCollision(myBoundingBox, tile.BoundingBox, myCurrentVelocity))
                    {
                        myCurrentVelocity.Y = 0;
                        SnapTopCollision(tile);
                    }
                    if (myBotCollision(myBoundingBox, tile.BoundingBox, myCurrentVelocity))
                    {
                        myCurrentVelocity.Y = -myVelocityThreshold.Y * Extensions.Signum(GameInfo.Gravity);
                        SnapBotCollision(tile);
                    }

                    if (CollisionManager.CheckLeft(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && myDirection)
                    {
                        myIsAlive = false;
                    }
                    if (CollisionManager.CheckRight(myBoundingBox, tile.BoundingBox, myCurrentVelocity) && !myDirection)
                    {
                        myIsAlive = false;
                    }
                }
            }

            foreach (Tile tile in Level.TilesOnAndAround(this))
            {
                if (tile.IsBlock)
                {
                    if (CollisionManager.Collision(myBoundingBox, tile.BoundingBox))
                    {
                        myIsAlive = false;
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
            SetOrigin(new Point(1, 1));
        }
    }
}
