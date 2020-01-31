﻿using System;
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

namespace TilesApp.Droid
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity, Animator.IAnimatorListener
    {
        LottieAnimationView animationView;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Splash);
            animationView = FindViewById<LottieAnimationView>(Resource.Id.splashScreen);
            animationView.AddAnimatorListener(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task.Run(() => Startup());
        }

        // Simulates background work that happens behind the splash screen
        void Startup()
        {
            Task.Run(async () =>
            {
                ConfigurationManager.Initialise(PCLAppConfig.FileSystemStream.PortableStream.Current);
                OdooXMLRPC.Start();
            }).Wait();
            StartActivity(new Intent(Android.App.Application.Context, typeof(MainActivity)));
            animationView.CancelAnimation();
        }

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
        public override void OnBackPressed() { }

    }
}