using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using Extensions;
using System.Data;
using MyDependencyServices;
using System.Linq;
using System.Diagnostics;
using Shared.Extensions;
using XLabs.Platform.Services.Media;
using Newtonsoft.Json;
using K3S.GenControls;
using Acr.UserDialogs;
using System.ComponentModel;
using Realms;

namespace K3S
{
    public partial class InterviewPage : ContentPage
    {
        #region VARIABLE
        RmAnswerItem AnswerItem;
        RmProjectItem ProjectItem;
        private List<RmGenericFormQuestionAndroidAnswerItem> listQuestionList;
        private List<RmGenericFormQuestionAndroidAnswerItem> listQuestionListNonRealm=new List<RmGenericFormQuestionAndroidAnswerItem>();
        private List<RmGenericFormQuestionAndroidAnswerItem> listPage;
        private List<RmLogicCheckItem> listLogicCheckNonRealm = new List<RmLogicCheckItem>();
        private List<RmFilterConditionItem> listFilterCondition;
        private List<RmGenericFormValueItem> listFinalResultValue;
        private string _currPageCode;
        private RmInterviewTimeItem interviewTimeItem;
        private List<string> listAccessPageCode;
        private DataTable tblQuestion;
        private DataTable tblResultEmpty;
        private DataTable tblLogicCheck;
        private DataTable tblPage;
        private object CurrentVersion;
        private string AnswerID;
      
        public double PercentCompleted
        {
            get;
            set;
        }

        #endregion

        #region CONSTRUCTOR

        public InterviewPage(RmProjectItem projectItem, RmAnswerItem item, List<RmGenericFormQuestionAndroidAnswerItem> finalQuestionList)
        {
            this.ProjectItem = projectItem;
            this.AnswerItem = item;
            this.listQuestionList = finalQuestionList;

            interviewTimeItem = new RmInterviewTimeItem
            {
                ID = Guid.NewGuid().ToString(),
                AnswerID = item.AnswerID,
                ProjectID = ProjectItem.ProjectID,
                StartOn = DateTime.Now.ToSafeString(),
                CompletedOn = DateTime.Now.ToSafeString()
            };

            RealmHelper.TimeInsert(interviewTimeItem);
            InitializeComponent();
           
        }

        #endregion

        #region METHOD

