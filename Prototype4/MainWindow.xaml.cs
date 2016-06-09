using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Tobii.EyeTracking.IO;

namespace Prototype4
{

    public enum WindowType
    {
        Map,Scroll,Swipe
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _zoomActionButtonDown;
        private bool _pauseButtonDown;
        
        private readonly EyeTrackerBrowser _browser;

        private bool _tracking;
        private IEyeTracker _tracker;

        private Point _leftGaze;
        private Point _rightGaze;
        private Point3D _headPos;

        private Point _current;
        private Point3D _initialHeadPos;

        private ZoneEnum zoneEnum;

        private WindowType windowType;

        public MainWindow()
        {
            InitializeComponent();

            Library.Init();

            _browser = new EyeTrackerBrowser();
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;

            MapControll.ZoneHaveChanged += MapControll_ZoneHaveChanged;

            _initialHeadPos = new Point3D(0, 0, 0);
            _headPos = new Point3D(0, 0, 0);

            windowType = WindowType.Map;


        }

        private void MapControll_ZoneHaveChanged(object sender, EventArgs e)
        {

        }

        //private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (MapControll.GetMapElement() != null && _headPos.Z > 0)
        //    {
        //        _zoomActionButtonDown = true;
        //        _initialHeadPos.Z = _headPos.Z;
        //    }
        //}

        //private void MainWindow_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (MapControll.GetMapElement() != null)
        //    {
        //        _zoomActionButtonDown = false;
        //    }
        //}

        //private void MainWindow_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    _pauseButtonDown = !_pauseButtonDown;

        //    if (_pauseButtonDown)
        //    {
        //        var zoneIndex = MapControll.getCurrentZone();
        //        ScrollControl.zoneChanged(zoneIndex);
        //        SwipeControl.HandleZoneChange(zoneIndex);
        //    }
        //}

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

            //_leftGaze.X = gd.LeftGazePoint2D.X * Width;
            //_leftGaze.Y = gd.LeftGazePoint2D.Y * Height;

            //_rightGaze.X = gd.RightGazePoint2D.X * Width;
            //_rightGaze.Y = gd.RightGazePoint2D.Y * Height;

            _leftGaze.X = gd.LeftGazePoint2D.X * 1920;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * 1200;

            _rightGaze.X = gd.RightGazePoint2D.X * 1920;
            _rightGaze.Y = gd.RightGazePoint2D.Y * 1200;

            if (!GazeHelper.SetCurrentPoint(ref _current, _leftGaze, _rightGaze))
                return;

            _current = PointFromScreen(_current);


            _headPos.Z = gd.LeftEyePosition3D.Z / 10;

            switch (windowType)
            {
                case WindowType.Map:
                    var zoomFactor = _headPos.Z - _initialHeadPos.Z;
                    MapControll.MapInteraction(_zoomActionButtonDown, _current, zoomFactor);
                    break;
                case WindowType.Swipe:

                    SwipeControl.MapInteraction(_current);
                    break;
                case WindowType.Scroll:
                    ScrollControl.MapInteraction(_current);
                    break;
            }
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.X:
                    switch (windowType)
                    {
                        case WindowType.Map:
                            windowType = WindowType.Swipe;
                            MapControll.SetInFocus(false);
                            var zoneIndex = MapControll.GetCurrentZone();
                            ScrollControl.ZoneChanged(zoneIndex);
                            SwipeControl.HandleZoneChange(zoneIndex);
                            SwipeControl.SetInFocus();
                            break;
                        case WindowType.Swipe:
                            windowType = WindowType.Scroll;
                            SwipeControl.SetInFocus(false);
                            ScrollControl.SetInFocus();
                            break;
                        case WindowType.Scroll:
                            windowType = WindowType.Map;
                            ScrollControl.SetInFocus(false);
                            MapControll.SetInFocus();
                            break;
                    }
                    break;
                case Key.Z:
                    _zoomActionButtonDown = false;
                    _initialHeadPos.Z = 0;
                    break;
            }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z)
            {
                _zoomActionButtonDown = true;
                if (_initialHeadPos.Z == 0)
                {
                    _initialHeadPos = _headPos;
                }
            }
        }
    };
}
