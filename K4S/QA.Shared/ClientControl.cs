using System;
using Xamarin.Forms;
using DataAccess;

namespace Shared
{
    public partial class  ClientControl: ContentPage
    {
        private IClientServices Services;
        public virtual void Initialize(IClientServices services)
        {
            
             this.Services = services;
        }

        public virtual void Process()
        {
            
        }
        
    }
}
