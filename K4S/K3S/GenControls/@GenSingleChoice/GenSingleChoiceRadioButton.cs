using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using MyDependencyServices;
using System.Data;
using Extensions;
using System.Diagnostics;

namespace K3S.GenControls._GenSingleChoice
{
    public class GenSingleChoiceRadioButton : ContentView
    {
        internal bool IsRequired;
      
        internal List<RmFilterConditionItem> listFilterCondition;
        internal List<RmGenericFormValueItem> listFinalResultValue;

        internal List<string> listAccessPageCode;
        internal string AnswerID;
        internal List<SingleAnswerChoice> ListChoice;
        internal DataTable tblFilterCondition;
        //internal DataTable tblFinalResultValue;
        StackLayout layout = new StackLayout();
        ScrollView scroll = new ScrollView();
        internal int FontSizeText;
        internal DataTable tblResultEmpty;

        public string QuestionCode { get; internal set; }

        internal void Process()
        {

            DataTable pivotTable = new DataTable();
            if (ListChoice.Count > 0)
            {
                foreach (var row in ListChoice)
                {
                    GenSingleRadioButtonUnit _buttonUnit = new GenSingleRadioButtonUnit();
                   
                    var _IsShow = true;
                    var _filter = listFilterCondition.Where(obj => obj.VariableName == row.SingleChoice_View_Answer_AnswerCodes_VariableName).ToList();
                    if (_filter.Count() == 0)
                    {
                        _IsShow = true;
                    }
                    else
                    {

                        if (DependencyService.Get<IDataTableExtensions>().IsNull(pivotTable) == true || pivotTable.Rows.Count == 0)
                        {
                            pivotTable = DependencyService.Get<ILogicCheck>().PivotTable(listFinalResultValue,tblResultEmpty);
                        }
                        _IsShow =DependencyService.Get<ILogicCheck>().checkLogic(tblFilterCondition, pivotTable, row.SingleChoice_View_Answer_AnswerCodes_VariableName, listAccessPageCode);
                    }

               
                    if (_IsShow == true)
                    {
                        layout.Children.Add(_buttonUnit);
                        _buttonUnit._SingleChoice_View_Answer_AnswerCodes_OtherSpecify_InType = row.SingleChoice_View_Answer_AnswerCodes_OtherSpecify_InType;
                        _buttonUnit._SingleChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName = row.SingleChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName;
                        _buttonUnit._SingleChoice_View_Answer_AnswerCodes_OtherSpecify = row.SingleChoice_View_Answer_AnswerCodes_OtherSpecify;
                        _buttonUnit._SingleChoice_View_Answer_AnswerCodes_CheckedCode = row.SingleChoice_View_Answer_AnswerCodes_CheckedCode;
                        _buttonUnit._SingleChoice_View_Answer_AnswerCodes_UnCheckedCode = row.SingleChoice_View_Answer_AnswerCodes_UnCheckedCode;
                        _buttonUnit._SingleChoice_View_Answer_AnswerCodes_VariableName = row.SingleChoice_View_Answer_AnswerCodes_VariableName;
                        _buttonUnit._SingleChoice_View_Answer_AnswerCoding = row.SingleChoice_View_Answer_AnswerCoding;
                        _buttonUnit._SingleChoice_View_Answer_AnswerIndex = row.SingleChoice_View_Answer_AnswerCoding;
                        _buttonUnit._SingleChoice_View_Answer_AnswerText = row.SingleChoice_View_Answer_AnswerText;
                        _buttonUnit.listAccessPageCode = listAccessPageCode;
                        //_buttonUnit.tblFinalResultValue = tblFinalResultValue;
                        _buttonUnit.tblResultEmpty = tblResultEmpty;
                        _buttonUnit.FontSizeText = FontSizeText;
                        _buttonUnit.listFinalResultValue = listFinalResultValue;
                        _buttonUnit.AnswerID = this.AnswerID;

                        _buttonUnit.Process();

                        _buttonUnit.CheckedChange += _buttonUnit_CheckedChange;
                        _buttonUnit.UserSelected += _buttonUnit_UserSelected;

                      

                    }
                   
                    else
                    {
                      
                    }
                    //layout.Children.Add(_buttonUnit);

                }
                Stopwatch w1 = new Stopwatch();
                w1.Start();
                string oldValue = "";
                var oldValueList = listFinalResultValue.Where(obj => obj.FieldName == QuestionCode).ToList();

                if (oldValueList.Count > 0)
                {
                    oldValue = oldValueList[0].FieldValue;
                }

                foreach (var control in layout.Children)
                {
                    if (oldValue == (control as GenControls._GenSingleChoice.GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerCodes_CheckedCode.ToString())
                    {
                        (control as GenControls._GenSingleChoice.GenSingleRadioButtonUnit).Checked = true;
                        if ((control as GenControls._GenSingleChoice.GenSingleRadioButtonUnit)._textBox != null)
                        {

                            var textOther = "";
                            var textOtherList = listFinalResultValue.Where(obj => obj.FieldName == (control as GenControls._GenSingleChoice.GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName).ToList();

                            if (textOtherList.Count > 0)
                            {
                                textOther = textOtherList[0].FieldValue;
                            }
                            (control as GenControls._GenSingleChoice.GenSingleRadioButtonUnit)._textBox.Text = textOther;
                        }
                    }
                    else
                    {
                        (control as GenControls._GenSingleChoice.GenSingleRadioButtonUnit).Checked = false;
                    }
                }
                       
                w1.Stop();
                MyDebugger.WriteLog("get old value " + w1.ElapsedMilliseconds);

            }
            scroll.Content = layout;
            Content = scroll;
        }

        string ErrorMess = "";
        public List<RmGenericFormValueItem> listResultValue
        {
            get

            {               
                var result = new List<RmGenericFormValueItem>();
              
                List<RmGenericFormValueItem> List_OtherSpecify = new List<RmGenericFormValueItem>();
           
                var row = new RmGenericFormValueItem();

                row.FieldName= QuestionCode;

                string value = "";
                string answer = "";

                for (int count = 0; count < layout.Children.Count; count++)
                {
                    View ctr = layout.Children[count];
                    if (ctr.GetType() == typeof(GenSingleRadioButtonUnit))
                    {
                        if ((ctr as GenSingleRadioButtonUnit).Checked == true)
                        {
                            value = (ctr as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerCoding.ToString();
                            answer = (ctr as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerText;
                            if ((ctr as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerCodes_OtherSpecify == true)
                            {
                                 var row_OtherSpecify = new RmGenericFormValueItem();
                               
                                row_OtherSpecify.FieldName = (ctr as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName;
                                if ((ctr as GenSingleRadioButtonUnit).OtherSpecifyFieldValue.IsEmpty())
                                {
                       
                                    ErrorMess = "Vui lòng nhập Ghi rõ";
                                    //UI.ShowError("Vui lòng nhập Ghi rõ");
                                    return null;
                                }
                                else
                                {
                                    row_OtherSpecify.FieldValue= (ctr as GenSingleRadioButtonUnit).OtherSpecifyFieldValue;
                                    row_OtherSpecify.AnswerText = (ctr as GenSingleRadioButtonUnit)._textBox.Text.Trim().ToUpper();
                                }
                                List_OtherSpecify.Add(row_OtherSpecify);
                            }
                        }
                        else
                        {
                            if ((ctr as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerCodes_OtherSpecify == true)
                            {
                                RmGenericFormValueItem row_OtherSpecify = new RmGenericFormValueItem();
                                row_OtherSpecify.FieldName= (ctr as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName;
                                row_OtherSpecify.FieldValue = "";
                                row_OtherSpecify.AnswerText = "";
                                List_OtherSpecify.Add(row_OtherSpecify);
                            }
                        }
                    }
                    else
                    {

                    }
                }
                row.FieldValue = value;
                row.AnswerText = answer;
                if (IsRequired == true)
                {
                    if (value.IsEmpty())
                    {
                        ErrorMess = "Vui lòng chọn một giá trị";
                        //UI.ShowError("Vui lòng chọn một giá trị");
                        return null;
                    }
                }
                else
                {
                }
                result.Add(row);
               
                foreach (var row_OtherSpecify in List_OtherSpecify)
                {
                    result.Add(row_OtherSpecify);
                   
                }                           
                 return result;              
            }
        }
        internal Dictionary<string, bool> CheckValidation()
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
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

        public event EventHandler UserSelected;
        private void _buttonUnit_UserSelected(object sender, EventArgs e)
        {
            if ((sender as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerCodes_OtherSpecify == false)
            {
                var handler = UserSelected;
                if (handler != null)
                    handler(this, EventArgs.Empty); ;
            }

        }

        private void _buttonUnit_CheckedChange(object sender, EventArgs e)
        {
            if ((sender as GenSingleRadioButtonUnit).Checked == true)
            {
                //(sender as GenSingleRadioButtonUnit).ChangeFormatCheckedState(true);
                for (int i = 0; i < layout.Children.Count; i++)
                {
                    View ctr = layout.Children[i];
                    if (ctr.GetType() == typeof(GenSingleRadioButtonUnit))
                    {
                        if ((sender as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerIndex != (ctr as GenSingleRadioButtonUnit)._SingleChoice_View_Answer_AnswerIndex)
                        {
                            (ctr as GenSingleRadioButtonUnit).UnChecked();
                            //(ctr as GenSingleRadioButtonUnit).ChangeFormatCheckedState(false);
                        }
                    }
                }

            }
        }

      


    }
}
