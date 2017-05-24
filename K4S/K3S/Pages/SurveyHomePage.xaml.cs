using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Acr.UserDialogs;

using Extensions;
using K3S.Model;
using MyDependencyServices;
using Xamarin.Forms;

using System.Linq;
using DataAccess;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Geolocator;
using System.IO;
using Shared.Extensions;

namespace K3S
{
    public partial class SurveyHomePage : ContentPage
    {         
        #region [VARIABLES]
        RmProjectItem ProjectItem;
        private ClientServices Services;

        #endregion

        #region [EVENTS]
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        private async void btnNew_Clicked(object sender, EventArgs e)
        {
            btnNew.IsEnabled = false;
            btnNew.Clicked -= btnNew_Clicked;
            await CreateNewSurvey();
            await Task.Delay(50);
            btnNew.IsEnabled = false;
            btnNew.Clicked += btnNew_Clicked;

        }

        #endregion

        #region [CONSTRUCTOR]       
        public SurveyHomePage(RmProjectItem _projectItem) 
        {
        
            InitializeComponent();
            ProjectItem = _projectItem;
          
            //btnNew.Command = new Command(async (object obj) => await CreateNewSurvey());
            btnNew.Text = string.Format("Chạm vào đây để tạo mới cuộc phỏng vấn dự án {0} với version BCH {1}", ProjectItem.ProjectNo, ProjectItem.CurrentVersion);
        
         
        }


        #endregion



        #region [METHOD]
        public async Task Process()
        {
            await LoadAnswerList();
        }
        async Task LoadAnswerList()
        {
            stackAnswer.Children.Clear();
            var answerList = RealmHelper.AnswerGetByProjectID(ProjectItem.ProjectID);// await App.dbAnswer.GetItemsAsync(ProjectItem.ProjectID);
            //var questionList = RealmHelper.QuestionGetByProjectID(ProjectItem.ProjectID);//  await App.dbGenericFormQuestionAndroid.GetItemsAsync(ProjectItem.ProjectID);
           
            foreach (var answer in answerList.OrderByDescending(ob => ob.CreatedOn))
            {
                var answercontrol = new AnswerView(answer,ProjectItem);
                answercontrol.InitializeServices(Services);
                 answercontrol.Process();
                answercontrol.Submitted += Answercontrol_SubmittedAsync;               
                stackAnswer.Children.Add(answercontrol);
            }
        }
        private async void  Answercontrol_SubmittedAsync(object sender, EventArgs e)
        {
            await LoadAnswerList();
        }
        internal void InitializeServices(ClientServices services)
        {
            this.Services = services;
        }
        async Task CreateNewSurvey()
        {
            var dg = Acr.UserDialogs.UserDialogs.Instance;
            try
            {              
                dg.ShowLoading("Vui lòng đợi trong giây lát...");
                await Task.Delay(100);
                #region [CREATE NEW ANSWER ITEM]
                var item = new RmAnswerItem();
                string ID = Guid.NewGuid().ToString();
                item.AnswerID = ID;
                item.ProjectID = ProjectItem.ProjectID;
                item.GreenID = "";
                item.EmailAddress = "";
                item.RespondentFullName = "";
                item.RespondentYoB = 0;
                item.RespondentGender = "";
                item.RespondentCity = ProjectItem.CityHandle;
                item.RespondentDistrict = "";
                item.RespondentWard = "";
                item.RespondentStreet = "";
                item.RespondentAddressLandmark = "";
                item.RespondentTelephone = "";
                item.CreatedOn = DateTime.Now.ToSafeString();
                item.CompletedOn = DateTime.Now.ToSafeString();
                item.IsSubmitted = false;
              
                item.CreatedOn = DateTime.Now.ToSafeString();
                item.QuestionVersion = ProjectItem.CurrentVersion.ToString();
                #endregion

                //ProjectItem.IsGPS = false;
                //ProjectItem.IsCamera = true;
                //ProjectItem.IsRecord = true;
      
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
                                itemImage.AnswerID = item.AnswerID;
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
                                        //IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
                                        //var resultConfirm = await Dialogs.ConfirmAsync("Bạn có muốn tiếp tục chụp hình không", "Chụp hình", "Có", "Không");
                                        //if (resultConfirm == true)
                                        //{
                                        //    stopCamera = false;
                                        //}
                                        //else
                                        //{
                                        //    stopCamera = true;
                                        //}

                                    }
                                    catch (Exception ex)
                                    {
                                        MyDebugger.WriteLog(ex.Message);
                                        stopCamera = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MyDebugger.WriteLog(ex.Message);
                                UI.ShowError("Chụp ảnh thất bại. Vui lòng tắt ứng dụng chạy lại hoặc chụp ảnh ngoài.");
                                stopCamera = true;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MyDebugger.WriteLog(ex.Message);
                        UI.ShowError("Chụp ảnh thất bại. Vui lòng tắt ứng dụng chạy lại");
                    }
                }
                #endregion

                var structQuestionnaire = await CreateAnswerQuestionnaireStructureTableAsync(ID);
                var finalQuestionList = new List<RmGenericFormQuestionAndroidAnswerItem>();
                foreach (var itemDic in structQuestionnaire)
                {
                    item.AnswerQuestionnaireStructureTable = itemDic.Key;
                    finalQuestionList = itemDic.Value;
                }

