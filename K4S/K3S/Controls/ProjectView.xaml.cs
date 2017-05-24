using System;
using System.Collections.Generic;
using System.Data;
using Xamarin.Forms;
using Acr.UserDialogs;
using Extensions;
using DataAccess;
using MyDependencyServices;
using Shared;
using K3S.Model;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Realms;

namespace K3S.Controls
{
    public partial class ProjectView : ContentView
    {
        #region [VARIABLES]
        private ClientServices Services;
        private RmProjectItem ProjectItem;
        string ProjectID { get; set; }
        #endregion

        #region [CONSTRUCTOR]
        public ProjectView(RmProjectItem item)
        {
            InitializeComponent();
            ProjectItem = item;
            ProjectID = item.ProjectID.ToString();
            lblProjectNo.Text = item.ProjectNo;
            lblProjectName.Text = item.ProjectName;
            lblVersionQne.Text = "Version: " + item.CurrentVersion.ToString();

            HeightRequest = 150;

        }

        private async void btnStartSurvey_Clicked(object sender, System.EventArgs e)
        {

            IUserDialogs _dg = UserDialogs.Instance;
            _dg.ShowLoading(string.Format("Đang truy cập dữ liệu của dự án {0}", ProjectItem.ProjectNo));
            try
            {
                
                string mess = "";
                var listHanhchanh = RealmHelper.WardGetAll();//  App.dbRealm.All<RmWardDistrictCityItem>().ToList();
                App._listHanhChanh = listHanhchanh;
               
                if (listHanhchanh == null || listHanhchanh.Count == 0)
                {
                    mess = "Không tìm thấy dữ liệu quận, phường .Vui lòng bấm nút Sync Ward hoặc liên hệ kĩ thuật để được phân thành phố thực hiện";
                }
                if (ProjectItem.CurrentVersion == 0)
                {
                    mess = Environment.NewLine + "Phiên bản hiện tại trên thiết bị bạn là 0.Vui lòng Sync dự án bạn muốn thực hiện";

                }
                var listQuestion = RealmHelper.QuestionGetByProjectID(ProjectItem.ProjectID);//  await App.dbGenericFormQuestionAndroid.GetItemsAsync(ProjectItem.ProjectID);
                App._listOriginalQuestion = listQuestion;
               bool true2 = false;
                foreach (var item in listQuestion)
                {
                    if (item.QuestionCode == "PGEND")
                    {
                        true2 = true;
                    }
                }

                if (true2 != true)
                {
                    mess += " Bạn bị lỗi trong quá trình Sync BCH.Vui lòng vào setting-> ứng dụng-> K3S-> Xóa dữ liệu ";
                }

                if (mess.Length > 0)
                {
                    UI.ShowError(mess);
                    return;
                }
                else
                {
                    await GoToSurvey(listQuestion);
                    _dg.HideLoading();
                }
            }
            catch (Exception ex)
            {
                _dg.HideLoading();
                UI.ShowError(ex.Message);
                return;
            }
           
        }

        private async Task GoToSurvey(List<RmGenericFormQuestionAndroidItem> listQuestion)
        {
            var page = new SurveyHomePage(ProjectItem);
            page.InitializeServices(Services);
            await page.Process();
            Application.Current.MainPage = page;
        }

        #endregion

        #region [EVENTS]
       


        public async void btnUpdate_Click(object sender, System.EventArgs e)
        {
         
            SyncRealm(ProjectID);        
        }

