using System;
using System.Windows;
using System.Windows.Controls;
using BasicEyetrackingSample;
using Tobii.EyeTracking.IO;

namespace WPF
{
    /// <summary>
    /// Interaction logic for ConnectToTracker.xaml
    /// </summary>
    public partial class Tracker : Window
    {
        private readonly EyeTrackerBrowser _browser;

        private IEyeTracker EyeTracker { get; set; }

        

        private Point3D _leftPos;
        private Point3D _rightPos;
        private Point2D _leftGaze;
        private Point2D _rightGaze;

        private Point _point;
        private Point _previousPoint;

        public event EventHandler EyeGazeMovement;
        //private Point3D _leftGaze;
        //private Point3D _rightGaze;


        public Tracker()
        {
            _browser = new EyeTrackerBrowser();
            StartBrowsing();
            _browser.EyeTrackerFound += _browser_EyetrackerFound;
            _browser.EyeTrackerRemoved += _browser_EyetrackerRemoved;
            _browser.EyeTrackerUpdated += _browser_EyetrackerUpdated;
            InitializeComponent();
        }

        private void TrackerCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CaliberationButton.IsEnabled = true;
            TrackButton.IsEnabled = true;
        }

        private void TrackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (GlobalValue.IsTracking == false)
            {
                try
                {
                    var etInfo = TrackerCombo.SelectedItem as EyeTrackerInfo;
                    if (etInfo != null)
                    {
                        StartTracking(etInfo);
                        GlobalValue.IsTracking = true;
                    }
                }
                catch (NullReferenceException)
                {
                    PrintDialog dialog = new PrintDialog();
                    dialog.PrintVisual(this, "No eyetracker were found");
                }
            }
           else if (GlobalValue.IsTracking == true)
            {
                StopTracking();
            }
        }

        private void CaliberationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var runner = new CalibrationRunner();

            try
            {
                // Start a new calibration procedure
                var etInfo = TrackerCombo.SelectedItem as EyeTrackerInfo;
                var result = runner.RunCalibration(etInfo.Factory.CreateEyeTracker());

                // Show a calibration plot if everything went OK
                if (result != null)
                {
                    var resultForm = new CalibrationResultForm();
                    resultForm.SetPlotData(result);
                    resultForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Not enough data to create a calibration (or calibration aborted).");
                }
            }
            catch (EyeTrackerException)
            {

            }
        }



        private void StartBrowsing()
        {
            _browser.StartBrowsing();
        }

        void _browser_EyetrackerUpdated(object sender, EyeTrackerInfoEventArgs e)
        {
            int index = TrackerCombo.Items.IndexOf(e);

            if (index >= 0)
            {
                TrackerCombo.Items[index] = e.EyeTrackerInfo;
            }
        }

        void _browser_EyetrackerRemoved(object sender, EyeTrackerInfoEventArgs e)
        {
            TrackerCombo.Items.Remove(e.EyeTrackerInfo);
        }

        private void _browser_EyetrackerFound(object sender, EyeTrackerInfoEventArgs e)
        {
            TrackerCombo.Items.Add(e.EyeTrackerInfo);
        }

        private void _tracker_GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            const double D = 10.0;

            _leftPos.X = e.GazeDataItem.LeftEyePosition3D.X / D;
            _leftPos.Y = e.GazeDataItem.LeftEyePosition3D.Y / D;
            _leftPos.Z = e.GazeDataItem.LeftEyePosition3D.Z / D;

            _rightPos.X = e.GazeDataItem.RightEyePosition3D.X / D;
            _rightPos.Y = e.GazeDataItem.RightEyePosition3D.Y / D;
            _rightPos.Z = e.GazeDataItem.RightEyePosition3D.Z / D;

            _leftGaze.Y = e.GazeDataItem.LeftGazePoint2D.Y;
            _leftGaze.X = e.GazeDataItem.LeftGazePoint2D.X;

            _point = new Point((int)(_leftGaze.X * 1920), (int)(_leftGaze.Y * 1200));

            gazeMovement(_point);

            //_leftGaze.X = e.GazeDataItem.LeftGazePoint3D.X / D;
            //_leftGaze.Y = e.GazeDataItem.LeftGazePoint3D.Y / D;
            //_leftGaze.Z = e.GazeDataItem.LeftGazePoint3D.Z / D;

            //_rightGaze.X = e.GazeDataItem.RightGazePoint3D.X / D;
            //_rightGaze.Y = e.GazeDataItem.RightGazePoint3D.Y / D;
            //_rightGaze.Z = e.GazeDataItem.RightGazePoint3D.Z / D;



            // Set which eyes to show
            //_showLeft = e.GazeDataItem.LeftValidity < 2;
            //_showRight = e.GazeDataItem.RightValidity < 2;

            //Action update = delegate ()
            //{
            //    if (_showLeft)
            //    {
            //        _eyePair.LeftEyePosition = _leftPos;
            //        _leftGazeVector.Point1 = _leftPos;
            //        _leftGazeVector.Point2 = _leftGaze;
            //    }
            //    else
            //    {
            //        Point3D farAway = new Point3D(1000.0, 1000.0, 1000.0);
            //        _eyePair.LeftEyePosition = farAway;
            //        _leftGazeVector.Point1 = farAway;
            //        _leftGazeVector.Point2 = farAway;
            //    }

            //    if (_showRight)
            //    {
            //        _eyePair.RightEyePosition = _rightPos;
            //        _rightGazeVector.Point1 = _rightPos;
            //        _rightGazeVector.Point2 = _rightGaze;
            //    }
            //    else
            //    {
            //        Point3D farAway = new Point3D(1000.0, 1000.0, 1000.0);
            //        _eyePair.RightEyePosition = farAway;
            //        _rightGazeVector.Point1 = farAway;
            //        _rightGazeVector.Point2 = farAway;
            //    }

            //};
            //Dispatcher.BeginInvoke(update);
        }

        public void gazeMovement(Point point)
        {
            if (GlobalValue.MapTracking && (point.X - _previousPoint.X > 20 || point.Y - _previousPoint.Y > 20))
            {
                _previousPoint = point;
                GlobalValue.Point = _previousPoint;
                EyeGazeMovement?.Invoke(this, EventArgs.Empty);
            }
        }

        public void StartTracking(EyeTrackerInfo eyeTrackerInfo)
        {
            try
            {
                EyeTracker = eyeTrackerInfo.Factory.CreateEyeTracker();
                EyeTracker.StartTracking();
                EyeTracker.GazeDataReceived += _tracker_GazeDataReceived;
                GlobalValue.IsTracking = true;
                this.Hide();
                TrackButton.Content = "Stop";
            }
            catch (Exception)
            {
                Console.WriteLine("Failure"); //add a error message here later
            }
            
        }

        public void StopTracking()
        {
            EyeTracker.GazeDataReceived -= _tracker_GazeDataReceived;
            EyeTracker.StopTracking();

            EyeTracker.Dispose();
            GlobalValue.IsTracking = false;
            this.Hide();
            TrackButton.Content = "Track";
        }
    }
}