                RealmHelper.AnswerInsert(item);
                //await App.dbAnswer.InsertItemAsync(item);

                await GoToSurvey(item, finalQuestionList);

                dg.HideLoading();

            }
            catch(Exception ex)
            {
                dg.HideLoading();
                MyDebugger.WriteLog("Cannot create new survey cause of " + ex.Message);

                UI.ShowError("Không thể tạo survey mới.Vui lòng liên hệ kĩ thuật kèm theo lỗi " + ex.Message);
            }
        }
        async Task GoToSurvey(RmAnswerItem item, List<RmGenericFormQuestionAndroidAnswerItem> _listfinalQuestionList)
        {
            var page = new RespondentInformationDetail(item, ProjectItem, true, _listfinalQuestionList);
            await page.Process();
            Application.Current.MainPage = page;
        }
        private async Task<Dictionary<string,List<RmGenericFormQuestionAndroidAnswerItem>>> CreateAnswerQuestionnaireStructureTableAsync(string AnswerID)
        {

            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                var _listOriginalQuestion = new List<RmGenericFormQuestionAndroidItem>();
                //var QuestionList = RealmHelper.QuestionGetByProjectID(ProjectItem.ProjectID);// await App.dbGenericFormQuestionAndroid.GetItemsAsync(ProjectItem.ProjectID);
                foreach (var item in App._listOriginalQuestion)
                {
                    var question = new RmGenericFormQuestionAndroidItem();
                    question.GroupRandom = item.GroupRandom;
                    question.HideCondition = item.HideCondition;
                    question.MultiChoice_Answer_AnswerList = item.MultiChoice_Answer_AnswerList;
                    question.MultiChoice_MaxChecks = item.MultiChoice_MaxChecks;
                    question.MultiChoice_MinChecks = item.MultiChoice_MinChecks;
                    question.MultiChoice_TypeOfNumberChecks = item.MultiChoice_TypeOfNumberChecks;
                    question.Number_AllowDecimalAnswer = item.Number_AllowDecimalAnswer;
                    question.Number_MaxValue = item.Number_MaxValue;
                    question.Number_MinValue = item.Number_MinValue;
                    question.OrderIndex = item.OrderIndex;
                    question.ProjectID = item.ProjectID.ToString();
                    question.QuestionCode = item.QuestionCode;
                    question.QuestionID = Guid.NewGuid().ToString();// item.QuestionID.ToString();
                    question.QuestionNameHTMLText = item.QuestionNameHTMLText;
                    question.QuestionTypeID = item.QuestionTypeID;
                    question.SingleChoice_AnswerFormat = item.SingleChoice_AnswerFormat;
                    question.SingleChoice_Answer_AnswerList = item.SingleChoice_Answer_AnswerList;
                    question.SingleChoice_DefaultText = item.SingleChoice_DefaultText;
                    _listOriginalQuestion.Add(question);
                }

                
                foreach (var item in _listOriginalQuestion)
                {
                    if (item.GroupRandom > 0)
                    {
                        dic.Add(item.QuestionCode, item.OrderIndex + ";" + item.GroupRandom);
                    }
                }

                foreach (var item in _listOriginalQuestion)
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
                            int replaceIndex = _listOriginalQuestion.FindIndex(i => i.OrderIndex == _orderIndex);// tblQuestion._FindIndex("OrderIndex", result[0].Split(';')[0]);

                            string replaceOrderIndex = _listOriginalQuestion[replaceIndex].OrderIndex.ToString();

                            string replaceCode = _listOriginalQuestion[replaceIndex].QuestionCode;
                            string oldOrderIndex = item.OrderIndex.ToString();
                            _listOriginalQuestion[replaceIndex].OrderIndex = item.OrderIndex;
                            item.OrderIndex = Convert.ToInt32(result[0].Split(';')[0]);
                            //tim order index cu update bang order index cu cua no
                            dic[replaceCode] = oldOrderIndex + ";" + item.GroupRandom;
                            dic.Remove(item.QuestionCode);
                        }
                    }
                    else
                    {

                    }
                   
                }
                _listOriginalQuestion = _listOriginalQuestion.OrderBy(i => i.OrderIndex).ToList();

                RealmHelper.QuestionAnswerInsertAll(AnswerID, _listOriginalQuestion);
                var finalQuestion = ReOrderIndex(_listOriginalQuestion);
                var convert_QuestionCode = ConvertShortOrderQuestionCode(finalQuestion);

                Dictionary<string, List<RmGenericFormQuestionAndroidAnswerItem>> returnObject = new Dictionary<string, List<RmGenericFormQuestionAndroidAnswerItem>>();
                returnObject.Add(convert_QuestionCode, finalQuestion);
                return returnObject;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        string ConvertShortOrderQuestionCode(List<RmGenericFormQuestionAndroidAnswerItem> _list)
        {
            string result = "";
            foreach (var item in _list)
            {
                result += item.QuestionCode+ "|";
            }
            return result;
        }
        private List<RmGenericFormQuestionAndroidAnswerItem> ReOrderIndex( List<RmGenericFormQuestionAndroidItem> _questionList)
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

     
    }
}
