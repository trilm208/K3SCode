using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using MyDependencyServices;
using Extensions;

namespace K3S.GenControls._GenMultiChoice
{
    public class GenMultiChoicesCheckButton: ContentView
    {
        internal int TypeOfNumberChecks;
        internal int MinChecks;
        internal int MaxChecks;
        internal string MultiChoice_IsContainNoneOfTheAbove;
        internal string QuestionCode;
        internal string AnswerID;
        internal DataTable tblFilterCondition;
        internal DataTable tblFinalResultValue;
        internal List<string> listAccessPageCode;
        internal List<MultiAnswerChoice> ListChoice;
        StackLayout layout = new StackLayout();
        ScrollView scroll = new ScrollView();
        public List<RmFilterConditionItem> listFilterCondition;
       
        public List<RmGenericFormValueItem> listFinalResultValue;
        internal DataTable tblResultEmpty;

        public List<RmGenericFormValueItem> listResultValue
        {
            get
            {

                var result = new List<RmGenericFormValueItem>();   
                List<RmGenericFormValueItem> List_OtherSpecify = new List<RmGenericFormValueItem>();

                var row = new RmGenericFormValueItem();

                row.FieldName = QuestionCode;

            
                int count_checked = 0;
                for (int i = 0; i < layout.Children.Count; i++)
                {
                    var ctr = layout.Children[i];
                    if (ctr.GetType() == typeof(GenMultiCheckButtonUnit))
                    {
                        var new_row = new RmGenericFormValueItem();
                        new_row.FieldName = (ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerCodes_VariableName;

                        if ((ctr as GenMultiCheckButtonUnit).Checked == true)
                        {
                            count_checked++;
                            new_row.FieldValue = (ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerCodes_CheckedCode.ToString();
                            new_row.AnswerText = (ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerText;
                        }
                        else
                        {
                            new_row.FieldValue = "";
                            new_row.AnswerText = "";
                        }
                        List_OtherSpecify.Add(new_row);

                        if ((ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerCodes_OtherSpecify == true)
                        {
                            var new_row_specify = new RmGenericFormValueItem();
                            new_row_specify.FieldName= (ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName;
                            new_row_specify.AnswerText = (ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerText;
                            string value_Specify = (ctr as GenMultiCheckButtonUnit).OtherSpecifyFieldValue;
                            if ((ctr as GenMultiCheckButtonUnit).Checked == true && value_Specify.IsEmpty())
                            {
                                ErrorMess = "Vui lòng nhập giá trị Ghi rõ";
                                //MyDebugger.WriteLog("Vui lòng nhập giá trị Ghi rõ");
                                //UI.ShowError( "Vui lòng nhập giá trị Ghi rõ");

                                (ctr as GenMultiCheckButtonUnit)._textBox.Focus();
                              
                                return null;
                            }
                            else
                            {
                                new_row_specify.FieldValue = (ctr as GenMultiCheckButtonUnit).OtherSpecifyFieldValue;
                                new_row_specify.AnswerText = (ctr as GenMultiCheckButtonUnit).OtherSpecifyFieldValue;
                            }
                            List_OtherSpecify.Add(new_row_specify);
                        }
                    }
                }

                foreach (var item in List_OtherSpecify)
                {
                    result.Add(item);
                }

                if (count_checked < MinChecks || count_checked > MaxChecks)
                {
                    ErrorMess = String.Format("Vui lòng chọn ít nhất {0} giá trị và nhiều nhất {1} giá trị ở câu {2}", MinChecks.ToString(), MaxChecks.ToString(), QuestionCode); ;
                    return null;
                }

                return result;
                
            }
        
        }

        public int FontSizeText { get; internal set; }

        internal void Process()
        {

            DataTable pivotTable = new DataTable();
            if (ListChoice.Count > 0)
            {
                foreach (var row in ListChoice)
                {
                    GenMultiCheckButtonUnit _buttonUnit = new GenMultiCheckButtonUnit();

                    var _IsShow = true;
                    var _filter = listFilterCondition.Where(obj => obj.VariableName == row.MultiChoice_View_Answer_AnswerCodes_VariableName).ToList();
                    if (_filter.Count() == 0)
                    {
                        _IsShow = true;
                    }
                    else
                    {
                        try
                        {
                            if(DependencyService.Get<IDataTableExtensions>().IsNull(pivotTable)==true || pivotTable.Rows.Count==0)
                            {
                                pivotTable = DependencyService.Get<ILogicCheck>().PivotTable(listFinalResultValue,tblResultEmpty);
                            }
                            _IsShow = DependencyService.Get<ILogicCheck>().checkLogic(tblFilterCondition, pivotTable, row.MultiChoice_View_Answer_AnswerCodes_VariableName, listAccessPageCode);
                        }
                        catch(Exception ex)
                        {
                            MyDebugger.WriteLog(ex.Message + ex.Source + ex.StackTrace);
                            UI.ShowError("Error with Filter Condition");
                           
                            return;
                        }
                    }

                    _buttonUnit._MultiChoice_View_Answer_AnswerText = row.MultiChoice_View_Answer_AnswerText;
                    _buttonUnit._MultiChoice_View_Answer_AnswerIndex = row.MultiChoice_View_Answer_AnswerIndex;
                    _buttonUnit._MultiChoice_View_Answer_AnswerCodes_VariableName = row.MultiChoice_View_Answer_AnswerCodes_VariableName;
                    _buttonUnit._MultiChoice_View_Answer_AnswerCodes_UnCheckedCode = row.MultiChoice_View_Answer_AnswerCodes_UnCheckedCode;
                    _buttonUnit._MultiChoice_View_Answer_AnswerCodes_CheckedCode = row.MultiChoice_View_Answer_AnswerCodes_CheckedCode;
                    _buttonUnit._MultiChoice_View_Answer_AnswerCodes_OtherSpecify = row.MultiChoice_View_Answer_AnswerCodes_OtherSpecify;
                    _buttonUnit._MultiChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName = row.MultiChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName;
                    _buttonUnit._MultiChoice_View_Answer_AnswerCodes_OtherSpecify_InType = row.MultiChoice_View_Answer_AnswerCodes_OtherSpecify_InType;
                    _buttonUnit._MultiChoice_View_Answer_IsSingle = row.MultiChoice_View_Answer_AnswerCodes_IsSingleChoice;
                    _buttonUnit.listAccessPageCode = listAccessPageCode;
                    _buttonUnit.FontSizeText = this.FontSizeText;
                    _buttonUnit.tblFinalResultValue = tblFinalResultValue;
                    _buttonUnit.tblResultEmpty = tblResultEmpty;
                    _buttonUnit.CheckedChange += _buttonUnit_CheckedChange;
                    _buttonUnit.AnswerID = this.AnswerID;

                    _buttonUnit.Process();
                    
                    if (_IsShow == true)
                    {
                        layout.Children.Add(_buttonUnit);

                    }
                    //tim cho selected
                    if (_IsShow == false)
                    {
                        _buttonUnit.SetEnable(false);
                        _buttonUnit.Checked = false;
                        _buttonUnit.SetHeightRequest(0);
                    }

                    else
                    {
                        string oldValue = "";
                        var oldValueList = listFinalResultValue.Where(obj => obj.FieldName ==row.MultiChoice_View_Answer_AnswerCodes_VariableName).ToList();
              
                        if (oldValueList.Count > 0)
                        {
                            oldValue = oldValueList[0].FieldValue;
                        }

                        if (oldValue == row.MultiChoice_View_Answer_AnswerCodes_CheckedCode.ToString())
                        {
                            _buttonUnit.Checked = true;
                        }
                        else
                        {
                            _buttonUnit.Checked = false;
                        }

                        if (_buttonUnit._textBox != null)
                        {

                            var textOther = "";
                            var textOtherList = listFinalResultValue.Where(obj => obj.FieldName == row.MultiChoice_View_Answer_AnswerCodes_VariableName+"_K").ToList();

                            if (textOtherList.Count > 0)
                            {
                                textOther = textOtherList[0].FieldValue;
                            }
                            _buttonUnit._textBox.Text = textOther;
                        }
                    }
                    //layout.Children.Add(_buttonUnit);

                }
            }
            scroll.Content = layout;
            Content = scroll;
               
        }

        private void _buttonUnit_CheckedChange(object sender, EventArgs e)
        {
            if ((sender as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_IsSingle == 1)
            {

                if ((sender as GenMultiCheckButtonUnit).Checked == true)
                {
                    for (int i = 0; i < layout.Children.Count; i++)
                    {
                        View ctr = layout.Children[i];
                        if (ctr.GetType() == typeof(GenMultiCheckButtonUnit))
                        {
                            if ((sender as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerIndex != (ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerIndex)
                            {
                                (ctr as GenMultiCheckButtonUnit).Checked = false;
                            }
                        }
                    }

                }
            }
            else
            {
                if ((sender as GenMultiCheckButtonUnit).Checked == true)
                {
                    for (int i = 0; i < layout.Children.Count; i++)
                    {
                        View ctr = layout.Children[i];
                        if (ctr.GetType() == typeof(GenMultiCheckButtonUnit))
                        {

                            if (((ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_IsSingle == 1 && (sender as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerIndex != (ctr as GenMultiCheckButtonUnit)._MultiChoice_View_Answer_AnswerIndex))
                            {
                                (ctr as GenMultiCheckButtonUnit).Checked = false;
                            }
                        }
                    }
                }
            }
        }
        string ErrorMess = "";
        internal Dictionary<string, bool> CheckValidation()
        {
            var result =new Dictionary<string, bool>();
            if (listResultValue == null)
            {
                result.Add(ErrorMess, false);
               
            }
            else
            {
                result.Add("", true);
            }
            return result;
        }


    }
}
