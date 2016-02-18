﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        private Point _current;
        private Point _previous;

        public MainWindow()
        {
            InitializeComponent();

            Library.Init();

            _browser = new EyeTrackerBrowser(EventThreadingOptions.CallingThread);
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;

            sw.Start();
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

        private bool IsSwipeAllowed = true;
        private Stopwatch sw = new Stopwatch();

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

            if (GazeHaveMoved(_current) && sw.ElapsedMilliseconds > 500)
            {
                if (IsGazeLeftSide() && IsSwipeAllowed)
                {
                    if (ImageContainer.SelectedIndex == 0)
                    {
                        Debug.WriteLine("Returning 1");
                        _previous = _current;
                        return;
                    }

                    // SWIPE RIGHT ~>>~>>~>> (PREV)
                    Debug.WriteLine("Prev");

                    IsSwipeAllowed = false;

                    ImageContainer.SelectedIndex--;
                    ImageContainer.RunSlideAnimation(ActualWidth);
                }
                else if (IsGazeRightSide() && IsSwipeAllowed)
                {
                    if (ImageContainer.SelectedIndex == ImageContainer.Items.Count - 1)
                    {
                        Debug.WriteLine("Returning 22");
                        _previous = _current;
                        return;
                    }

                    // SWIPE LEFT <<~<<~<<~ (NEXT)
                    Debug.WriteLine("Next");
                    IsSwipeAllowed = false;

                    ImageContainer.SelectedIndex++;
                    ImageContainer.RunSlideAnimation(-ActualWidth);
                }

                IsSwipeAllowed = true;
                _previous = _current;

                Debug.WriteLine(sw.ElapsedMilliseconds);
                sw.Restart();
            }

            //InvalidateVisual();
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    const int length = 150;
        //    Point leftPoint = new Point(0, 0);
        //    Point rightPoint = new Point(ActualWidth - length, 0);

        //    base.OnRender(drawingContext);

        //    drawingContext.DrawRectangle(Brushes.Coral, null, new Rect(leftPoint, new Size(length, ActualHeight)));
        //    drawingContext.DrawRectangle(Brushes.Coral, null, new Rect(rightPoint, new Size(length, ActualHeight)));

        //    var currentPoint = new Point((int)_current.X, (int)_current.Y);

        //    drawingContext.DrawEllipse(Brushes.Transparent, new Pen(Brushes.Red, 5), currentPoint, 125, 125);
        //    drawingContext.DrawEllipse(Brushes.WhiteSmoke, new Pen(Brushes.Transparent, 5), currentPoint, 3, 3);
        //    drawingContext.DrawText(new FormattedText(currentPoint.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Calibri"), 15, Brushes.CornflowerBlue), new Point(currentPoint.X, currentPoint.Y - 15));
        //}

        private bool IsActionButtonDown = false;

        private void ActionButtonDown(object sender, MouseButtonEventArgs eventArgs)
        {
            if (EnableActionButton.IsChecked.HasValue && EnableActionButton.IsChecked.Value)
                IsActionButtonDown = true;
        }

        private void ActionButtonUp(object sender, MouseButtonEventArgs eventArgs)
        {
            IsActionButtonDown = false;
        }

        private bool GazeHaveMoved(Point currentPoint)
        {
            // For swipe events we only check for changes in X coordinates
            if (Math.Abs(_previous.X - currentPoint.X) > 20 || Math.Abs(_previous.Y - currentPoint.Y) > 20)
                return true;
            return false;
        }

        private const int SwipeWidthArea = 120;

        private bool IsGazeLeftSide()
        {
            if (!(_current.X < SwipeWidthArea && _current.X >= 0))
                return false;

            var middle = (Height / 2);
            return _current.Y > middle - 100 && _current.Y < middle + 100;
        }

        private bool IsGazeRightSide()
        {
            if (!(_current.X > Width - SwipeWidthArea && _current.X <= Width))
                return false;

            var middle = (Height / 2);
            return _current.Y > middle - 100 && _current.Y < middle + 100;
        }

        //private bool IsGazePassedMiddle()
        //{
        //    if (_current.X > _previous.X && _current.X > Width / 2)
        //        return true;
        //    if (_current.X < _previous.X && _current.X < Width / 2)
        //        return true;
        //    return false;
        //}
    }
}
