using Extensions;
using K3S.Model;
using MyDependencyServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace K3S.GenControls._GenSingleChoice
{
   public class GenSingleChoiceCombo :ContentView
    {
        internal bool IsRequired;

        internal List<RmFilterConditionItem> listFilterCondition;
        internal List<RmGenericFormValueItem> listFinalResultValue;

        internal List<string> listAccessPageCode;
        internal string AnswerID;
        internal List<SingleAnswerChoice> ListChoice;
        internal DataTable tblFilterCondition;
        internal DataTable tblFinalResultValue;
        internal string DefaultText;
        StackLayout layout = new StackLayout();

        Picker _picker;
        public string QuestionCode { get; internal set; }

        Dictionary<string, string> _dicValue;
        List<String> _list;
        internal DataTable tblResultEmpty;

        public List<RmGenericFormValueItem> listResultValue
        {
            get
            {
                try
                {
                    var result = new List<RmGenericFormValueItem>();

                    var row = new RmGenericFormValueItem();

                    row.FieldName = QuestionCode;

                    string value = "";
                    string answer = "";

                    answer = _picker.SelectedItem.ToString();

                    var r = _dicValue.Where(obj => obj.Value == answer).ToList();

                    if (r.Count == 0)
                    {
                        ErrorMess = "Vui lòng chọn giá trị";
                        //UI.ShowError("Vui lòng chọn giá trị");
                        return null;
                    }


                    value = r[0].Key.ToString();

                    row.FieldValue = value;
                    row.AnswerText = answer;
                    if (IsRequired == true)
                    {
                        if (value.IsEmpty())
                        {
                            ErrorMess = "Vui lòng chọn một giá trị";
                            return null;
                        }
                    }
                    else
                    {
                    }
                    result.Add(row);


                    return result;
                }
                catch
                {
                    ErrorMess = "Vui lòng chọn một giá trị";
                    return null;
                }
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
        string ErrorMess = "";

        internal void Process()
        {
            _picker = new Picker();
            _dicValue = new Dictionary<string, string>();
            _list = new List<string>();
            _picker.Title = DefaultText;
          
            if (ListChoice.Count > 0)
            {
              
                foreach (var row in ListChoice)
                {
                   

                    var _IsShow = true;
                    var _filter = listFilterCondition.Where(obj => obj.VariableName == row.SingleChoice_View_Answer_AnswerCodes_VariableName).ToList();
                    if (_filter.Count() == 0)
                    {
                        _IsShow = true;
                    }
                    else
                    {
                        _IsShow = DependencyService.Get<ILogicCheck>().checkLogic(tblFilterCondition, tblFinalResultValue, row.SingleChoice_View_Answer_AnswerCodes_VariableName, listAccessPageCode);
                    }

                    if (_IsShow == true)
                    {
                        _list.Add(row.SingleChoice_View_Answer_AnswerText);
                        _dicValue.Add(row.SingleChoice_View_Answer_AnswerCoding.ToString(), row.SingleChoice_View_Answer_AnswerText);

                    }
                    //tim cho selected
                    if (_IsShow == false)
                    {
                        //_buttonUnit.SetEnable(false);
                        //_buttonUnit.Checked = false;
                        //_buttonUnit.SetHeightRequest(0);
                    }

                 
                  
                }
            }

            string oldValue = "";
            var oldValueList = listFinalResultValue.Where(obj => obj.FieldName == QuestionCode).ToList();

            if (oldValueList.Count > 0)
            {
                oldValue = oldValueList[0].FieldValue;
            }

            _picker.ItemsSource = _list;

            int i = 0;
            foreach (var item in _dicValue)
            {
                if(item.Key==oldValue)
                {
                  
                    _picker.SelectedIndex = i;
                    break;
                }
                i++;
                            
            }     
            if(_picker.SelectedIndex<0)
            {
                
            }
            layout.Children.Add(_picker);
            Content = layout;

        }
    }
}
