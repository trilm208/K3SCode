using System;
using Xamarin.Forms;
using K3S.Controls;
using K3S;

namespace K3S
{
	public class SwitcherPage : ContentPage
	{
		public SwitcherPage()
		{
			Title = "Pager Layout w/ Indicators";

			var none = new Button {
				HorizontalOptions = LayoutOptions.Center,
				Text = "No pager indicator",
				Command = new Command(async (obj) => await Navigation.PushAsync(new MainPage(CarouselLayout.IndicatorStyleEnum.None)))
			};
			var dots = new Button {
				HorizontalOptions = LayoutOptions.Center,
				Text = "Dots",
				Command = new Command(async (obj) => await Navigation.PushAsync(new MainPage(CarouselLayout.IndicatorStyleEnum.Dots)))
			};
			var tabs = new Button {
				HorizontalOptions = LayoutOptions.Center,
				Text = "Tabs",
				Command = new Command(async (obj) => await Navigation.PushAsync(new MainPage(CarouselLayout.IndicatorStyleEnum.Tabs)))
			};
			Content = new StackLayout {
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.Center,
				Spacing = 20,
				Children = {
					none,
					dots,
					tabs
				}
			};
		}
	}
}

