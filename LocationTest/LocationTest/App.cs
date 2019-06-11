﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LocationTest.Views;
using LocationTest.ViewModels.Base;
using LocationTest.Pages;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LocationTest
{
    public partial class App : Application
    {
        private static ViewModelLocator _locator;

        public static ViewModelLocator Locator
        {
            get { return _locator = _locator ?? new ViewModelLocator(); }
        }

        public App()
        {
            MainPage = new NavigationPage(new MainPage());
        }
        
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
