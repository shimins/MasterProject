using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Maps.MapControl.WPF;
using Tobii.EyeTracking.IO;

namespace ImageControl
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
        private Point3D _headPos;

        private Point2D _current;
        private Point2D _previous;


        public MainWindow()
        {
            InitializeComponent();
            //Set the map mode to Aerial with labels
            map.Mode = new AerialMode(false);

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


                    _tracker.StartTracking();

                    _tracker.GazeDataReceived += _tracker_GazeDataReceived;

                    _trackButton.Content = "Stop";
                    _tracking = true;
                }

            }
        }

        private void _tracker_GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            // Convert to centimeters
            var gd = e.GazeDataItem;

            _leftGaze.X = gd.LeftGazePoint2D.X * Width;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * Height;

            _rightGaze.X = gd.RightGazePoint2D.X * Width;
            _rightGaze.Y = gd.RightGazePoint2D.Y * Height;


            if ((_leftGaze.X < 0 && _rightGaze.X < 0) || gd.LeftEyePosition3D.Z < 0) return;
            if (_leftGaze.X > 0 && _rightGaze.X > 0)
            {
                _current = new Point2D((_leftGaze.X + _rightGaze.X) / 2, (_leftGaze.Y + _rightGaze.Y) / 2);
            }
            else if (_rightGaze.X > 0)
            {
                _current = new Point2D(_rightGaze.X, _rightGaze.Y);
            }
            else if (_leftGaze.X > 0)
            {
                _current = new Point2D(_leftGaze.X, _leftGaze.Y);
            }
            if (GazeHaveMoved(_current))
            {
                _previous = _current;
            }
            InvalidateVisual();
        }

        private bool GazeHaveMoved(Point2D currentPoint)
        {
            if (Math.Abs(_previous.X - currentPoint.X) > 100 || Math.Abs(_previous.Y - currentPoint.Y) > 100)
            {
                return true;
            }
            return false;
        }

    }
}