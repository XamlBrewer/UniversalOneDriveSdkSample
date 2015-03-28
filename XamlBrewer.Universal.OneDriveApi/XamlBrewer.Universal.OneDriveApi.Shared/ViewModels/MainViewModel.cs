namespace XamlBrewer.Universal.OneDriveApi.ViewModels
{
    using Mvvm;
    using OneDrive;
    using Services;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Windows.Input;

    class MainViewModel : BindableBase
    {
        private IEnumerable<ODItem> files;
        private ODItem selectedFile;

        public IEnumerable<ODItem> Files
        {
            get { return files; }
            set { this.SetProperty(ref files, value); }
        }

        public ODItem SelectedFile
        {
            get { return selectedFile; }
            set { this.SetProperty(ref selectedFile, value); }
        }

        public ICommand InitializeCommand
        {
            get { return new RelayCommand(this.Initialize_Executed); }
        }

        public ICommand CreateAssetsCommand
        {
            get { return new RelayCommand(this.CreateAssets_Executed); }
        }

        public ICommand BrowseCommand
        {
            get { return new RelayCommand(this.Browse_Executed); }
        }

        private async void Initialize_Executed()
        {
            // Login
            await MyOneDrive.Login();

            // Connect to root folder
            await MyOneDrive.SetRootFolderAsCurrent();
        }

        private async void CreateAssets_Executed()
        {
            // Connect to root folder
            await MyOneDrive.SetRootFolderAsCurrent();

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

        private async void Browse_Executed()
        {
            // Connect to root folder
            await MyOneDrive.SetRootFolderAsCurrent();

            // Connect to working folder (TODO: refactor API)
            await MyOneDrive.CreateChildFolderInCurrentFolder("OneDrive SDK Test");

            // Get files
            this.Files = await MyOneDrive.GetFilesFromCurrentFolder();
        }
    }
}
