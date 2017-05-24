using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using MyDependencyServices;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Acr.UserDialogs;
using DataAccess;

namespace K3S
{
	public partial class LoginPopupPage : Rg.Plugins.Popup.Pages.PopupPage
	{
		ClientServices Services;
		public event EventHandler Authenticated;
		public LoginPopupPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			FrameContainer.HeightRequest = -1;

			CloseImage.Rotation = 30;
			CloseImage.Scale = 0.3;
			CloseImage.Opacity = 0;

			LoginButton.Scale = 0.3;
			LoginButton.Opacity = 0;

			UsernameEntry.TranslationX = PasswordEntry.TranslationX = -10;
			UsernameEntry.Opacity = PasswordEntry.Opacity = 0;
		}


		protected async override Task OnAppearingAnimationEnd()
		{
			var translateLength = 400u;
			await Task.WhenAll(
				UsernameEntry.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength),
				UsernameEntry.FadeTo(1),
				(new Func<Task>(async () =>
				{
					await Task.Delay(200);
					await Task.WhenAll(
						PasswordEntry.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength),
						PasswordEntry.FadeTo(1));

				}))());

			await Task.WhenAll(
				CloseImage.FadeTo(1),
				CloseImage.ScaleTo(1, easing: Easing.SpringOut),
				CloseImage.RotateTo(0),
				LoginButton.ScaleTo(1),
				LoginButton.FadeTo(1));
		}

		protected async override Task OnDisappearingAnimationBegin()
		{
			var taskSource = new TaskCompletionSource<bool>();

			var currentHeight = FrameContainer.Height;

			await Task.WhenAll(
				UsernameEntry.FadeTo(0),
				PasswordEntry.FadeTo(0),
				LoginButton.FadeTo(0));

			FrameContainer.Animate("HideAnimation", d =>
			{
				FrameContainer.HeightRequest = d;
			},
			start: currentHeight,
			end: 170,
			finished: async (d, b) =>
			{
				await Task.Delay(300);
				taskSource.TrySetResult(true);
			});

			await taskSource.Task;
		}

		private async void OnLogin(object sender, EventArgs e)
        {

            IUserDialogs Dialog = UserDialogs.Instance;
            Dialog.ShowLoading("Đang đăng nhập");
               
         
			var username = UsernameEntry.Text.Trim();
			var password = DependencyService.Get<IMd5HashExtensions>().GetMd5Hash(PasswordEntry.Text.Trim());

			var query = DataAccess.DataQuery.Create("Security", "ws_Session_Authenticate", new { Username = username, PasswordHash = password, FacID = "1" });
			var ds = Services.Execute(query);
			if (DependencyService.Get<IDataSetExtension>().IsNull(ds) == true)
			{
                Dialog.HideLoading();
				await DisplayAlert("Lỗi đăng nhập", Services.LastError, "Thử lại");
				return;
			}
			if (ds.Tables[0].Rows[0][0].ToString().ToUpper() != "OK")
			{
                Dialog.HideLoading();
				await DisplayAlert("Lỗi đăng nhập", ds.Tables[0].Rows[0][0].ToString().ToUpper(), "Thử lại");
				return;
			}

            var id = ds.Tables[0].Rows[0].Item("UserID");
			await Save(username, password,id);
			if (Authenticated != null)
			{
				Authenticated(this, null);
			}

            Dialog.HideLoading();
			CloseAllPopup();
			
		}
		private async Task Save(string last_username, string last_password,string last_id)
		{
            await SettingHelper.SaveSetting("last_userid", last_id);
            await SettingHelper.SaveSetting("last_username", last_username);
			await SettingHelper.SaveSetting("last_password", last_password);
		}
		internal void Initialize(ClientServices services)
		{
			this.Services = services;
		}

		private void OnCloseButtonTapped(object sender, EventArgs e)
		{
			CloseAllPopup();
		}

		protected override bool OnBackgroundClicked()
		{
			CloseAllPopup();

			return false;
		}

		private async void CloseAllPopup()
		{
			await Navigation.PopAllPopupAsync();

		}
	}
}
