using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Super_Mario
{
    class Button : StaticObject
    {
        private SpriteFont my8bitFont;
        private Rectangle myOffset;
        private OnClick myIsClicked; //Uncertain about naming
        private string myDisplayText;

        public Button(Vector2 aPosition, Point aSize, OnClick aClickFunction, string aDisplayText) : base(aPosition, aSize)
        {
            this.myIsClicked = aClickFunction;
            this.myDisplayText = aDisplayText;

            this.myOffset = new Rectangle(
                (int)aPosition.X - (aSize.X / 96),
                (int)aPosition.Y - (aSize.Y / 96),
                aSize.X + (aSize.X / 48),
                aSize.Y + (aSize.Y / 48));
        }

        public override void Update()
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            if (IsClicked())
            {
                myIsClicked();
            }
            if (IsHold())
            {
                myBoundingBox = myOffset;
            }
        }

        public override void Draw(SpriteBatch aSpriteBatch)
        {
            Rectangle tempDrawRect = new Rectangle((int)Camera.Position.X + myBoundingBox.X, myBoundingBox.Y,
                myBoundingBox.Width, myBoundingBox.Height);

            aSpriteBatch.Draw(myTexture, tempDrawRect, null, Color.White);
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, myDisplayText, tempDrawRect.Center.ToVector2(), Color.Black, 1.1f);
        }

        protected bool IsClicked()
        {
            return 
                KeyMouseReader.LeftClick() && 
                myBoundingBox.Contains(KeyMouseReader.GetCurrentMouseState.Position);
        }
        protected bool IsHold()
        {
            return myBoundingBox.Contains(KeyMouseReader.GetCurrentMouseState.Position);
        }

        public delegate void OnClick();

        public static void Play(MainGame aGame, GameWindow aWindow)
        {
            aGame.ChangeState(new PlayState(aGame, aWindow));
        }
        public static void Back(MainGame aGame, GameWindow aWindow)
        {
            aGame.ChangeState(new MenuState(aGame, aWindow));
        }
        public static void Editor(MainGame aGame)
        {
            aGame.ChangeState(new EditorState(aGame));
        }
        public static void Leaderboard(MainGame aGame)
        {
            aGame.ChangeState(new LeaderboardState(aGame));
        }
        public static void Exit(MainGame aGame)
        {
            aGame.Exit();
        }

        public void LoadContent()
        {
            myTexture = ResourceManager.RequestTexture("Border");
            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
