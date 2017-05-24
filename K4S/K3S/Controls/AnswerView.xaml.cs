using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using Extensions;
using MyDependencyServices;
using System.Linq;
using Acr.UserDialogs;

using System.Diagnostics;
using DataAccess;
using Newtonsoft.Json;
using System.Data;
using Plugin.Geolocator;
using Realms;

namespace K3S
{
    public partial class AnswerView : ContentView
    {
        #region [KHAI BÁO BIẾN]

        RmAnswerItem AnswerItem;
        RmProjectItem ProjectItem;
        private bool IsCompleted;
        private ClientServices Services;
        public event EventHandler Submitted;

        #endregion

        #region [CONSTRUCTOR]
        public AnswerView(RmAnswerItem aItem, RmProjectItem pItem)
        {
            InitializeComponent();
            this.AnswerItem = aItem;
            string ID = aItem.AnswerID;
            var tempAnswerItem = App.dbRealm.All<RmAnswerItem>().Where(o => o.AnswerID==ID).First();
            this.ProjectItem = pItem;
            var PageCode = App._listOriginalQuestion.Where(o => o.QuestionTypeID == 0).ToList();
            var lastPageCode = PageCode[PageCode.Count - 1].QuestionCode;
            IsCompleted = false;
            string mess = "Ver:" + tempAnswerItem.QuestionVersion + " ";
            if (tempAnswerItem.AccessQuestionCodeList != null && tempAnswerItem.AccessQuestionCodeList != "")
            {
                var codes = (List<String>)Newtonsoft.Json.JsonConvert.DeserializeObject(tempAnswerItem.AccessQuestionCodeList, typeof(List<String>));
                if (codes.Count > 0)
                {
                    var lastCode = codes[codes.Count()-1];
                    if (lastCode == lastPageCode)
                    {
                        IsCompleted = true;
                        mess += "Hoàn thành";
                    }
                    else
                    {
                        mess +=lastCode;
                    }
                }
                else
                {
                    mess += "Chưa phỏng vấn";
                }
            }
            else
            {
                mess += "Chưa hoàn thành";
            }
            try
            {
                if (tempAnswerItem.LastestSubmittedOn != null && tempAnswerItem.LastestSubmittedOn.Length > 0)
                {
                    mess += " & Submitted:" + tempAnswerItem.LastestSubmittedOn;
                    //frame.BackgroundColor = Color.Green;
                }
                else
                {
                    mess += " & Chưa submit";
                }
            }
            catch(Exception ex)
            {

            }
           

            txtStatus.Text = mess;

        }
        #endregion

        #region [METHOD]

        internal void InitializeServices(ClientServices services)
        {
            this.Services = services;
        }

        internal void Process()
        {
            txtGreenID.Text = AnswerItem.GreenID;
            txtRespondentFullName.Text = AnswerItem.RespondentFullName + "   ";          
        }

