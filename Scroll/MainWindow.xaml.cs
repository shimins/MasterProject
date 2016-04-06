using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using mshtml;
using Tobii.EyeTracking.IO;

namespace Scroll
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly EyeTrackerBrowser _browser;

        private bool _tracking;
        private IEyeTracker _tracker;

        private Point _leftGaze;
        private Point _rightGaze;

        private Point _current;
        private Point _previous;

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);


        public MainWindow()
        {
            InitializeComponent();

            Library.Init();

            _browser = new EyeTrackerBrowser();
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;
        }

        private void ScrollAction(int scrollSpeed)
        {
            var scrollFactor = _current.Y > Height/2 ? 1: -1;

            var html = Browser.Document as mshtml.HTMLDocument;
            html.parentWindow.scrollBy(0, scrollFactor * scrollSpeed);
            //html.parentWindow.scrollBy(0, 100);
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

            if (_tracker != null)
            {
                _tracker.Dispose();
            }
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

            var doc = Browser.Document as mshtml.HTMLDocument;
            if (doc != null)
            {
                IHTMLStyleSheet style = doc.createStyleSheet("", 0);
                style.cssText = @"body { cursor: none; } a, > a, a:-webkit-any-link { cursor: none !important; } a:hover { cursor: none !important; }  ";
            }
        }

        private int ScrollSpeed()
        {
            if (_current.Y > Height*0.75 || _current.Y < Height*0.25)
            {
                if (_current.Y > Height*0.9 || _current.Y < Height*0.10)
                {
                    return 10;
                }
                return 3;
            }
            return 1;
        }

        private void _tracker_GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            // Convert to centimeters
            var gd = e.GazeDataItem;

            _leftGaze.X = gd.LeftGazePoint2D.X * 1920;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * 1200;

            _rightGaze.X = gd.RightGazePoint2D.X * 1920;
            _rightGaze.Y = gd.RightGazePoint2D.Y * 1200;

            if (!SetCurrentPoint(ref _current, _leftGaze, _rightGaze))
                return;
            _current = PointFromScreen(_current);

            if (GazeHaveMoved(_current))
            {
                MoveCursor();
            }
            if (_current.Y > Height * 0.72 || _current.Y < Height * 0.28)
            {
                var scrollSpeed = ScrollSpeed();
                ScrollAction(scrollSpeed);
            }

            _previous = _current;
        }

        private void MoveCursor()
        {
            if (Browser != null)
            {
                var point = PointToScreen(_current);
                SetCursorPos(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
                Console.WriteLine(_current);
            }
        }

        private bool GazeHaveMoved(Point currentPoint)
        {
            // For swipe events we only check for changes in X coordinates
            if (Math.Abs(_previous.X - currentPoint.X) > 20 || Math.Abs(_previous.Y - currentPoint.Y) > 20)
                return true;
            return false;
        }

        private static bool SetCurrentPoint(ref Point currentPoint, Point leftGaze, Point rightGaze)
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
                currentPoint = new Point(rightGaze.X, rightGaze.Y);
                return true;
            }
            if (leftGaze.X > 0)
            {
                currentPoint = new Point(leftGaze.X, leftGaze.Y);
                return true;
            }
            return false;
        }

        private void Browser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            textBlock.Text = Browser.Source.ToString();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (textBlock.Text != Browser.Source.ToString())
            {
                Browser.Navigate(textBlock.Text);
            }
        }

        private void Browser_OnMouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void Browser_OnMouseLeave(object sender, MouseEventArgs e)
        {
            //Browser.Cursor = Cursors.Arrow;
        }

        private void Browser_OnMouseMove(object sender, MouseEventArgs e)
        {
            var html = Browser.Document as mshtml.HTMLDocument;

        }
    }
}
