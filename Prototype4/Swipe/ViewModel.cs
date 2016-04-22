using System.Collections.Generic;

namespace Prototype4.Swipe
{
    class ViewModel
    {
        public List<ViewItem> ViewItems { get; set; }

        private static readonly string[] Folders = {"A", "B", "C", "D", "E", "F", "G", "H", "I"};

        public ViewModel()
        {
            ViewItems = new List<ViewItem>(5);

            for (var i = 1; i <= 20; i++)
            {
                ViewItems.Add(new ViewItem
                {
                    Image = "/Prototype4;component/Swipe/Images/A/" + i + ".jpg"
                });
            }
        }

        //public void SetImagePack(int index)
        //{
        //    var imagePack = Folders[index];

        //    for (var i = 1; i <= 5; i++)
        //    {
        //        ViewItems[i-1] = new ViewItem
        //        {
        //            Image = "/Prototype4;component/Swipe/Images/" + imagePack + "/" + i + ".jpg"
        //        };
        //    }
        //}
    }

    public class ViewItem
    {
        public string Image { get; set; }
    }
}
