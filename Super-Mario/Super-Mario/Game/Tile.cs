using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class Tile : StaticObject
    {
        private List<Tile> myHistory; //Used for pathfinding
        private AnimationManager 
            myItemBlockAnimation,
            myGravityBlockAnimation;
        private bool myIsBlock;
        private char myTileType;
        private int myTileForm;

        public List<Tile> History
        {
            get => myHistory;
            set => myHistory = value;
        }

        public bool IsBlock
        {
            get => myIsBlock;
            set => myIsBlock = value;
        }
        public char TileType
        {
            get => myTileType;
            set => myTileType = value;
        }

        public int TileForm
        {
            get => myTileForm;
            set => myTileForm = value;
        }

        public Vector2 GetCenter()
        {
            return new Rectangle(DestRect.X - (int)myOrigin.X, DestRect.Y - (int)myOrigin.Y, mySize.X, mySize.Y).Center.ToVector2();
        }

        public Tile(Vector2 aPosition, Point aSize, char aTileType) : base(aPosition, aSize)
        {
            this.myTileType = aTileType;

            switch(aTileType)
            {
                case '#': case '/': case '^': case '¤':
                    myIsBlock = true;
                    break;
                default:
                    myIsBlock = false;
                    break;
            }

            this.myTileForm = 0;
            this.myOrigin = Vector2.Zero;
            this.myBoundingBox = new Rectangle((int)aPosition.X, (int)aPosition.Y, aSize.X, aSize.Y);
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myTileType)
            {
                case '/':
                    myItemBlockAnimation?.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myBoundingBox, new Point(32), Color.White, 0.0f, myOrigin, SpriteEffects.None);
                    break;
                case '¤':
                    myGravityBlockAnimation?.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myBoundingBox, new Point(32), Color.White, 0.0f, myOrigin, SpriteEffects.None);
                    break;
                default:
                    if (myTexture != null)
                    {
                        aSpriteBatch.Draw(myTexture, myBoundingBox,
                            null, Color.White, 0.0f, myOrigin, SpriteEffects.None, 0.0f);
                    }
                    break;
            }
        }

        public void SetTexture()
        {
            switch(myTileType)
            {
                case '*':
                    myTexture = ResourceManager.RequestTexture("Flag");
                    break;
                case '%':
                    myTexture = ResourceManager.RequestTexture("Ladder");
                    break;
                case '^':
                    myTexture = ResourceManager.RequestTexture("Coin_Block");
                    break;
                case '/':
                    myTexture = ResourceManager.RequestTexture("Item_Block");
                    break;
                case '¤':
                    myTexture = ResourceManager.RequestTexture("Gravity_Block");
                    break;
                case '(':
                    myTexture = ResourceManager.RequestTexture("Empty_Block");
                    break;
                case '=':
                    myTexture = ResourceManager.RequestTexture("Mushroom-00");
                    break;
                case ')':
                    myTexture = ResourceManager.RequestTexture("Mushroom-01");
                    break;
                case ',':
                    myTexture = ResourceManager.RequestTexture("FireFlower");
                    break;
                case '#':
                    myTexture = ResourceManager.RequestTexture("Grass-0" + myTileForm.ToString());
                    break;
                default:
                    myTexture = null;
                    break;
            }

            switch (myTileType)
            {
                case '/':
                    myItemBlockAnimation = new AnimationManager(new Point(4, 1), 0.3f, true);
                    break;
                case '¤':
                    myGravityBlockAnimation = new AnimationManager(new Point(4, 2), 0.07f, true);
                    break;
                default:
                    myItemBlockAnimation = null;
                    myGravityBlockAnimation = null;
                    break;
            }

            if (myTexture != null)
            {
                SetColorData();
            }
        }
        public void SetTextureEditor()
        {
            switch (myTileType)
            {
                case '*':
                    myTexture = ResourceManager.RequestTexture("Flag");
                    break;
                case '?':
                    myTexture = ResourceManager.RequestTexture("Mario_Idle");
                    break;
                case '%':
                    myTexture = ResourceManager.RequestTexture("Ladder");
                    break;
                case '&':
                    myTexture = ResourceManager.RequestTexture("Goomba_Editor");
                    break;
                case '"':
                    myTexture = ResourceManager.RequestTexture("Koopa_Editor");
                    break;
                case '^':
                    myTexture = ResourceManager.RequestTexture("Coin_Block");
                    break;
                case '/':
                    myTexture = ResourceManager.RequestTexture("Item_Block");
                    break;
                case '¤':
                    myTexture = ResourceManager.RequestTexture("Gravity_Block");
                    break;
                case '(':
                    myTexture = ResourceManager.RequestTexture("Empty_Block");
                    break;
                case ',':
                    myTexture = ResourceManager.RequestTexture("FireFlower");
                    break;
                case '#':
                    myTexture = ResourceManager.RequestTexture("Grass-0" + myTileForm.ToString());
                    break;
                default:
                    myTexture = null;
                    break;
            }

            switch (myTileType)
            {
                case '/':
                    myItemBlockAnimation = new AnimationManager(new Point(4, 1), 0.3f, true);
                    break;
                case '¤':
                    myGravityBlockAnimation = new AnimationManager(new Point(4, 2), 0.07f, true);
                    break;
                default:
                    myItemBlockAnimation = null;
                    myGravityBlockAnimation = null;
                    break;
            }

            if (myTexture != null)
            {
                SetColorData();
            }
        }
    }
}
