namespace XamlBrewer.Universal.OneDriveApi.ViewModels
{
    using Mvvm;
    using OneDrive;
    using Services;
    using System.Windows.Input;

    class MainViewModel : BindableBase
    {


        public ICommand InitializeCommand
        {
            get { return new RelayCommand(this.Initialize_Executed); }
        }

        private async void Initialize_Executed()
        {
            await MyOneDrive.Login();
        }
    }
}
