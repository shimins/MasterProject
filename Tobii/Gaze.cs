using System;
using Tobii.EyeTracking.IO;

namespace Tobii
{
    public class Gaze
    {
        private Point2D _current;
        private Point2D _previous;

        private const double MoveLength = 0.03;

        public Gaze()
        {
            _current = new Point2D();
            _previous = new Point2D();
        }

        public void OnGazeData(Point2D leftPoint, Point2D rightPoint)
        {
            if (!(leftPoint.X > -1.0))
                return;

            _current = new Point2D((leftPoint.X + rightPoint.X) / 2, (leftPoint.Y + rightPoint.Y) / 2);
            if (!GazeHaveMoved(_current))
                return;

            _previous = _current;
        }

        public int GetX(double width)
        {
            return (int) (_previous.X * width);
        }
        public int GetY(double height)
        {
            return (int) (_previous.Y * height);
        }

        private bool GazeHaveMoved(Point2D currentPoint)
        {
            return Math.Abs(_previous.X - currentPoint.X) > MoveLength || Math.Abs(_previous.Y - currentPoint.Y) > MoveLength;
        }

        public void Clear()
        {
            _current.X = 0;
            _current.Y = 0;
        }
    }
}
