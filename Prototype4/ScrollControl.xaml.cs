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
        private int _currentPosition;
        private int _currentScrollFactor = 1;

        public ScrollControl()
        {
            InitializeComponent();
            textBlock.Text = File.ReadAllText("res/text1.txt");
        }

        public void zoneChanged(int i)
        {
            textBlock.Text = File.ReadAllText("res/text" + i +".txt");
        }

        public void MapInteraction(Point current)
        {
            var scrollFactor = current.Y - 337 > Height / 2 ? 1 : -1;
            CalculateScrollSpeed(scrollFactor, current);
            _currentPosition += Convert.ToInt32(_scrollSpeed * _currentScrollFactor);
            if (_currentPosition >= 0)
            {
                textBlock.ScrollToVerticalOffset(_currentPosition);
            }
            else _currentPosition = 0;
        }

        private void CalculateScrollSpeed(int newScrollFactor, Point current)
        {
            var y = current.Y - 337;
            if (y > Height * 0.35 && y < Height * 0.65)
            {
                _scrollSpeed = 0;
            }
            else if (y > Height * 0.25 && y < Height * 0.75)
            {
                _scrollSpeed = 1;
            }
            else if (y > Height * 0.15 && y < Height * 0.85)
            {
                _scrollSpeed = 2;
            }
            else
            {
                if (newScrollFactor * _currentScrollFactor > 0 && _scrollSpeed <= 5)
                {
                    _scrollSpeed += 0.5;
                }
                if (newScrollFactor * _currentScrollFactor < 0)
                {
                    _scrollSpeed = 1;
                    _currentScrollFactor = newScrollFactor;
                }
            }
        }
    }
}
