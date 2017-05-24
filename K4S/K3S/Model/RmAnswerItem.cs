using Realms;
using System;


namespace K3S.Model
{
    public class RmAnswerItem: Realms.RealmObject
    {
        [PrimaryKey]
        public string AnswerID
        {
            get;
            set;
        }
        public string ProjectID
        {
            get;
            set;
        }
        public String RespondentFullName
        {
            get;
            set;
        }
        public String RespondentAddressLandmark
        {
            get;
            set;
        }
        public String RespondentStreet
        {
            get;
            set;
        }
        public String RespondentWard
        {
            get;
            set;
        }
        public String RespondentDistrict
        {
            get;
            set;
        }
        public String RespondentCity
        {
            get;
            set;
        }
        public String RespondentTelephone
        {
            get;
            set;
        }
        public String RespondentGender
        {
            get;
            set;
        }
        public int RespondentYoB
        {
            get;
            set;
        }
     
        public string AccessPageList
        {
            get;
            set;
        }
     
        public string QuestionVersion
        {
            get;
            set;
        }
        public string AppVersion
        {
            get;
            set;
        }
     
       
        public string EmailAddress
        {
            get;
            set;
        }
        public string GreenID { get;  set; }

        public string CreatedOn
        {
            get;
            set;
        }

        public bool IsSubmitted
        {
            get;
            set;
        }
        public string CompletedOn
        {
            get;
            set;
        }

        public string LastestSubmittedOn
        {
            get;
            set;
        }

        public string AnswerQuestionnaireStructureTable { get; set; }
        public string AccessQuestionCodeList { get; internal set; }
    }
}

 

