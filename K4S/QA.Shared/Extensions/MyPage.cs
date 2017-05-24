
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Extensions
{
    public class MyPage: ContentPage
    {
        public ClientServices Services;
        public MyPage(ClientServices clientService)
        {
            Services = clientService;
        }
    }
}
