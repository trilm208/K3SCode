using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using MyDependencyServices;
using System.Data;
using Newtonsoft.Json;
using K3S.GenControls._GenMultiChoice;

namespace K3S.GenControls
{
    public class GenMultiChoices : GenControl
    {
        internal string QuestionNameHTMLText;
        internal string QuestionCode;

        public string ProjectID { get; set; }

        public string QuestionID { get; set; }

        public override List<RmGenericFormValueItem> GetListResultValue()
        {
            return listResultValue;
        }

        public string QuestionName { set; get; }

        public string MultiChoice_Answer_AnswerList { get; set; }

        public string MultiChoice_IsContainNoneOfTheAbove { get; set; }

        public int MinChecks { get; set; }

        public int MaxChecks { get; set; }

        public int TypeOfNumberChecks { get; set; }

        internal List<RmFilterConditionItem> listFilterCondition;
        internal List<RmGenericFormValueItem> listFinalResultValue;
        internal List<string> listAccessPageCode;
        internal string AnswerID;
        internal string AnswerList;
        private DataTable tblFinalResultValue;
        private GenMultiChoicesCheckButton genMultiChoicesCheckButton;
        private DataTable tblFilterCondition;
        internal DataTable tblResultEmpty;

        public override Dictionary<string, bool> CheckValidation()
        {
            
            if (genMultiChoicesCheckButton != null)
            {
                return genMultiChoicesCheckButton.CheckValidation();
            }
            else
            {
                return genMultiChoicesCheckButton.CheckValidation();
            }
        }


        public List<RmGenericFormValueItem> listResultValue
        {
            get { return genMultiChoicesCheckButton.listResultValue; }
        }

        public int FontSizeText { get; internal set; }

        internal void Process()
        {
            var layout = new StackLayout();
            tblFinalResultValue = DependencyService.Get<IConvertExtensions>().ToTable<RmGenericFormValueItem>(listFinalResultValue);
            tblFilterCondition = DependencyService.Get<IConvertExtensions>().ToTable<RmFilterConditionItem>(listFilterCondition);
            HtmlFormattedLabel txtQuestionName = new HtmlFormattedLabel
            {
                FontSize = FontSizeText,
                Text = DependencyService.Get<ILogicCheck>().TextDisplay(QuestionNameHTMLText, AnswerID.ToString(), listAccessPageCode,tblResultEmpty,listFinalResultValue)
            };
            layout.Children.Add(txtQuestionName);
            var listChoice = JsonConvert.DeserializeObject<List<MultiAnswerChoice>>(AnswerList);
            var list = new List<string>();

            foreach (var choice in listChoice)
            {
                if (choice.IsRandom== "1")
                {
                    list.Add(choice.MultiChoice_View_Answer_AnswerCodes_VariableName);
                }
            }

            List<string> listexists = new List<string>();
            var temptbl = new List<MultiAnswerChoice>();

            if(list.Count>0)
            {
                var rnd = new Random();
                var result = list.OrderBy(item => rnd.Next()).ToList();
                foreach (var choice in listChoice)
                {
                    if (choice.IsRandom == "1")
                    {
                        // tim 1id khac cung co chung Y

                        foreach (var dr in listChoice)
                        {
                            if (dr.MultiChoice_View_Answer_AnswerCodes_VariableName == result[0])
                            {
                                temptbl.Add(dr);
                            }
                        }

                        result.RemoveAt(0);

                    }
                    else
                    {
                        temptbl.Add(choice);
                    }
                }
                
            }
            else
            {
                temptbl = listChoice;
            }
            listChoice = temptbl;

            genMultiChoicesCheckButton = new _GenMultiChoice.GenMultiChoicesCheckButton();

            genMultiChoicesCheckButton.TypeOfNumberChecks = TypeOfNumberChecks;
            genMultiChoicesCheckButton.MinChecks = MinChecks;
            genMultiChoicesCheckButton.MaxChecks = MaxChecks;
            genMultiChoicesCheckButton.MultiChoice_IsContainNoneOfTheAbove = MultiChoice_IsContainNoneOfTheAbove;
            genMultiChoicesCheckButton.QuestionCode = QuestionCode;
            genMultiChoicesCheckButton.ListChoice = listChoice ;
            genMultiChoicesCheckButton.AnswerID = AnswerID;
            genMultiChoicesCheckButton.listFilterCondition = listFilterCondition;
            genMultiChoicesCheckButton.listFinalResultValue = listFinalResultValue;
            genMultiChoicesCheckButton.tblFilterCondition = tblFilterCondition;
            genMultiChoicesCheckButton.tblFinalResultValue = tblFinalResultValue;
            genMultiChoicesCheckButton.listAccessPageCode = this.listAccessPageCode;
            genMultiChoicesCheckButton.tblResultEmpty = this.tblResultEmpty;
            genMultiChoicesCheckButton.FontSizeText = this.FontSizeText;
            genMultiChoicesCheckButton.Process();

            layout.Children.Add(genMultiChoicesCheckButton);

            this.Content = layout;

        }
    }
}
