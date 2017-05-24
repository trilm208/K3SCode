using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using System.Security.Cryptography;
using System.Data;
using MyDependencyServices;
using K3S.Droid;

[assembly: Dependency(typeof(ListExtensions_Android))]

namespace MyDependencyServices
{
    public class ListExtensions_Android : IListExtensions
    {
      
        public List<T> Shuffle<T>(List<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public DataTable ToTable<T>(List<T> list)
        {
            try
            {
                return list.ToTable<T>();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}