        private Task<string> Submit(string userid)
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            try
            {

                Services = new ClientServices();
                List<string> listAccessQuestionCode = new List<string>();
                string ProjectID = ProjectItem.ProjectID;
                string AnswerID = AnswerItem.AnswerID;

                return Task.Run(() =>
                {
                    var con = RealmConfiguration.DefaultConfiguration;
                    con.SchemaVersion = 1;
                    var realm = Realms.Realm.GetInstance(con);

                    var _listQuestion = realm.All<RmGenericFormQuestionAndroidItem>().Where(o => o.ProjectID == ProjectID).ToList();// (ProjectItem.ProjectID);// await App.dbGenericFormQuestionAndroid.GetItemsAsync(ProjectItem.ProjectID);// ApplicationExtensions.getSetting("templateDOC_GenericFormQuestionAndroid_" + ProjectID); //Intent.GetStringExtra("QuestionProject") ?? "";
                    var _data = realm.All<RmGenericFormValueItem>().Where(o => o.AnswerID == AnswerID).ToList();// RealmHelper.ResultValueGetByAnswer(AnswerItem.AnswerID);
                    var _answerItem = realm.All<RmAnswerItem>().Where(o => o.AnswerID == AnswerID).First();
                    if (_listQuestion.Count == 0)
                    {

                        tcs.SetResult("Lưu thất bại.Không tìm thấy BCH của dự án này.Liên hệ kĩ thuật");
                        return tcs.Task;
                    }
                    if (_data.Count == 0)
                    {
                        tcs.SetResult("Lưu thất bại.Không tìm thấy dữ liệu trả lời được lưu trữ.Liên hệ kĩ thuật");
                        return tcs.Task;
                    }
                    List<String> listAccessPageCode = new List<string>();
                    try
                    {
                        listAccessPageCode = (List<String>)Newtonsoft.Json.JsonConvert.DeserializeObject(_answerItem.AccessPageList, typeof(List<String>));
                    }
                    catch
                    {
                        tcs.SetResult("BCH chưa được trả lời");
                        return tcs.Task;
                    }
                    realm.Write(() =>
                    {
                        for (int i = _data.Count - 1; i >= 0; i--)
                        {
                            var c_row = _data[i];
                            if (listAccessPageCode.Contains(c_row.PageCode) == false)
                            {
                                c_row.FieldValue = "";
                                c_row.AnswerText = "";
                            }
                        }                       
                    });
                  
                    var query = DataAccess.DataQuery.Create("Docs", "ws_DOC_InterviewTablet_SaveRealm", new
                    {
                        AnswerID = _answerItem.AnswerID,
                        GreenID = _answerItem.GreenID,
                        ProjectID = _answerItem.ProjectID,
                        RespondentFullName = _answerItem.RespondentFullName,
                        RespondentAddressLandmark = _answerItem.RespondentAddressLandmark,
                        RespondentStreet = _answerItem.RespondentStreet,
                        RespondentWard = _answerItem.RespondentWard,
                        RespondentDistrict = _answerItem.RespondentDistrict,
                        RespondentCity = _answerItem.RespondentCity,
                        RespondentTelephone = _answerItem.RespondentTelephone,
                        RespondentGender = _answerItem.RespondentGender,
                        RespondentYoB = _answerItem.RespondentYoB,
                        RespondentStatus = "Ok",
                        AttendentDate = _answerItem.CreatedOn,
                        QuestionVersion = _answerItem.QuestionVersion,
                        AppVersion = Version.Plugin.CrossVersion.Current.Version.ToString(),
                        EmailAddress = _answerItem.EmailAddress,
                        AnswerQuestionnaireStructureTable = _answerItem.AnswerQuestionnaireStructureTable,
                        AccessQuestionCodeList = _answerItem.AccessQuestionCodeList,
                        AccessPageList = _answerItem.AccessPageList,
                        UserID = userid
                    });

                    //lay all value
                    string QuestionIDs = "";
                    string AnswerTexts = "";
                    string QuestionCodes = "";
                    string PageCodes = "";
                    string FieldNames = "";
                    string FieldValues = "";
                    string FieldTypes = "";
                    string PageIDs = "";

                    foreach (var row in _data)
                    {
                        QuestionIDs += row.QuestionID + "~";
                        QuestionCodes += row.QuestionCode + "~";
                        PageCodes += row.PageCode + "~";
                        FieldNames += row.FieldName + "~";
                        FieldTypes += row.FieldType + "~";
                        PageIDs += row.PageID + "~";
                        if (listAccessPageCode.Contains(row.PageCode))
                        {
                            AnswerTexts += row.AnswerText + "~";
                            FieldValues += row.FieldValue + "~";
                        }
                        else
                        {
                            AnswerTexts += " " + "~";
                            FieldValues += " " + "~";
                        }
                    }
                    query += DataAccess.DataQuery.Create("Docs", "ws_DOC_InterviewGenericFormValues_Save1Time", new
                    {
                        AnswerID = AnswerID,
                        QuestionIDs,
                        ProjectID = ProjectID,
                        AnswerTexts,
                        QuestionCodes,
                        PageCodes,
                        FieldNames,
                        FieldValues,
                        FieldTypes,
                        UserID = userid
                    });
                    var interviewTimeList = realm.All<RmInterviewTimeItem>().Where(o => o.AnswerID == AnswerID).ToList();
                    if (interviewTimeList.Count > 0)
                    {
                        foreach (var timeInterview in interviewTimeList)
                        {
                            string completeOn = timeInterview.CompletedOn;
                            if (completeOn == null)
                                completeOn = timeInterview.StartOn;
                            query += DataAccess.DataQuery.Create("Docs", "ws_DOC_InterviewTime_Save", new
                            {
                                ProjectID = ProjectID,
                                AnswerID = AnswerID,
                                ID = timeInterview.ID,
                                StartedOn = timeInterview.StartOn,
                                CompletedOn = completeOn,
                                UserID = userid
                            });
                        }
                    }
                    var imageProfiles = realm.All<RmProfileImageItem>().Where(o => o.AnswerID == AnswerID).ToList();
                    if (imageProfiles.Count > 0)
                    {
                        {
                            foreach (var pic in imageProfiles)
                            {
                                try
                                {
                                    //var data64 = DependencyService.Get<IFileHelper>().ReadAllBytes(pic.PathImage);
                                    query += DataAccess.DataQuery.Create("Docs", "ws_DOC_InterviewImageRespontdents_Save", new
                                    {
                                        ID = 0,
                                        ProjectID = ProjectID,
                                        AnswerID = AnswerID,
                                        Image = pic.ImageBase64,
                                        UserID = userid
                                    });
                                }
                                catch (Exception ex)
                                {
                                    UI.ShowError(ex.Message);

                                }
                            }
                        }
                    }
                    DataSet ds = new DataSet();
                    try
                    {
                        //ds = Services.Execute(query);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetResult("Vui lòng kiểm tra đường truyền mạng hoặc " + ex.Message);
                        return tcs.Task;
                    }
                    //if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
                    //{
                    //    tcs.SetResult(Services.LastError);
                    //    return tcs.Task;
                    //}
                    //realm.Write(() =>
                    //{
                    //    _answerItem.IsSubmitted = true;
                    //    _answerItem.LastestSubmittedOn = ds.Tables[0].Rows[0]["ServerDateTime"].ToString();
                    //});              
                    tcs.SetResult("Ok");
                    return tcs.Task;
                });             
            }
            catch (Exception ex)
            {
                tcs.SetResult(string.Format("Vui lòng liên hệ kĩ thuật vì quá trình submit bị lỗi {0}", ex.Message));
                return tcs.Task;             
            }
        }

