using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Firebase.Storage;
using Java.Lang;
using Project_OCS_Second.DataModels;

namespace Project_OCS_Second.Adapters
{
    class SearchAdapter : RecyclerView.Adapter, IOnSuccessListener
    {

        public event EventHandler<SearchAdapterClickEventArgs> ItemClick;

        List<Product> productlist;
        List<string> imagepaths;
        List<string> imageurls = new List<string>();

        SearchAdapterViewHolder holder;

        //public string productimageurl;
        //ImageView productiv;

        public SearchAdapter(List<Product> data, List<string> imgs)
        {
            productlist = data;
            imagepaths = imgs;


        }

        public void OnSuccess(Java.Lang.Object result)
        {
            imageurls.Add(result.ToString());

            

        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;

            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SearchRow, parent, false);
            var vh = new SearchAdapterViewHolder(itemView, OnClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var product = productlist[position]; 
            //var url = imageurls[position];


            // Replace the contents of the view with that element
            holder = viewHolder as SearchAdapterViewHolder;

            GetProductImage(imagepaths[position]);

            //ImageService.Instance.LoadUrl(imageurls[position])
            //    .Retry(3, 200)
            //    .DownSample(100, 100)
            //    .Into(holder.productimage);

            ImageService.Instance.LoadUrl(imageurls[position])
                .Retry(3, 200)
                .DownSample(100, 100)
                .Into(holder.productimage);

            holder.productname.Text = product.productname;
            holder.unitprice.Text = 250.ToString();


            




            // holder.productimage.Text = product.amount.ToString();
            //holder.proidtext.Text = product.ID;


            //StorageReference ppref = FirebaseStorage.Instance.GetReference($"products/{product.category}/{product.productname}_image");

            //ppref.GetDownloadUrl().AddOnSuccessListener(Application.Context, this);

            //holder.ppicurl.Text = user.ProfileImage != null ? user.ProfileImage.ToString() : "No URL";

        }

        void GetProductImage(string url)
        {
            StorageReference ppref = FirebaseStorage.Instance.GetReference($"products/{url}");

            ppref.GetDownloadUrl().AddOnSuccessListener(this);
        }

        void SetProductImage(string imgurl, ImageView iv)
        {

            //ImageService.Instance.LoadUrl(imgurl)
            //    .Retry(3, 200)
            //    .DownSample(100, 100)
            //    .Into(iv);

        }

        public override int ItemCount => productlist.Count;

        void OnClick(SearchAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);

       
        //void OnLongClick(DataAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class SearchAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }

        public TextView productname { get; set; }
        public TextView unitprice { get; set; }
        public ImageView productimage { get; set; }
        //public ImageView productpicurl { get; set; }




        public SearchAdapterViewHolder(View itemView, Action<SearchAdapterClickEventArgs> clickListener) : base(itemView)
        {
            //TextView = v;
            productname = (TextView)itemView.FindViewById(Resource.Id.searchresultname);
            unitprice = (TextView)itemView.FindViewById(Resource.Id.searchresultprice);
            productimage = (ImageView)itemView.FindViewById(Resource.Id.searchresultiv);


            //proidtext = (TextView)itemView.FindViewById(Resource.Id.idtext);
            // productpicurl = (ImageView)itemView.FindViewById(Resource.Id.productlistiv);

            itemView.Click += (sender, e) => clickListener(new SearchAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            //itemView.LongClick += (sender, e) => longClickListener(new DataAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class SearchAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}