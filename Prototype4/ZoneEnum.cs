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
        public List<ZoneBorder> _zoneList;
        private ZoneBorder _zoneBorder;
        private const double Constant = 8192/3;

        public ZoneEnum()
        {
            _zoneList = new List<ZoneBorder>();
            for (var i = 0; i < 9; i++)
            {
                _zoneList.Add(_zoneBorder = new ZoneBorder(i));
            }
        }
    }

    class ZoneBorder
    {
        public double BorderX;
        public double BorderY;
        private const double Constant = 8192/3;

        public ZoneBorder(int i)
        {
            if (((i + 1) % 3) != 0)
            {
                BorderX = 8192 / 3 * ((i + 1) % 3);
            }
            else
            {
                BorderX = 8192;
            }
            if (i < 3)
            {
                BorderY = 8192 / 3;
            }
            else if (i >= 3 && i < 6)
            {
                BorderY = 8192 / 3 * 2;
            }
            else
            {
                BorderY = 8192;
            }
        }

        public bool IsInside(Point point)
        {
            return (point.X <= BorderX && point.X > BorderX - Constant && point.Y <= BorderY && point.Y > BorderY - Constant);
        }
    }
}
