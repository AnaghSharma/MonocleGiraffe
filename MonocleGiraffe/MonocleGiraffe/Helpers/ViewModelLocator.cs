﻿using GalaSoft.MvvmLight.Ioc;
using MonocleGiraffe.Portable.ViewModels;
using MonocleGiraffe.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace MonocleGiraffe.Helpers
{
    public class ViewModelLocator : Portable.ViewModels.PageKeyHolder, IViewModelLocator
    {
        public ViewModelLocator()
        {
            if (DesignMode.DesignModeEnabled)
                InitDesignTime();
            else
                Init();
            
        }

        private void Init()
        {
            frontPageViewModel = new Lazy<FrontPageViewModel>();
            transfersPageViewModel = new Lazy<TransfersPageViewModel>();
            browserPageViewModel = new Lazy<BrowserPageViewModel>();

            SimpleIoc.Default.Register<SubGalleryPageViewModel>();
        }

        private void InitDesignTime()
        {
            Init();
        }

        //TODO: Remove this eventually
        private Lazy<TransfersPageViewModel> transfersPageViewModel;
        public TransfersPageViewModel TransfersPageViewModel { get { return transfersPageViewModel.Value; } }

        private Lazy<FrontPageViewModel> frontPageViewModel;
        public FrontPageViewModel FrontPageViewModel { get { return frontPageViewModel.Value; } }

        private Lazy<BrowserPageViewModel> browserPageViewModel;
        public BrowserPageViewModel BrowserPageViewModel { get { return browserPageViewModel.Value; } }

        public SubGalleryPageViewModel SubGalleryPageViewModel { get { return SimpleIoc.Default.GetInstance<SubGalleryPageViewModel>(); } }

        public TransfersViewModel TransfersViewModel { get { return TransfersPageViewModel; } }

        public static ViewModelLocator GetInstance()
        {
            return (ViewModelLocator)Template10.Common.BootStrapper.Current.Resources["Locator"];
        }
    }
}
