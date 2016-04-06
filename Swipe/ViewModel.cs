using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            // A, B or C
            const string imagePack = "B";

            for (var i = 1; i <= 15; i++)
            {
                ViewItems.Add(new ViewItem
                {
                    Image = "/Swipe;component/Images/" + imagePack + "/" + i + ".jpg"
                });
            }
        }
    }

    public class ViewItem
    {
        public string Image { get; set; }
    }
}
