using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;

namespace MyDependencyServices
{
    public interface ILogicCheck
    {
        bool checkLogic(DataTable filterTable, DataTable dataTable, string _variableName, List<string> listAccessPageCode);
        //string TextDisplay(string s, string AnswerID, List<String> listPageCodeAccessed, DataTable tblFinalResultValue,DataTable tblResultEmpty);
        string TextDisplay(string s, string AnswerID, List<String> listPageCodeAccessed, DataTable tblResultEmpty,List<RmGenericFormValueItem> listResultValue);
        //Dictionary<string,string>  checkLogicCheckPostSkip(DataTable tblQuestion,List<RmGenericFormQuestionAndroidAnswerItem> listPage, List<RmGenericFormQuestionAndroidAnswerItem> listQuestion, List<RmGenericFormValueItem> listValue, DataTable listLogicCheck, string _currPageCode, List<string> listPageCodeAccessed, DataTable tblResultEmpty);
        DataTable PivotTable(DataTable dt,DataTable tblResultEmpty);
        DataTable PivotTable(List<RmGenericFormValueItem> listData, DataTable tblResultEmpty);
        DataTable ToTableQuestionList(List<RmGenericFormQuestionAndroidAnswerItem> list);
        DataTable CreatePivotTableEmptyData(List<RmGenericFormValueItem> dt);
        Dictionary<string, string> checkLogicCheckPostSkip(List<RmLogicCheckItem> listLogic, List<RmGenericFormQuestionAndroidAnswerItem> listQuestion, DataTable tblPage, List<RmGenericFormValueItem> listFinalResultValue, string currPageCode, List<string> listAccessPageCode, DataTable tblResultEmpty);
        Dictionary<string, string> checkLogicCheckPostSkip2(List<RmLogicCheckItem> listLogic, DataTable tblQuestion, DataTable tblPage, List<RmGenericFormValueItem> listFinalResultValue, string currPageCode, List<string> listAccessPageCode, DataTable tblResultEmpty);
    }
}
