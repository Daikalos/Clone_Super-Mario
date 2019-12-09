using Microsoft.Xna.Framework;

namespace Super_Mario
{
    static class CollisionManager
    {
        public static bool Collision(Rectangle aRectangle1, Rectangle aRectangle2)
        {
            return aRectangle1.Intersects(aRectangle2);
        }

        public static bool PixelCollision(GameObject aObject1, GameObject aObject2)
        {
            if (!aObject1.BoundingBox.Intersects(aObject2.BoundingBox))
            {
                return false;
            }

            float tempColLeft = MathHelper.Max(aObject1.DestRect.X, aObject2.DestRect.X);
            float tempColTop = MathHelper.Max(aObject1.DestRect.Y, aObject2.DestRect.Y);
            float tempColRight = MathHelper.Min(aObject1.DestRect.Right, aObject2.DestRect.Right);
            float tempColBot = MathHelper.Min(aObject1.DestRect.Bottom, aObject2.DestRect.Bottom);

            for (int col = (int)tempColLeft; col < tempColRight; col++)
            {
                for (int row = (int)tempColTop; row < tempColBot; row++)
                {
                    if (aObject1.GetPixel(col, row).A > 127 && aObject2.GetPixel(col, row).A > 127)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool CheckAbove(GameObject aObject1, GameObject aObject2, Vector2 aVelocity)
        {
            return
                aObject1.BoundingBox.Top + aVelocity.Y < aObject2.BoundingBox.Bottom &&
                aObject1.BoundingBox.Bottom > aObject2.BoundingBox.Bottom &&
                aObject1.BoundingBox.Right > aObject2.BoundingBox.Left &&
                aObject1.BoundingBox.Left < aObject2.BoundingBox.Right;
        }

        public static bool CheckBelow(GameObject aObject1, GameObject aObject2, Vector2 aVelocity)
        {
            return 
                aObject1.BoundingBox.Bottom + aVelocity.Y > aObject2.BoundingBox.Top &&
                aObject1.BoundingBox.Top < aObject2.BoundingBox.Top &&
                aObject1.BoundingBox.Right > aObject2.BoundingBox.Left &&
                aObject1.BoundingBox.Left < aObject2.BoundingBox.Right;
        }

        public static bool CheckLeft(GameObject aObject1, GameObject aObject2, Vector2 aVelocity)
        {
            return
                aObject1.BoundingBox.Left + aVelocity.X < aObject2.BoundingBox.Right &&
                aObject1.BoundingBox.Right > aObject2.BoundingBox.Right &&
                aObject1.BoundingBox.Bottom > aObject2.BoundingBox.Top &&
                aObject1.BoundingBox.Top < aObject2.BoundingBox.Bottom;
        }

        public static bool CheckRight(GameObject aObject1, GameObject aObject2, Vector2 aVelocity)
        {
            return
                aObject1.BoundingBox.Right + aVelocity.X > aObject2.BoundingBox.Left &&
                aObject1.BoundingBox.Left < aObject2.BoundingBox.Left &&
                aObject1.BoundingBox.Bottom > aObject2.BoundingBox.Top &&
                aObject1.BoundingBox.Top < aObject2.BoundingBox.Bottom;
        }
    }
}
