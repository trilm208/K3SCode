using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;
using System.Diagnostics;
using Extensions;

namespace K3S
{
    public static class RealmHelper
    {
        public static List<RmWardDistrictCityItem> WardGetByCity(string cityHandle)
        {
            return App.dbRealm.All<RmWardDistrictCityItem>().Where(o => o.City == cityHandle).ToList();
        }

        public static Realms.Realm realm
        {
            get
            {
                return App.dbRealm;
            }
        }

        public static List<RmProjectItem> ProjectGetAll()
        {
            return App.dbRealm.All<RmProjectItem>().ToList();
        }

        public static RmProjectItem ProjectGetByID(string ID)
        {
            var x= App.dbRealm.All<RmProjectItem>().Where(o => o.ProjectID == ID).ToList();
            if (x.Count > 0)
                return x[0];
            else return null;
        }

        public static void ProjectAdd(RmProjectItem item)
        {
            App.dbRealm.Write(() =>
            {
                App.dbRealm.Add<RmProjectItem>(item);
            });
        }

        public static void ProjectDelete(RmProjectItem projectItem)
        {
            App.dbRealm.Write(() =>
            {
                App.dbRealm.Remove(projectItem);
            });
           
        }

        public static void WardInsertAll(List<RmWardDistrictCityItem> listWardDistrictCity)
        {
            App.dbRealm.Write(() =>
            {
                App.dbRealm.RemoveAll<RmWardDistrictCityItem>();
                foreach (var item in listWardDistrictCity)
                {
                    var ward = App.dbRealm.CreateObject<RmWardDistrictCityItem>();
                    ward.City = item.City;
                    ward.District = item.District;
                    ward.Ward = item.Ward;
                }
            });
        }

        public static List<RmWardDistrictCityItem> WardGetAll()
        {
            return App.dbRealm.All<RmWardDistrictCityItem>().ToList();
        }

        public static List<RmAnswerItem> AnswerGetByProjectID(string projectID)
        {
            return App.dbRealm.All<RmAnswerItem>().Where(o => o.ProjectID == projectID).ToList();
        }

        public static List<RmGenericFormQuestionAndroidItem> QuestionGetByProjectID(string projectID)
        {         
            return App.dbRealm.All<RmGenericFormQuestionAndroidItem>().Where(o => o.ProjectID == projectID).ToList();
        }

        public static void ProjectStructureUpdate(string projectID,List<RmGenericFormQuestionAndroidItem> list_Question, List<RmLogicCheckItem> list_LogicCheck, List<RmFilterConditionItem> list_FilterCondition)
        {
            var oldListQ = App.dbRealm.All<RmGenericFormQuestionAndroidItem>().Where(o => o.ProjectID == projectID);
            
            var oldLogic = App.dbRealm.All<RmLogicCheckItem>().Where(o => o.ProjectID == projectID);

            var oldFilter = App.dbRealm.All<RmFilterConditionItem>().Where(o => o.ProjectID == projectID);
            Stopwatch w1 = new Stopwatch();
            w1.Start();
            App.dbRealm.Write(() =>
            {
                if(oldListQ!=null)
                   App.dbRealm.RemoveRange(oldListQ);

                if (oldLogic != null)
                    App.dbRealm.RemoveRange(oldLogic);

                if (oldFilter != null)
                    App.dbRealm.RemoveRange(oldFilter);
                
                foreach (var item in list_Question)
                {                 
                        var question = App.dbRealm.CreateObject<RmGenericFormQuestionAndroidItem>();
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
                }

                foreach (var item in list_LogicCheck)
                {
                    var logic = App.dbRealm.CreateObject<RmLogicCheckItem>();
                    logic.Comments = item.Comments;
                    logic.FromPage = item.FromPage;
                    logic.SkipToPage = item.SkipToPage;
                    logic.SkipType = item.SkipType;
                    logic.SkipLogic = item.SkipLogic;
                    logic.ProjectID = item.ProjectID.ToString();
                }

                foreach (var item in list_FilterCondition)
                {
                    var filter = App.dbRealm.CreateObject<RmFilterConditionItem>();
                    filter.VariableName = item.VariableName;
                    filter.FilterCondition = item.FilterCondition;
                    filter.ProjectID = item.ProjectID.ToString();
                }

            });
            w1.Stop();
            MyDebugger.WriteLog("Save data take " + w1.ElapsedMilliseconds + " miliseconds");

        }

        public static List<RmGenericFormValueItem> ResultValueGetByAnswer(string answerID)
        {
            return realm.All<RmGenericFormValueItem>().Where(o => o.AnswerID == answerID).ToList();
        }

