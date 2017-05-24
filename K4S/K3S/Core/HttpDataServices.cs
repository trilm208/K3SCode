using DataAccess;
using Xamarin.Forms;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using MyDependencyServices;

namespace DataAccess
{
    public class HttpDataServices
    {
        
        public string HttpURL { get; private set; }

        public CookieContainer HttpCookies { get; private set; }

        public string HttpUserAgent { get; private set; }

        private static ManualResetEvent allDone = new ManualResetEvent(false);

        HttpClient client;

        public HttpDataServices(string webServiceURL)
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            this.HttpURL =String.Format(@"http://{0}:{1}/DataAccess.ashx",NetworkExtensions.ServerIP,NetworkExtensions.ServerPort);
            this.HttpCookies = new CookieContainer();
            this.HttpUserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";

        }




        public   DataSet Execute(RequestCollection requests)
        {
            //#region [WEBSERVICE]
            //var xml = requests.ToXml();
            //var text =await DependencyService.Get<IHttpPost>().HttpPost(xml, this);
            //var bytes = Convert.FromBase64String(text);
            //var ds = DependencyService.Get<ISerializer>().Decompress<DataSet>(bytes);
            //return ds;
            //#endregion

            #region [CONNECT DIRECT DATABASE]
            var ds = DependencyService.Get<IDataProvider>().Excute(requests);
            return ds;
            #endregion
        }

        public async Task<DataSet> ExecuteAsync(RequestCollection requests)
        {
            //#region [WEBSERVICE]
            //var xml = requests.ToXml();
            //var text =await DependencyService.Get<IHttpPost>().HttpPost(xml, this);
            //var bytes = Convert.FromBase64String(text);
            //var ds = DependencyService.Get<ISerializer>().Decompress<DataSet>(bytes);
            //return ds;
            //#endregion

            #region [CONNECT DIRECT DATABASE]
            var ds = DependencyService.Get<IDataProvider>().ExcuteAsync(requests);
            return ds;
            #endregion
        }

        private static Stream GetStreamForResponse(HttpWebResponse webResponse)
        {
            Stream stream;

            stream = webResponse.GetResponseStream();
            //switch (webResponse..ToUpperInvariant())
            //{
            //    case "GZIP":
            //        stream = new GZipStream(webResponse.GetResponseStream(), CompressionMode.Decompress);
            //        break;

            //    case "DEFLATE":
            //        stream = new DeflateStream(webResponse.GetResponseStream(), CompressionMode.Decompress);
            //        break;

            //    default:
            //        stream = webResponse.GetResponseStream();
            //        break;
            //}
            return stream;
        }

      
    }
}