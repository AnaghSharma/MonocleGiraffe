﻿
using MonocleGiraffe.Helpers;
using SharpImgur.APIWrappers;
using SharpImgur.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonocleGiraffe.Models
{
    public enum GalleryItemType { Image, Album, Animation }

    public class GalleryItem : NotifyBase
    {
        private Image image;
            
        private const string baseUrl = "http://i.imgur.com/";

        public GalleryItem(Image image)
        {
            this.image = image;
            SetThumbnails();
        }

        public string Title
        {
            get
            {
                return image.Title;
            }
        }

        public string Link
        {
            get
            {
                return image.Link;
            }
        }

        public string Mp4
        {
            get
            {
                return image.Mp4;
            }
        }

        public string Description
        {
            get
            {
                return image.Description;
            }
        }

        public string UploaderName
        {
            get
            {
                return image.AccountUrl;
            }
        }

        private string smallThumbnail;
        public string SmallThumbnail
        {
            get
            {
                return smallThumbnail;
            }
            set
            {
                if (smallThumbnail != value)
                {
                    smallThumbnail = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string bigThumbnail;
        public string BigThumbnail
        {
            get
            {
                return bigThumbnail;
            }
            set
            {
                if (bigThumbnail != value)
                {
                    bigThumbnail = value;
                    NotifyPropertyChanged();
                }
            }
        }
       
        public GalleryItemType ItemType
        {
            get { return GetImageType(); }
        }

        private ObservableCollection<GalleryItem> albumImages= new ObservableCollection<GalleryItem>();
        public ObservableCollection<GalleryItem> AlbumImages
        {
            get
            {
                if (albumImages.Count == 0)
                    LoadAlbumImages();
                return albumImages;
            }
        }

        public int Width
        {
            get { return image.Width; }
        }

        public int Height
        {
            get { return image.Height; }
        }

        public bool IsAnimated
        {
            get { return image.Animated; }
        }

        private async void LoadAlbumImages()
        {
            if (image.IsAlbum)
            {
                SharpImgur.Models.Album album = await SharpImgur.APIWrappers.Album.GetAlbum(image.Id);
                foreach (var image in album.Images)
                {
                    albumImages.Add(new GalleryItem(image));
                }
            }            
        }

        private GalleryItemType GetImageType()
        {
            if (image.IsAlbum)
            {
                return GalleryItemType.Album;
            }
            else if (image.Animated)
            {
                return GalleryItemType.Animation;
            }
            else
            {
                return GalleryItemType.Image;
            }
        }        

        private ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> Comments
        {
            get
            {
                if (comments.Count == 0)
                    LoadComments(image.Id);
                return comments;
            }
        }        

        private async void SetThumbnails()
        {
            string thumbnailId;
            if (image.IsAlbum)
            {
                var album = await SharpImgur.APIWrappers.Album.GetAlbum(image.Id);
                thumbnailId = album.Cover;
            }
            else
            {
                thumbnailId = image.Id;
            }
            SmallThumbnail = baseUrl + thumbnailId + "s.jpg";
            BigThumbnail = baseUrl + thumbnailId + "b.jpg";
        }

        private async void LoadComments(string imageId)
        {
            var commentsList = await Gallery.GetComments(imageId);
            foreach (var comment in commentsList)
            {
                comments.Add(comment);
            }
        }        
    }
}