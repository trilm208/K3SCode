using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Extensions;
using K3S.Model;
using Xamarin.Forms;

namespace K3S
{

    public partial class RespondentInformationDetail : ContentPage
    {
        #region [VARIABLES]
        RmAnswerItem item;
        RmProjectItem ProjectItem;
      
        public bool IsNew
        {
            get;
            set;
        }
  
        private List<RmGenericFormQuestionAndroidAnswerItem> finalQuestionList;
    
        #endregion

        public RespondentInformationDetail(RmAnswerItem _item, RmProjectItem _projectItem, bool _IsNew, List<RmGenericFormQuestionAndroidAnswerItem> _listFinalQuestion) 
        {
            this.finalQuestionList = _listFinalQuestion;         
        
            this.ProjectItem = _projectItem;
            this.item = _item;
            this.IsNew = _IsNew;
            InitializeComponent();
        }

        #region [METHODS]
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        async System.Threading.Tasks.Task<bool> ValidateForm()
        {
            if (txtGreenID.Text == null || txtGreenID.Text.Trim().Length == 0)
            {
                UI.ShowError("Mã xanh không thể trống");
                return false;
            }
            if (txtFullName.Text == null || txtFullName.Text.Trim().Length == 0)
            {
                UI.ShowError("Họ và tên không thể trống");
                return false;
            }
            if (txtGender.SelectedItem == null)
            {
                UI.ShowError("Giới tính không thể trống");
                return false;
            }
            if (txtYoB.SelectedItem == null || txtYoB.SelectedItem.ToString().Trim().Length == 0)
            {
                UI.ShowError("Năm sinh không thể trống");
                return false;
            }
            int namsinh = 0;
            if (int.TryParse(txtYoB.SelectedItem.ToString().Trim(), out namsinh) == false)
            {
                UI.ShowError("Năm sinh phải là số nguyên");
                return false;
            }
            if (namsinh == 0)
            {
                UI.ShowError("Năm sinh phải là số nguyên");
                return false;
            }
            if (txtTelephone.Text == null || txtTelephone.Text.Trim().Length < 7)
            {
                UI.ShowError("Số điện thoại ngắn hơn 7 kí tự");
                return false;
            }
            if (txtDistrict.SelectedItem == null)
            {
                UI.ShowError("Quận không thể trống");
                return false;
            }
            if (txtWard.SelectedItem == null)
            {
                UI.ShowError("Phường không thể trốn");
                return false;
            }
            if (txtStreet.Text == null || txtStreet.Text.Trim().Length == 0)
            {
                UI.ShowError("Đường không thể trống");
                return false;
            }

            if (txtAddress.Text == null || txtAddress.Text.Trim().Length == 0)
            {
                UI.ShowError("Số nhà không thể trống");
                return false;
            }
            return true;
        }
    
        internal async Task Process()
        {
           
            
            foreach (var dr_hanhchanh in App._listHanhChanh)
            {
                if (txtDistrict.Items.IndexOf(dr_hanhchanh.District) < 0)
                    txtDistrict.Items.Add(dr_hanhchanh.District);
            }
            int minYOB = ProjectItem.MinYOB;
            int maxYOB = ProjectItem.MaxYOB;
            string acceptGender = ProjectItem.AcceptGender;

            var acceptGenderList = acceptGender.Split(',');

            foreach(var gender in acceptGenderList)
            {
                txtGender.Items.Add(gender);    
            }
            if (acceptGenderList.Length == 1)
                txtGender.SelectedIndex = 0;
            for (int i = minYOB; i <= maxYOB; i++)
            {
                txtYoB.Items.Add(i.ToString());
            }
            if (IsNew == false)
            {
                await LoadData();
            }
        }

        async Task LoadData()
        {
            txtDistrict.SelectedItem = item.RespondentDistrict.ToString();
            txtFullName.Text = item.RespondentFullName;
            txtGreenID.Text = item.GreenID;
            txtYoB.SelectedItem = item.RespondentYoB.ToString();
            txtGender.SelectedItem = item.RespondentGender.ToString();         
            txtStreet.Text = item.RespondentStreet.ToString();
            txtAddress.Text = item.RespondentAddressLandmark.ToString();
            txtTelephone.Text = item.RespondentTelephone.ToString();
            txtEmail.Text = item.EmailAddress;
            txtWard.SelectedItem = item.RespondentWard.ToString();
        }

        #endregion

        #region [EVENTS]

        void txtDistrict_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (txtDistrict.SelectedItem == null)
            {
                txtWard.ItemsSource = new List<String>();
                txtWard.SelectedIndex = -1;
            }
            else
            {     
                var items = new List<String>();
                foreach (var row in App._listHanhChanh)
                {
                    if (row.City==ProjectItem.CityHandle && row.District == txtDistrict.SelectedItem.ToString())
                    {
                        if (!items.Contains(row.Ward))
                            items.Add(row.Ward);
                    }
                }
                txtWard.ItemsSource = items;
                txtWard.SelectedIndex = -1;
            }
        }

        public async void btnSave_Clicked(object sender, EventArgs args)
        {
            if (ValidateForm().Result == false)
            {
                return;
            }
            RealmHelper.realm.Write(() =>
            {
                item.GreenID = txtGreenID.Text.Trim();
                if (txtEmail.Text == null)
                    item.EmailAddress = "";
                else
                {
                    item.EmailAddress = txtEmail.Text.Trim();
                }
                item.RespondentFullName = txtFullName.Text.Trim().ToUpper();
                item.RespondentYoB = Convert.ToInt32(txtYoB.SelectedItem.ToString());
                item.RespondentGender = txtGender.SelectedItem.ToString();
                item.RespondentCity = ProjectItem.CityHandle;
                item.RespondentDistrict = txtDistrict.SelectedItem.ToString().ToUpper().Trim();
                item.RespondentWard = txtWard.SelectedItem.ToString().ToUpper().Trim();
                item.RespondentStreet = txtStreet.Text.Trim().ToUpper().Trim();
                item.RespondentAddressLandmark = txtAddress.Text.Trim().ToUpper().Trim();
                item.CompletedOn = DateTime.Now.ToSafeString();
                item.RespondentTelephone = txtTelephone.Text.Trim();
            });
           
            
            //await App.dbInterviewTime.InsertItemAsync(interviewTime);

            var page = new InterviewPage(ProjectItem, item, finalQuestionList);
            await page.Process();
            Application.Current.MainPage = page;
        }

        public async void btnBack_Clicked(object sender, EventArgs args)
        {
            var page = new SurveyHomePage(ProjectItem);
            await page.Process();
            Application.Current.MainPage = page;
        }

        #endregion






    }
}