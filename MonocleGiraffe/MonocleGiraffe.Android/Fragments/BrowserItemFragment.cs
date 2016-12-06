using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MonocleGiraffe.Portable.Models;
using FFImageLoading.Views;
using FFImageLoading;
using Android.Media;
using MonocleGiraffe.Android.Controls;
using MonocleGiraffe.Android.Activities;
using Android.Support.V7.Widget;
using GalaSoft.MvvmLight.Helpers;
using Android.Text;

namespace MonocleGiraffe.Android.Fragments
{
    public class BrowserItemFragment : global::Android.Support.V4.App.Fragment
    {
        private IGalleryItem item;
        private bool isAlbum;
        private List<Binding> bindings = new List<Binding>();

        public const string POSITION_ARG = "position";

        public static BrowserItemFragment NewInstance(int position)
        {
            BrowserItemFragment ret = new BrowserItemFragment();
            Bundle args = new Bundle(1);
            args.PutInt(POSITION_ARG, position);
            ret.Arguments = args;
            return ret;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var position = Arguments.GetInt(POSITION_ARG);
            item = (Activity as BrowserActivity)?.Vm.Images.ElementAt(position);
            isAlbum = item.ItemType == GalleryItemType.Album;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            int layout = isAlbum ? Resource.Layout.Tmpl_Album : Resource.Layout.Tmpl_Image;
            return inflater.Inflate(layout, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            RenderHeader(item);
            if (isAlbum)
                RenderAlbum(item);
            else
                RenderImage(item);
            base.OnViewCreated(view, savedInstanceState);
            AnalyticsHelper.SendView("BrowserItem");
        }

        private void RenderHeader(IGalleryItem item)
        {
            Title.Text = item.Title;
            SubTitle.SetText(Html.FromHtml($"by <font color='#528ACA'>{item.UploaderName}</font> � {item.Ups} points"), TextView.BufferType.Spannable);
        }

        private void RenderImage(IGalleryItem item)
        {           
            MainImage.RenderContent(item);
            var hasDescription = !string.IsNullOrEmpty(item.Description);
            var descView = Description;
            if (hasDescription)
                descView.Text = item.Description;
            else
                descView.Visibility = ViewStates.Gone;
        }

        public IGalleryItem Item { get; set; }

        private void RenderAlbum(IGalleryItem item)
        {
            Item = item;
            Title.Text = item.Title;
            AlbumRecyclerView.SetLayoutManager(new PrefetchLinearLayoutManager(Context));
            bindings.Add(this.SetBinding(() => Item.AlbumImages).WhenSourceChanges(UpdateAlbumAdapter));
        }

        private void UpdateAlbumAdapter()
        {
            if (Item.AlbumImages == null)
                return;
            var adapter = Item.AlbumImages.GetRecyclerAdapter(BindViewHolder, Resource.Layout.Tmpl_Item_Album);
            AlbumRecyclerView.SetAdapter(adapter);
        }
        
        private void BindViewHolder(CachingViewHolder holder, GalleryItem item, int position)
        {
            var hasTitle = !string.IsNullOrEmpty(item.Title);
            var titleView = holder.FindCachedViewById<TextView>(Resource.Id.TitleTextView);
            if (hasTitle)
                titleView.Text = item.Title;
            else
                titleView.Visibility = ViewStates.Gone;

            var image = holder.FindCachedViewById<ImageControl>(Resource.Id.MainImage);
			//this.Activity.RegisterForContextMenu(image);
            image.RenderContent(item);

            var hasDescription = !string.IsNullOrEmpty(item.Description);
            var descView = holder.FindCachedViewById<TextView>(Resource.Id.DescriptionTextView);
            if (hasDescription)
                descView.Text = item.Description;
            else
                descView.Visibility = ViewStates.Gone;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            bindings.ForEach(b => b.Detach());
        }

        #region Views

        private ImageControl mainImage;
        public ImageControl MainImage
        {
            get
            {
                if (isAlbum)
                    return null;
                mainImage = mainImage ?? View.FindViewById<ImageControl>(Resource.Id.MainImage);
                return mainImage;
            }
        }

        private TextView title;
        public TextView Title
        {
            get
            {
                title = title ?? View.FindViewById<TextView>(Resource.Id.TitleTextView);
                return title;
            }
        }

        private TextView subTitle;
        public TextView SubTitle
        {
            get
            {
                subTitle = subTitle ?? View.FindViewById<TextView>(Resource.Id.SubTitleTextView);
                return subTitle;
            }
        }

        private RecyclerView albumRecyclerView;
        public RecyclerView AlbumRecyclerView
        {
            get
            {
                albumRecyclerView = albumRecyclerView ?? View.FindViewById<RecyclerView>(Resource.Id.AlbumRecyclerView);
                return albumRecyclerView;
            }
        }

        private TextView description;
        public TextView Description
        {
            get
            {
                description = description ?? View.FindViewById<TextView>(Resource.Id.DescriptionTextView);
                return description;
            }
        }

        #endregion
    }

	class PrefetchLinearLayoutManager : LinearLayoutManager
	{
		private Context context;
		private int? prefetchSize;

		public PrefetchLinearLayoutManager(Context context, int? prefetchSize = null) : base(context)
		{
			this.context = context;
			this.prefetchSize = prefetchSize;
		}

		public PrefetchLinearLayoutManager(IntPtr i, JniHandleOwnership o) : base(i, o)
		{ }

		public PrefetchLinearLayoutManager(Context context, IAttributeSet a, int i1, int i2) : base(context, a, i1, i2)
		{ 
			this.context = context;
		}

		public PrefetchLinearLayoutManager(Context context, int orientation, bool isReverse) : base(context, orientation, isReverse)
		{
			this.context = context;
		}

		protected override int GetExtraLayoutSpace(RecyclerView.State state)
		{
			return prefetchSize ?? 1920 * 2;
		}
	}
}