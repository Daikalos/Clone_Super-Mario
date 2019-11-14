using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class Tile : GameObject
    {
        private List<Tile> myHistory; //Used for pathfinding

        private Rectangle mySourceRect;
        private char myTileType;
        private float myRotation;
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

        public Vector2 GetCenter()
        {
            return new Rectangle(myBoundingBox.X - (int)myOrigin.X, myBoundingBox.Y - (int)myOrigin.Y, mySize.X, mySize.Y).Center.ToVector2();
        }
        public Vector2 Position
        {
            get => myPosition;
            set => myPosition = value;
        }

        public Tile(Vector2 aPosition, Point aSize) : base(aPosition, aSize)
        {
            this.myTileForm = 0;
            this.myOrigin = Vector2.Zero;
            this.myBoundingBox = new Rectangle((int)aPosition.X, (int)aPosition.Y, aSize.X, aSize.Y);
        }

        public void Update()
        {
            myBoundingBox = new Rectangle((int)myPosition.X + (int)myOrigin.X, (int)myPosition.Y + (int)myOrigin.Y, mySize.X, mySize.Y);
        }

        public override void Draw(SpriteBatch aSpriteBatch)
        {
            if (myTexture != null)
            {
                aSpriteBatch.Draw(myTexture, myBoundingBox,
                    mySourceRect, Color.White, myRotation, myOrigin, SpriteEffects.None, 0.0f);
            }
        }

        public void SetTexture()
        {

        }

        public void SetTextureEditor()
        {

        }
    }
}
