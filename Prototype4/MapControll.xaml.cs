using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Prototype4
{
    /// <summary>
    /// Interaction logic for MapControll.xaml
    /// </summary>
    public partial class MapControll
    {
        public event EventHandler ZoneHaveChanged;

        private readonly ZoneEnum _zoneEnum;
        private int _currentZone = 4;

        public MapControll()
        {
            InitializeComponent();

            var group = new TransformGroup();

            var st = new ScaleTransform();
            group.Children.Add(st);

            var tt = new TranslateTransform();
            group.Children.Add(tt);

            mapElement.RenderTransform = group;
            mapElement.RenderTransformOrigin = new Point(0.0, 0.0);

            _zoneEnum = new ZoneEnum();
        }

        public UIElement GetMapElement()
        {
            return mapElement;
        }

        private static TranslateTransform GetTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
        }

        private static ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
                .Children.First(tr => tr is ScaleTransform);
        }

        private void zoom_event(double zoomFactor, Point current)
        {
            if(Math.Abs(zoomFactor) <= 3) return;
            if (mapElement != null)
            {
                var st = GetScaleTransform(mapElement);
                var tt = GetTransform(mapElement);

                double zoom = zoomFactor > 0 ? -.01 * st.ScaleX : .01 * st.ScaleX;

                if (st.ScaleX + zoom > .1 && st.ScaleY + zoom < 5)
                {
                    Point relative = mapElement.PointFromScreen(current);
                    relative.X += 447;

                    double abosuluteX = relative.X * st.ScaleX + tt.X;
                    double abosuluteY = relative.Y * st.ScaleY + tt.Y;

                    st.ScaleX += zoom;
                    st.ScaleY += zoom;

                    tt.X = abosuluteX - relative.X * st.ScaleX;
                    tt.Y = abosuluteY - relative.Y * st.ScaleY;
                    CheckZoneChange(mapElement.PointFromScreen(current));
                }
            }
        }

        private void EyeMoveDuringAction(Point current)
        {
            if (mapElement == null) return;
            if (mapElement.PointFromScreen(current).X < 0 || mapElement.PointFromScreen(current).X > mapElement.RenderSize.Width
                || mapElement.PointFromScreen(current).Y < 0 || mapElement.PointFromScreen(current).X > mapElement.RenderSize.Width)
                return;
            if (Math.Abs(current.X - (Width / 2 + 447)) > 250 || Math.Abs(current.Y - (Height / 2)) > 250)
            {
                var tt = GetTransform(mapElement);
                tt.X -= (current.X - (Width / 2 + 447)) * 0.025;
                tt.Y -= (current.Y - Height / 2) * 0.025;
                CheckZoneChange(mapElement.PointFromScreen(current));
            }
        }

        private void CheckZoneChange(Point current)
        {
            var zoneBorder = _zoneEnum.ZoneList.SingleOrDefault(x => x.IsInside(current));

            if (zoneBorder != null && _zoneEnum.ZoneList.IndexOf(zoneBorder) != _currentZone)
            {
                _currentZone = _zoneEnum.ZoneList.IndexOf(zoneBorder);
                ZoneHaveChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int GetCurrentZone()
        {
            return _currentZone;
        }

        public void MapInteraction(bool zoomActionButtonDown, Point current, double zoomfactor)
        {
            if (zoomActionButtonDown)
                zoom_event(zoomfactor, current);
            else
                EyeMoveDuringAction(current);
        }

        public void SetInFocus(bool focus = true)
        {
            Border.Visibility = focus ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
