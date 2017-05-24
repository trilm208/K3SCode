using System;
using Realms;
namespace K3S.Model
{
    public class RmWardDistrictCityItem:RealmObject
    {
        
        public string Ward
        {
            get;
            set;
        }
        public string District
        {
        	get;
            set;
        }

        public string City
        {
        	get;
            set;
        }

    }
}
