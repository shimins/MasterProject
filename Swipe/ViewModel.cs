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
                Image = "/Swipe;component/Images/kingfisher.jpg"
            });
            ViewItems.Add(new ViewItem
            {
                Image = "/Swipe;component/Images/underwater.jpg"
            });
            ViewItems.Add(new ViewItem
            {
                Image = "/Swipe;component/Images/park.jpg"

            });
            ViewItems.Add(new ViewItem
            {
                Image = "/Swipe;component/Images/timesquare.jpg"
            });
            ViewItems.Add(new ViewItem
            {
                Image = "/Swipe;component/Images/bird.jpg"
            });
        }
    }

    public class ViewItem
    {
        public string Image { get; set; }
    }
}
