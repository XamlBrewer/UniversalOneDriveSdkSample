namespace XamlBrewer.Universal.OneDriveApi.ViewModels
{
    using Mvvm;
    using OneDrive;
    using Services;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Windows.Input;

    class MainViewModel : BindableBase
    {
        public ICommand InitializeCommand
        {
            get { return new RelayCommand(this.Initialize_Executed); }
        }

        private async void Initialize_Executed()
        {
            // Login
            await MyOneDrive.Login();

            // Create Assets
            await MyOneDrive.CreateChildFolderInCurrentFolder("OneDrive SDK Test");

            for (int i = 1; i < 9; i++)
            {
                Debug.WriteLine("Creating file {0}", i);
                await MyOneDrive.PutNewFileToParentItemAsync(
                    MyOneDrive.CurrentFolder.ItemReference(),
                    string.Format("Sample file {0}.txt", i),
                    string.Format("This is the content of sample file {0}", i).AsStream(),
                    ItemUploadOptions.Default);
            }
        }
    }
}
