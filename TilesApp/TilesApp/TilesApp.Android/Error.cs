using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Animation;
using Com.Airbnb.Lottie;
using System.Threading.Tasks;
using TilesApp.Services;
using Xamarin.Forms;
using PCLAppConfig;
using Android.Widget;
using System.Threading;

namespace TilesApp.Droid
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = false, NoHistory = true)]
    public class Error : Activity, Animator.IAnimatorListener
    {
        LottieAnimationView animationView;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashError);
            animationView = FindViewById<LottieAnimationView>(Resource.Id.splashError);
            animationView.AddAnimatorListener(this);
        }


        // Simulates background work that happens behind the splash screen
        
        public void OnAnimationCancel(Animator animation)
        {
        }

        public void OnAnimationEnd(Animator animation)
        {
        }

        public void OnAnimationRepeat(Animator animation)
        {
        }

        public void OnAnimationStart(Animator animation)
        {
        }
    }
}