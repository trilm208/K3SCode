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
using System.Data;
using FastMember;

namespace K3S.Droid
{
    public static class ConvertExtensionsAndroid
    {
        public static DataTable ToTable<T>( this List<T> listModel)
        {
            //IEnumerable<T> data = listModel;            
            Type type = typeof(T);
            var table = new DataTable();
            using (var reader = ObjectReader.Create(listModel))// GetProperties(type)))
            {
                table.Load(reader);
            }
            return table;
        }

    }
}