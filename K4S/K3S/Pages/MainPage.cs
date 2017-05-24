using System;
using Xamarin.Forms;
using K3S.Controls;
using CustomLayouts.ViewModels;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Extensions;

namespace K3S
{
	public class MainPage : ContentPage
	{
		View _tabs;
		RelativeLayout relativeLayout;
		CarouselLayout.IndicatorStyleEnum _indicatorStyle;
		SwitcherPageViewModel viewModel;

		public MainPage(CarouselLayout.IndicatorStyleEnum indicatorStyle)
		{
            try
            {
                var newSurveyArea = new NewSurveyView
                {
                    HeightRequest = 60
                };
                var label = new Label
                {
                    FontSize = 30,
                    Text = "Interviewed List"
                };
              
                var stack = new StackLayout();
               
                _indicatorStyle = indicatorStyle;

                viewModel = new SwitcherPageViewModel();
                BindingContext = viewModel;

                Title = _indicatorStyle.ToString();

                relativeLayout = new RelativeLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

               
                var pagesCarousel = CreatePagesCarousel();
                var dots = CreatePagerIndicatorContainer();
                _tabs = CreateTabs();

                switch (pagesCarousel.IndicatorStyle)
                {
                    case CarouselLayout.IndicatorStyleEnum.Tabs:
                        var newButtonHeight = 200;
                       var tabsHeight  = 18;

                        relativeLayout.Children.Add(newSurveyArea,
                          Constraint.Constant(0),
                           Constraint.Constant(0),
                          Constraint.RelativeToParent(parent => parent.Width),
                          Constraint.Constant(tabsHeight)
                      );

                          relativeLayout.Children.Add(_tabs,
                          Constraint.Constant(0),
                          Constraint.Constant(newButtonHeight+20),
                          Constraint.RelativeToParent(parent => parent.Width),
                          Constraint.Constant(tabsHeight)
                      );
                       
                        relativeLayout.Children.Add(pagesCarousel,
                                                     Constraint.Constant(0),
                                                     Constraint.Constant(newButtonHeight+ tabsHeight + 45),

                           Constraint.RelativeToParent((parent) => { return parent.Width; }),
                           Constraint.RelativeToView(_tabs, (parent, sibling) => { return parent.Height - tabsHeight; })
                       );
                        break;
                   
                }

                Content = relativeLayout;
            }
            catch (Exception ex)
            {
                UI.ShowError(ex.Message);
            }
		}         
        CarouselLayout CreatePagesCarousel ()
		{
			var carousel = new CarouselLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				IndicatorStyle = _indicatorStyle,
				ItemTemplate = new DataTemplate(typeof(HomeView))
			};

			carousel.SetBinding(CarouselLayout.ItemsSourceProperty, "Pages");
			carousel.SetBinding(CarouselLayout.SelectedItemProperty, "CurrentPage", BindingMode.TwoWay);

			return carousel;
		}

		View CreatePagerIndicatorContainer()
		{
			return new StackLayout {
				Children = { CreatePagerIndicators() }
			};
		}

		View CreatePagerIndicators()
		{
			var pagerIndicator = new PagerIndicatorDots() { DotSize = 5, DotColor = Color.Black };
			pagerIndicator.SetBinding (PagerIndicatorDots.ItemsSourceProperty, "Pages");
			pagerIndicator.SetBinding (PagerIndicatorDots.SelectedItemProperty, "CurrentPage");
			return pagerIndicator;
		}

		View CreateTabsContainer()
		{
			return new StackLayout {
				Children = { CreateTabs() }
			};
		}

		View CreateTabs()
		{
			var pagerIndicator = new PagerIndicatorTabs() { HorizontalOptions = LayoutOptions.CenterAndExpand };
			pagerIndicator.RowDefinitions.Add(new RowDefinition() { Height = 50 });
			pagerIndicator.SetBinding (PagerIndicatorTabs.ItemsSourceProperty, "Pages");
			pagerIndicator.SetBinding (PagerIndicatorTabs.SelectedItemProperty, "CurrentPage");

			return pagerIndicator;
		}
	}

	public class SpacingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var items = value as IEnumerable<HomeViewModel>;

			var collection = new ColumnDefinitionCollection();
			foreach(var item in items)
			{
				collection.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			}
			return collection;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

