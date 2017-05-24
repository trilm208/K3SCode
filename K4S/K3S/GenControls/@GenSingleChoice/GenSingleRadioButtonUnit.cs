using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K3S.Model;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using MyDependencyServices;
using XLabs;

namespace K3S.GenControls._GenSingleChoice
{
    public class GenSingleRadioButtonUnit : ContentView
    {
        internal string _SingleChoice_View_Answer_AnswerCodes_OtherSpecify_InType;
        internal string _SingleChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName;
        internal bool _SingleChoice_View_Answer_AnswerCodes_OtherSpecify;
        internal string _SingleChoice_View_Answer_AnswerCodes_CheckedCode;
        internal int _SingleChoice_View_Answer_AnswerCodes_UnCheckedCode;
        internal int _SingleChoice_View_Answer_AnswerCoding;
        internal string _SingleChoice_View_Answer_AnswerCodes_VariableName;
        internal int _SingleChoice_View_Answer_AnswerIndex;
        internal string _SingleChoice_View_Answer_AnswerText;
        internal List<string> listAccessPageCode;
        internal string AnswerID;
        internal Entry _textBox;
        //internal DataTable tblFinalResultValue;
        List<String> listContaint = new List<string>();
        private CustomRadioButton _radioButton;
        private Label _label;
        public event EventHandler CheckedChange;
        public event EventHandler UserSelected;
        private StackLayout layout = new StackLayout();
        private StackLayout layoutSub = new StackLayout();
        internal int FontSizeText;
        internal DataTable tblResultEmpty;
        public List<RmGenericFormValueItem> listFinalResultValue;

        internal void Process()
        {
            layoutSub.Orientation = StackOrientation.Horizontal;
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) => OnLabelClicked();
            tgr.NumberOfTapsRequired = 1;
            layout.GestureRecognizers.Add(tgr);

            if (_SingleChoice_View_Answer_AnswerCodes_OtherSpecify == true)
            {
                _radioButton = new XLabs.Forms.Controls.CustomRadioButton();
                _radioButton.TextColor = Color.Red;
                _radioButton.FontSize = FontSizeText;
              
                //_radioButton.Text =DependencyService.Get<ILogicCheck>().TextDisplay(_SingleChoice_View_Answer_AnswerText, AnswerID.ToString(), listAccessPageCode, tblFinalResultValue);
                _label = new Label();
                _label.Text = DependencyService.Get<ILogicCheck>().TextDisplay(_SingleChoice_View_Answer_AnswerText, AnswerID.ToString(), listAccessPageCode, tblResultEmpty, listFinalResultValue);
                _label.FontSize = FontSizeText;
                _label.TextColor = Color.Black;               
                _label.FontAttributes = FontAttributes.Bold;
                _label.VerticalTextAlignment = TextAlignment.Center;
                _textBox = new Entry();
                if (_SingleChoice_View_Answer_AnswerCodes_OtherSpecify_InType == "Integer")
                {
                    _textBox.Keyboard = Keyboard.Numeric;
                }
                else
                {
                    _textBox.Keyboard = Keyboard.Default;
                }

                _radioButton.CheckedChanged += _radioButton_CheckedChange;
                layoutSub.Children.Add(_radioButton);
               
                layoutSub.Children.Add(_label);
                
                layout.Children.Add(layoutSub);               
                layout.Children.Add(_textBox);              
            }
            else
            {
                _radioButton = new XLabs.Forms.Controls.CustomRadioButton();
               
                _radioButton.CheckedChanged += _radioButton_CheckedChange;
            
                _label = new Label();
                _label.Text = DependencyService.Get<ILogicCheck>().TextDisplay(_SingleChoice_View_Answer_AnswerText, AnswerID.ToString(), listAccessPageCode, tblResultEmpty, listFinalResultValue);
                _label.FontSize = FontSizeText;
                _label.TextColor = Color.Black;
                _label.VerticalTextAlignment = TextAlignment.Center;
                _label.FontAttributes = FontAttributes.Bold;
                layoutSub.Children.Add(_radioButton);
                layoutSub.Children.Add(_label);
                layout.Children.Add(layoutSub);   
            }
            
            Content = layout;
        }

        internal void SetHeightRequest(int v)
        {
            layout.HeightRequest = 0;
            layout.IsVisible = false;
        }

        private void OnLabelClicked()
        {
            _radioButton.Checked = true;
            var handler = UserSelected;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void _radioButton_CheckedChange(object sender, EventArgs<bool> e)
        {
            if (_radioButton.Checked == true)
            {
                if (_label != null)
                    _label.TextColor = Color.Red;
                if (_textBox != null)
                {
                    _textBox.IsEnabled = true;
                    //_textBox.Focus();
                }
                {
                    var handler = CheckedChange;
                    if (handler != null)
                        handler(this, EventArgs.Empty);
                }

            }
            else
            {
                if(_label!=null)
                _label.TextColor = Color.Black;
                if (_textBox != null)
                {
                    _textBox.IsEnabled = false;
                }

            }
        }

       
        public string OtherSpecifyFieldValue
        {
            set
            {
                if (_textBox != null)
                {
                    _textBox.Text = value;
                }
            }
            get
            {
                if (_textBox != null && _textBox.IsEnabled == true)
                {
                    if (_textBox.Text == null)
                        return "";
                    return _textBox.Text.Trim();
                }
                else
                {
                    return "";
                }
            }
        }
        internal void SetEnable(bool p)
        {
            _radioButton.IsEnabled = p;

        }
        public bool Checked
        {
            get
            {
                if (_radioButton == null || _radioButton.Checked == null)
                    return false;
                return _radioButton.Checked;
            }
            set
            {
                _radioButton.Checked = value;
                _radioButton_CheckedChange(null, null);
            }
        }

        internal void UnChecked()
        {
            _radioButton.Checked = false;
            _radioButton_CheckedChange(null, null);
        }

        internal void ChangeFormatCheckedState(bool v)
        {
            if (_radioButton != null)
            {
                if (v == true)
                {
                    _radioButton.TextColor = Color.Red;
                   
                }
                else
                {
                    _radioButton.TextColor = Color.Black;
                }
            }
          
        }

    }
}
