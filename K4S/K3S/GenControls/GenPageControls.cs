using K3S.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;
using System.Data;
using System.Diagnostics;
using Extensions;

namespace K3S.GenControls
{
    public class GenPageControls : ContentView
    {
        internal List<RmGenericFormQuestionAndroidAnswerItem> listPage;
        internal List<RmFilterConditionItem> listFilterCondition;
        internal List<RmGenericFormValueItem> listFinalResultValue;
        internal List<string> listAccessPageCode;
        internal string AnswerID;
        private ScrollView scroll;
        public StackLayout layout;     
        internal int FontSizeText;

        public DataTable tblResultEmpty;

        public GenPageControls()
        {
            layout = new StackLayout();
            scroll = new ScrollView();
            
            Content = scroll;
        }
        internal void Process()
        {
            layout.Children.Clear();
            foreach (var _rowQuestion in listPage)
            {
                if (_rowQuestion.QuestionTypeID == 1)
                {
                    Stopwatch w1 = new Stopwatch();
                    w1.Start();
                   var control = new GenSingleChoice();                 
                    (control as GenSingleChoice).QuestionNameHTMLText = _rowQuestion.QuestionNameHTMLText;
                    (control as GenSingleChoice).QuestionCode = _rowQuestion.QuestionCode;
                    (control as GenSingleChoice).AnswerFormat = _rowQuestion.SingleChoice_AnswerFormat;
                    (control as GenSingleChoice).listFilterCondition = listFilterCondition;
                  
                    (control as GenSingleChoice).DefaultText = _rowQuestion.SingleChoice_DefaultText;
                    (control as GenSingleChoice).listFinalResultValue = listFinalResultValue;
                    (control as GenSingleChoice).listAccessPageCode = listAccessPageCode;                  
                    (control as GenSingleChoice).listFinalResultValue = listFinalResultValue;
                    (control as GenSingleChoice).IsRequired = true;
                    (control as GenSingleChoice).AnswerID = AnswerID;
                    (control as GenSingleChoice).tblResultEmpty = tblResultEmpty;
                    (control as GenSingleChoice).FontSizeText = FontSizeText;
                    if (_rowQuestion.SingleChoice_Answer_AnswerList.ToUpper() != "NULL")
                        (control as GenSingleChoice).AnswerList = _rowQuestion.SingleChoice_Answer_AnswerList;                   
                    control.Process();
                    (control as GenSingleChoice).UserSelected += GenPageControls_UserSelected;
                    layout.Children.Add(control);
                    w1.Stop();
                    MyDebugger.WriteLog(string.Format("Create {0} control in {1} miliseconds", _rowQuestion.QuestionCode, w1.ElapsedMilliseconds));
                }

                if (_rowQuestion.QuestionTypeID == 2)
                {
                    Stopwatch w1 = new Stopwatch();
                    w1.Start();

                    var control = new GenMultiChoices();
                    (control as GenMultiChoices).QuestionNameHTMLText = _rowQuestion.QuestionNameHTMLText;
                    (control as GenMultiChoices).QuestionCode = _rowQuestion.QuestionCode;
                    (control as GenMultiChoices).listFilterCondition = listFilterCondition;
                    (control as GenMultiChoices).listFinalResultValue = listFinalResultValue;
                    (control as GenMultiChoices).listAccessPageCode = listAccessPageCode;
                    (control as GenMultiChoices).listFinalResultValue = listFinalResultValue;
                    (control as GenMultiChoices).TypeOfNumberChecks = _rowQuestion.MultiChoice_TypeOfNumberChecks;
                    (control as GenMultiChoices).MinChecks = _rowQuestion.MultiChoice_MinChecks;
                    (control as GenMultiChoices).MaxChecks = _rowQuestion.MultiChoice_MaxChecks;
                    (control as GenMultiChoices).tblResultEmpty = tblResultEmpty;
                    (control as GenMultiChoices).FontSizeText = FontSizeText;

                    (control as GenMultiChoices).AnswerID = AnswerID;
                    
                    if (_rowQuestion.MultiChoice_Answer_AnswerList.ToUpper() != "NULL")
                        (control as GenMultiChoices).AnswerList = _rowQuestion.MultiChoice_Answer_AnswerList;
                    control.Process();
                   
                    layout.Children.Add(control);
                    w1.Stop();
                    MyDebugger.WriteLog(string.Format("Create {0} control in {1} miliseconds", _rowQuestion.QuestionCode, w1.ElapsedMilliseconds));
                }


                if (_rowQuestion.QuestionTypeID  == 7)
                {
                    Stopwatch w1 = new Stopwatch();
                    w1.Start();
                    var control = new GenNumber();

                    (control as GenNumber).AnswerID = AnswerID;
                    (control as GenNumber).QuestionCode = _rowQuestion.QuestionCode;
                    (control as GenNumber).IsRequired = true;
                    (control as GenNumber).QuestionNameHTMLText = _rowQuestion.QuestionNameHTMLText;
                    (control as GenNumber).MaxValue =Convert.ToDecimal( _rowQuestion.Number_MaxValue);
                    (control as GenNumber).MinValue = Convert.ToDecimal(_rowQuestion.Number_MinValue);
                    (control as GenNumber).AllowDecimalAnswer = _rowQuestion.Number_AllowDecimalAnswer;                
                    (control as GenNumber).listAccessPageCode = listAccessPageCode;
                    (control as GenNumber).listFinalResultValue = listFinalResultValue;
                    (control as GenNumber).tblResultEmpty = tblResultEmpty;
                    (control as GenNumber).FontSizeText = FontSizeText;
                    control.Process();
                    layout.Children.Add(control);
                    w1.Stop();
                    MyDebugger.WriteLog(string.Format("Create {0} control in {1} miliseconds", _rowQuestion.QuestionCode, w1.ElapsedMilliseconds));
                }
                if (_rowQuestion.QuestionTypeID == 3)
                {
                    Stopwatch w1 = new Stopwatch();
                    w1.Start();
                    var control = new GenFreeText();

                    (control as GenFreeText).AnswerID = AnswerID;
                    (control as GenFreeText).FontSizeText = FontSizeText;
                    (control as GenFreeText).QuestionCode = _rowQuestion.QuestionCode;
                    (control as GenFreeText).IsRequired = true;
                    (control as GenFreeText).QuestionNameHTMLText = _rowQuestion.QuestionNameHTMLText;
                    (control as GenFreeText).listFinalResultValue = listFinalResultValue;
                    (control as GenFreeText).listAccessPageCode = listAccessPageCode;
                    (control as GenFreeText).tblResultEmpty = tblResultEmpty;
                    control.Process();
                    layout.Children.Add(control);
                    w1.Stop();
                    MyDebugger.WriteLog(string.Format("Create {0} control in {1} miliseconds", _rowQuestion.QuestionCode, w1.ElapsedMilliseconds));

                }

                if (_rowQuestion.QuestionTypeID == 8)
                {
                    var control = new GenGPS();

                    (control as GenGPS).AnswerID = AnswerID;
                    (control as GenGPS).FontSizeText = FontSizeText;
                    (control as GenGPS).QuestionCode = _rowQuestion.QuestionCode;
                    (control as GenGPS).IsRequired = true;
                    (control as GenGPS).QuestionNameHTMLText = _rowQuestion.QuestionNameHTMLText;
                    (control as GenGPS).listFinalResultValue = listFinalResultValue;
                    (control as GenGPS).listAccessPageCode = listAccessPageCode;
                    (control as GenGPS).tblResultEmpty = tblResultEmpty;
                    control.Process();
                    layout.Children.Add(control);
                }

                if (_rowQuestion.QuestionTypeID == 4)
                {
                    Stopwatch w1 = new Stopwatch();
                    w1.Start();
                    var control = new GenLabelText();

                    (control as GenLabelText).FontSizeText = FontSizeText;

                    (control as GenLabelText).AnswerID = AnswerID;
                    (control as GenLabelText).QuestionCode = _rowQuestion.QuestionCode;
                 
                    (control as GenLabelText).QuestionNameHTMLText = _rowQuestion.QuestionNameHTMLText;
                    (control as GenLabelText).listFinalResultValue = listFinalResultValue;
                    (control as GenLabelText).listAccessPageCode = listAccessPageCode;
                    (control as GenLabelText).tblResultEmpty = tblResultEmpty;
                    control.Process();
                    layout.Children.Add(control);
                    w1.Stop();
                    MyDebugger.WriteLog(string.Format("Create {0} control in {1} miliseconds", _rowQuestion.QuestionCode, w1.ElapsedMilliseconds));
                }
                scroll.Content = layout;
            }
        }

        public event EventHandler UserSelected;
        private void GenPageControls_UserSelected(object sender, EventArgs e)
        {
            var handler = UserSelected;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
