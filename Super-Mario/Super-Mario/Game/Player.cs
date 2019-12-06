using System.Collections.Generic;
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
        private List<Fireball> myFireballs;
        private PlayerState myPlayerState;
        private Point
            myBigSize,
            mySaveSize;
        private SpriteEffects myFlipSprite;
        private bool
            myIsAlive,
            myIsMoving,
            myIsClimbing,
            myIsInvincible,
            myIsSuperForm,
            myIsFireForm,
            myDrawSprite;
        private float
            myInvincibleTimer,
            myInvincibleDelay,
            mySuperPowerUpTimer,
            mySuperPowerUpDelay,
            myFirePowerUpTimer,
            myFirePowerUpDelay,
            myDrawPlayerTimer,
            myDrawPlayerDelay;
        private int myLives;
        private string myLoadTexture;

        public int Lives
        {
            get => myLives;
        }
        public bool IsAlive
        {
            get => myIsAlive;
        }

        public Player(Vector2 aPosition, Point aSize, Vector2 aVelocity, Vector2 aVelocityThreshold, float aGravity, float aInvincibleDelay, float aSuperPowerUpDelay, float aFirePowerUpDelay, int someLives) : base(aPosition, aSize, aVelocity, aVelocityThreshold, aGravity)
        {
            this.myLives = someLives;
            this.myInvincibleDelay = aInvincibleDelay;
            this.mySuperPowerUpDelay = aSuperPowerUpDelay;
            this.myFirePowerUpDelay = aFirePowerUpDelay;

            this.myWalkingAnimation = new AnimationManager(new Point(3, 1), 0.1f, true);
            this.myClimbingAnimation = new AnimationManager(new Point(2, 1), 0.3f, true);
            this.myFireballs = new List<Fireball>();
            this.myPosition += new Vector2(0, Level.TileSize.Y - mySize.Y);
            this.myBigSize = new Point(mySize.X, mySize.Y * 2);
            this.mySaveSize = mySize;
            this.myPlayerState = PlayerState.isWalking;
            this.myFlipSprite = SpriteEffects.None;
            this.myIsAlive = true;
            this.myDrawSprite = true;
            this.myDrawPlayerDelay = 0.10f; //Fixed value
        }

        public void Update(GameWindow aWindow, GameTime aGameTime)
        {
            base.Update();

            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    Movement(aWindow, aGameTime, myVelocity.X);
                    Idle();
                    Jump();
                    Fireball(aGameTime);
                    Collisions();
                    break;
                case PlayerState.isClimbing:
                    Movement(aWindow, aGameTime, myVelocity.X / 2);
                    Climbing(aGameTime);
                    Idle();
                    Fireball(aGameTime);
                    Collisions();
                    break;
                case PlayerState.isFalling:
                    Movement(aWindow, aGameTime, myVelocity.X);
                    Idle();
                    ThrowFireball();
                    UpdateFireball(aGameTime);
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
                            myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myBoundingBox, mySize, Color.White, 0.0f, myOrigin, myFlipSprite);
                        }
                        else
                        {
                            aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
                        }
                        break;
                    case PlayerState.isClimbing:
                        if (myIsClimbing)
                        {
                            myClimbingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myBoundingBox, mySize, Color.White, 0.0f, myOrigin, SpriteEffects.None);
                        }
                        else
                        {
                            aSpriteBatch.Draw(myTexture, myBoundingBox, new Rectangle(0, 0, mySize.X, mySize.Y), Color.White, 0.0f, myOrigin, myFlipSprite, 0.0f);
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

            DrawFireball(aSpriteBatch);
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
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, myCurrentVelocity))
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
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, myCurrentVelocity))
                    {
                        tile.TileType = '(';
                        tile.IsBlock = true;
                        tile.SetTexture();

                        Tile tempTile = Level.TileAtPos(new Vector2(tile.GetCenter().X, tile.GetCenter().Y - Level.TileSize.Y)).Item1;
                        if (tempTile.TileType == '-')
                        {
                            switch (StaticRandom.RandomNumber(0, 3))
                            {
                                case 0:
                                    tempTile.TileType = '=';
                                    break;
                                case 1:
                                    tempTile.TileType = ')';
                                    break;
                                case 2:
                                    tempTile.TileType = ',';
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
                    if (CollisionManager.CheckAbove(myBoundingBox, tile.BoundingBox, myCurrentVelocity))
                    {
                        myCurrentVelocity.Y = 0.0f;
                        myPosition.Y = tile.Position.Y + tile.Size.Y;

                        if (myIsSuperForm)
                        {
                            tile.TileType = '-';
                            tile.IsBlock = false;
                            tile.SetTexture();
                        }
                    }
                    if (CollisionManager.CheckBelow(myBoundingBox, tile.BoundingBox, myCurrentVelocity))
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
                    if (CollisionManager.CheckBelow(myBoundingBox, enemy.BoundingBox, myCurrentVelocity))
                    {
                        enemy.IsAlive = false;
                        enemy.HasCollided = true;
                        myCurrentVelocity.Y = -myVelocityThreshold.Y;

                        GameInfo.AddScore(enemy.BoundingBox.Center.ToVector2(), 100);

                        break;
                    }
                    else
                    {
                        if (myBoundingBox.Bottom + myCurrentVelocity.Y > enemy.BoundingBox.Top)
                        {
                            if (CollisionManager.CheckAbove(myBoundingBox, enemy.BoundingBox, myCurrentVelocity))
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
        }
        private void CollisionItem()
        {
            foreach (Tile tile in Level.TilesOnAndAround(this))
            {
                if (CollisionManager.Collision(myBoundingBox, tile.BoundingBox))
                {
                    if (tile.TileType == '=' && !myIsSuperForm && !myIsFireForm)
                    {
                        myPosition.Y -= (myBigSize.Y - mySize.Y);
                        mySize = myBigSize;

                        myIsSuperForm = true;
                        mySuperPowerUpTimer = mySuperPowerUpDelay;

                        tile.TileType = '-';
                        tile.SetTexture();
                    }
                    if (tile.TileType == ',' && !myIsFireForm)
                    {
                        if (myIsSuperForm)
                        {
                            mySuperPowerUpTimer = 0;
                            myIsSuperForm = false;
                        }

                        myPosition.Y -= (myBigSize.Y - mySize.Y);
                        mySize = myBigSize;

                        myIsFireForm = true;
                        myFirePowerUpTimer = myFirePowerUpDelay;

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
                myCurrentVelocity.Y = -myVelocityThreshold.Y;
                myPlayerState = PlayerState.isFalling;
            }
        }
        public void ReduceHealth(int aValue)
        {
            if (!myIsInvincible)
            {
                if (!myIsSuperForm && !myIsFireForm)
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

                        myCurrentVelocity.Y = myVelocityThreshold.Y * 0.8f;
                        myPlayerState = PlayerState.isDead;
                    }
                }
                else
                {
                    mySuperPowerUpTimer = 0;
                    myFirePowerUpTimer = 0;

                    myIsInvincible = true;
                    myInvincibleTimer = myInvincibleDelay;
                }
            }
        }

        private void Fireball(GameTime aGameTime)
        {
            ThrowFireball();
            UpdateFireball(aGameTime);
        }
        private void ThrowFireball()
        {
            if (myIsFireForm)
            {
                if (KeyMouseReader.KeyPressed(Keys.LeftControl))
                {
                    if (myFlipSprite == SpriteEffects.None)
                    {
                        Fireball tempFireball = new Fireball(new Vector2(myPosition.X + mySize.X, myPosition.Y + mySize.Y / 2),
                            new Point(16), new Vector2(3.0f, 0.0f), new Vector2(3.0f, 5.0f), 0.7f, false);
                        tempFireball.SetTexture("Fireball");

                        myFireballs.Add(tempFireball);
                    }
                    if (myFlipSprite == SpriteEffects.FlipHorizontally)
                    {
                        Fireball tempFireball = new Fireball(new Vector2(myPosition.X - 16, myPosition.Y + mySize.Y / 2),
                            new Point(16), new Vector2(3.0f, 0.0f), new Vector2(3.0f, 5.0f), 0.6f, true);
                        tempFireball.SetTexture("Fireball");

                        myFireballs.Add(tempFireball);
                    }
                }
            }
        }
        private void UpdateFireball(GameTime aGameTime)
        {
            for (int i = myFireballs.Count - 1; i >= 0; i--)
            {
                myFireballs[i].Update(aGameTime);
                if (!myFireballs[i].IsAlive)
                {
                    myFireballs.RemoveAt(i);
                }
            }
        }
        private void DrawFireball(SpriteBatch aSpriteBatch)
        {
            for (int i = myFireballs.Count - 1; i >= 0; i--)
            {
                myFireballs[i].Draw(aSpriteBatch);
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
            Invincibility(aGameTime);
            SuperPowerUp(aGameTime);
            FirePowerUp(aGameTime);
        }
        private void Invincibility(GameTime aGameTime)
        {
            if (myInvincibleTimer > 0)
            {
                myInvincibleTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
                DrawPlayer(aGameTime);
            }
            else if (myIsInvincible)
            {
                myInvincibleTimer = 0;
                myIsInvincible = false;
                myDrawSprite = true;
            }
        }
        private void SuperPowerUp(GameTime aGameTime)
        {
            if (mySuperPowerUpTimer > 0)
            {
                mySuperPowerUpTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
                if (mySuperPowerUpTimer < (mySuperPowerUpDelay / 4))
                {
                    DrawPlayer(aGameTime);
                }
            }
            else if (myIsSuperForm)
            {
                mySize = mySaveSize;
                myIsSuperForm = false;
                mySuperPowerUpTimer = 0;

                myDrawSprite = true;

                myPosition.Y += (myBigSize.Y - mySize.Y);
            }
        }
        private void FirePowerUp(GameTime aGameTime)
        {
            if (myFirePowerUpTimer > 0)
            {
                myFirePowerUpTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
                if (myFirePowerUpTimer < (myFirePowerUpDelay / 4))
                {
                    DrawPlayer(aGameTime);
                }
            }
            else if (myIsFireForm)
            {
                mySize = mySaveSize;
                myIsFireForm = false;
                myFirePowerUpTimer = 0;

                myDrawSprite = true;

                myPosition.Y += (myBigSize.Y - mySize.Y);
            }
        }
        private void DrawPlayer(GameTime aGameTime)
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

        private void TextureState()
        {
            if (myIsSuperForm)
            {
                myLoadTexture = "-Super";
            }
            if (myIsFireForm)
            {
                myLoadTexture = "-Fire";
            }

            if (!myIsSuperForm && !myIsFireForm)
            {
                myLoadTexture = string.Empty;
            }

            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (myIsMoving)
                    {
                        myTexture = ResourceManager.RequestTexture("Mario" + myLoadTexture + "_Walking");
                    }
                    else
                    {
                        myTexture = ResourceManager.RequestTexture("Mario" + myLoadTexture + "_Idle");
                    }
                    break;
                case PlayerState.isClimbing:
                    myTexture = ResourceManager.RequestTexture("Mario" + myLoadTexture + "_Climbing");
                    break;
                case PlayerState.isFalling:
                    myTexture = ResourceManager.RequestTexture("Mario" + myLoadTexture + "_Jumping");
                    break;
                case PlayerState.isDead:
                    myTexture = ResourceManager.RequestTexture("Mario_Death");
                    break;
            }
        }
    }
}
