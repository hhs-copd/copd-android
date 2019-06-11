using LocationTest.ViewModels;
using LocationTest.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LocationTest.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly IDictionary<Type, Type> viewModelRouting = new Dictionary<Type, Type>()
        {
            { typeof(LinePlotViewModel), typeof(LinePlotView) },
        };

        public void NavigateTo<TDestinationViewModel>(object navigationContext = null)
        {
            Type pageType = this.viewModelRouting[typeof(TDestinationViewModel)];

            if (Activator.CreateInstance(pageType, navigationContext) is Page page)
            {
                Application.Current.MainPage.Navigation.PushAsync(page);
            }
        }

        public void NavigateTo(Type destinationType, object navigationContext = null)
        {
            Type pageType = this.viewModelRouting[destinationType];

            if (Activator.CreateInstance(pageType, navigationContext) is Page page)
            {
                Application.Current.MainPage.Navigation.PushAsync(page);
            }
        }

        public void NavigateBack()
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}