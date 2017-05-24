using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using MyDependencyServices;
using Extensions;
using XLabs.Forms.Controls;
using System.Data;
using System.Diagnostics;

namespace K3S.GenControls
{
    public class GenFreeText : GenControl
    {
        internal string AnswerID;
        internal string QuestionCode;
        internal bool IsRequired;
        internal string QuestionNameHTMLText;
        internal List<RmGenericFormValueItem> listFinalResultValue;
        internal List<string> listAccessPageCode;

        public StackLayout layout;
        private ScrollView scroll;
        private ExtendedEntry txtInputBox;
        internal int FontSizeText;
        internal DataTable tblResultEmpty;

        internal void Process()
        {
            layout = new StackLayout();
            scroll = new ScrollView();
            Stopwatch w1 = new Stopwatch();
            w1.Start();
            //var tblFinalResultValue = DependencyService.Get<IConvertExtensions>().ToTable<RmGenericFormValueItem>(listFinalResultValue);
            //w1.Stop();
            //MyDebugger.WriteLog(string.Format("convert list to table {0} miliseconds", w1.ElapsedMilliseconds));
            //w1.Restart();
            //var browser = new WebView();
            //var htmlSource = new HtmlWebViewSource();
            var text= DependencyService.Get<ILogicCheck>().TextDisplay(QuestionNameHTMLText, AnswerID.ToString(), listAccessPageCode,tblResultEmpty,listFinalResultValue);
            w1.Stop();
            MyDebugger.WriteLog(string.Format("text display {0} miliseconds", w1.ElapsedMilliseconds));
            w1.Restart();
            var tvLabel = new HtmlFormattedLabel
            {
                FontSize = FontSizeText,
                Text = text
            };
            layout.Children.Add(tvLabel);


            txtInputBox = new ExtendedEntry
            {            
                HasBorder = true                                          
            };
           
            var listValue = listFinalResultValue.Where(o => o.FieldName == QuestionCode).ToList();
            w1.Stop();
            MyDebugger.WriteLog(string.Format("old value {0} miliseconds", w1.ElapsedMilliseconds));
            w1.Restart();
            if (listValue.Count > 0)
            {
                txtInputBox.Text = listValue[0].FieldValue;
            }
            else
            {
                txtInputBox.Text = "";
            }
            txtInputBox.HeightRequest = 120;
            txtInputBox.BackgroundColor = Color.FromRgb(219, 186, 186);
            layout.Children.Add(txtInputBox);
         
            this.Content = layout;

            w1.Stop();
            MyDebugger.WriteLog(string.Format("done create {0} miliseconds", w1.ElapsedMilliseconds));

        }
        public List<RmGenericFormValueItem> listResultValue
        {
            get
            {
                List<RmGenericFormValueItem> List_OtherSpecify = new List<RmGenericFormValueItem>();
                var result = new List<RmGenericFormValueItem>();

                var row = new RmGenericFormValueItem();

                row.FieldName = QuestionCode;

                string value = "";
                if (txtInputBox.Text == null)
                    value = "";
                else
                {
                    value = txtInputBox.Text.ToUpper();
                }

                if (value.IsEmpty())
                {
                    ErrorMess = String.Format("Nhập giá trị tại câu {0}", QuestionCode);
                    //MyDebugger.WriteLog(mess);

                    //UI.ShowError("Nhập giá trị tại câu " + QuestionCode);
                    txtInputBox.Focus();
                    return null;
                }
                row.FieldValue = value;
                row.AnswerText = value;
                result.Add(row);
                return result;
            }
        }
        string ErrorMess = "";
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
        public override List<RmGenericFormValueItem> GetListResultValue()
        {
            return listResultValue;
        }

    }
}
