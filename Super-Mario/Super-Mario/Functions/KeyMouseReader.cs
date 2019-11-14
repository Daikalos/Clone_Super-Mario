using Microsoft.Xna.Framework.Input;

namespace Super_Mario
{
    static class KeyMouseReader
    {
        public static KeyboardState
            myCurrentKeyState,
            myPreviousKeyState = Keyboard.GetState();
        public static MouseState
            myCurrentMouseState,
            myPreviousMouseState = Mouse.GetState();

        public static bool KeyPressed(Keys aKey)
        {
            return myCurrentKeyState.IsKeyDown(aKey) && myPreviousKeyState.IsKeyUp(aKey);
        }
        public static bool KeyHold(Keys aKey)
        {
            return myCurrentKeyState.IsKeyDown(aKey);
        }
        public static KeyboardState GetPreviousState
        {
            get => myPreviousKeyState;
        }


        public static bool LeftClick()
        {
            return myCurrentMouseState.LeftButton == ButtonState.Pressed && myPreviousMouseState.LeftButton == ButtonState.Released;
        }
        public static bool RightClick()
        {
            return myCurrentMouseState.RightButton == ButtonState.Pressed && myPreviousMouseState.RightButton == ButtonState.Released;
        }

        public static bool LeftHold()
        {
            return myCurrentMouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool RightHold()
        {
            return myCurrentMouseState.RightButton == ButtonState.Pressed;
        }

        //Should be called at beginning of Update in Game
        public static void Update()
        {
            myPreviousKeyState = myCurrentKeyState;
            myCurrentKeyState = Keyboard.GetState();

            myPreviousMouseState = myCurrentMouseState;
            myCurrentMouseState = Mouse.GetState();
        }
    }
}