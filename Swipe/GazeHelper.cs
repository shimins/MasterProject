using Tobii.EyeTracking.IO;

namespace Swipe
{
    public static class GazeHelper
    {
        public static bool SetCurrentPoint(ref Point2D currentPoint, Point2D leftGaze, Point2D rightGaze)
        {
            if (leftGaze.X < 0 && rightGaze.X < 0)
                return false;

            if (leftGaze.X > 0 && rightGaze.X > 0)
            {
                currentPoint = new Point2D((leftGaze.X + rightGaze.X) / 2, (leftGaze.Y + rightGaze.Y) / 2);
                return true;
            }

            if (rightGaze.X > 0)
            {
                currentPoint =  new Point2D(rightGaze.X, rightGaze.Y);
                return true;
            }
            if (leftGaze.X > 0)
            {
                currentPoint =  new Point2D(leftGaze.X, leftGaze.Y);
                return true;
            }
            return false;
        }
    }
}
