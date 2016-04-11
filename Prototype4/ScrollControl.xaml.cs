using System;
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
        }

        public void zoneChanged(int i)
        {
            textBlock.Text = File.ReadAllText("res/text" + i +".txt");
        }

        public void MapInteraction(Point current)
        {
            var scrollFactor = (current.Y - 326) > Height / 2 ? 1 : -1;
            CalculateScrollSpeed(scrollFactor, current);
            _currentPosition += _scrollSpeed * _currentScrollFactor;
            if (_currentPosition >= 0)
            {
                textBlock.ScrollToVerticalOffset(_currentPosition);
            }
            else _currentPosition = 0;
        }

        private void CalculateScrollSpeed(int newScrollFactor, Point current)
        {
            var y = current.Y - 326;
            if (y >= Height * 0.3 && y <= Height * 0.7)
            {
                _scrollSpeed = 0;
                return;
            }
            if (y > Height * 0.2 && y < Height * 0.8)
            {
                _scrollSpeed = 1;
            }
            else if(y < Height * 0.1 && y > Height * 0.9)
            {
                _scrollSpeed = 2;
            }
            if (newScrollFactor * _currentScrollFactor > 0 && _scrollSpeed <= 5 && _scrollSpeed > 1)
            {
                _scrollSpeed += 0.25;
            }
            if (newScrollFactor * _currentScrollFactor < 0)
            {
                _currentScrollFactor = newScrollFactor;
            }
        }
    }
}
