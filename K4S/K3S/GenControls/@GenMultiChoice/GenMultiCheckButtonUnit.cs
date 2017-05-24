using MyDependencyServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using K3S.Model;

namespace K3S.GenControls._GenMultiChoice
{
    public class GenMultiCheckButtonUnit : ContentView
    {
        internal DataTable tblFinalResultValue;
        internal string _MultiChoice_View_Answer_AnswerText;
        internal string _MultiChoice_View_Answer_AnswerCodes_VariableName;
        internal int _MultiChoice_View_Answer_AnswerCodes_UnCheckedCode;
        internal int _MultiChoice_View_Answer_AnswerCodes_CheckedCode;
        internal bool _MultiChoice_View_Answer_AnswerCodes_OtherSpecify;
        internal string _MultiChoice_View_Answer_AnswerCodes_OtherSpecify_VariableName;
        internal string _MultiChoice_View_Answer_AnswerCodes_OtherSpecify_InType;
        internal List<string> listAccessPageCode;
        internal string AnswerID;

        public Entry _textBox;
        public int _MultiChoice_View_Answer_AnswerIndex { get; internal set; }
        
        private StackLayout layout = new StackLayout();
        private StackLayout layoutSub = new StackLayout();
        private CheckBox _checkEdit;
        private Label _label;
        internal int _MultiChoice_View_Answer_IsSingle;
        internal DataTable tblResultEmpty;
        public List<RmGenericFormValueItem> listFinalResultValue;

        internal void Process()
        {

            layoutSub.Orientation = StackOrientation.Horizontal;
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (s, e) => OnLabelClicked();
            tgr.NumberOfTapsRequired = 1;
            layout.GestureRecognizers.Add(tgr);

            _checkEdit = new CheckBox();          
            _label = new Label();
            _label.Text = DependencyService.Get<ILogicCheck>().TextDisplay(_MultiChoice_View_Answer_AnswerText, AnswerID.ToString(), listAccessPageCode, tblResultEmpty, listFinalResultValue);// tblFinalResultValue,tblResultEmpty);
            _label.FontSize = FontSizeText;
            _label.TextColor = Color.Black;
            _label.FontAttributes = FontAttributes.Bold;
            _label.VerticalTextAlignment = TextAlignment.Center;
            layoutSub.GestureRecognizers.Add(new TapGestureRecognizer((view) => OnLabelClicked()));            
            _checkEdit.CheckedChanged += _checkEdit_CheckedChanged; ;
            layoutSub.Children.Add(_checkEdit);
            layoutSub.Children.Add(_label);
            layout.Children.Add(layoutSub);

            if (_MultiChoice_View_Answer_AnswerCodes_OtherSpecify == true)
            {
                _textBox = new Entry();
                layout.Children.Add(_textBox);
                if (_MultiChoice_View_Answer_AnswerCodes_OtherSpecify_InType == "Integer")
                {
                    _textBox.Keyboard = Keyboard.Numeric;
                }
                else
                {
                    _textBox.Keyboard = Keyboard.Default;
                }
               
            }

            _checkEdit.CheckedChanged += _checkEdit_CheckedChanged;

            Content = layout;
        }

        internal void SetEnable(bool p)
        {
            _checkEdit.IsEnabled = p;

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
                    return _textBox.Text.Trim();
                }
                else
                {
                    return "";
                }
            }
        }

        public bool Checked
        {
            get
            {
                if (_checkEdit == null || _checkEdit.Checked == null)
                    return false;
                return _checkEdit.Checked;
            }
            set
            {
                if (_checkEdit != null)
                {
                    _checkEdit.Checked = value;
                    _checkEdit_CheckedChanged(null, null);
                }
            }
        }

        public int FontSizeText { get; internal set; }

        public event EventHandler CheckedChange;
        private void _checkEdit_CheckedChanged(object sender, XLabs.EventArgs<bool> e)
        {
            if (_checkEdit.Checked == true)
            {
                if (_textBox != null)
                {        
                    _textBox.IsEnabled = true;
                }
                var handler = CheckedChange;
                if (handler != null)
                    handler(this, EventArgs.Empty);

            }
            else
            {

                if (_textBox != null)
                {
                  
                    _textBox.IsEnabled = false;
                }

            }
        }

        private void OnLabelClicked()
        {
          if(_checkEdit!=null )
            {
                if(_checkEdit.Checked==true)
                {
                    _checkEdit.Checked = false;
                }
                else
                {
                    _checkEdit.Checked = true;
                }
            }
        }

        internal void SetHeightRequest(int v)
        {

            layout.HeightRequest = v;
            layout.IsVisible = false;
        }
    }
}
