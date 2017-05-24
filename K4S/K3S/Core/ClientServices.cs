using DataAccess;
using System.Collections.Generic;
using System.Data;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;

using Extensions;
using MyDependencyServices;

namespace DataAccess
{
    public class ClientServices : IClientServices
    {
        private Config Config { get; set; }

        private HttpDataServices DataService { get; set; }

        private DataTable Localization { get; set; }

        private DataTable Setting { get; set; }

        private List<string> Permissions { get; set; }

        private List<string> UserPermissions { get; set; }

        public string LastError { get; private set; }

        private Dictionary<string, object> SharedInfo = new Dictionary<string, object>();

        public string FacID { get; private set; }
       

        public ClientServices()
        {
            //this.Config = DataAccess.Config.Load();
            FacID = "CL";
            this.DataService = new HttpDataServices("http://10.0.2.2:8080/DataAccess.ashx");
            this.Permissions = new List<string>();
            this.UserPermissions = new List<string>();
        }

        public void Initialize()
        {
            //var query = DataQuery.Cached("Application", "ws_Localization_Get", new { Culture = Config["Culture"] });
            //query += DataQuery.Create("Application", "ws_Settings_List");
            //var ds = this.Execute(query);
            //this.Localization = ds.Tables(0);
            //this.Setting = ds.Tables(1);
        }

        //public async void LoadPermissions()
        //{
        //    var query = DataQuery.Create("Security", "ws_Permissions_List", new { FacID =this.FacID });
        //    query += DataQuery.Create("Security", "ws_UserPermissions_List",
        //    new { 
        //        FacID =this.FacID
        //    });

        //    var ds = this.Execute(query);

        //    if (DependencyService.Get<MyDependencyServices.IDataSetExtension>().IsNull(ds) == true)
        //    {
              
        //        return;
        //    }
        //    Permissions.Clear();
           
        //    var table = ds.Tables[0];
        //    foreach (DataRow row in table.Rows)
        //    {
        //        Permissions.Add(row["Key"].ToString().ToUpper());
        //    }
        //    UserPermissions.Clear();
          
        //    table = ds.Tables[1];
        //    foreach (DataRow row in table.Rows)
        //    {
        //        UserPermissions.Add(row["Key"].ToString().ToUpper());
        //    }
        //}

      

        public void LoadSettings(string FacID)
        {
            
        }

        public string Localize(string name)
        {
            return "";
        }

        public string Localize(string category, string name)
        {
           
            return "";
        }

        public string GetSetting(string name)
        {
           
            return "";
        }

        public string GetSetting(string category, string name)
        {
          
            return "";
        }

        public   DataSet Execute(RequestCollection requests)
        {
            this.LastError = "";
            try
            {
                var ds = DataService.Execute(requests);

                var table = ds.Tables[0];
                try
                {
                    if (table.TableName == "Error")
                    {
                        var row = table.Rows[0];

                        var message = row["Message"].ToString();
                        var source = row["Source"].ToString();
                        var stackTrace = row["StackTrace"].ToString();
                        var helpLink = row["HelpLink"].ToString();

                        this.LastError = message;
                        ds = null;
                    }
                }
                catch
                {

                }
                return ds;
            }
            catch(Exception ex)
            {
                UI.ShowError("Kết nối máy chủ thất bại.Chi tiết lỗi:"+ ex.Message);
                return null;
            }
           
        }
        public async Task<DataSet> ExecuteAsync(RequestCollection requests)
        {
            this.LastError = "";
            try
            {
                return null;
                //var ds = await DataService.ExecuteAsync(requests);

                //var table = ds.Tables[0];
                //try
                //{
                //    if (table.TableName == "Error")
                //    {
                //        var row = table.Rows[0];

                //        var message = row["Message"].ToString();
                //        var source = row["Source"].ToString();
                //        var stackTrace = row["StackTrace"].ToString();
                //        var helpLink = row["HelpLink"].ToString();

                //        this.LastError = message;
                //        ds = null;
                //    }
                //}
                //catch
                //{

                //}
                //return ds;
            }
            catch (Exception ex)
            {
                UI.ShowError("Kết nối máy chủ thất bại.Chi tiết lỗi:"+ex.Message);
                return null;
            }

        }
        public void SetInformation(string key, object value)
        {
            if (SharedInfo.ContainsKey(key))
                SharedInfo[key] = value;
            else
                SharedInfo.Add(key, value);
        }

        public object GetInformation(string key)
        {
            if (SharedInfo.ContainsKey(key))
                return SharedInfo[key];

            return null;
        }

        public object this[string key]
        {
            get { return GetInformation(key); }
            set { SetInformation(key, value); }
        }

     
        public bool HasPermission(string key)
        {
            key = key.ToUpper();
            if (UserPermissions.Contains(key))
                return true;

            if (Permissions.Contains(key))
                return false;

            var query = DataQuery.Create("Security", "ws_Permissions_Create", new { Key = key });
            this.Execute(query);

            Permissions.Add(key);

            return false;
        }
    }
}