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
using Tobii.EyeTracking.IO;

namespace Prototype4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool zoomActionButtonDown;
        private bool pauseButtonDown;

        private readonly EyeTrackerBrowser _browser;

        private bool _tracking;
        private IEyeTracker _tracker;

        private Point _leftGaze;
        private Point _rightGaze;
        private Point3D _headPos;

        private Point _current;
        private Point3D _initialHeadPos;

        public MainWindow()
        {
            InitializeComponent();

            Library.Init();

            _browser = new EyeTrackerBrowser();
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;

            _initialHeadPos = new Point3D(0, 0, 0);
            _headPos = new Point3D(0, 0, 0);
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MapControll.GetMapElement() != null && _headPos.Z > 0)
            {
                zoomActionButtonDown = true;
                _initialHeadPos.Z = _headPos.Z;
            }
        }

        private void MainWindow_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MapControll.GetMapElement() != null)
            {
                zoomActionButtonDown = false;
            }
        }

        private void MainWindow_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            pauseButtonDown = !pauseButtonDown;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _browser.StartBrowsing();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _browser.StopBrowsing();

            _tracker?.Dispose();
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

            var gd = e.GazeDataItem;

            _leftGaze.X = gd.LeftGazePoint2D.X * Width;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * Height;

            _rightGaze.X = gd.RightGazePoint2D.X * Width;
            _rightGaze.Y = gd.RightGazePoint2D.Y * Height;


            _headPos.Z = gd.LeftEyePosition3D.Z / 10;



            if ((_leftGaze.X < 0 && _rightGaze.X < 0) || _headPos.Z < 0) return;
            if (!SetCurrentPoint(ref _current, _leftGaze, _rightGaze))
                return;
            if (!pauseButtonDown)
            {
                var zoomFactor = _headPos.Z - _initialHeadPos.Z;
                MapControll.mapInteraction(zoomActionButtonDown, _current, zoomFactor);
            }
            else
            {
                ScrollControl.mapInteraction(_current);
            }

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
    }
}