        async Task ConvertToNewVersionAsync()
        {
            var structQuestionnaire = await CreateAnswerQuestionnaireStructureTableAsync();
            var finalQuestionList = new List<RmGenericFormQuestionAndroidAnswerItem>();

            RealmHelper.realm.Write(() =>
            {
                AnswerItem.QuestionVersion = ProjectItem.CurrentVersion.ToString();
                AnswerItem.AccessPageList = "";
                AnswerItem.AccessQuestionCodeList = "";

                foreach (var itemDic in structQuestionnaire)
                {
                    AnswerItem.AnswerQuestionnaireStructureTable = itemDic.Key;
                    finalQuestionList = itemDic.Value;
                }
            });

            var page = new RespondentInformationDetail(AnswerItem, ProjectItem, false, finalQuestionList);
            await page.Process();
            Application.Current.MainPage = page;

        }

        private async Task<Dictionary<string, List<RmGenericFormQuestionAndroidAnswerItem>>> CreateAnswerQuestionnaireStructureTableAsync()
        {

            Dictionary<string, string> dic = new Dictionary<string, string>();

            var QuestionList = RealmHelper.QuestionGetByProjectID(ProjectItem.ProjectID);
            //var QuestionList = await App.dbGenericFormQuestionAndroid.GetItemsAsync(ProjectItem.ProjectID);

            foreach (var item in QuestionList)
            {
                if (item.GroupRandom > 0)
                {
                    dic.Add(item.QuestionCode, item.OrderIndex + ";" + item.GroupRandom);
                }
            }

            foreach (var item in QuestionList)
            {
                if (item.GroupRandom > 0)
                {
                    List<string> c_list = new List<string>();

                    foreach (string c_dic in dic.Values)
                    {
                        if (c_dic.Split(';')[1] == item.GroupRandom.ToString())
                        {
                            c_list.Add(c_dic);
                        }
                    }
                    var result = DependencyService.Get<IListExtensions>().Shuffle<String>(c_list);
                    if (result.Count >= 1)
                    {
                        //tim 1 order index trong dong order index do

                        int _orderIndex = Convert.ToInt32(result[0].Split(';')[0]);
                        int replaceIndex = QuestionList.FindIndex(i => i.OrderIndex == _orderIndex);// tblQuestion._FindIndex("OrderIndex", result[0].Split(';')[0]);

                        string replaceOrderIndex = QuestionList[replaceIndex].OrderIndex.ToString();

                        string replaceCode = QuestionList[replaceIndex].QuestionCode;
                        string oldOrderIndex = item.OrderIndex.ToString();
                        QuestionList[replaceIndex].OrderIndex = item.OrderIndex;
                        item.OrderIndex = Convert.ToInt32(result[0].Split(';')[0]);
                        //tim order index cu update bang order index cu cua no
                        dic[replaceCode] = oldOrderIndex + ";" + item.GroupRandom;
                        dic.Remove(item.QuestionCode);
                    }

                }
                else
                {

                }
                QuestionList = QuestionList.OrderBy(i => i.OrderIndex).ToList();

            }
            var finalQuestion = ReOrderIndex(QuestionList);
            var convert_QuestionCode = ConvertShortOrderQuestionCode(finalQuestion);

            Dictionary<string, List<RmGenericFormQuestionAndroidAnswerItem>> returnObject = new Dictionary<string, List<RmGenericFormQuestionAndroidAnswerItem>>();
            returnObject.Add(convert_QuestionCode, finalQuestion);
            return returnObject;
        }

