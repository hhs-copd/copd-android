using LocationTest.Services.Navigation;
using Unity;

namespace LocationTest.ViewModels.Base
{
    public class ViewModelLocator
    {
        private readonly IUnityContainer _container;

        public ViewModelLocator()
        {
            this._container = new UnityContainer();

            // ViewModels

            this._container.RegisterType<LinePlotViewModel>();

            // Services     
            this._container.RegisterType<INavigationService, NavigationService>();
        }

        public LinePlotViewModel LineViewModel => this._container.Resolve<LinePlotViewModel>();
    }
}
