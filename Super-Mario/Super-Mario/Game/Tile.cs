using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class Tile : StaticObject
    {
        private List<Tile> myHistory; //Used for pathfinding
        private char myTileType;
        private int myTileForm;

        public List<Tile> History
        {
            get => myHistory;
            set => myHistory = value;
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
            return new Rectangle(myBoundingBox.X - (int)myOrigin.X, myBoundingBox.Y - (int)myOrigin.Y, mySize.X, mySize.Y).Center.ToVector2();
        }

        public Tile(Vector2 aPosition, Point aSize) : base(aPosition, aSize)
        {
            this.myTileForm = 0;
            this.myOrigin = Vector2.Zero;
            this.myBoundingBox = new Rectangle((int)aPosition.X, (int)aPosition.Y, aSize.X, aSize.Y);
        }

        public override void Update()
        {
            myBoundingBox = new Rectangle((int)myPosition.X + (int)myOrigin.X, (int)myPosition.Y + (int)myOrigin.Y, mySize.X, mySize.Y);
        }

        public override void Draw(SpriteBatch aSpriteBatch)
        {
            if (myTexture != null)
            {
                aSpriteBatch.Draw(myTexture, myBoundingBox,
                    null, Color.White, 0.0f, myOrigin, SpriteEffects.None, 0.0f);
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
                case '#':
                    myTexture = ResourceManager.RequestTexture("Grass-0" + myTileForm.ToString());
                    break;
                default:
                    myTexture = null;
                    break;
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
                case '#':
                    myTexture = ResourceManager.RequestTexture("Grass-0" + myTileForm.ToString());
                    break;
                default:
                    myTexture = null;
                    break;
            }
        }
    }
}