        string ConvertShortOrderQuestionCode(List<RmGenericFormQuestionAndroidAnswerItem> _list)
        {
            string result = "";
            foreach (var item in _list)
            {
                result += item.QuestionCode + "|";
            }
            return result;
        }

        async Task GoToSurvey(RmAnswerItem aItem, RmProjectItem pItem)
        {
            var finalQuestionList = await CreateFinalQuestionListFromOldStructure(aItem, pItem);
            var page = new RespondentInformationDetail(aItem, pItem, false, finalQuestionList);
            await page.Process();
            Application.Current.MainPage = page;
        }

        private async Task<List<RmGenericFormQuestionAndroidAnswerItem>> CreateFinalQuestionListFromOldStructure(RmAnswerItem aItem, RmProjectItem pItem)
        {

            return RealmHelper.QuestionAnswertGetByAnswerID(aItem.AnswerID);
        }

        private List<RmGenericFormQuestionAndroidAnswerItem> ReOrderIndex(List<RmGenericFormQuestionAndroidItem> _questionList)
        {
            List<RmGenericFormQuestionAndroidAnswerItem> result = new List<RmGenericFormQuestionAndroidAnswerItem>();
            for (int i = 0; i < _questionList.Count; i++)
            {
                //_questionList[i].OrderIndex = i + 1;
                var newitem = new RmGenericFormQuestionAndroidAnswerItem();
                var item = _questionList[i];
                newitem.OrderIndex = i + 1;
                newitem.GroupRandom = item.GroupRandom;
                newitem.HideCondition = item.HideCondition;
                newitem.MultiChoice_Answer_AnswerList = item.MultiChoice_Answer_AnswerList;
                newitem.MultiChoice_MaxChecks = item.MultiChoice_MaxChecks;
                newitem.MultiChoice_MinChecks = item.MultiChoice_MinChecks;
                newitem.MultiChoice_TypeOfNumberChecks = item.MultiChoice_TypeOfNumberChecks;
                newitem.Number_AllowDecimalAnswer = item.Number_AllowDecimalAnswer;
                newitem.Number_MaxValue = item.Number_MaxValue;
                newitem.Number_MinValue = item.Number_MinValue;
                newitem.OrderIndex = item.OrderIndex;
                newitem.ProjectID = item.ProjectID.ToString();
                newitem.QuestionCode = item.QuestionCode;
                newitem.QuestionID = Guid.NewGuid().ToString();// item.newitemID.ToString();
                newitem.QuestionNameHTMLText = item.QuestionNameHTMLText;
                newitem.QuestionTypeID = item.QuestionTypeID;
                newitem.SingleChoice_AnswerFormat = item.SingleChoice_AnswerFormat;
                newitem.SingleChoice_Answer_AnswerList = item.SingleChoice_Answer_AnswerList;
                newitem.SingleChoice_DefaultText = item.SingleChoice_DefaultText;

                result.Add(newitem);

            }
            return result;
        }

