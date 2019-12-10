
namespace TilesApp.Rfid
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Xamarin.Forms;

    public static class LifecycleExtensions
    {
        public static void BindWithLifecycle(this Page page, ILifecycle lifecycleViewModel)
        {
            page.BindingContext = lifecycleViewModel;
            page.Appearing += (sender, e) =>
             {
                 System.Diagnostics.Debug.WriteLine("Page Appearing: {0}", page.Title);
//                 lifecycleViewModel.Shown();
             };
            page.Disappearing += (sender, e) =>
              {
                  System.Diagnostics.Debug.WriteLine("Page Disappearing: {0}", page.Title);
//                  lifecycleViewModel.Hidden();
              };
        }
    }
}
