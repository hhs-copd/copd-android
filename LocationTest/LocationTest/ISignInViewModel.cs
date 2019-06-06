using System;
using System.ComponentModel;
using System.Windows.Input;

namespace LocationTest
{
    public interface ISignInViewModel : INotifyPropertyChanged
    {
        string WelcomeMessage { get; set; }

        bool IsNotLoggedIn { get; set; }

        ICommand SignInCommand { get; set; }
    }
}
