using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace K3S.GenControls
{
    public class GenControl: ContentView
    {
        public virtual Dictionary<string, bool> CheckValidation()
        {
            return null;
        }
        public virtual List<Model.RmGenericFormValueItem> GetListResultValue()
        {
            return null;
        }

    }
}
