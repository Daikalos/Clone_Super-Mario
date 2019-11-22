using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class ExitButton : Button
    {
        public ExitButton(Vector2 aPosition, Point aSize) : base(aPosition, aSize)
        {

        }

        public override void Update(MainGame aGame, GameWindow aWindow)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            if (IsClicked())
            {
                aGame.ChangeState(new PlayState(aGame, aWindow));
            }

            if (IsHold())
            {
                myBoundingBox = myOffset;
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch)
        {
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, "Exit", myBoundingBox.Center.ToVector2(), Color.Black, 1.1f);
            aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White);
        }
    }
}
