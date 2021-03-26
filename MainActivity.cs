using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Gms.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;
using Android.Content;
using Newtonsoft.Json;
using Java.Util;
using Firebase;
using System;
using Project_OCS_Second.DataModels;
using Java.Lang;
using Project_OCS_Second;
using Project_OCS_Second.Fragments;
using Android;
using Plugin.Media;
using Android.Graphics;
using Firebase.Storage;
using Android.Support.Design.Widget;
using Project_OCS_Second.Services;
using Android.Content.Res;
using Plugin.Connectivity;

namespace Project_OCS_Second
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IOnSuccessListener
    {


        TextInputLayout Fnametext;
        TextInputLayout passtext;
        TextInputLayout emtext;

        Button submit;
        Button login;

        public ImageView profimg;

        public FirebaseFirestore database;

        DatabaseServices databaseservice = new DatabaseServices();
        //UserServices userservice = new UserServices();

        public UploadImageFragment uploadimg; 

        public bool connected;


        //public FirebaseFirestore database;

        StorageReference storageReference; 

        public static List<User> listOfUser = new List<User>();
        private static byte[] imageArray;
        //private string url;

        //private Bitmap bitmap;
        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);



            //var seconds = TimeSpan.FromSeconds(1);
            //Xamarin.Forms.Device.StartTimer(seconds,
            //    () =>
            //    {
            //        CheckConnection();
            //    });


            ConnectViews();
            database = databaseservice.GetDataBase();

            FetchUserData();




        }

        public void OnSuccess(Java.Lang.Object result)
        {
            
                var snapshot = (QuerySnapshot)result;

                //while (!snapshot.IsEmpty)
                //{
                //    snapshot = (QuerySnapshot)result;
                //}

                if (!snapshot.IsEmpty)
                {
                    var documents = snapshot.Documents;

                    listOfUser.Clear();

                    foreach (DocumentSnapshot item in documents)
                    {
                        User user = new User();
                        user.ID = item.Id;
                        user.Fullname = item.Get("fullname").ToString();
                        user.Email = item.Get("email") != null ? item.Get("email").ToString() : "";
                        user.password = item.Get("password") != null ? item.Get("password").ToString() : "";
                        //user.imagepath = item.Get("profileimage") != null ? item.Get("profileimage").ToString() : "";

                        listOfUser.Add(user);
                    }
                }
            
            
        }




        void ConnectViews()
        {
            Fnametext = (TextInputLayout)FindViewById(Resource.Id.fullnametext);
            emtext = (TextInputLayout)FindViewById(Resource.Id.emailtext);
            passtext = (TextInputLayout)FindViewById(Resource.Id.passwordtext);
            submit = (Button)FindViewById(Resource.Id.submit_btn);
            login = (Button)FindViewById(Resource.Id.login_btn);
            profimg = (ImageView)FindViewById(Resource.Id.profileiv);




            submit.Click += (sender, e) =>
            {

                if (imageArray == null)
                {
                    imageArray = System.IO.File.ReadAllBytes("Resources/Images/DefaultUser.png");
                }

                storageReference = FirebaseStorage.Instance.GetReference($"users/{Fnametext.EditText.Text}_image");
                storageReference.PutBytes(imageArray);
                
                storageReference.GetDownloadUrl().AddOnSuccessListener(this);


                HashMap map = new HashMap();
                map.Put("fullname", Fnametext.EditText.Text);
                map.Put("email", emtext.EditText.Text);
                map.Put("password", passtext.EditText.Text);
                //map.Put("profileimage", url);



                DocumentReference docRef = database.Collection("users").Document();

                docRef.Set(map);

                Toast.MakeText(this, "User Registered", ToastLength.Long).Show();
                FetchUserData();
            };


            login.Click += (sender, e) => 
            {
                string loginfname = Fnametext.EditText.Text.ToString();
                string loginemail = emtext.EditText.Text.ToString();
                string loginpass = passtext.EditText.Text.ToString();

                //listOfUser=userservice.FetchUserData("user");

                //while (listOfUser.Count == 0)
                //{
                //}



                foreach (var item in listOfUser)
                {
                    Console.WriteLine("Fullname: " + item.Fullname);

                }
                    foreach (var item in listOfUser)
                {
                    if (item.Fullname.Equals(loginfname) && item.Email.Equals(loginemail) && item.password.Equals(loginpass))
                    {

                        Intent secondpageintent = new Intent(this, typeof(SecondPageActivity));
                        secondpageintent.PutExtra("accepteduser", JsonConvert.SerializeObject(item));
                        StartActivity(secondpageintent);
                        return;
                    }

                }

                Toast.MakeText(this, "Invalid Credentials", ToastLength.Long).Show();
                return;

            };

            profimg.Click += (sender, e) =>
            {
                RequestPermissions(permissionGroup, 0);

                var trans = SupportFragmentManager.BeginTransaction();
                uploadimg = new UploadImageFragment();
                
                uploadimg.Show(trans,"profilepicmethod");

                uploadimg.uploadImage += (Bitmap bitmap, byte[] imgary) =>
                {
                    profimg.SetImageBitmap(bitmap);
                    imageArray = imgary;
                    uploadimg.Dismiss();
                };

            };




            
           


        }






        public void FetchUserData()
        {
            database.Collection("users").Get()
                .AddOnSuccessListener(this);

        }







        //void LoginAcitivity(User user)
        //{

        //}


        //public async void UploadPhoto()
        //{

        //    await CrossMedia.Current.Initialize();

        //    if (!CrossMedia.Current.IsPickPhotoSupported)
        //    {
        //        Toast.MakeText(this, "Upload not supported on this device", ToastLength.Short).Show();
        //        return;
        //    }

        //    var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
        //    {
        //        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
        //        CompressionQuality = 40

        //    });

        //    // Convert file to byre array, to bitmap and set it to our ImageView

        //    imageArray = System.IO.File.ReadAllBytes(file.Path);
        //    bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
        //    profimg.SetImageBitmap(bitmap);
        //    uploadimg.Dismiss();

        //    //profimg.SetImageBitmap(bitmap);

        //}


        //private async void CheckConnection()
        //{
        //    if (!CrossConnectivity.Current.IsConnected)
        //        ShowConnectionAlert();
        //    else
        //        return;

                      
        //}

        //private void ShowConnectionAlert()
        //{
            
        //        Android.Support.V7.App.AlertDialog.Builder connectionchanged = new Android.Support.V7.App.AlertDialog.Builder(this);
        //        connectionchanged.SetTitle("NO INTERNET CONNECTION!!!");
        //        connectionchanged.SetMessage("You are currently disconnected");
        //        connectionchanged.SetPositiveButton("Try Again", (senderAlert, args) =>
        //        { });
            
        //}
        //CrossConnectivity.Current.ConnectivityChanged += async(sender, args) =>

        //{
        //    Debug.WriteLine($"Connectivity changed to {args.IsConnected}");
        //};


























        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        
        








    }
}