        #endregion

        #region [EVENT]
        private async void btnSubmit_Clicked(object sender, System.EventArgs e)
        {
            //check
            btnSubmit.IsEnabled = false;
            btnSubmit.Clicked -= btnSubmit_Clicked;
            try
            {
                //save to db
                var userid = SettingHelper.GetSetting("last_userid", "");

                if (userid == "")
                {
                    UI.ShowError("Vui lòng đăng nhập");
                    btnSubmit.IsEnabled = true;
                    btnSubmit.Clicked += btnSubmit_Clicked;
                    return;
                }

                if (AnswerItem.AccessPageList == "" || AnswerItem.AccessQuestionCodeList == "")
                {
                    UI.ShowError("Không thể submit dữ liệu trống");
                    btnSubmit.IsEnabled = true;
                    btnSubmit.Clicked += btnSubmit_Clicked;
                    return;
                }

                if (AnswerItem.QuestionVersion != ProjectItem.CurrentVersion.ToString())
                {

                    UI.ShowError(String.Format("Bạn không thể submit dữ liệu vì version của mã phiếu này và version mới nhất của BCH dự án là không tương thích.Vui lòng bấm Edit để chuyển đổi mã phiếu này sang version mới nhất và chạy Next đến câu cuối cùng để kiểm tra", 9000));
                    btnSubmit.IsEnabled = true;
                    btnSubmit.Clicked += btnSubmit_Clicked;
                    return;
                }
                
                if(AnswerItem.IsSubmitted==true)
                {
                    if(IsCompleted==false)
                    {
                        IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
                        var resultConfirm = await Dialogs.ConfirmAsync("Bạn đã submit dữ liệu lên máy chủ và BCH chưa kết thúc.Bạn có muốn tiếp tục submit kết quả lên máy chủ?.Dữ liệu của bạn sẽ cập nhật từ lần submit cuối cùng","Submit","Đồng ý","Để sau");
                        if (resultConfirm == true)
                        {

                        }
                        else
                        {
                            btnSubmit.IsEnabled = true;
                            btnSubmit.Clicked += btnSubmit_Clicked;
                            return;
                        }
                    }
                    else
                    {
                        IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
                        var resultConfirm = await Dialogs.ConfirmAsync("Bạn đã submit dữ liệu lên máy chủ.Bạn có muốn tiếp tục submit kết quả lên máy chủ?.Dữ liệu của bạn sẽ cập nhật từ lần submit cuối cùng", "Submit", "Đồng ý", "Để sau");
                        if (resultConfirm == true)
                        {

                        }
                        else
                        {
                            btnSubmit.IsEnabled = true;
                            btnSubmit.Clicked += btnSubmit_Clicked;
                            return;
                        }
                    }
                }
                else //issumit=false
                {
                    if (IsCompleted == false)
                    {
                        IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
                        var resultConfirm = await Dialogs.ConfirmAsync("BCH chưa kết thúc.Bạn có muốn tiếp tục submit kết quả lên máy chủ?.Dữ liệu của bạn sẽ cập nhật từ lần submit cuối cùng","Submit", "Đồng ý", "Để sau");
                        if (resultConfirm == true)
                        {

                        }
                        else
                        {
                            btnSubmit.IsEnabled = true;
                            btnSubmit.Clicked += btnSubmit_Clicked;
                            return;
                        }
                    }
                    else
                    {
                        
                    }
                }
                string result = "";
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Đang gửi kết quả lên máy chủ..."))
                {
                    //await Task.Delay(10000);
                    result = await Submit(userid);
                }
              
                if(result!="Ok")
                {
                    UI.ShowError(result);
                    btnSubmit.IsEnabled = true;
                    btnSubmit.Clicked += btnSubmit_Clicked;
                    return;
                }
                else
                {
                    UI.ShowSuccess("Đã submit thành công");

                    btnSubmit.IsEnabled = true;
                    btnSubmit.Clicked += btnSubmit_Clicked;
                    var handler = Submitted;
                    if (handler != null)
                        handler(this, EventArgs.Empty);

                    return;
                }


            }
            catch(Exception ex)
            {
                btnSubmit.IsEnabled = true;
                btnSubmit.Clicked += btnSubmit_Clicked;
                UI.ShowError(string.Format("Vui lòng liên hệ kĩ thuật vì quá trình submit bị lỗi {0}", ex.Message));
                MyDebugger.WriteLog(string.Format("Vui lòng liên hệ kĩ thuật vì quá trình submit bị lỗi {0}", ex.Message));
            }
            // update submitted and reload list
        }

