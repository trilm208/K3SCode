using System;
using System.Collections.Generic;
using System.Linq;
using K3S;
using Xamarin.Forms;

namespace CustomLayouts.ViewModels
{
	public class SwitcherPageViewModel : BaseViewModel
	{
		public SwitcherPageViewModel()
		{
			Pages = new List<HomeViewModel>() {
				new HomeViewModel { Title = "UnCompleted", Background = Color.White, ImageSource = "delivery_complete.png" },
				new HomeViewModel { Title = "Completed", Background = Color.Red, ImageSource = "completed_tasks.png" },
				
			};

			CurrentPage = Pages.First();
		}

		IEnumerable<HomeViewModel> _pages;
		public IEnumerable<HomeViewModel> Pages {
			get {
				return _pages;
			}
			set {
				SetObservableProperty (ref _pages, value);
				CurrentPage = Pages.FirstOrDefault ();
			}
		}

		HomeViewModel _currentPage;
		public HomeViewModel CurrentPage {
			get {
				return _currentPage;
			}
			set {
				SetObservableProperty (ref _currentPage, value);
			}
		}
	}

	public class HomeViewModel : BaseViewModel, ITabProvider
	{
		public HomeViewModel() {}

		public string Title { get; set; }
		public Color Background { get; set; }
		public string ImageSource { get; set; }
	}
}

