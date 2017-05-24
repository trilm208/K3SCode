using Xamarin.Forms;
using Shared.DependencyService;
using K3S.Droid;
using System;
using System.Net;
using DataAccess;

[assembly: Dependency(typeof(NetWork_Android))]
namespace K3S.Droid
{
    public class NetWork_Android : DSNetWorkExtensions
    {
        public bool CheckNetworkAvailable()
        {
            string CheckUrl =string.Format("http://{0}","google.com");

            try 
            {
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);

                iNetRequest.Timeout = 5000;

                WebResponse iNetResponse = iNetRequest.GetResponse();

                iNetResponse.Close ();

                return true;

            }
            catch (WebException ex) {
                
                return false;
            }
        }
    }
}