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

        private Point origin;
        private Point center;
        private UIElement child = null;
        private bool actionButtonDown;

        private readonly EyeTrackerBrowser _browser;

        private bool _tracking;
        private IEyeTracker _tracker;

        private Point2D _leftGaze;
        private Point2D _rightGaze;
        private Point3D _headPos;

        private Point2D _current;
        private Point2D _previous;

        private Point3D _initialHeadPos;


        public MainWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            Library.Init();

            _browser = new EyeTrackerBrowser();
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;

            _initialHeadPos = new Point3D(0,0,0);
            _headPos = new Point3D(0, 0, 0);
            center = new Point(960, 600);

            child = Image;
            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            child.RenderTransform = group;
            child.RenderTransformOrigin = new Point(0.5, 0.5);
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



                double zoom = zoomFactor > 0 ? -.01 : .01;
                if (st.ScaleX < .4 || st.ScaleY < .4)
                    return;


                Point relative = new Point();

                var abosuluteX = relative.X * st.ScaleX + tt.X;
                var abosuluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += st.ScaleX*zoom;
                st.ScaleY += st.ScaleY*zoom;

                tt.X = abosuluteX - relative.X * st.ScaleX;
                tt.Y = abosuluteY - relative.Y * st.ScaleY;
            }
        }

        private void EyeMoveDuringAction(Point point)
        {
            if (child != null)
            {
                //Console.WriteLine(point);
                //var tt = getTransform(child);
                //Vector vector = point - center;
                //tt.X += vector.X;
                //tt.Y += vector.Y;
                //origin = new Point(tt.X, tt.Y);
            }
        }

        
        private void ActionButtonDown(object sender, MouseButtonEventArgs eventArgs)
        {
            if (child != null && _headPos.Z > 0)
            {
                actionButtonDown = true;

                _initialHeadPos.Z = _headPos.Z;

                //var tt = getTransform(child);
                //origin = new Point(tt.X, tt.Y);
            }
        }

        //does nothing right now
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

            _leftGaze.X = gd.LeftGazePoint2D.X * Width;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * Height;

            _rightGaze.X = gd.RightGazePoint2D.X * Width;
            _rightGaze.Y = gd.RightGazePoint2D.Y * Height;

            
            if ((_leftGaze.X < 0 && _rightGaze.X < 0 )|| gd.LeftEyePosition3D.Z < 0) return;
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
            if (actionButtonDown)
            {
                if (GazeHaveMoved(_current))
                {
                    _previous = _current;
                    var point = new Point(_current.X, _current.Y);
                    EyeMoveDuringAction(point);
                }
                if (HeadHaveMoved(_initialHeadPos.Z))
                {
                    var zoomFactor = _headPos.Z - _initialHeadPos.Z;
                    zoom_event(zoomFactor);
                }
                InvalidateVisual();
            }
            _headPos.Z = gd.LeftEyePosition3D.Z / 10;
        }

        private bool HeadHaveMoved(double initialPosition)
        {
            //TODO forrandre int til hva enn du mener er komfortabel nok til å telle som head movement
            if (Math.Abs(_headPos.Z - initialPosition) > 5)
            {
                return true;
            }
            return false;
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
