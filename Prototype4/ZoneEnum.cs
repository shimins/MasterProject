using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.TextFormatting;
using Microsoft.Maps.MapControl.WPF;

namespace Prototype4
{
    class ZoneEnum
    {
        public List<ZoneBorder> ZoneList;

        public ZoneEnum()
        {
            ZoneList = new List<ZoneBorder>();
            for (var i = 0; i < 9; i++)
            {
                ZoneList.Add(new ZoneBorder(i));
            }
        }
    }

    class ZoneBorder
    {
        public double BorderX;
        public double BorderY;
        private const double Constant = 2730;

        public ZoneBorder(int i)
        {
            if (((i + 1) % 3) != 0)
            {
                BorderX = Constant * ((i + 1) % 3);
            }
            else
            {
                BorderX = Constant * 3;
            }
            if (i < 3)
            {
                BorderY = Constant;
            }
            else if (i >= 3 && i < 6)
            {
                BorderY = Constant * 2;
            }
            else
            {
                BorderY = Constant * 3;
            }
        }

        public bool IsInside(Point point)
        {
            return (point.X <= BorderX && point.X > BorderX - Constant && point.Y <= BorderY && point.Y > BorderY - Constant);
        }
    }
}
