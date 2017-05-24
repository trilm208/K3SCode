
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3S.Model
{
    public class RmInterviewTimeItem: Realms.RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string AnswerID { get; set; }
        public string ProjectID { get; set; }
        public string UserID { get; set; }
        public string StartOn { get; set; }
        public string CompletedOn { get; set; }

    }
}
