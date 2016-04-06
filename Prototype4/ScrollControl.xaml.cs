using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Prototype4
{
    /// <summary>
    /// Interaction logic for ScrollControl.xaml
    /// </summary>
    public partial class ScrollControl : UserControl
    {

        private double scrollSpeed;
        private int currentPosition = 0;
        private int currentScrollFactor = 1;

        public ScrollControl()
        {
            InitializeComponent();
            textBlock.Text = File.ReadAllText("res/text1.txt");
        }


        public void mapInteraction(Point current)
        {
            var scrollFactor = current.Y - 337 > Height / 2 ? 1 : -1;
            calculateScrollSpeed(scrollFactor, current);
            currentPosition += Convert.ToInt32(scrollSpeed * currentScrollFactor);
            Console.WriteLine(currentPosition);
            if (currentPosition >= 0)
            {
                textBlock.ScrollToVerticalOffset(currentPosition);
            }
            else currentPosition = 0;
        }

        private void calculateScrollSpeed(int newScrollFactor, Point current)
        {
            var Y = current.Y - 337;
            if (Y > Height * 0.35 && Y < Height * 0.65)
            {
                scrollSpeed = 0;
            }
            else if(Y > Height * 0.25 && Y < Height * 0.75)
            {
                scrollSpeed = 1;
            }
            else if (Y > Height * 0.15 && Y < Height * 0.85)
            {
                scrollSpeed = 2;
            }
            else
            {
                if (newScrollFactor * currentScrollFactor > 0 && scrollSpeed <= 5)
                {
                    scrollSpeed += 0.5;
                }
                if (newScrollFactor * currentScrollFactor < 0)
                {
                    scrollSpeed = 1;
                    currentScrollFactor = newScrollFactor;
                }
            }
        }
    }
}
