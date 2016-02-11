﻿using System;
using System.Diagnostics;
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

            _leftGaze.X = gd.LeftGazePoint2D.X * Width;
            _leftGaze.Y = gd.LeftGazePoint2D.Y * Height;

            _rightGaze.X = gd.RightGazePoint2D.X * Width;
            _rightGaze.Y = gd.RightGazePoint2D.Y * Height;

            if (_leftGaze.X < 0 && _rightGaze.X < 0) return;
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
                // if(GazeIsLeftSide(_current) {
                // 
                //  }


                if (_current.X > _previous.X)
                {
                    if (ImageContainer.SelectedIndex == 0)
                    {
                        Debug.WriteLine("Returning 1");
                        _previous = _current;
                        return;
                    }

                    // SWIPE RIGHT ~>>~>>~>> (PREV)
                    Debug.WriteLine("Prev");
                    if (rightCount++ > 2)
                    {
                        rightCount = 0;
                        leftCount = 0;

                        ImageContainer.SelectedIndex--;
                        //ImageContainer.RunSlideAnimation(-ActualWidth, _current.X);
                        ImageContainer.RunSlideAnimation(ActualWidth);
                    }
                    else
                    {
                        temp = _current.X;
                        //ImageContainer.RunSlideAnimation(_previous.X, _current.X);
                        rightCount++;
                    }
                }
                else
                {
                    if (ImageContainer.SelectedIndex == ImageContainer.Items.Count - 1)
                    {
                        Debug.WriteLine("Returning 22");
                        _previous = _current;
                        return;
                    }

                    // SWIPE LEFT <<~<<~<<~ (NEXT)
                    Debug.WriteLine("Next");
                    if (leftCount++ > 2)
                    {
                        rightCount = 0;
                        leftCount = 0;

                        ImageContainer.SelectedIndex++;
                        //ImageContainer.RunSlideAnimation(ActualWidth, _previous.X);
                        ImageContainer.RunSlideAnimation(-ActualWidth);
                    }
                    else
                    {
                        //ImageContainer.RunSlideAnimation(_current.X, _previous.X);
                        leftCount++;
                    }
                }

                _previous = _current;
            }
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var point = new Point(_current.X, _current.Y);
            drawingContext.DrawEllipse(Brushes.Transparent, new Pen(Brushes.Red, 10), point, 10, 10);
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
