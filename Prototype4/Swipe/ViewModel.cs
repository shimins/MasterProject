using System.Collections.Generic;

namespace Prototype4.Swipe
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
                    Image = "/Prototype4;component/Swipe/Images/" + imagePack + "/" + i + ".jpg"
                });
            }
        }
    }

    public class ViewItem
    {
        public string Image { get; set; }
    }
}
