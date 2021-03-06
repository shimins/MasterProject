﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MouseMode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly UIElement child = null;
        private bool actionButtonDown;

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
            //this.SizeToContent = SizeToContent.WidthAndHeight;

            Library.Init();

            _browser = new EyeTrackerBrowser();
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;

            _initialHeadPos = new Point3D(0,0,0);
            _headPos = new Point3D(0, 0, 0);

            child = Image;
            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            child.RenderTransform = group;
            child.RenderTransformOrigin = new Point(0.0, 0.0);
        }


        private TranslateTransform getTransform(UIElement element)
        {
            return (TranslateTransform) ((TransformGroup) element.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform getScaleTransform(UIElement element)
        {
            return (ScaleTransform) ((TransformGroup) element.RenderTransform)
                .Children.First(tr => tr is ScaleTransform);
        }

        private void reset()
        {
            if (Image != null)
            {
                var st = getScaleTransform(Image);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                var tt = getTransform(Image);
                tt.X = 0.5;
                tt.Y = 0.5;
            }
        }
        
        private void zoom_event(double zoomFactor)
        {
            if (child != null)
            {


                var st = getScaleTransform(child);
                var tt = getTransform(child);

                double zoom = zoomFactor > 0 ? -.01 * st.ScaleX : .01 * st.ScaleX;
                Console.WriteLine("zoom");

                if (st.ScaleX + zoom > .1 && st.ScaleY + zoom < 5)
                {
                    Point relative = child.PointFromScreen(_current);


                    double abosuluteX = relative.X * st.ScaleX + tt.X;
                    double abosuluteY = relative.Y * st.ScaleY + tt.Y;

                    st.ScaleX += zoom;
                    st.ScaleY += zoom;

                    tt.X = abosuluteX - relative.X * st.ScaleX;
                    tt.Y = abosuluteY - relative.Y * st.ScaleY;
                }
            }
        }

        private void EyeMoveDuringAction()
        {
            if(child == null)return;
            if(child.PointFromScreen(_current).X < 0 || child.PointFromScreen(_current).X > child.RenderSize.Width 
                || child.PointFromScreen(_current).Y < 0 || child.PointFromScreen(_current).X > child.RenderSize.Width)return;
            var tt = getTransform(child);
            tt.X -= (_current.X -  Width / 2) * 0.01;
            tt.Y -= (_current.Y - Height / 2) * 0.01;
        }

        
        private void ActionButtonDown(object sender, MouseButtonEventArgs eventArgs)
        {
            if (child != null && _headPos.Z > 0)
            {
                actionButtonDown = true;

                _initialHeadPos.Z = _headPos.Z;
            }
        }

        private void ActionButtonUp(object sender, MouseButtonEventArgs eventArgs)
        {
            if (child != null)
            {
                actionButtonDown = false;
            }
        }

        private void reset(object sender, MouseButtonEventArgs eventArgs)
        {
            reset();
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
        }

        private void _tracker_GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            // Convert to centimeters
            var gd = e.GazeDataItem;

            _leftGaze.X = gd.LeftGazePoint2D.X* Width;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * Height;

            _rightGaze.X = gd.RightGazePoint2D.X * Width;
            _rightGaze.Y = gd.RightGazePoint2D.Y * Height;

            _headPos.Z = gd.LeftEyePosition3D.Z / 10;

            if ((_leftGaze.X < 0 && _rightGaze.X < 0 )|| _headPos.Z < 0) return;
            if (!SetCurrentPoint(ref _current, _leftGaze, _rightGaze))
                return;
            if ((_current.X > 1400 || _current.X < 500 || _current.Y > 700 || _current.Y < 500))
            {
                EyeMoveDuringAction();
            }
            if (HeadHaveMoved(_initialHeadPos.Z) && actionButtonDown)
            {
                var zoomFactor = _headPos.Z - _initialHeadPos.Z;
                zoom_event(zoomFactor);
            }
        }

        private bool HeadHaveMoved(double initialPosition)
        {
            if (Math.Abs(_headPos.Z - initialPosition) > 1)
            {
                return true;
            }
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
    }
}
