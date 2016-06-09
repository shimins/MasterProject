using System.IO;
using System.Windows;

namespace Prototype4
{
    /// <summary>
    /// Interaction logic for ScrollControl.xaml
    /// </summary>
    public partial class ScrollControl
    {
        private double _scrollSpeed;
        private double _currentPosition;
        private int _currentScrollFactor = 1;

        public ScrollControl()
        {
            InitializeComponent();
            textBlock.Text = File.ReadAllText("res/text4.txt");
            Border.Visibility = Visibility.Hidden;;
        }

        public void ZoneChanged(int i)
        {
            textBlock.Text = File.ReadAllText("res/text" + i +".txt");
        }

        public void MapInteraction(Point current)
        {
            var scrollFactor = (current.Y - 326) > Height / 2 ? 1 : -1;
            //var scrollFactor = t.Y > Height / 2 ? 1 : -1;
            SetScrollSpeed(scrollFactor, current);
            _currentPosition += _scrollSpeed * _currentScrollFactor;
            if (_currentPosition >= 0)
            {
                textBlock.ScrollToVerticalOffset(_currentPosition);
            }
            else
                _currentPosition = 0;
        }

        private void SetScrollSpeed(int newScrollFactor, Point current)
        {
            var y = current.Y - 326;
            //var y = current.Y;
            if (y >= Height * 0.4 && y <= Height * 0.6)
            {
                _scrollSpeed = 0;
                return;
            }
            if (newScrollFactor * _currentScrollFactor > 0 && _scrollSpeed <= 3)
            {
                _scrollSpeed += 0.005;
            }
            if (newScrollFactor * _currentScrollFactor < 0)
            {
                _currentScrollFactor = newScrollFactor;
                if (y > Height * 0.3 && y < Height * 0.7)
                    _scrollSpeed = 1;
                else if (y < Height * 0.2 && y > Height * 0.8)
                    _scrollSpeed = 2;
                else
                    _scrollSpeed = 3;
            }
        }

        public void SetInFocus(bool focus = true)
        {
            Border.Visibility = focus ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
