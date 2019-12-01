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
        private float myTextSize;

        public string DisplayText
        {
            get => myDisplayText;
        }

        public Button(Vector2 aPosition, Point aSize, OnClick aClickFunction, string aDisplayText, float aTextSize) : base(aPosition, aSize)
        {
            this.myIsClicked = aClickFunction;
            this.myDisplayText = aDisplayText;
            this.myTextSize = aTextSize;

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
                myIsClicked?.Invoke();
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
            StringManager.DrawStringMid(aSpriteBatch, my8bitFont, myDisplayText, tempDrawRect.Center.ToVector2(), Color.Black, myTextSize);
        }

        public bool IsClicked()
        {
            return 
                KeyMouseReader.LeftClick() && 
                myBoundingBox.Contains(KeyMouseReader.GetCurrentMouseState.Position);
        }
        public bool IsHold()
        {
            return myBoundingBox.Contains(KeyMouseReader.GetCurrentMouseState.Position);
        }

        public delegate void OnClick();

        public static void Back(MainGame aGame, GameWindow aWindow)
        {
            aGame.ChangeState(new MenuState(aGame, aWindow));
        }
        public static void Editor(MainGame aGame, GameWindow aWindow)
        {
            aGame.ChangeState(new EditorState(aGame, aWindow));
        }
        public static void Leaderboard(MainGame aGame)
        {
            aGame.ChangeState(new LeaderboardState(aGame));
        }
        public static void Exit(MainGame aGame)
        {
            aGame.Exit();
        }
        public static void LoadLevel()
        {

        }

        public void LoadContent()
        {
            myTexture = ResourceManager.RequestTexture("Border");
            my8bitFont = ResourceManager.RequestFont("8-bit");
        }
    }
}
