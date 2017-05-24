using System;
using Realms;
namespace K3S.Model
{
    public class GenericFormQuestionAndroidAnswerItem
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string QuestionID
        {
            get;
            set;
        }

      
        public string AnswerID { get; set; }

        public string ProjectID
        {
            get;
            set;
        }
        public int QuestionTypeID
        {
            get;
            set;
        }
        public int OrderIndex
        {
            get;
            set;
        }
       
        public string QuestionCode
        {
            get;
            set;
        }
      
        public string QuestionNameHTMLText
        {
            get;
            set;
        }
      
        public string SingleChoice_Answer_AnswerList
        {
            get;
            set;
        }
      
       
        public int SingleChoice_AnswerFormat
        {
            get;
            set;
        }
      
        public string SingleChoice_DefaultText
        {
            get;
            set;
        }
        public string Number_MaxValue
        {
            get;
            set;
        }
        public string Number_MinValue
        {
            get;
            set;
        }
        public bool Number_AllowDecimalAnswer
        {
            get;
            set;
        }
                      
        public string MultiChoice_Answer_AnswerList
        {
            get;
            set;
        }
       
        public int MultiChoice_MinChecks
        {
            get;
            set;
        }
        public int MultiChoice_MaxChecks
        {
            get;
            set;
        }
        public int MultiChoice_TypeOfNumberChecks
        {
            get;
            set;
        }
       
        public int GroupRandom
        {
            get;
            set;
        }

        public string HideCondition
        {
            get;
            set;
        }
    }
}
