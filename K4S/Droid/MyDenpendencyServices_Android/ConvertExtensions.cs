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
using K3S.Droid.MyDenpendencyServices_Android;
using MyDependencyServices;
using System.Data;
using FastMember;
using System.Reflection;
using System.Drawing;
using K3S.Model;
using System.IO;
using System.ComponentModel;

[assembly: Dependency(typeof(ConvertExtensions))]

namespace K3S.Droid.MyDenpendencyServices_Android
{
    public class ConvertExtensions : IConvertExtensions
    {
        public  string ImageToStringBase64(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            // Create a byte array of file stream length
            byte[] ImageData = new byte[fs.Length];
            //Read block of bytes from stream into the byte array
            fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
            //Close the File Stream
            fs.Close();
            string _base64String = Convert.ToBase64String(ImageData);
            return _base64String;
        }



        public DataTable ToTable<T>(List<T> data)
        {
            try
            {
                PropertyDescriptorCollection props =
            TypeDescriptor.GetProperties(typeof(T));
                DataTable table = new DataTable();
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
                object[] values = new object[props.Count];
                foreach (T item in data)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    table.Rows.Add(values);
                }
                return table;

            }
            catch(Exception)
            {
                return null;
            }
            //IEnumerable<T> data = listModel;            
            //Type type = typeof(T);
            //var table = new DataTable();
            //using (var reader = ObjectReader.Create(listModel))// GetProperties(type)))
            //{
            //    table.Load(reader);
            //}
            //return table;
        }
    
        private static string[] GetProperties(Type type)
        {        
         
            var properties = type.GetProperties();
            string[] result = new string[properties.Count()];
            int i = 0;
            foreach (var property in properties)
            {
                result[i] = property.Name;
                i++;             
            }
            return result;
        }

     
    }
}