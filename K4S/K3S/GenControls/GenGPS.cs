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
using Plugin.Geolocator;
using Acr.UserDialogs;
using Xamarin.Forms.Maps;
using System.Data;

namespace K3S.GenControls
{
    public class GenGPS : ContentView
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
        private Label labelX;
        private Label labelY;
        private Label labelAddress;
        internal DataTable tblResultEmpty;

        internal void Process()
        {
            //layout = new StackLayout();
            //scroll = new ScrollView();

            //var tblFinalResultValue = DependencyService.Get<IConvertExtensions>().ToTable<RmGenericFormValueItem>(listFinalResultValue);

            //var text = DependencyService.Get<ILogicCheck>().TextDisplay(QuestionNameHTMLText, AnswerID.ToString(), listAccessPageCode, tblFinalResultValue,tblResultEmpty);
            //var tvLabel = new HtmlFormattedLabel
            //{
            //    FontSize = FontSizeText,
            //    Text = text
            //};
            //layout.Children.Add(tvLabel);

            //Button btnDetectedGPS = new Button();
            //btnDetectedGPS.Text = "Xác định GPS";
            //btnDetectedGPS.TextColor = Color.White;
            //btnDetectedGPS.FontSize = 12;
            //btnDetectedGPS.FontAttributes = FontAttributes.Bold;
            //btnDetectedGPS.BackgroundColor = Color.Red;
         
            //btnDetectedGPS.Clicked += btnDetectedGPS_Clicked;
            //layout.Children.Add(btnDetectedGPS);

            //var label1 = new Label();
            //label1.Text = "Kinh độ:";
            //label1.FontSize = FontSizeText;
            //label1.HorizontalOptions = LayoutOptions.Start;
            //label1.FontAttributes = FontAttributes.Bold;

            //labelX = new Label();
            //labelX.Text = "Chưa xác định";
            //labelX.FontSize = FontSizeText;
            //labelX.HorizontalOptions = LayoutOptions.EndAndExpand;
            //labelX.FontAttributes = FontAttributes.Bold;

            //var label2 = new Label();
            //label2.Text = "Vĩ độ:";
            //label2.FontSize = FontSizeText;
            //label2.FontAttributes = FontAttributes.Bold;

            //labelY = new Label();
            //labelY.Text = "Chưa xác định";
            //labelY.FontSize = FontSizeText;
            //labelY.FontAttributes = FontAttributes.Bold;

        
            //Grid myGrid = new Grid();
            //myGrid.HorizontalOptions = LayoutOptions.FillAndExpand;

            //myGrid.RowDefinitions = new RowDefinitionCollection
            //{
            //    new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) },
            //    new RowDefinition { Height = GridLength.Auto }
             
            //};

            //myGrid.ColumnDefinitions = new ColumnDefinitionCollection
            //{
            //      new ColumnDefinition { Width = new GridLength(60, GridUnitType.Absolute) },
            //      new ColumnDefinition { Width = GridLength.Auto }
              
            //};

            ////I need Rows 1,2,3 to span two columns
            //myGrid.Children.Add( label1, 0, 0);
            //myGrid.Children.Add(label2, 0, 1);
            //myGrid.Children.Add(labelX, 1, 0);
            //myGrid.Children.Add(labelY, 1, 1);
          
            //layout.Children.Add(myGrid);
          
            //this.Content = layout;

        }

        private async void btnDetectedGPS_Clicked(object sender, EventArgs e)
        {
            var dlg = Acr.UserDialogs.UserDialogs.Instance;
            dlg.ShowLoading("Đang nhận diện GPS.Vui lòng đợi...");

            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(30000));

                if (position != null)
                {
                    #region INSERT GPS INTO DATABASE    
                    dlg.HideLoading();
                    labelX.Text = position.Longitude.ToString();
                    labelY.Text = position.Latitude.ToString();
                    #endregion
                }

                else
                {
                    dlg.HideLoading();
                    UI.ShowError("Không tìm thấy dữ liệu GPS.Dự án cần dữ liệu GPS.Vui lòng chắc chắn GPS của bạn được bật");
                    return;
                }
            }
            catch (Exception ex)
            {
                dlg.HideLoading();
                UI.ShowError("Dự án cần dữ liệu GPS.Vui lòng chắc chắn GPS của bạn được bật (Error code: " + ex.Message + " " + ex.Source);
                return;
            }
        }

        public List<RmGenericFormValueItem> listResultValue
        {
            get
            {
                List<RmGenericFormValueItem> List_OtherSpecify = new List<RmGenericFormValueItem>();
                var result = new List<RmGenericFormValueItem>();

                var row = new RmGenericFormValueItem();

                row.FieldName = QuestionCode;

                string value = txtInputBox.Text.ToUpper();

                if (IsRequired == true && value.IsEmpty())
                {
                    string mess = String.Format("Nhập giá trị tại câu {0}", QuestionCode);
                    MyDebugger.WriteLog(mess);
                    UI.ShowError("Nhập giá trị tại câu " + QuestionCode);
                    txtInputBox.Focus();
                    return null;
                }
                row.FieldValue = value;
                row.AnswerText = value;
                result.Add(row);
                return result;
            }
        }
        internal bool CheckValidation()
        {
            if (listResultValue == null)
            {
                return false;
            }
            return true;
        }

    }
}
