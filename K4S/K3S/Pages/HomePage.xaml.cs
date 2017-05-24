using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using DataAccess;
using Extensions;
using K3S.Controls;
using K3S.Model;
using MyDependencyServices;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using System.Linq;
using Realms;

namespace K3S
{
    public partial class HomePage : ContentPage
    {
        #region [VARIABLES]
        ClientServices Services;

        private Realm realm;
          
        #endregion

        #region [CONSTRUCTOR]


        public HomePage()
        {
            try
            {
                var con = RealmConfiguration.DefaultConfiguration;
                con.SchemaVersion = 1;
                realm = Realms.Realm.GetInstance(con);
                InitializeComponent();
                this.Title = "K4S Version " + Version.Plugin.CrossVersion.Current.Version.ToString();

            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region [METHOD]

        void InitializeServices()
        {
            Services = new ClientServices();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        internal async Task Process()
        {
            InitializeServices();
            var last_username = SettingHelper.GetSetting("last_username", "Guest");
            txtUsername.Text = "Hi " + last_username;
            gProjectList.Children.Clear();
            await LoadLocalProjectList();
            //tu dong lay danh sach du an ve neu ko lay duoc thi lay danh sach du an cu gan nhat cua user
            //var CheckNetworkAvailable = DependencyService.Get<Shared.DependencyService.DSNetWorkExtensions>().CheckNetworkAvailable();
            //if (CheckNetworkAvailable == false)
            //{
            //    UI.ShowError("Không có kết nối internet.Dữ liệu sẽ ấy lần tải gần trên thiết bị", 5000);
            //}
            //else
            //{
            //    btnUpdate_Click(null, null);
            //}
        }

        async Task LoadLocalProjectList()
        {
            var projects = RealmHelper.ProjectGetAll();//  App.dbRealm.All<RmProjectItem>().ToList() ;
            DisplayProjectList(projects);
        }

        void DisplayProjectList(List<RmProjectItem> projects)
        {
            int i = 0;
            gProjectList.Children.Clear();
            foreach (RmProjectItem dr in projects)
            {
                ProjectView c_project = new ProjectView(dr);
                c_project.InitializeServices(Services);
                gProjectList.Children.Add(c_project, 0, i);
                i++;
            }
        }
       
        async Task GetNewestProjectListRealm()
        {
            // kiem tra neu project do co roi thi khong can update lai version
            var _username = SettingHelper.GetSetting("last_username", "");
            if (_username == "")
            {
                //return "Đăng nhập trước khi cập nhập danh sách dự án tham gia"; // use must login before load
            }

            var query = DataAccess.DataQuery.Create("Docs", "ws_L_Project_ListForK3SRealm", new
            {
                Username = _username
            });

            var ds =  Services.Execute(query);

            if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
            {
                //return Services.LastError;
            }
            else
            {
                //save to local device     
                var tblProject = ds.Tables[0];
                //await App.dbProject.DeleteItemsAsync();   

                var oldProject = App.dbRealm.All<RmProjectItem>();
                try
                {
                    var _listProject = DependencyService.Get<IDataTableExtensions>().ToModel<RmProjectItem>(tblProject);
                    foreach (var item in _listProject)
                    {
                        var queryTemp = item.ProjectID;
                        var projectItem = RealmHelper.ProjectGetByID(item.ProjectID);// App.dbRealm.All<RmProjectItem>().Where(o => o.ProjectID == queryTemp).First() ;

                        if (projectItem == null)
                        {
                            item.CurrentVersion = 0;
                            RealmHelper.ProjectAdd(item);
                        }
                        else
                        {
                            //int currentVersion = projectItem.CurrentVersion;
                            RealmHelper.realm.Write(() =>
                            {

                                projectItem.AcceptGender = item.AcceptGender;
                                projectItem.CityHandle = item.CityHandle;
                                projectItem.FontSize = item.FontSize;
                                projectItem.IsAutoNextPage = item.IsAutoNextPage;
                                projectItem.IsCamera = item.IsCamera;
                                projectItem.IsGPS = item.IsGPS;
                                projectItem.IsRecord = item.IsRecord;
                                projectItem.MaxYOB = item.MaxYOB;
                                projectItem.MinYOB = item.MinYOB;
                                projectItem.ProjectName = item.ProjectName;
                                projectItem.ProjectNo = item.ProjectNo;

                            });
                        }
                    }
                    foreach (var item in oldProject)
                    {
                        if (IsActivedProjectRealm(item, _listProject) == false)
                        {
                            RealmHelper.ProjectDelete(item);
                        }
                    }
                }
                catch(Exception ex)
                {
                    UI.ShowError(ex.Message);
                    return;
                }
                                
            }
        }
       
        bool IsActivedProjectRealm(RmProjectItem item, List<RmProjectItem> list)
        {
            foreach (var i in list)
            {
                if (item.ProjectID == i.ProjectID)
                {
                    return true;
                }
            }
            return false;
        }

        async Task UpdateWardDistrictCity()
        {
            Acr.UserDialogs.IUserDialogs Dialog = Acr.UserDialogs.UserDialogs.Instance;
            Dialog.ShowLoading("Đang download danh sách phường & quận từ máy chủ...");

        

            try
            {
                var query = DataAccess.DataQuery.Create("KadenceDB", "ws_L_WardDistrictCity_ListAll");
                var ds =  Services.Execute(query);
                if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
                {
                    Dialog.HideLoading();
                    UI.ShowError("Tải thất bại có lỗi.Kiễm tra kết nối internet.Mã lỗi: " + Services.LastError, 4000);
                    return;
                }
                else
                {
                    Dialog.HideLoading();
                    Dialog.ShowLoading("Đang lưu dữ liệu phường & quận mới nhất.Vui lòng đợi quá trình hoàn tất...");
                    var tblWardDistrictCity = ds.Tables[0];
                    //await App.dbWardDistrictCity.DeleteItemsAsync();
                    List<RmWardDistrictCityItem> _listWardDistrictCity = DependencyService.Get<IDataTableExtensions>().ToModel<RmWardDistrictCityItem>(tblWardDistrictCity);

                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    RealmHelper.WardInsertAll(_listWardDistrictCity);
                    watch.Stop();

                    MyDebugger.WriteLog(string.Format("Realm: inserted  records into Realm in {0} milliseconds\n", watch.ElapsedMilliseconds));
                    Dialog.HideLoading();
                    UI.ShowSuccess("Cập nhật danh sách phường& quận thành công.");
                }
            }
            catch (Exception ex)
            {
                Dialog.HideLoading();
                UI.ShowError("Kết nối máy chủ thất bại..." + Services.LastError, 4000);
            }
        }
        #endregion

        #region [EVENTS]
        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            Acr.UserDialogs.IUserDialogs Dialog = Acr.UserDialogs.UserDialogs.Instance;
            Dialog.ShowLoading("Đang cập nhật danh sách dự án...");
            await GetNewestProjectListRealm();
            await LoadLocalProjectList();
          
          

            Dialog.HideLoading();
       
        }

       

        private async void btnUpdateWard_Click(object sender, EventArgs e)
        {
            await UpdateWardDistrictCity();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {

            LoginPopupPage _loginPopup = new LoginPopupPage();
            _loginPopup.Authenticated += LoginPopup_Authenticated;
            _loginPopup.Initialize(Services);
            await Navigation.PushPopupAsync(_loginPopup);
        }

        void LoginPopup_Authenticated(object sender, EventArgs e)
        {
            var last_username = SettingHelper.GetSetting("last_username", "Guest");
            txtUsername.Text = last_username;
            btnUpdate_Click(null, null);
        }
        #endregion

    }

    internal class Person: RealmObject
    {
        public string Name { get; set; }
    }
}
