using System;
using System.Diagnostics;
using System.Windows;
using Tobii.EyeTracking.IO;

namespace Swipe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EyeTrackerBrowser _browser;

        private bool _tracking;
        private IEyeTracker _tracker;

        private Point2D _leftGaze;
        private Point2D _rightGaze;

        private Point _current;
        private Point _previous;

        private bool _isSwipeAllowed = true;
        private readonly Stopwatch _sw = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            Library.Init();

            _browser = new EyeTrackerBrowser(EventThreadingOptions.CallingThread);
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;

            _sw.Start();
        }


        void _browser_EyetrackerUpdated(object sender, EyeTrackerInfoEventArgs e)
        {
            int index = _trackerCombo.Items.IndexOf(e);

            if (index >= 0)
            {
                _trackerCombo.Items[index] = e.EyeTrackerInfo;
            }
        }

        void _browser_EyetrackerRemoved(object sender, EyeTrackerInfoEventArgs e)
        {
            _trackerCombo.Items.Remove(e.EyeTrackerInfo);
        }

        private void _browser_EyetrackerFound(object sender, EyeTrackerInfoEventArgs e)
        {
            _trackerCombo.Items.Add(e.EyeTrackerInfo);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _browser.StartBrowsing();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _browser.StopBrowsing();

            _tracker?.Dispose();
        }

        private void _trackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_tracking)
            {
                _tracker.GazeDataReceived -= _tracker_GazeDataReceived;
                _tracker.StopTracking();

                _tracker.Dispose();
                _trackButton.Content = "Track";
                _tracking = false;
            }
            else
            {
                EyeTrackerInfo etInfo = _trackerCombo.SelectedItem as EyeTrackerInfo;
                if (etInfo != null)
                {
                    _tracker = etInfo.Factory.CreateEyeTracker();

                    _tracker.StartTracking();

                    _tracker.GazeDataReceived += _tracker_GazeDataReceived;
                    //_tracker.TrackBoxChanged += _tracker_TrackBoxChanged;             

                    _trackButton.Content = "Stop";
                    _tracking = true;
                }

            }
        }

        private void _tracker_GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            var gd = e.GazeDataItem;

            _leftGaze.X = gd.LeftGazePoint2D.X * 1920;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * 1200;

            _rightGaze.X = gd.RightGazePoint2D.X * 1920;
            _rightGaze.Y = gd.RightGazePoint2D.Y * 1200;

            if (!GazeHelper.SetCurrentPoint(ref _current, _leftGaze, _rightGaze))
                return;

            _current = PointFromScreen(_current);

            if (GazeHaveMoved(_current) && _sw.ElapsedMilliseconds > 750)
            {
                if (IsGazeLeftSide() && _isSwipeAllowed)
                {
                    // SWIPE RIGHT ~>>~>>~>> (PREV)
                    Debug.WriteLine("Prev");

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
                    Debug.WriteLine("Next");
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

                Debug.WriteLine(_sw.ElapsedMilliseconds);
                _sw.Restart();
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
    }
}
