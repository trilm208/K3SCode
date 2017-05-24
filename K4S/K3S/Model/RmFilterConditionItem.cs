using System;

namespace K3S.Model
{
    public class RmFilterConditionItem: Realms.RealmObject
    {
       
       
        public string ProjectID
        {
            get;
            set;
        }
        public string VariableName
        {
            get;
            set;
        }
        public string FilterCondition
        {
            get;
            set;
        }
      
    }
}
