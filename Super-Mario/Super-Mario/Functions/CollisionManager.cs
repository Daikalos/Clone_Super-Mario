using Microsoft.Xna.Framework;

namespace Super_Mario
{
    static class CollisionManager
    {
        public static bool Collision(Rectangle aRectangle1, Rectangle aRectangle2) //Does not serve a direct purpose atm 
        {
            if (aRectangle1.Intersects(aRectangle2))
            {
                return true;
            }
            return false;
        }

        public static bool CheckAbove(Rectangle aRectangle1, Rectangle aRectangle2)
        {
            if (aRectangle1.Bottom > aRectangle1.Top && aRectangle1.Bottom < aRectangle2.Bottom)
            {
                return true;
            }
            return false;
        }

        public static bool CheckBelow(Rectangle aRectangle1, Rectangle aRectangle2)
        {
            if (aRectangle1.Top < aRectangle2.Bottom && aRectangle1.Top > aRectangle2.Top)
            {
                return true;
            }
            return false;
        }

        public static bool CheckLeft(Rectangle aRectangle1, Rectangle aRectangle2)
        {
            if (aRectangle1.Left < aRectangle2.Right && aRectangle1.Left > aRectangle2.Left)
            {
                return true;
            }
            return false;
        }

        public static bool CheckRight(Rectangle aRectangle1, Rectangle aRectangle2)
        {
            if (aRectangle1.Right > aRectangle2.Left && aRectangle1.Right < aRectangle2.Right)
            {
                return true;
            }
            return false;
        }
    }
}
