using System.Windows;

namespace Prototype4
{
    public static class GazeHelper
    {
        public static bool SetCurrentPoint(ref Point currentPoint, Point leftGaze, Point rightGaze)
        {
            if (leftGaze.X < 0 && rightGaze.X < 0)
                return false;

            if (leftGaze.X > 0 && rightGaze.X > 0)
            {
                currentPoint = new Point((leftGaze.X + rightGaze.X) / 2, (leftGaze.Y + rightGaze.Y) / 2);
                return true;
            }

            if (rightGaze.X > 0)
            {
                currentPoint =  new Point(rightGaze.X, rightGaze.Y);
                return true;
            }
            if (leftGaze.X > 0)
            {
                currentPoint =  new Point(leftGaze.X, leftGaze.Y);
                return true;
            }
            return false;
        }
    }
}