        public static List<RmFilterConditionItem> FilterGetByProjectID(string projectID)
        {
            return realm.All<RmFilterConditionItem>().Where(o => o.ProjectID == projectID).ToList();
        }

        public static List<RmLogicCheckItem> LogicGetByProjectID(string projectID)
        {
            return realm.All<RmLogicCheckItem>().Where(o => o.ProjectID == projectID).ToList();
        }

        public static void ImageInsert(RmProfileImageItem itemImage)
        {
            App.dbRealm.Write(() =>
           {
               App.dbRealm.Add(itemImage);
           });
          
        }

        public static void AnswerInsert(RmAnswerItem item)
        {
            try
            {
                realm.Write(() =>
                {
                    realm.Add(item);
                });

            }
            catch(Exception ex)
            {

            }
        }

        public static void AnswerUpdate(string ID,RmAnswerItem answerItem)
        {

            var oldItem = App.dbRealm.All<RmAnswerItem>().Where(o => o.AnswerID == ID).First();

            App.dbRealm.Remove(oldItem);

            App.dbRealm.Add(answerItem);
        }

        public static void AnswerUpdate(RmAnswerItem answerItem)
        {
            throw new NotImplementedException();
        }   

        public static void ResultValueAdd(List<RmGenericFormValueItem> listFinalResultValue)
        {
            throw new NotImplementedException();
        }

        public static void TimeUpdate(RmInterviewTimeItem interviewTimeItem)
        {
            throw new NotImplementedException();
        }

        public static void ResultValueUpdate(List<RmGenericFormValueItem> listFinalResultValue)
        {
            throw new NotImplementedException();
        }

        public static void  ResultValueInsertAll(List<RmGenericFormValueItem> listFinalResultValue,string AnswerID, string projectID)
        {
            try
            {

                 realm.Write( ()=>
               {

                   foreach (var item in listFinalResultValue)
                   {
                       //var result = realm.CreateObject<RmGenericFormValueItem>();
                       //result.ID = item.ID;
                       //result.AnswerID = AnswerID;
                       //result.AnswerText = item.AnswerText;
                       //result.FieldName = item.FieldName;
                       //result.FieldType = item.FieldType;
                       //result.FieldValue = item.FieldValue;
                       //result.PageCode = item.PageCode;
                       //result.ProjectID = item.ProjectID;
                       //result.QuestionType = item.QuestionType;
                       realm.Add(new RmGenericFormValueItem
                       {
                           ID = item.ID,
                           AnswerID = AnswerID,
                           AnswerText = item.AnswerText,
                           FieldName = item.FieldName,
                           FieldType = item.FieldType,
                           FieldValue = item.FieldValue,
                           PageCode = item.PageCode,
                           ProjectID = item.ProjectID,
                           QuestionType = item.QuestionType,
                           QuestionCode=item.QuestionCode,
                           QuestionID=item.QuestionID
                       });


                   }

               });
            }
            catch (Exception ex)
            {
                UI.ShowError("Error when ResultValueInsertAll");
                return;
            }
        }

        public static void TimeInsert(RmInterviewTimeItem interviewTime)
        {
            try
            {
                realm.Write(() =>
                   {
                       realm.Add<RmInterviewTimeItem>(interviewTime);
                   });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void QuestionAnswerInsertAll(string AnswerID,List<RmGenericFormQuestionAndroidItem> listQuestion)
        {
            try
            {
                var oldListQA = App.dbRealm.All<RmGenericFormQuestionAndroidAnswerItem>().Where(o => o.AnswerID == AnswerID);
                Stopwatch w1 = new Stopwatch();
                w1.Start();
                App.dbRealm.Write(() =>
                {
                    if (oldListQA != null)
                        App.dbRealm.RemoveRange(oldListQA);

                    foreach (var item in listQuestion)
                    {
                        var question = App.dbRealm.CreateObject<RmGenericFormQuestionAndroidAnswerItem>();
                        question.ID = Guid.NewGuid().ToString();
                        question.AnswerID = AnswerID;
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
                    }
                });

                //var x = realm.All<RmGenericFormQuestionAndroidAnswerItem>().Where(o => o.AnswerID == AnswerID);
                w1.Stop();
                MyDebugger.WriteLog("Save data take " + w1.ElapsedMilliseconds + " miliseconds");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        internal static List<RmGenericFormQuestionAndroidAnswerItem> QuestionAnswertGetByAnswerID(string answerID)
        {
            Stopwatch w1 = new Stopwatch();
            w1.Start();
            var result= realm.All<RmGenericFormQuestionAndroidAnswerItem>().Where(o => o.AnswerID==answerID).ToList(); 
            w1.Stop();
            MyDebugger.WriteLog("Take 2 " + w1.ElapsedMilliseconds);
            return result;
        }
    }
}
