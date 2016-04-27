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
            var random = new Random(zoneIndex);
            ImageContainer.SelectedIndex = random.Next(0, ImageContainer.Items.Count);
            ImageContainer.RefreshViewPort(ImageContainer.SelectedIndex);
        }

        public void MapInteraction(Point current)
        {
            _current = current;

            if (GazeHaveMoved(_current) && _sw.ElapsedMilliseconds > 750)
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
                }

                _isSwipeAllowed = true;
                _previous = _current;

                _sw.Restart();
                Debug.WriteLine("Restarting");
            }
        }


        private bool GazeHaveMoved(Point currentPoint)
        {
            // For swipe events we only check for changes in X coordinates
            if (Math.Abs(_previous.X - currentPoint.X) > 10 || Math.Abs(_previous.Y - currentPoint.Y) > 10)
                return true;
            return false;
        }

        private const int SwipeWidthArea = 120;

        private bool IsGazeLeftSide()
        {
            if (!(_current.X < SwipeWidthArea && _current.X >= 0))
                return false;

            var middle = (Height / 2);
            return _current.Y > middle - SwipeWidthArea && _current.Y < middle + SwipeWidthArea;
        }

        private bool IsGazeRightSide()
        {
            if (!(_current.X > Width - SwipeWidthArea && _current.X <= Width))
                return false;

            var middle = (Height / 2);
            return _current.Y > middle - SwipeWidthArea && _current.Y < middle + SwipeWidthArea;
        }

        public void setInFocus(bool focus)
        {
            if (focus)
            {
                Border.Visibility = Visibility.Visible;
            }
            else
            {
                Border.Visibility = Visibility.Hidden;
            }
        }
    }
}
