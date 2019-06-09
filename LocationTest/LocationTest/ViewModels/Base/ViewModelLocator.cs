using System;
using System.Collections.Generic;
using System.Text;
using LocationTest.Services.Navigation;
using Unity;

namespace LocationTest.ViewModels.Base
{
    public class ViewModelLocator
    {
        readonly IUnityContainer _container;

        public ViewModelLocator()
        {
            _container = new UnityContainer();

            // ViewModels
            
            _container.RegisterType<LinePlotViewModel>();

            // Services     
            _container.RegisterType<INavigationService, NavigationService>();
        }

        public LinePlotViewModel LineViewModel
        {
            get { return _container.Resolve<LinePlotViewModel>(); }
        }
    }
    }