        private void SetProcessBar()
        {
            prbCompleted.Progress = CalPercentCompleted();
        }
        private double CalPercentCompleted()
        {
            try
            {
                if (listAccessPageCode == null || _currPageCode == null)
                    return 0;

                if (listPage == null || listPage.Count == 0)
                    return 0;

                int total = listPage.Count;


                for (int i = 0; i < total; i++)
                {
                    if (listPage[i].QuestionCode == _currPageCode)
                        return (i * 1.0) / total;
                }
                return 0;
            }
            catch
            {
                return 0;
            }

        }
        internal async Task Process()
        {
            var _dg = Acr.UserDialogs.UserDialogs.Instance;
            // tao BCH
            try
            {

                this.Padding = new Thickness(10, 20, 0, 10);
                prbCompleted.SetBinding(ProgressBar.ProgressProperty, "PercentCompleted");
                prbCompleted.BindingContext = this;
                SetProcessBar();
                CurrentVersion = ProjectItem.CurrentVersion;
                AnswerID = AnswerItem.AnswerID;
            
                _dg.ShowLoading("Đang tải bảng câu hỏi...");
               
                //create control
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
             
                var listLogicCheckRealm = RealmHelper.LogicGetByProjectID(ProjectItem.ProjectID); // await App.dbLogicCheck.GetItemsAsync(ProjectItem.ProjectID);                                
                listFilterCondition = RealmHelper.FilterGetByProjectID(ProjectItem.ProjectID);// await App.dbFilterCondition.GetItemsAsync(ProjectItem.ProjectID);
                listFinalResultValue = RealmHelper.ResultValueGetByAnswer(AnswerItem.AnswerID);
                stopwatch.Stop();
                MyDebugger.WriteLog(string.Format("Load FilterCondition success!!! {0}", stopwatch.ElapsedMilliseconds));
                CopyToNonRealm(listLogicCheckRealm);
                stopwatch.Restart();
             // await App.dbGenericFormValue.GetItemsAsync(AnswerItem.AnswerID);
                stopwatch.Stop();
                MyDebugger.WriteLog(string.Format("Load GenericFormValue success!!! {0}", stopwatch.ElapsedMilliseconds));
                bool _IsNew = false;
                stopwatch.Restart();
                try
                {
                    listPage = listQuestionListNonRealm.Where(a => a.QuestionTypeID == 0).ToList();
                    tblPage = DependencyService.Get<IConvertExtensions>().ToTable<RmGenericFormQuestionAndroidAnswerItem>(listPage);
                    tblLogicCheck = DependencyService.Get<IConvertExtensions>().ToTable<RmLogicCheckItem>(listLogicCheckNonRealm);
                    tblQuestion = DependencyService.Get<IConvertExtensions>().ToTable<RmGenericFormQuestionAndroidAnswerItem>(listQuestionListNonRealm);

                }
                catch (Exception)
                {

                }
                if (listFinalResultValue.Count == 0)
                {
                    _IsNew = true;
                    listFinalResultValue = await CreateAnswerAsync(listQuestionList);
                    RealmHelper.ResultValueInsertAll(listFinalResultValue,AnswerID, ProjectItem.ProjectID);
                    //await SaveAndLoadNewData(listFinalResultValue, _IsNew);
                 
                    listFinalResultValue = RealmHelper.realm.All<RmGenericFormValueItem>().Where(o => o.AnswerID == AnswerID).ToList();

                }
                stopwatch.Stop();
                MyDebugger.WriteLog(string.Format("Load CreateAnswerAsync success!!! {0}", stopwatch.ElapsedMilliseconds));

                stopwatch.Restart();
               
                ProcessListAccessPageCode();
                stopwatch.Stop();
                MyDebugger.WriteLog(string.Format("Process list Access Page success!!! {0}", stopwatch.ElapsedMilliseconds));
                stopwatch.Restart();
                tblResultEmpty = DependencyService.Get<ILogicCheck>().CreatePivotTableEmptyData(listFinalResultValue);
                stopwatch.Stop();
               
                GoToPage(_currPageCode);
                _dg.HideLoading();
            }
            catch (Exception ex)
            {
                _dg.HideLoading();
                MyDebugger.WriteLog("Mess=" + ex.Message + " Source=" + ex.Source + " Trace=" + ex.StackTrace);
            }
        }
        private void CopyToNonRealm(List<RmLogicCheckItem> listLogicRealm)
        {
            listQuestionListNonRealm = new List<RmGenericFormQuestionAndroidAnswerItem>();
            foreach (var item in listQuestionList)
            {
                var question = new RmGenericFormQuestionAndroidAnswerItem();
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
                listQuestionListNonRealm.Add(question);
            }
            foreach(var item in listLogicRealm)
            {
                var nonRealmItem = new RmLogicCheckItem();
                nonRealmItem.Comments = item.Comments;
                nonRealmItem.FromPage = item.FromPage;
                nonRealmItem.ProjectID = item.ProjectID;
                nonRealmItem.SkipLogic = item.SkipLogic;
                nonRealmItem.SkipToPage = item.SkipToPage;
                listLogicCheckNonRealm.Add(nonRealmItem);
            }
         
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }   
        private void GoToPage(string PageCode)
        {
            MyDebugger.WriteLog("Start Go To Page " + PageCode);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //try
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        ClearOldControl();
            //    });
               
            //}
            //catch(Exception ex)
            //{

            //}
            watch.Stop();
            MyDebugger.WriteLog(string.Format("Clear old control in {0} miliseconds", watch.ElapsedMilliseconds));
            watch.Restart();


            try
            {
                int _nextPage = 0;

                //var _currentPageItemList = tblPage.Select(string.Format("QuestionCode='{0}'", PageCode));//  listPage.Where(obj => obj.QuestionCode == PageCode).ToList();
                //if (_currentPageItemList.Count == 0)
                //{
                //    MyDebugger.WriteLog(string.Format("Cannot find {0} ", PageCode));
                //    UI.ShowError(string.Format("Cannot find {0} ", PageCode));
                //    return;
                //}
                int _currentPage = 0;
                foreach (DataRow r in tblPage.Rows)
                {
                    if(r.Item("QuestionCode")==PageCode)
                    {
                        _currentPage =(int) r["OrderIndex"];
                        break;
                    }
                }
                //int _currentPage = _currentPageItemList[0].OrderIndex;

                if (_currentPage ==(int)tblPage.Rows[tblPage.Rows.Count-1]["OrderIndex"])// listPage[listPage.Count - 1].OrderIndex)
                {
                    _nextPage = 99999;
                }
                else
                {
                    //vi tri row hien tai
                    int _findIndex = -2;
                    for (int i = 0; i < tblPage.Rows.Count; i++)
                    {
                        if ((int)tblPage.Rows[i]["OrderIndex"] == _currentPage)
                        {
                            _findIndex = i;
                            break;
                        }
                    }
                    if (_findIndex == -2)
                    {
                        MyDebugger.WriteLog(string.Format("Cannot find {0} in listPage", _currentPage));
                        UI.ShowError(string.Format("Cannot find {0} in listPage", _currentPage));
                    }
                    _findIndex = _findIndex + 1;

                    _nextPage =(int) tblPage.Rows[_findIndex]["OrderIndex"];// listPage[_findIndex].OrderIndex;

                }
                watch.Stop();
                MyDebugger.WriteLog(string.Format("From Clear old control to CreateAPage in {0} miliseconds", watch.ElapsedMilliseconds));
                watch.Restart();
                CreateAPage(_currentPage, _nextPage, PageCode);

                watch.Stop();
                MyDebugger.WriteLog(string.Format("CreateAPage in {0} miliseconds", watch.ElapsedMilliseconds));

            }
            catch (Exception ex)
            {
                MyDebugger.WriteLog(ex.Message);
                UI.ShowError(ex.Message);

            }

            watch.Stop();
            MyDebugger.WriteLog(string.Format("End Go To Page {0} in {1} milisenconds", PageCode, watch.ElapsedMilliseconds));

            SetProcessBar();


        }
        private void CreateAPage(int _pageIndex, int _nextPageIndex, string _pageCode)
        {
            try
            {
             
                Stopwatch watch = new Stopwatch();
                watch.Start();
                //var _tableAPage = tblQuestion.Select(string.Format("OrderIndex>'{0}' AND OrderIndex<'{1}'", _pageIndex, _nextPageIndex));
                var _tableAPage =  listQuestionListNonRealm.Where(obj => (obj.OrderIndex > _pageIndex) && (obj.OrderIndex < _nextPageIndex)).ToList();
               

                if (_tableAPage.Count == 0)
                {
                    MyDebugger.WriteLog(string.Format("Cannot find any question in page {0}", _pageCode));
                    UI.ShowError(string.Format("Cannot find any question in page {0}", _pageCode));
                }

                if (_tableAPage.Count() > 0)
                {
                    watch.Stop();
                    MyDebugger.WriteLog(string.Format("CreateAPage get _tableAPage in {0} miliseconds", watch.ElapsedMilliseconds));
                    watch.Restart();
                    CreateControlsAPage(_tableAPage);
                    watch.Stop();
                    MyDebugger.WriteLog(string.Format("CreateAPage CreateControlsAPage in {0} miliseconds", watch.ElapsedMilliseconds));

                }
            }
            catch (Exception ex)
            {
                MyDebugger.WriteLog(ex.Message);
                UI.ShowError(ex.Message);
            }
        }
        private void CreateControlsAPage(List<RmGenericFormQuestionAndroidAnswerItem> _tableAPage)
        {
            try
            {
                GenPageControls page = new GenPageControls();
                page.listPage = _tableAPage;
                page.listFilterCondition = listFilterCondition;
                page.AnswerID = this.AnswerItem.AnswerID.ToString();
                try
                {
                    page.FontSizeText = this.ProjectItem.FontSize;
                }
                catch
                {
                    page.FontSizeText = 13;
                }
                //var data = tblFinalResultValue.Copy();

                Stopwatch w1 = new Stopwatch();
                //w1.Start();
                //var data = JsonConvert.DeserializeObject<List<RmGenericFormValueItem>>(JsonConvert.SerializeObject(listFinalResultValue));// listFinalResultValue.Clone<GenericFormValueItem>();
                //w1.Stop();

                //MyDebugger.WriteLog("1: " + w1.ElapsedMilliseconds);
                w1.Start();
                //w1.Restart();
                //var data = new List<RmGenericFormValueItem>();
                //foreach (var item in listFinalResultValue)
                //{
                //    data.Add(new RmGenericFormValueItem
                //    {
                //        AnswerID = item.AnswerID,
                //        AnswerText = item.AnswerText,
                //        FieldName = item.FieldName,
                //        FieldType = item.FieldType,
                //        FieldValue = item.FieldValue,
                //        PageCode = item.PageCode,
                //        ProjectID = item.ProjectID,
                //        QuestionType = item.QuestionType
                //    });
                //}
                //w1.Stop();
                //MyDebugger.WriteLog("2: " + w1.ElapsedMilliseconds);
                //w1.Restart();
                for (int i = listAccessPageCode.Count - 1; i >= 0; i--)
                {
                    var c_row = listFinalResultValue[i];
                    if (listAccessPageCode.Contains(c_row.PageCode) == false && _currPageCode != c_row.PageCode)
                    {
                        RealmHelper.realm.Write(() => {

                            c_row.FieldValue = "";
                            c_row.AnswerText = "";

                        });
                    }
                }
                w1.Stop();
                MyDebugger.WriteLog("3: " + w1.ElapsedMilliseconds);
                w1.Restart();
                page.listFinalResultValue = listFinalResultValue;
                page.listAccessPageCode = listAccessPageCode;
                page.tblResultEmpty = tblResultEmpty;
                page.Process();
                page.UserSelected += Page_UserSelected;

                stackQuestionControls.Children.Add(page);
                w1.Stop();
                MyDebugger.WriteLog("4: " + w1.ElapsedMilliseconds);
                w1.Restart();
            }
            catch (Exception ex)
            {
                MyDebugger.WriteLog(ex.Message);
                UI.ShowError(ex.Message);
            }
        }       
        private void ProcessListAccessPageCode()
        {
            listAccessPageCode = new List<string>();

            string s_ListAccessQuestionCode = AnswerItem.AccessPageList;

            if (s_ListAccessQuestionCode == null || s_ListAccessQuestionCode.IsEmpty())
            {
                _currPageCode = listPage[0].QuestionCode;
                listAccessPageCode.Add(_currPageCode);
            }
            else
            {
                var oldListAccessPageCode = (List<String>)Newtonsoft.Json.JsonConvert.DeserializeObject(s_ListAccessQuestionCode, typeof(List<String>));
                foreach (string pageIndex in oldListAccessPageCode)
                {
                    if (pageIndex.IsNotEmpty())
                    {
                        listAccessPageCode.Add(pageIndex);
                    }
                }
                if (listAccessPageCode.Count == 0)
                {
                    _currPageCode = listPage[0].QuestionCode;
                }
                else
                {
                    if (listAccessPageCode.Count >= 2)
                    {
                        _currPageCode = listAccessPageCode[listAccessPageCode.Count - 1];
                        listAccessPageCode.RemoveAt(listAccessPageCode.Count - 1);
                    }
                    else
                    {
                        _currPageCode = listAccessPageCode[listAccessPageCode.Count - 1];
                    }
                }
            }
        }
        private void ClearOldControl()
        {
            stackQuestionControls.Children.Clear();
        }
        private async Task SaveAnswerValue(List<RmGenericFormValueItem> listFinalResultValue)
        {


            RealmHelper.ResultValueAdd(listFinalResultValue);
            //await App.dbGenericFormValue.DeleteItemsAsync(AnswerItem.ProjectID);

            //foreach (var item in listFinalResultValue)
            //{
            //    App.dbGenericFormValue.InsertItemAsync(item);
            //}
        }
        private async Task<List<RmGenericFormValueItem>> CreateAnswerAsync(List<RmGenericFormQuestionAndroidAnswerItem> listQuestionList)
        {
            try
            {
                var result = new List<RmGenericFormValueItem>();
                foreach (var question in listQuestionList)
                {
                    int QuestionTypeID = question.QuestionTypeID;
                    string PageID = FindPageOrderIndex(question.OrderIndex);
                    string PageCode = FindPageCode(question.OrderIndex, listQuestionList);

                    switch (QuestionTypeID)
                    {
                        case 1:
                            {
                                #region SA
                                {
                                    var new_row = new RmGenericFormValueItem();
                                    new_row.ID = Guid.NewGuid().ToString();
                                    new_row.AnswerID = AnswerItem.AnswerID.ToString();
                                    new_row.QuestionID = question.QuestionID.ToString();
                                    new_row.FieldName = question.QuestionCode;
                                    new_row.QuestionCode = question.QuestionCode;
                                    new_row.QuestionType = "SA";
                                    new_row.PageCode = PageCode.ToString();
                                    new_row.FieldValue = "";
                                    new_row.FieldType = "double";
                                    result.Add(new_row);

                                    try
                                    {
                                        var answer_list = JsonConvert.DeserializeObject<List<SingleAnswerChoice>>(question.SingleChoice_Answer_AnswerList);
                                        var code = question.QuestionCode;
                                        foreach (var r in answer_list)
                                        {

                                            if (r.SingleChoice_View_Answer_AnswerCodes_OtherSpecify == true)
                                            {
                                                var c_new_row = new RmGenericFormValueItem();
                                                c_new_row.ID = Guid.NewGuid().ToString();
                                                c_new_row.AnswerID = AnswerItem.AnswerID;
                                                c_new_row.QuestionID = question.QuestionID.ToString();
                                                c_new_row.QuestionCode = question.QuestionCode;
                                                c_new_row.QuestionType = "SA";
                                                c_new_row.PageCode = PageCode.ToString();
                                                c_new_row.FieldName = r.SingleChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName;
                                                c_new_row.FieldValue = "";
                                                if (r.SingleChoice_View_Answer_AnswerCodes_OtherSpecify_InType == "Integer")
                                                {
                                                    c_new_row.FieldType = "double";
                                                }
                                                else
                                                {
                                                    if (r.SingleChoice_View_Answer_AnswerCodes_OtherSpecify_InType == "Normal TextBox")
                                                    {
                                                        c_new_row.FieldType = "string";
                                                    }
                                                }
                                                result.Add(c_new_row);
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MyDebugger.WriteLog(string.Format("Error at {0} : {1}", question.QuestionCode, ex.Message));
                                        UI.ShowError(question.QuestionCode + " " + ex.Message);
                                    }

                                    break;
                                }
                                #endregion  
                            }
                        case 2:
                            {
                                #region MA
                                {
                                    try
                                    {
                                        var s_answerlist = question.MultiChoice_Answer_AnswerList;
                                        if (s_answerlist == null || s_answerlist.IsEmpty())
                                        {
                                            var code = question.QuestionCode;
                                            UI.ShowError("Bị lỗi tại " + question.QuestionCode);
                                            return null;
                                        }
                                        var answer_list = JsonConvert.DeserializeObject<List<MultiAnswerChoice>>(question.MultiChoice_Answer_AnswerList);
                                        //var answer_list = JsonConvert.DeserializeObject<List<MultiAnswerChoice>>(question.MultiChoice_Answer_AnswerList);// question.MultiChoice_Answer_AnswerList.JsonStringToDataTable();

                                        foreach (var r in answer_list)
                                        {
                                            var new_row = new RmGenericFormValueItem();
                                            new_row.ID = Guid.NewGuid().ToString();
                                            new_row.AnswerID = AnswerItem.AnswerID;
                                            new_row.QuestionID = question.QuestionID.ToString();
                                            new_row.QuestionCode = question.QuestionCode;
                                            new_row.QuestionType = "MA";
                                            new_row.PageCode = PageCode.ToString();
                                            new_row.FieldName = r.MultiChoice_View_Answer_AnswerCodes_VariableName;
                                            new_row.FieldValue = "";
                                            new_row.FieldType = "double";
                                            result.Add(new_row);

                                            if (r.MultiChoice_View_Answer_AnswerCodes_OtherSpecify == true)
                                            {
                                                var c_new_row = new RmGenericFormValueItem();
                                                c_new_row.ID = Guid.NewGuid().ToString();
                                                c_new_row.AnswerID = AnswerItem.AnswerID;
                                                c_new_row.QuestionID = question.QuestionID.ToString();
                                                c_new_row.QuestionCode = question.QuestionCode;
                                                c_new_row.QuestionType = "MA";
                                                c_new_row.PageCode = PageCode.ToString();
                                                c_new_row.FieldName = r.MultiChoice_View_Answer_AnswerCodes_VariableName + "_K";
                                                c_new_row.FieldValue = "";

                                                if (r.MultiChoice_View_Answer_AnswerCodes_OtherSpecify_InType == "Integer")
                                                {
                                                    c_new_row.FieldType = "double";
                                                }
                                                else
                                                {
                                                    if (r.MultiChoice_View_Answer_AnswerCodes_OtherSpecify_InType == "Normal TextBox")
                                                    {
                                                        c_new_row.FieldType = "string";
                                                    }
                                                }
                                                result.Add(c_new_row);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MyDebugger.WriteLog(string.Format("Error at {0} : {1}", question.QuestionCode, ex.Message));
                                        UI.ShowError("Bị lỗi tại " + question.QuestionCode);
                                    }
                                    break;
                                }
                                #endregion
                            }
                        case 3:
                            {
                                #region OE
                                var new_row = new RmGenericFormValueItem();
                                new_row.ID = Guid.NewGuid().ToString();
                                new_row.AnswerID = AnswerItem.AnswerID;
                                new_row.QuestionID = question.QuestionID.ToString();
                                new_row.QuestionCode = question.QuestionCode;
                                new_row.QuestionType = "OE";
                                new_row.FieldName = question.QuestionCode;
                                new_row.FieldValue = "";
                                new_row.FieldType = "string";
                                new_row.PageCode = PageCode.ToString();
                                result.Add(new_row);
                                break;
                                #endregion
                            }
                        case 7:
                            {
                                #region Number
                                var new_row = new RmGenericFormValueItem();
                                new_row.ID = Guid.NewGuid().ToString();
                                new_row.AnswerID = AnswerItem.AnswerID;
                                new_row.QuestionID = question.QuestionID.ToString();
                                new_row.FieldName = question.QuestionCode;
                                new_row.QuestionCode = question.QuestionCode;
                                new_row.QuestionType = "Number";
                                new_row.FieldValue = "";
                                new_row.PageCode = PageCode.ToString();
                                new_row.FieldType = "double";
                                result.Add(new_row);
                                break;
                                #endregion                              
                            }

                        default:
                            break;
                    }
                }
                return result;
            }

            catch (Exception ex)
            {
                MyDebugger.WriteLog(string.Format("Error {0}", ex.Message));
                UI.ShowError(ex.Message);
                return null;
            }
        }

      
        private string FindPageCode(int p, List<RmGenericFormQuestionAndroidAnswerItem> listQuestionList)
        {

            try
            {
                for (int i = 0; i < listPage.Count; i++)
                {
                    var row = listPage[i];
                    if (p > row.OrderIndex)
                    {
                        if (i <= listPage.Count - 2)
                        {
                            if (p < ConvertSafe.ToInt32(listPage[i + 1].OrderIndex))
                            {
                                return row.QuestionCode;
                            }
                        }
                        else
                        {
                            return listPage[i].QuestionCode;
                        }
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                MyDebugger.WriteLog(string.Format("Error when find page code {1} {0}", ex.Message, p));
                UI.ShowError(ex.Message);
                return "";
            }
        }
        private string FindPageOrderIndex(int p)
        {
            try
            {
                for (int i = 0; i < listPage.Count; i++)
                {
                    var itemPage = listPage[i];
                    if (p > ConvertSafe.ToInt32(itemPage.OrderIndex))
                    {
                        if (i <= listPage.Count() - 2)
                        {
                            if (p < ConvertSafe.ToInt32(listPage[i + 1].OrderIndex))
                            {
                                return itemPage.OrderIndex.ToString();
                            }
                        }
                        else
                        {
                            return listPage[i].OrderIndex.ToString();
                        }
                    }


                }
                return "";

            }
            catch (Exception ex)
            {
                UI.ShowError(ex.Message);
                return "";
            }
        }
        private bool ValidateDataInput(List<RmGenericFormValueItem> result_data)
        {
            foreach (var item in result_data)
            {
                if (item.FieldName.Contains("~") || item.AnswerText.Contains("~"))
                {
                    return false;
                }
            }
            return true;
        }
        private Dictionary<String, String> checkLogicCheckPostSkip ( List<RmGenericFormValueItem> _listFinalResultValue, List<string> _listAccessPageCode)
        {
            return DependencyService.Get<ILogicCheck>().checkLogicCheckPostSkip2(listLogicCheckNonRealm, tblQuestion, tblPage, _listFinalResultValue, _currPageCode, _listAccessPageCode, tblResultEmpty);                
        }

        #endregion

        #region EVENT
        private async void Page_UserSelected(object sender, EventArgs e)
        {
            btnNext_Clicked(null, null);                   
        }
        private async void btnBack_Clicked(object sender, EventArgs args)
        {
            try
            {

                btnBack.IsEnabled = false;
                btnBack.Clicked -= btnBack_Clicked;
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Đang quay lại"))
                {
                    await Task.Delay(50);
                    await BackAsync();
                }
                Task.WaitAll();
                await Task.Delay(50);
            
                btnBack.IsEnabled = true;
                btnBack.Clicked += btnBack_Clicked;

                //GoToPage(_currPageCode);


            }
            catch (Exception ex)
            {
                btnBack.IsEnabled = true;
                btnBack.Clicked += btnBack_Clicked;
                MyDebugger.WriteLog(string.Format("Error when try to back {0}", ex.Message));
                UI.ShowError(ex.Message);
            }
        }
        private Task<bool> BackAsync()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
         
            return Task.Run(() =>
            {
                if (listAccessPageCode.Count <= 0)
                {

                    tcs.SetResult(false);
                    return tcs.Task;
                }
                _currPageCode = listAccessPageCode[listAccessPageCode.Count - 1];
                listAccessPageCode.RemoveAt(listAccessPageCode.Count - 1);

                if (listAccessPageCode.Count >= 2 && listAccessPageCode[listAccessPageCode.Count - 1] == listAccessPageCode[listAccessPageCode.Count - 2])
                {
                    listAccessPageCode.RemoveAt(listAccessPageCode.Count - 1);
                }
                tcs.SetResult(true);
                Device.BeginInvokeOnMainThread(() =>
                {
                    stackQuestionControls.Children.Clear();
                    GoToPage(_currPageCode);
                });

                //stackQuestionControls.Children.Clear();
                //GoToPage(_currPageCode);
              
                return tcs.Task;
            });

        }
        private Task<string> DoWorkAsync(string answerID)
        {        
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();          
            string oldPageCode = _currPageCode;
            return Task.Run(() =>
            {
                try
                {
                    var con = RealmConfiguration.DefaultConfiguration;
                    con.SchemaVersion = 1;
                    using (Realm realm = Realm.GetInstance(con))
                    {
                        Stopwatch watch = new Stopwatch();
                        watch.Start();

                        #region Get Realm finalResultValue
                        var listFinalResultValue = realm.All<RmGenericFormValueItem>().Where(o => o.AnswerID == answerID).ToList();
                        watch.Stop();
                        MyDebugger.WriteLog(string.Format("DoWorkAsync Realm Get finalResultValue take {0} milisencond", watch.ElapsedMilliseconds));
                        #endregion

                        #region Get Realm Get answer
                        watch.Restart();
                        var answer = realm.All<RmAnswerItem>().Where(o => o.AnswerID == answerID).First();
                        watch.Stop();
                        MyDebugger.WriteLog(string.Format("DoWorkAsync Realm Get answer take {0} milisencond", watch.ElapsedMilliseconds));
                        watch.Restart();
                        #endregion

                        #region Get currentItemAtPageCode
                        var currentItemAtPageCode = listFinalResultValue.Where(o => o.PageCode == _currPageCode).ToList();
                        watch.Stop();
                        #endregion

                        #region Get currentItemAtPageCode
                        MyDebugger.WriteLog(string.Format("DoWorkAsync Get currentItemAtPageCode take {0} milisencond", watch.ElapsedMilliseconds));
                        watch.Restart();
                        #endregion

                        #region  Add currentItemAtPageCode to  resultFinalCurrentQuestionChange

                        var resultFinalCurrentQuestionChange = new List<RmGenericFormValueItem>();

                        foreach (var item in currentItemAtPageCode)
                        {
                            var changeItem = new RmGenericFormValueItem();
                            changeItem.ID = item.ID;
                            changeItem.AnswerID = item.AnswerID;
                            changeItem.AnswerText = item.AnswerText;
                            changeItem.FieldName = item.FieldName;
                            changeItem.FieldType = item.FieldType;
                            changeItem.FieldValue = item.FieldValue;
                            changeItem.PageCode = item.PageCode;
                            changeItem.ProjectID = item.ProjectID;
                            changeItem.QuestionType = item.QuestionType;
                            changeItem.QuestionCode = item.QuestionCode;
                            changeItem.QuestionID = item.QuestionID;

                            resultFinalCurrentQuestionChange.Add(changeItem);
                        }
                        watch.Stop();
                        MyDebugger.WriteLog(string.Format("DoWorkAsync Add currentItemAtPageCode to  resultFinalCurrentQuestionChange take {0} milisencond", watch.ElapsedMilliseconds));
                        #endregion

                        #region VALIDATION CURRENT DATA AT PAGE AND ADD TO RESULT
                        watch.Restart();
                        for (int i = 0; i < stackQuestionControls.Children.Count; i++)
                        {
                            View v = stackQuestionControls.Children[i];
                            if (v.GetType() == typeof(GenPageControls))
                            {
                                foreach (GenControl control in (v as GenPageControls).layout.Children)
                                {
                                    Stopwatch w1 = new Stopwatch();
                                    w1.Start();
                                    //var typeControl = control.GetType();
                                    try
                                    {
                                        if(control.GetType()!= typeof(GenLabelText))
                                        {
                                            var resultCheck = (control as GenControl).CheckValidation();
                                            string ErrorMess = "";
                                            bool checkResult = true;
                                            foreach (var item in resultCheck)
                                            {
                                                ErrorMess = item.Key;
                                                checkResult = item.Value;
                                            }
                                            if (checkResult == true)
                                            {
                                                var result_data = control.GetListResultValue();

                                                if (ValidateDataInput(result_data) == false)
                                                {
                                                    tcs.SetResult("Vui lòng không nhập kí tự ~ trong câu trả lời");
                                                    return tcs.Task;
                                                }
                                                foreach (var row in result_data)
                                                {
                                                    foreach (var item in resultFinalCurrentQuestionChange)
                                                    {
                                                        if (item.FieldName == row.FieldName)
                                                        {
                                                            item.FieldValue = row.FieldValue;
                                                            item.AnswerText = row.AnswerText;
                                                        }
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                tcs.SetResult(ErrorMess);
                                                return tcs.Task;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        tcs.SetResult("Lỗi khi validation data.Liên hệ kĩ thuật.Chi tiết: " + ex.Message);
                                        return tcs.Task;
                                    }

                                }
                                #region COMMENT OLD VERSION
                                //if (typeControl == typeof(GenSingleChoice))
                                //{
                                //    var resultCheck = (control as GenSingleChoice).CheckValidation();
                                //    string ErrorMess = "";
                                //    bool checkResult = true;
                                //    foreach (var item in resultCheck)
                                //    {
                                //        ErrorMess = item.Key;
                                //        checkResult = item.Value;
                                //    }
                                //    if (checkResult == true)
                                //    {                                       
                                //        var result_data = (control as GenSingleChoice).listResultValue;

                                //        if (ValidateDataInput(result_data) == false)
                                //        {
                                //            tcs.SetResult("Vui lòng không nhập kí tự ~ trong câu trả lời");
                                //            return tcs.Task;
                                //        }
                                //        //int currentRowIndex = -1;
                                //        foreach (var row in result_data)
                                //        {
                                //            resultFinalCurrentQuestionChange.Add(row);
                                //            //for (int index = 0; index < listFinalResultValue.Count; index++)
                                //            //{
                                //            //    if (listFinalResultValue[index].FieldName == row.FieldName)
                                //            //    {
                                //            //        currentRowIndex = index;
                                //            //        break;
                                //            //    }
                                //            //}
                                //            //try
                                //            //{
                                //            //    realm.Write(() =>
                                //            //    {
                                //            //        listFinalResultValue[currentRowIndex].FieldValue = row.FieldValue;
                                //            //        listFinalResultValue[currentRowIndex].AnswerText = row.AnswerText;
                                //            //    });

                                //            //}
                                //            //catch (Exception ex)
                                //            //{
                                //            //    tcs.SetResult(ex.Message);
                                //            //    return tcs.Task;

                                //            //}
                                //        }
                                //    }
                                //    else
                                //    {
                                //        tcs.SetResult(ErrorMess);
                                //        return tcs.Task;
                                //    }
                                //}
                                //if (typeControl == typeof(GenNumber))
                                //{
                                //    if ((control as GenNumber).CheckValidation() == true)
                                //    {
                                //        // update vao table
                                //        var result_data = (control as GenNumber).listResultValue;
                                //        if (ValidateDataInput(result_data) == false)
                                //        {
                                //            tcs.SetResult("Vui lòng không nhập kí tự ~ trong câu trả lời");
                                //            return tcs.Task;
                                //        }

                                //        foreach (var row in result_data)
                                //        {
                                //            resultFinalCurrentQuestionChange.Add(row);                                              
                                //        }
                                //    }
                                //    else
                                //    {

                                //    }
                                //}
                                //if (typeControl == typeof(GenFreeText))
                                //{
                                //    var resultCheck = (control as GenFreeText).CheckValidation();
                                //    string ErrorMess = "";
                                //    bool checkResult = true;
                                //    foreach (var item in resultCheck)
                                //    {
                                //        ErrorMess = item.Key;
                                //        checkResult = item.Value;
                                //    }
                                //    if (checkResult == true)
                                //    {
                                //        // update vao table
                                //        var result_data = (control as GenFreeText).listResultValue;

                                //        if (ValidateDataInput(result_data) == false)
                                //        {
                                //            tcs.SetResult("Vui lòng không nhập kí tự ~ trong câu trả lời");
                                //            return tcs.Task;


                                //        }
                                //        foreach (var row in result_data)
                                //        {
                                //            resultFinalCurrentQuestionChange.Add(row);
                                //        }
                                //    }
                                //    else
                                //    {
                                //        tcs.SetResult(ErrorMess);
                                //        return tcs.Task;
                                //    }

                                //}
                                //if (typeControl == typeof(GenMultiChoices))
                                //{
                                //    var resultCheck = (control as GenMultiChoices).CheckValidation();
                                //    string ErrorMess = "";
                                //    bool checkResult = true;
                                //    foreach (var item in resultCheck)
                                //    {
                                //        ErrorMess = item.Key;
                                //        checkResult = item.Value;
                                //    }
                                //    if (checkResult == true)
                                //    {
                                //        // update vao table
                                //        var result_data = (control as GenMultiChoices).listResultValue;
                                //        if (ValidateDataInput(result_data) == false)
                                //        {
                                //            tcs.SetResult("Vui lòng không nhập kí tự ~ trong câu trả lời");
                                //            return tcs.Task;


                                //        }
                                //        foreach (var row in result_data)
                                //        {
                                //            resultFinalCurrentQuestionChange.Add(row);
                                //        }
                                //    }
                                //    else
                                //    {
                                //        tcs.SetResult(ErrorMess);
                                //        return tcs.Task;
                                //    }


                                //}
                                #endregion
                            }
                        }
                        watch.Stop();
                        MyDebugger.WriteLog(string.Format("DoWorkAsync VALIDATION CURRENT DATA AT PAGE AND ADD TO RESULT take {0} milisencond", watch.ElapsedMilliseconds));

                        #endregion

                        #region [SAVE NEW DATA]
                        watch.Restart();
                      
                        var currentAtCurrentPage = listFinalResultValue.Where(o => o.PageCode == _currPageCode).ToList();
                        realm.Write(() =>
                        {
                            foreach (var item in resultFinalCurrentQuestionChange)
                            {
                                var fieldName = item.FieldName;

                                var newValueItem = listFinalResultValue.Where(o => o.FieldName == fieldName).First();

                                if (newValueItem != null)
                                {
                                    newValueItem.FieldValue = item.FieldValue;
                                    newValueItem.AnswerText = item.AnswerText;
                                }

                            }                           
                        });
                        watch.Stop();
                        MyDebugger.WriteLog(string.Format("DoWorkAsync SAVE NEW DATA TO REALM take {0} milisencond", watch.ElapsedMilliseconds));


                        #endregion

                        #region [LOGIC CHECK]
                        watch.Restart();

                        Dictionary<string, string> questionCodeNextPageResult = checkLogicCheckPostSkip(listFinalResultValue, listAccessPageCode);
                        if (questionCodeNextPageResult.Count != 1)
                        {
                            tcs.SetResult("Error logic at " + _currPageCode);
                            return tcs.Task;
                        }
                        string questionCodeNextPage = "";
                        string errorComment = "";
                        foreach (var item in questionCodeNextPageResult)
                        {
                            questionCodeNextPage = item.Key;
                            errorComment = item.Value;
                        }
                        if (questionCodeNextPage == "")
                        {
                            tcs.SetResult(errorComment);
                            return tcs.Task;
                        }
                        watch.Stop();
                        MyDebugger.WriteLog(string.Format("DoWorkAsync TOTAL RUN LOGIC CHECK take {0} milisencond", watch.ElapsedMilliseconds));

                        #endregion

                        #region [ProcessWithAccessPageList]
                        watch.Restart();                   
                        if (_currPageCode != questionCodeNextPage)
                        {
                          

                            var ProcessWithAccessPageListResult=ProcessWithAccessPageList( questionCodeNextPage);
                            watch.Stop();
                            MyDebugger.WriteLog(string.Format("DoWorkAsync TOTAL ProcessWithAccessPageList take {0} milisencond", watch.ElapsedMilliseconds));
                            if (ProcessWithAccessPageListResult[2]!="OK")
                            {
                                tcs.SetResult(ProcessWithAccessPageListResult[2]);
                                return tcs.Task;
                            }
                          
                            realm.Write(() =>
                           {                             
                               answer.AccessPageList = ProcessWithAccessPageListResult[0];
                               answer.AccessQuestionCodeList = ProcessWithAccessPageListResult[1];
                           });
                           
                        }
                        #endregion

                        tcs.SetResult(errorComment);


                    }
                    return tcs.Task;

                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        stackQuestionControls.Children.Clear();
                        GoToPage(oldPageCode);
                    });
                    tcs.SetResult(ex.Message);
                    return tcs.Task;
                }
            });
        }
        private List<String> ProcessWithAccessPageList(string questionCodeNextPage)
        {            
            List<String> result = new List<string>();
            try
            {
                int _currentPage = -1;
                int countQuestion = listQuestionListNonRealm.Count;
                for (int index = 0; index < countQuestion; index++)// tblQuestion.Rows.Count; index++)
                {
                    if (tblQuestion.Rows[index].Item("QuestionCode") == _currPageCode)
                    {
                        _currentPage = index;
                        break;
                    }
                }

                var _currentCode = listQuestionListNonRealm[_currentPage + 1].QuestionCode;//  tblQuestion.Rows[_currentPage + 1].Item("QuestionCode");// listQuestionList[_currentPage + 1].QuestionCode;
                if (listAccessPageCode.Count > 0)
                {
                    if (listAccessPageCode[listAccessPageCode.Count - 1] != questionCodeNextPage)
                    {
                        if (listAccessPageCode.Contains(questionCodeNextPage))
                        {
                            //xoa di nhung so phia sau page do
                            int lastIndexNextPage = listAccessPageCode.IndexOf(questionCodeNextPage);
                            listAccessPageCode.RemoveRange(lastIndexNextPage + 1, listAccessPageCode.Count - lastIndexNextPage - 1);
                        }
                        else
                        {
                            listAccessPageCode.Add(_currPageCode);
                        }
                    }
                }
                else
                {
                    listAccessPageCode.Add(_currPageCode);
                }
                if (listAccessPageCode.Count >= 2 && listAccessPageCode[listAccessPageCode.Count - 1] == listAccessPageCode[listAccessPageCode.Count - 2])
                {
                    listAccessPageCode.RemoveAt(listAccessPageCode.Count - 1);
                }
                _currPageCode = questionCodeNextPage;

                if (listAccessPageCode != null)
                {
                    var ListAccessQuestionCode = new List<String>();
                    foreach (var code in listAccessPageCode)
                    {
                        int j = listQuestionListNonRealm.Where(o => o.QuestionCode == code).ToList()[0].OrderIndex;
                        ListAccessQuestionCode.Add(listQuestionListNonRealm[j].QuestionCode);
                    }
                    string s_ListAccessPageCode = Newtonsoft.Json.JsonConvert.SerializeObject(listAccessPageCode);
                    string s_ListAccessQuestionCode = Newtonsoft.Json.JsonConvert.SerializeObject(ListAccessQuestionCode);
                    try
                    {
                        result.Add(s_ListAccessPageCode);
                        result.Add(s_ListAccessQuestionCode);
                        result.Add("OK");
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            stackQuestionControls.Children.Clear();
                            GoToPage(_currPageCode);
                        });

                    }
                    catch (Exception ex)
                    {
                        result.Add(ex.StackTrace);
                        result.Add(ex.Source);
                        result.Add(ex.Message);

                    }

                }

            }
            catch(Exception ex)
            {
                result.Add(ex.StackTrace);
                result.Add(ex.Source);
                result.Add(ex.Message);
            }
            return result;
        }    
        private async void btnNext_Clicked(object sender, EventArgs e)
        {
            try
            {
                string Error = "";
                btnNext.IsEnabled = false;
                btnNext.Clicked -= btnNext_Clicked;
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Đang chuyển trang..."))
                {
                    //await Task.Delay(100);
                    Error=await DoWorkAsync(AnswerID);
                    
                   
                }
                Task.WaitAll();
                await Task.Delay(30);
                btnNext.IsEnabled = true;
                btnNext.Clicked += btnNext_Clicked;

                if (Error.Length>0)
                {
                    UI.ShowError(Error);
                }
               
             


            }
            catch (Exception ex)
            {

                UI.ShowError(ex.Message);
                MyDebugger.WriteLog(ex.Message);
                MyDebugger.WriteLog(ex.StackTrace);
                MyDebugger.WriteLog(ex.Source);
                btnNext.IsEnabled = true;
                btnNext.Clicked += btnNext_Clicked;
            }
        }
        private async Task SaveCurrentData(List<int> _listIndex, List<RmGenericFormValueItem> listFinalResultValue)
        {
            RealmHelper.ResultValueUpdate(listFinalResultValue);
        }
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            btnSave.IsEnabled = false;
            btnSave.Clicked -= btnSave_Clicked;

            RealmHelper.realm.Write(() =>
            {
                interviewTimeItem.CompletedOn = DateTime.Now.ToSafeString();
            });
         
            try
            {
                string result = "";
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Đang lưu..."))
                {
                    result = await SaveAsync();
                }
                if (result.Length > 0)
                {
                    UI.ShowError(result);
                    btnSave.IsEnabled = true;
                    btnSave.Clicked += btnSave_Clicked;
                }
                else
                {
                    var page = new SurveyHomePage(ProjectItem);
                    await page.Process();
                    Application.Current.MainPage = page;
                }
            }
            catch(Exception ex)
            {
                btnSave.IsEnabled = true;
                btnSave.Clicked += btnSave_Clicked;
            }
         



            //watch.Stop();
            //MyDebugger.WriteLog("Saved Interview on " + watch.ElapsedMilliseconds.ToString());

            //var x = await App.dbGenericFormValue.GetItemsAsync(AnswerItem.AnswerID);
            //Dg.HideLoading();
        }
        private Task<string> SaveAsync()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            var listSaveValue = CopyToNonRealm(listFinalResultValue);
            return Task.Run(() =>
            {
                try
                {
                    if (listAccessPageCode == null || listAccessPageCode.Count == 0)
                    {
                        UI.ShowError("Không có dự liệu thích hợp (Mã lỗi 100)");
                        tcs.SetResult("Không có dự liệu thích hợp (Mã lỗi 100)");
                        return tcs.Task;
                    }
                    listAccessPageCode.Add(_currPageCode);
                    var ListAccessQuestionCode = new List<String>();
                    foreach (var code in listAccessPageCode)
                    {
                        int j = listQuestionListNonRealm.Where(o => o.QuestionCode == code).ToList()[0].OrderIndex;
                        ListAccessQuestionCode.Add(listQuestionListNonRealm[j].QuestionCode);
                    }
                    string s_ListAccessPageCode = Newtonsoft.Json.JsonConvert.SerializeObject(listAccessPageCode);
                    var con = RealmConfiguration.DefaultConfiguration;
                    con.SchemaVersion = 1;

                    var realm = Realm.GetInstance(con);
                    var itemAnswer = realm.All<RmAnswerItem>().Where(o => o.AnswerID == AnswerID).First();
                    var oldvalue = realm.All<RmGenericFormValueItem>().Where(o => o.AnswerID == AnswerID);

                    realm.Write(() =>
                    {
                    //interviewTimeItem.CompletedOn = DateTime.Now.ToSafeString();
                    if (oldvalue != null)
                        {
                            realm.RemoveRange(oldvalue);
                        }
                        itemAnswer.AccessPageList = s_ListAccessPageCode;
                        itemAnswer.AccessQuestionCodeList = Newtonsoft.Json.JsonConvert.SerializeObject(ListAccessQuestionCode);
                        itemAnswer.QuestionVersion = CurrentVersion.ToString();
                        foreach (var item in listSaveValue)
                        {
                            var result = realm.CreateObject<RmGenericFormValueItem>();
                            result.ID = item.ID;
                            result.AnswerID = AnswerID;
                            result.AnswerText = item.AnswerText;
                            result.FieldName = item.FieldName;
                            result.FieldType = item.FieldType;
                            result.FieldValue = item.FieldValue;
                            result.PageCode = item.PageCode;
                            result.ProjectID = item.ProjectID;
                            result.QuestionType = item.QuestionType;
                            result.QuestionCode = item.QuestionCode;
                            result.QuestionID = item.QuestionID;
                            
                        }

                    });

                    tcs.SetResult("");
                    return tcs.Task;
                }
                catch(Exception ex)
                {
                    tcs.SetResult(ex.Message);
                    return tcs.Task;
                }
            });
       
            
        }
        private List<RmGenericFormValueItem> CopyToNonRealm(List<RmGenericFormValueItem> _list)
        {
            var resultList = new List<RmGenericFormValueItem>();
            foreach (var item in _list)
            {

                var result = new RmGenericFormValueItem();
                result.ID = item.ID;
                result.AnswerID = AnswerID;
                result.AnswerText = item.AnswerText;
                result.FieldName = item.FieldName;
                result.FieldType = item.FieldType;
                result.FieldValue = item.FieldValue;
                result.PageCode = item.PageCode;
                result.ProjectID = item.ProjectID;
                result.QuestionType = item.QuestionType;
                result.QuestionID = item.QuestionID;
                result.QuestionCode = item.QuestionCode;
                resultList.Add(result);
            }
            return resultList;
        }

        #endregion


    }
}
