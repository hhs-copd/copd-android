using LocationTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocationTest.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LinePlotView : ContentPage
	{
        public object Parameter { get; set; }

        public LinePlotView(object parameter)
        {
            InitializeComponent();

            Parameter = parameter;

            BindingContext = App.Locator.LineViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var linePlotViewModel = BindingContext as LinePlotViewModel;
            if (linePlotViewModel != null) linePlotViewModel.OnAppearing(Parameter);
        }
    }
}
