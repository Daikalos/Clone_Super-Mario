using Microsoft.Xna.Framework;

namespace Super_Mario
{
    static class CollisionManager
    {
        public static bool Collision(Rectangle aRectangle1, Rectangle aRectangle2)
        {
            return aRectangle1.Intersects(aRectangle2);
        }

        public static bool CheckAbove(Rectangle aRectangle1, Rectangle aRectangle2, Vector2 aVelocity)
        {
            return
                aRectangle1.Top + aVelocity.Y < aRectangle2.Bottom &&
                aRectangle1.Bottom > aRectangle2.Bottom &&
                aRectangle1.Right > aRectangle2.Left &&
                aRectangle1.Left < aRectangle2.Right;
        }

        public static bool CheckBelow(Rectangle aRectangle1, Rectangle aRectangle2, Vector2 aVelocity)
        {
            return 
                aRectangle1.Bottom + aVelocity.Y > aRectangle2.Top &&
                aRectangle1.Top < aRectangle2.Top &&
                aRectangle1.Right > aRectangle2.Left &&
                aRectangle1.Left < aRectangle2.Right;
        }

        public static bool CheckLeft(Rectangle aRectangle1, Rectangle aRectangle2, Vector2 aVelocity)
        {
            return
                aRectangle1.Left + aVelocity.X < aRectangle2.Right &&
                aRectangle1.Right > aRectangle2.Right &&
                aRectangle1.Bottom > aRectangle2.Top &&
                aRectangle1.Top < aRectangle2.Bottom;
        }

        public static bool CheckRight(Rectangle aRectangle1, Rectangle aRectangle2, Vector2 aVelocity)
        {
            return
                aRectangle1.Right + aVelocity.X > aRectangle2.Left &&
                aRectangle1.Left < aRectangle2.Left &&
                aRectangle1.Bottom > aRectangle2.Top &&
                aRectangle1.Top < aRectangle2.Bottom;
        }
    }
}
