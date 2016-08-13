using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Ioc;
using MonocleGiraffe.Android.Activities;
using MonocleGiraffe.Android.ViewModels;

namespace MonocleGiraffe.Android
{
    public static class App
    {
        private static ViewModelLocator locator;

        public static ViewModelLocator Locator
        {
            get
            {
                if (locator == null)
                {
                    DispatcherHelper.Initialize();

                    var nav = new NavigationService();

                    SimpleIoc.Default.Register<INavigationService>(() => nav);

                    nav.Configure(ViewModelLocator.FrontPageKey, typeof(FrontActivity));
                    nav.Configure(ViewModelLocator.SubGalleryPageKey, typeof(SubGalleryActivity));

                    locator = new ViewModelLocator();
                }

                return locator;
            }
        }
    }
}