        private async void SyncRealm(string projectID)
        {          
            Stopwatch w1 = new Stopwatch();
            w1.Start();
            string _username = SettingHelper.GetSetting("last_username", "");
            if (_username == "")
            {
                return;
            }
            w1.Stop();
            MyDebugger.WriteLog("Get username :" + w1.ElapsedMilliseconds + " miliseconds");
            await Task.Delay(100);

            var _downloadResult = await DownloadDataRealm(_username);
            
            if (_downloadResult == null)
            {
             
                return;
            }
          
            var tableQuestions = _downloadResult[0];
            var tableLogicChecks = _downloadResult[1];
            var tableFilters = _downloadResult[2];
            var tableProject = _downloadResult[3];
        
            int currentVersion = (int)tableProject.Rows[0]["CurrentVersion"];
            try
            {             
                App.dbRealm.Write(() =>
               {
                   ProjectItem.CurrentVersion = currentVersion;
                   ProjectItem.FontSize = (int)tableProject.Rows[0]["FontSize"];
                   ProjectItem.IsAutoNextPage = (bool)tableProject.Rows[0]["IsAutoNextPage"];
                   ProjectItem.IsGPS =false;
                   ProjectItem.IsRecord = false;
                   ProjectItem.IsCamera = (bool)tableProject.Rows[0]["IsCamera"];
               });

                using (UserDialogs.Instance.Loading("Đang lưu vào Realm...", null, null, true, MaskType.Black))
                {
                   
                    bool resultSaveRealm = await SaveDataToRealm(tableQuestions, tableLogicChecks, tableFilters);
                    if (resultSaveRealm == false)
                    {
                        UI.ShowError("Quá trình cập nhật BCH thất bại. Thử lại sau...");
                        return;
                    }
                   
                    lblVersionQne.Text = "Version: " + currentVersion.ToString();
                }
                UI.ShowSuccess("Cập nhật hoàn tất.");
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<bool> SaveDataToRealm(DataTable tableQuestions, DataTable tableLogicChecks, DataTable tableFilters)
        {
            try
            {            
                var list_Question = DependencyService.Get<IDataTableExtensions>().ToModel<RmGenericFormQuestionAndroidItem>(tableQuestions);
                var list_LogicCheck = DependencyService.Get<IDataTableExtensions>().ToModel<RmLogicCheckItem>(tableLogicChecks);
                var list_FilterCondition = DependencyService.Get<IDataTableExtensions>().ToModel<RmFilterConditionItem>(tableFilters);                     
           
                RealmHelper.ProjectStructureUpdate(ProjectItem.ProjectID, list_Question,list_LogicCheck,list_FilterCondition);             
                return true;
            }
            catch(Exception ex)
            {
               return false;
            }
        }
        #endregion

        #region [METHOD]
        public void InitializeServices(ClientServices _services)
        {
            this.Services = _services;

        }

        private async Task<List<DataTable>> DownloadDataRealm(string _username)
        {
            try
            {
                var dg = Acr.UserDialogs.UserDialogs.Instance;
                dg.ShowLoading("Đang download dữ liệu BCH từ máy chủ...");
                await Task.Delay(100);
                Stopwatch w1 = new Stopwatch();
                w1.Start();
                List<DataTable> _listResult = new List<DataTable>();
                var query = DataAccess.DataQuery.Create("Docs", "ws_DOC_GenericFormQuestionAndroid_GetForRealm", new
                {
                    ProjectID,
                    Username = _username
                });
               

                query += DataAccess.DataQuery.Create("Docs", "ws_DOC_LogicCheck_GetForRealm", new
                {
                    ProjectID,
                    Username = _username
                });
                query += DataAccess.DataQuery.Create("Docs", "ws_DOC_FilterCondition_GetForRealm", new
                {
                    ProjectID,
                    Username = _username
                });
                query += DataAccess.DataQuery.Create("Docs", "ws_L_Project_GetForRealm", new
                {
                    ProjectID,
                    Username = _username
                });

                var ds =  Services.Execute(query);
                if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
                {
                    dg.HideLoading();
                    UI.ShowError("Xảy ra lỗi trong quá trình download dữ liệu dự án từ máy chủ.Vui lòng thử lại sau hoặc kiểm tra kết nối Internet của bạn: " + Services.LastError);
                    return null;
                }

                _listResult.Add(ds.Tables[0]);
                _listResult.Add(ds.Tables[1]);
                _listResult.Add(ds.Tables[2]);
                _listResult.Add(ds.Tables[3]);
                w1.Stop();
                dg.HideLoading();
                MyDebugger.WriteLog("Download take :" + w1.ElapsedMilliseconds + " miliseconds");
                return _listResult;
            }
            catch (Exception ex)
            {
                UI.ShowError("Lỗi khi cập nhật phiên bản BCH.Vui lòng liên hệ kĩ thuật để hổ trợ " + ex.Message);
                MyDebugger.WriteLog("Error when sync version project " + ex.Message);
                return null;
            }
        }
       
        #endregion
    }
}
