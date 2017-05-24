using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using MyDependencyServices;
using System.Data;
using System.Diagnostics;
using Extensions;

namespace K3S.GenControls
{
    public  class GenLabelText : GenControl
    {
        internal string AnswerID;
        internal string QuestionCode;
        internal string QuestionNameHTMLText;
        internal List<RmGenericFormValueItem> listFinalResultValue;
        internal List<string> listAccessPageCode;
        internal int FontSizeText;
        internal DataTable tblResultEmpty;
        private StackLayout layout;
        internal void Process()
        {
            layout = new StackLayout();
            Stopwatch w1 = new Stopwatch();
            w1.Start();
            //var tblFinalResultValue = DependencyService.Get<IConvertExtensions>().ToTable<RmGenericFormValueItem>(listFinalResultValue);
            //w1.Stop();
            //MyDebugger.WriteLog(string.Format("convert list to table {0} miliseconds",  w1.ElapsedMilliseconds));
            //w1.Restart();
            //Forms9Patch.Label tvLabel = new Forms9Patch.Label
            //{
            //    Text = DependencyService.Get<ILogicCheck>().TextDisplay(QuestionNameHTMLText, AnswerID.ToString(), listAccessPageCode, tblFinalResultValue)
            //};

            var text = DependencyService.Get<ILogicCheck>().TextDisplay(QuestionNameHTMLText, AnswerID.ToString(), listAccessPageCode, tblResultEmpty, listFinalResultValue);
            w1.Stop();
            MyDebugger.WriteLog(string.Format("get text display {0} miliseconds", w1.ElapsedMilliseconds));
            w1.Restart();
            var tvLabel = new HtmlFormattedLabel
            {
                FontSize = FontSizeText,
                Text =text
            };
            layout.Children.Add(tvLabel);



            this.Content = layout;
        }
    }
}
