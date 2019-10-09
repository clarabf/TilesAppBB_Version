using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TilesApp
{
    public class ProgressBar : Xamarin.Forms.View, Xamarin.Forms.IElementConfiguration<Xamarin.Forms.ProgressBar>
    {
        public IPlatformElementConfiguration<T, Xamarin.Forms.ProgressBar> On<T>() where T : IConfigPlatform
        {
            throw new NotImplementedException();
        }
        public Xamarin.Forms.Color ProgressColor { get; set; }
        public double Progress { get; private set; }

        public System.Threading.Tasks.Task<bool> ProgressTo(double value, uint length, Xamarin.Forms.Easing easing)
        {
            throw new NotImplementedException();
        }
        public ProgressBar()
        {
            var progressBar = new ProgressBar
            {
                Progress = 0.2,
            };

            // animate the progression to 80%, in 250ms
            progressBar.ProgressTo(0.8, 250, Easing.Linear);

            Debug.WriteLine("Animation completed");
        }
        
    }
}
