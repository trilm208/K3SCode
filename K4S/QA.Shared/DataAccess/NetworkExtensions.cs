using System;
using System.Net;
using System.Threading.Tasks;
using Plugin.Connectivity;
using XLabs.Forms;
using XLabs.Platform.Services;

namespace DataAccess
{
    public static class NetworkExtensions
    {
        public static string ServerIP = "124.158.14.32";

        public static int ServerPort = 80;

        public static string ServerHost = string.Format("{0}:{1}", ServerIP,ServerPort);

        public static async Task<bool> CheckNetworkAvailable()
        {
            var result = await CrossConnectivity.Current.IsRemoteReachable(ServerIP, ServerPort, 50);
            return result;
        }
       
        public static bool IsNetworkAvailable
        {
            get
            {
                var result= CrossConnectivity.Current.IsRemoteReachable(ServerIP, ServerPort,  5000);
                return result.Result;
            }

        }
    }
}