        private async void btnView_Clicked(object sender, System.EventArgs e)
        {

            try
            {
                btnEdit.IsEnabled = false;
                btnEdit.Clicked -= btnView_Clicked;
                #region CAMERA
                if (ProjectItem.IsCamera == true)
                {
                    try
                    {
                        bool stopCamera = false;
                        while (stopCamera == false)
                        {
                            try
                            {
                                CameraViewModel cameraOps = null;
                                cameraOps = new CameraViewModel();
                                await cameraOps.TakePicture();
                                var itemImage = new RmProfileImageItem();
                                itemImage.ID = Guid.NewGuid().ToString();
                                itemImage.AnswerID = AnswerItem.AnswerID;
                                itemImage.CreatedOn = DateTime.Now.ToSafeString();
                                var ImageSource = cameraOps.ImageSource;

                                RealmHelper.ImageInsert(itemImage);

                                if (ImageSource == null)
                                {
                                }
                                else
                                {
                                    App.dbRealm.Write(() =>
                                    {
                                        itemImage.PathImage = cameraOps.PathImage;


                                        itemImage.ImageBase64 = DependencyService.Get<IConvertExtensions>().ImageToStringBase64(cameraOps.PathImage);
                                    });

                                    try
                                    {
                                        stopCamera = true;


                                    }
                                    catch (Exception ex)
                                    {
                                        await Task.Delay(50);
                                        btnEdit.IsEnabled = true;
                                        btnEdit.Clicked += btnView_Clicked;
                                        MyDebugger.WriteLog(ex.Message);
                                        stopCamera = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                await Task.Delay(50);
                                btnEdit.IsEnabled = true;
                                btnEdit.Clicked += btnView_Clicked;
                                MyDebugger.WriteLog(ex.Message);
                                UI.ShowError("Chụp ảnh thất bại. Vui lòng tắt ứng dụng chạy lại hoặc chụp ảnh ngoài.");
                                stopCamera = true;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(50);
                        btnEdit.IsEnabled = true;
                        btnEdit.Clicked += btnView_Clicked;
                        MyDebugger.WriteLog(ex.Message);
                        UI.ShowError("Chụp ảnh thất bại. Vui lòng tắt ứng dụng chạy lại");
                    }
                }
                #endregion

                if (AnswerItem.QuestionVersion != ProjectItem.CurrentVersion.ToString())
                {

                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    using (UserDialogs.Instance.Loading(string.Format("Vui lòng đợi đang chuyển sang version mới nhất.Vui lòng chạy lại BCH đến câu trả lời cuối trước đó", AnswerItem.QuestionVersion, ProjectItem.CurrentVersion), null, null, true, MaskType.Black))
                    {
                        await ConvertToNewVersionAsync();
                    }
                    watch.Stop();
                    MyDebugger.WriteLog(string.Format("Convert and go AnswerID {0} from version {1} to version {2} take {3} miliseconds", AnswerItem.AnswerID, AnswerItem.QuestionVersion, ProjectItem.CurrentVersion, watch.ElapsedMilliseconds));
                }
                else
                {

                    using (UserDialogs.Instance.Loading("Đang truy cập dữ liệu...", null, null, true, MaskType.Black))
                    {
                        await Task.Delay(100);
                        btnEdit.IsEnabled = true;
                        btnEdit.Clicked += btnView_Clicked;
                        await GoToSurvey(AnswerItem, ProjectItem);
                 
                    }

                }
             

            }
            catch (Exception ex)
            {
                await Task.Delay(50);
                btnEdit.IsEnabled = true;
                btnEdit.Clicked += btnView_Clicked;
                MyDebugger.WriteLog(ex.Message);
                UI.ShowError(ex.Message);
            }
        }

        #endregion
       
    }
}
