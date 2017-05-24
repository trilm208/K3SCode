
using MyDependencyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using K3S.Model;
using Extensions;
using System.Globalization;
using System.Data;

namespace K3S.GenControls
{
     public class GenNumber: GenControl
    {
        internal string AnswerID;
        internal string QuestionCode;
        internal bool IsRequired;
        internal string QuestionNameHTMLText;
        internal decimal MaxValue;
        internal decimal MinValue;
        internal bool AllowDecimalAnswer;
        internal List<RmGenericFormValueItem> listFinalResultValue;

        public List<string> listAccessPageCode { get; internal set; }
        StackLayout layout;
        private Entry txtInputBox;
        internal DataTable tblResultEmpty;
        private string ErrorMess;

        public List<RmGenericFormValueItem> listResultValue
        {
            get
            {
                List<RmGenericFormValueItem> List_OtherSpecify = new List<RmGenericFormValueItem>();
                var result = new List<RmGenericFormValueItem>();
                var row = new RmGenericFormValueItem();
                row.FieldName= QuestionCode;
                decimal value;

                if (AllowDecimalAnswer == true)
                {
                    var culture = CultureInfo.InvariantCulture;
                    try
                    {
                        value = Convert.ToDecimal(txtInputBox.Text.Trim().Replace(",", "."));
                    }
                    catch
                    {
                        ErrorMess = "Trường này chỉ chấp nhận số thập phân";
                        txtInputBox.Focus();
                        return null;
                    }
                }
                else
                {
                    int valueresult;
                    if (int.TryParse(txtInputBox.Text, out valueresult) == false)
                    {
                        ErrorMess = "Trường này chỉ chấp nhận số nguyên";
                        txtInputBox.Focus();
                        return null;
                    }
                    else
                    {
                        value = valueresult;
                    }
                    value = valueresult;                
                }

                if (IsRequired == true && txtInputBox.Text.IsEmpty())
                {
                    ErrorMess = "Giá trị không thể trống";                 
                    txtInputBox.Focus();
                    return null;
                }
                if (value < MinValue)
                {
                    ErrorMess= String.Format("Giá trị bé  hơn giá trị cho phép từ {0} đến {1}", MinValue.ToString(), MaxValue.ToString());              
                    txtInputBox.Focus();
                    return null;
                }
                if (value > MaxValue)
                {
                    ErrorMess = String.Format("Giá trị lớn  hơn giá trị cho phép từ {0} đến {1}", MinValue.ToString(), MaxValue.ToString());                
                    txtInputBox.Focus();
                    return null;
                }
                row.FieldValue = value.ToString();
                row.AnswerText = value.ToString();
  
                result.Add(row); 
                return result;
            }        
        }

        public int FontSizeText { get; internal set; }

        public override List<RmGenericFormValueItem> GetListResultValue()
        {
            return listResultValue;
        }

        public override Dictionary<string, bool> CheckValidation()
        {
          
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            if (listResultValue == null)
            {
                result.Add(ErrorMess, false);
                return result;
            }
            result.Add("", true);
            return result;
        }


        public void Process()
        {
            layout = new StackLayout();
            var tblFinalResultValue = DependencyService.Get<IConvertExtensions>().ToTable<RmGenericFormValueItem>(listFinalResultValue);
            var tvLabel = new HtmlFormattedLabel
            {
              
                Text = DependencyService.Get<ILogicCheck>().TextDisplay(QuestionNameHTMLText, AnswerID.ToString(), listAccessPageCode,tblResultEmpty,listFinalResultValue)
            };
            layout.Children.Add(tvLabel);


            txtInputBox = new Entry();
            txtInputBox.Keyboard = Keyboard.Numeric;


            var listValue = listFinalResultValue.Where(o => o.FieldName == QuestionCode).ToList();
            if(listValue.Count>0)
            {
                txtInputBox.Text = listValue[0].FieldValue;
            }
            else
            {
                txtInputBox.Text = "";
            }

            layout.Children.Add(txtInputBox);
            this.Content = layout;
            
        }
    }
}
