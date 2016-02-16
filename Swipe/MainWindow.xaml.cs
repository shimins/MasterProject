using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
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

        private Point2D _current;
        private Point2D _previous;

        public MainWindow()
        {
            InitializeComponent();

            Library.Init();

            _browser = new EyeTrackerBrowser();
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;
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

                    //UpdateTrackBox();
                    //UpdateScreen();

                    _tracker.StartTracking();

                    _tracker.GazeDataReceived += _tracker_GazeDataReceived;
                    //_tracker.TrackBoxChanged += _tracker_TrackBoxChanged;             

                    _trackButton.Content = "Stop";
                    _tracking = true;
                }

            }
        }

        private int leftCount = 0;
        private int rightCount = 0;

        private double temp = 0;


        private void _tracker_GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            // Convert to centimeters
            var gd = e.GazeDataItem;

            var widthFactor = 1920/ActualWidth;
            var heightFactor = 1200/ActualHeight;

            _leftGaze.X = gd.LeftGazePoint2D.X * 1920;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * 1200;

            _rightGaze.X = gd.RightGazePoint2D.X * 1920;
            _rightGaze.Y = gd.RightGazePoint2D.Y * 1200;

            if (!GazeHelper.SetCurrentPoint(ref _current, _leftGaze, _rightGaze))
                return;

            if (GazeHaveMoved(_current))
            {
                // if(GazeIsLeftSide(_current) {
                // 
                //  }


                //if (_current.X > _previous.X)
                //{
                //    if (ImageContainer.SelectedIndex == 0)
                //    {
                //        Debug.WriteLine("Returning 1");
                //        _previous = _current;
                //        return;
                //    }

                //    // SWIPE RIGHT ~>>~>>~>> (PREV)
                //    Debug.WriteLine("Prev");
                //    if (rightCount++ > 2)
                //    {
                //        rightCount = 0;
                //        leftCount = 0;

                //        ImageContainer.SelectedIndex--;
                //        //ImageContainer.RunSlideAnimation(-ActualWidth, _current.X);
                //        ImageContainer.RunSlideAnimation(ActualWidth);
                //    }
                //    else
                //    {
                //        temp = _current.X;
                //        //ImageContainer.RunSlideAnimation(_previous.X, _current.X);
                //        rightCount++;
                //    }
                //}
                //else
                //{
                //    if (ImageContainer.SelectedIndex == ImageContainer.Items.Count - 1)
                //    {
                //        Debug.WriteLine("Returning 22");
                //        _previous = _current;
                //        return;
                //    }

                //    // SWIPE LEFT <<~<<~<<~ (NEXT)
                //    Debug.WriteLine("Next");
                //    if (leftCount++ > 2)
                //    {
                //        rightCount = 0;
                //        leftCount = 0;

                //        ImageContainer.SelectedIndex++;
                //        //ImageContainer.RunSlideAnimation(ActualWidth, _previous.X);
                //        ImageContainer.RunSlideAnimation(-ActualWidth);
                //    }
                //    else
                //    {
                //        //ImageContainer.RunSlideAnimation(_current.X, _previous.X);
                //        leftCount++;
                //    }
                //}


                _previous = _current;
            }
            InvalidateVisual();
        }



        protected override void OnRender(DrawingContext drawingContext)
        {
            const int length = 150;
            Point leftPoint = new Point(0, 0);
            Point rightPoint = new Point(ActualWidth-length, 0);

            base.OnRender(drawingContext);

            drawingContext.DrawRectangle(Brushes.Coral, null,  new Rect(leftPoint, new Size(length, ActualHeight)));
            drawingContext.DrawRectangle(Brushes.Coral, null,  new Rect(rightPoint, new Size(length, ActualHeight)));

            var point = new Point((int)_current.X, (int)_current.Y);
            drawingContext.DrawEllipse(Brushes.Transparent, new Pen(Brushes.Red, 5), point, 125, 125);
            drawingContext.DrawEllipse(Brushes.WhiteSmoke, new Pen(Brushes.Transparent, 5), point, 3, 3);
            drawingContext.DrawText(new FormattedText(PointFromScreen(point).ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Calibri"), 15, Brushes.CornflowerBlue), new Point(point.X, point.Y-80));
            drawingContext.DrawText(new FormattedText(PointToScreen(point).ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Calibri"), 15, Brushes.CornflowerBlue), new Point(point.X, point.Y-60));
            drawingContext.DrawText(new FormattedText(point.X + " - " + point.Y, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Calibri"), 15, Brushes.DarkMagenta), point);
        }

        private bool GazeHaveMoved(Point2D currentPoint)
        {
            if (Math.Abs(_previous.X - currentPoint.X) > 200 /*|| Math.Abs(_previous.Y - currentPoint.Y) > 75 */)
            {
                return true;
            }
            return false;
        }
    }
}
