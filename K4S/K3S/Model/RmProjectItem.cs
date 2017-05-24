using Realms;
using System;


namespace K3S.Model
{
    public class RmProjectItem: Realms.RealmObject
    {
        [PrimaryKey]
        public string ProjectID { get; set; }
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public int CurrentVersion { get;  set; }
        public string CityHandle { get;  set; }
        public int MinYOB { get;  set; }
        public int MaxYOB { get;  set; }
        public string AcceptGender { get;  set; }
        public int FontSize { get; set; }
        public bool IsGPS { get;  set; }
        public bool IsCamera { get;  set; }
        public bool IsRecord { get;  set; }
        public bool IsAutoNextPage { get;  set; }
        public string ReturnPageCodeNextVersion { get; set; }
    }
}
