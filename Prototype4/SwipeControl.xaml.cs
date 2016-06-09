using System;
using System.Diagnostics;
using System.Windows;

namespace Prototype4
{
    /// <summary>
    /// Interaction logic for SwipeControl.xaml
    /// </summary>
    public partial class SwipeControl
    {
        private Point _current;
        private Point _previous;

        private bool _isSwipeAllowed = true;
        private readonly Stopwatch _sw = new Stopwatch();

        public SwipeControl()
        {
            InitializeComponent();
            _sw.Start();
            Border.Visibility = Visibility.Hidden;
        }

        public void HandleZoneChange(int zoneIndex)
        {
            ViewModelTest.SetImagePack(zoneIndex);
            ImageContainer.RefreshViewPort(2);
        }

        public void MapInteraction(Point current)
        {
            _current = current;

            if (GazeHaveMoved(_current) && _sw.ElapsedMilliseconds > 900)
            {
                if (IsGazeLeftSide() && _isSwipeAllowed)
                {
                    // SWIPE RIGHT ~>>~>>~>> (PREV)

                    _isSwipeAllowed = false;

                    if (ImageContainer.SelectedIndex == 0)
                    {
                        ImageContainer.SelectedIndex = ImageContainer.Items.Count - 1;
                    }
                    else
                    {
                        ImageContainer.SelectedIndex--;
                    }
                    ImageContainer.RunSlideAnimation(ActualWidth);

                    _isSwipeAllowed = true;
                    _sw.Restart();
                }
                else if (IsGazeRightSide() && _isSwipeAllowed)
                {
                    // SWIPE LEFT <<~<<~<<~ (NEXT)
                    _isSwipeAllowed = false;

                    if (ImageContainer.SelectedIndex == ImageContainer.Items.Count - 1)
                    {
                        ImageContainer.SelectedIndex = 0;
                    }
                    else
                    {
                        ImageContainer.SelectedIndex++;
                    }
                    ImageContainer.RunSlideAnimation(-ActualWidth);

                    _isSwipeAllowed = true;
                    _sw.Restart();
                }

                _previous = _current;
            }
        }

        private bool GazeHaveMoved(Point currentPoint)
        {
            return Math.Abs(_previous.X - currentPoint.X) > 15 || Math.Abs(_previous.Y - currentPoint.Y) > 25;
        }

        private const int SwipeWidthArea = 75;

        private bool IsGazeLeftSide()
        {
            if (!(_current.X < SwipeWidthArea && _current.X >= 0))
                return false;

            var middle = (Height / 2);
            return _current.Y > middle - SwipeWidthArea && _current.Y < middle+SwipeWidthArea;
        }
        
        private bool IsGazeRightSide()
        {
            if (!(_current.X > Width-SwipeWidthArea && _current.X <= Width))
                return false;

            var middle = (Height / 2);
            return _current.Y > middle-SwipeWidthArea && _current.Y < middle+SwipeWidthArea;
        }

        public void SetInFocus(bool focus = true)
        {
            Border.Visibility = focus ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
