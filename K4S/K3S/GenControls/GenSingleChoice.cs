using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using Newtonsoft.Json;
using K3S.GenControls._GenSingleChoice;
using MyDependencyServices;
using System.Data;

namespace K3S.GenControls
{
    public class GenSingleChoice : GenControl
    {
        internal string QuestionNameHTMLText;
        internal string QuestionCode;
        internal int AnswerFormat;
        internal string DefaultText;
        internal List<string> listAccessPageCode;
        internal bool IsRequired;
        internal List<RmFilterConditionItem> listFilterCondition;
        internal List<RmGenericFormValueItem> listFinalResultValue;
        internal string AnswerID;
        internal string AnswerList;
        private StackLayout stack = new StackLayout();
        private GenSingleChoiceRadioButton _genSingleChoiceRadioButton;
        private GenSingleChoiceCombo _genSingleChoiceCombo;
        internal int FontSizeText;
        internal DataTable tblResultEmpty;

        internal List<RmGenericFormValueItem> listResultValue
        {
            get
            {
                if (_genSingleChoiceRadioButton != null)
                {
                    return _genSingleChoiceRadioButton.listResultValue;
                }
               
                else
                {
                    return _genSingleChoiceCombo.listResultValue;
                }
            }
        }

        public override List<RmGenericFormValueItem> GetListResultValue()
        {
            return listResultValue;
        }
        internal void Process()
        {

            //var tblFinalResultValue = DependencyService.Get<IConvertExtensions>().ToTable<RmGenericFormValueItem>(listFinalResultValue);
            var stack1 = new StackLayout
            {
                Children =
                {
                    new HtmlFormattedLabel
                    {
                       FontSize=FontSizeText,
                        Text= DependencyService.Get<ILogicCheck>().TextDisplay(QuestionNameHTMLText, AnswerID.ToString(), listAccessPageCode,tblResultEmpty, listFinalResultValue)
                    },
                }
            };

            var listChoice = JsonConvert.DeserializeObject<List<SingleAnswerChoice>>(AnswerList);

            var list = new List<string>();

            foreach (var choice in listChoice)
            {
                if (choice.IsRandom == 1)
                {
                    list.Add(choice.SingleChoice_View_Answer_AnswerCodes_VariableName);
                }
            }
            List<string> listexists = new List<string>();
            var tempListAnswer = new List<SingleAnswerChoice>();
            if (list.Count > 1)
            {
                var rnd = new Random();
                var result = list.OrderBy(item => rnd.Next()).ToList();
             
                foreach (var choice in listChoice)
                {
                    if (choice.IsRandom == 1)
                    {
                        // tim 1id khac cung co chung Y

                        foreach (var dr in listChoice)
                        {
                            if (dr.SingleChoice_View_Answer_AnswerCodes_VariableName == result[0])
                            {
                                tempListAnswer.Add(dr);
                            }
                        }

                        result.RemoveAt(0);

                    }
                    else
                    {
                        tempListAnswer.Add(choice);
                    }
                }
            }
            else
            {
                tempListAnswer = listChoice;
            }

            listChoice = tempListAnswer;

            if (AnswerFormat == 1)
            {
                _genSingleChoiceRadioButton = new GenSingleChoiceRadioButton();
                _genSingleChoiceRadioButton.QuestionCode = QuestionCode;
                _genSingleChoiceRadioButton.IsRequired = this.IsRequired;
                _genSingleChoiceRadioButton.ListChoice = listChoice;
                _genSingleChoiceRadioButton.tblFilterCondition = DependencyService.Get<IConvertExtensions>().ToTable<RmFilterConditionItem>(listFilterCondition);
                //_genSingleChoiceRadioButton.tblFinalResultValue = tblFinalResultValue;
                _genSingleChoiceRadioButton.listFilterCondition = listFilterCondition;
                _genSingleChoiceRadioButton.listFinalResultValue = listFinalResultValue;
                _genSingleChoiceRadioButton.listAccessPageCode = listAccessPageCode;
                _genSingleChoiceRadioButton.AnswerID = this.AnswerID;
                _genSingleChoiceRadioButton.FontSizeText = FontSizeText;
                _genSingleChoiceRadioButton.tblResultEmpty = tblResultEmpty;
                _genSingleChoiceRadioButton.Process();
                _genSingleChoiceRadioButton.UserSelected += _genSingleChoiceRadioButton_UserSelected;
                stack1.Children.Add(_genSingleChoiceRadioButton);
            }
            else
            {
                _genSingleChoiceCombo = new GenSingleChoiceCombo();
                _genSingleChoiceCombo.QuestionCode = QuestionCode;
                _genSingleChoiceCombo.IsRequired = this.IsRequired;
                _genSingleChoiceCombo.ListChoice = listChoice;
                _genSingleChoiceCombo.tblFilterCondition = DependencyService.Get<IConvertExtensions>().ToTable<RmFilterConditionItem>(listFilterCondition);
                //_genSingleChoiceCombo.tblFinalResultValue = tblFinalResultValue;
                _genSingleChoiceCombo.listFilterCondition = listFilterCondition;
                _genSingleChoiceCombo.listFinalResultValue = listFinalResultValue;
                _genSingleChoiceCombo.listAccessPageCode = listAccessPageCode;
                _genSingleChoiceCombo.AnswerID = this.AnswerID;
                _genSingleChoiceCombo.DefaultText = this.DefaultText;
                _genSingleChoiceCombo.tblResultEmpty = tblResultEmpty;
                _genSingleChoiceCombo.Process();

                stack1.Children.Add(_genSingleChoiceCombo);
            }
            this.Content = stack1;
        }

        public event EventHandler UserSelected;
        private void _genSingleChoiceRadioButton_UserSelected(object sender, EventArgs e)
        {
            var handler = UserSelected;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public override Dictionary<string, bool> CheckValidation()
        {
           
            if (_genSingleChoiceRadioButton != null)
            {
                return _genSingleChoiceRadioButton.CheckValidation();
            }
            else
            {
               
                return _genSingleChoiceCombo.CheckValidation();
            }
        }
    }
}
