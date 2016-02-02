using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swipe
{
    class ViewModel
    {
        public List<ViewItem> ViewItems { get; set; }

        public ViewModel()
        {
            ViewItems = new List<ViewItem>();
            ViewItems.Add(new ViewItem
            {
                Image = "/Swipe;component/Images/orangutan.jpg"

            });
            ViewItems.Add(new ViewItem
            {
                Image = "/Swipe;component/Images/park.jpg"

            });
            ViewItems.Add(new ViewItem
            {
                Image = "/Swipe;component/Images/timesquare.jpg"
            });
        }
    }

    public class ViewItem
    {
        public string Image { get; set; }
    }
}
