
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace K3S.Model
{
    public class RmProfileImageItem :Realms.RealmObject
    {

        [PrimaryKey]
        public string ID { get; set; }
        public string AnswerID { get; set; }

        public string PathImage { get; set; }
       
        public string CreatedOn { get; set; }

        public string ImageBase64 { get; set; }
    }
}
