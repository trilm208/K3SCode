using System;

namespace K3S.Model
{
    public class RmLogicCheckItem:Realms.RealmObject
    { 
           
       public string ProjectID
        {
            get;
            set;
        }
        public string FromPage
        {
            get;
            set;
        }
        public string SkipType
        {
            get;
            set;
        }
        public string SkipLogic
        {
        	get;
            set;
        }
        public string SkipToPage
        {
            get;
            set;
        }
        public string Comments
        {
            get;
            set;
        }
   
    }
}
