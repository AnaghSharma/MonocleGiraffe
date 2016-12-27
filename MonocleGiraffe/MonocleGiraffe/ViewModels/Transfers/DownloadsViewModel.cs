﻿using MonocleGiraffe.Models;
using MonocleGiraffe.Portable.Models;
using MonocleGiraffe.Portable.ViewModels.Transfers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.ApplicationModel;
using Windows.Networking.BackgroundTransfer;

namespace MonocleGiraffe.ViewModels.Transfers
{
    public class DownloadsViewModel : Portable.ViewModels.Transfers.DownloadsViewModel
    {
        public DownloadsViewModel() : base(DownloadItemFactory, DesignMode.DesignModeEnabled)
        {
            if (DesignMode.DesignModeEnabled)
                InitDesignTime();
        }

        public static async Task<IDownloadItem> DownloadItemFactory(string url)
        {
            return await DownloadItem.Create(Downloader, url);
        }

        private static BackgroundDownloader downloader;
        private static BackgroundDownloader Downloader
        {
            get
            {
                downloader = downloader ?? new BackgroundDownloader();
                return downloader;
            }
        }

        protected override async Task LoadExistingDownloads()
        {
            IReadOnlyList<DownloadOperation> oldDownloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            foreach (var d in oldDownloads)
                Downloads.Add(await DownloadItem.Create(d));
        }

        protected override void InitDesignTime()
        {
            Downloads = new ObservableCollection<IDownloadItem>();
            Downloads.Add(new DownloadItem { TotalSize = 100, CurrentSize = 50, Name = "Unhand me woman", State = DownloadStates.PENDING });
            Downloads.Add(new DownloadItem { TotalSize = 100, CurrentSize = 70, Name = "Learn Python for Real", State = DownloadStates.CANCELED });
            Downloads.Add(new DownloadItem { TotalSize = 100, CurrentSize = 20, Name = "The full story", State = DownloadStates.SUCCESSFUL });
            Downloads.Add(new DownloadItem { TotalSize = 100, CurrentSize = 90, Name = "Goodbye EU", State = DownloadStates.DOWNLOADING });
        }
    }
}
    