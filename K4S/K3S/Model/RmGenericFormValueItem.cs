
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K3S.Model
{
    public class RmGenericFormValueItem :Realms.RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string AnswerID { get; set; }

        public string QuestionID { get; set; }

        public string ProjectID { get; set; }

        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        public string QuestionCode { get; set; }

        public string PageCode { get; set; }

        public string FieldType { get; set; }

        public string AnswerText { get; set; }

        public int PageID { get; set; }

        public string QuestionType { get; set; }
    }
}
