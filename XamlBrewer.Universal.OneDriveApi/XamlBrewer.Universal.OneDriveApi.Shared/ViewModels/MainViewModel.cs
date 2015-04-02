namespace XamlBrewer.Universal.OneDriveApi.ViewModels
{
    using Mvvm;
    using OneDrive;
    using Services;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Input;

    class MainViewModel : BindableBase
    {
        private IEnumerable<ODItem> files;
        private ODItem selectedFile;
        private string selectedName;
        private string selectedContent;
        private IEnumerable<ODItem> changes;
        private bool isBusy;

        // TODO: introduce WorkingFolder

        public IEnumerable<ODItem> Files
        {
            get { return files; }
            set { this.SetProperty(ref files, value); }
        }

        public ODItem SelectedFile
        {
            get { return selectedFile; }
            set
            {
                this.SetProperty(ref selectedFile, value);
                if (this.selectedFile == null)
                {
                    this.SelectedName = string.Empty;
                    this.SelectedContent = string.Empty;
                }
                else
                {
                    this.SelectedName = this.selectedFile.Name;
                    this.DownloadCommand.Execute(null);
                }
            }
        }

        public string SelectedName
        {
            get { return selectedName; }
            set { this.SetProperty(ref selectedName, value); }
        }

        public string SelectedContent
        {
            get { return selectedContent; }
            set { this.SetProperty(ref selectedContent, value); }
        }

        public IEnumerable<ODItem> Changes
        {
            get { return this.changes; }
            set { this.SetProperty(ref this.changes, value); }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set { this.SetProperty(ref isBusy, value); }
        }

        public ICommand LoginCommand
        {
            get { return new RelayCommand(this.Login_Executed); }
        }

        public ICommand LogoutCommand
        {
            get { return new RelayCommand(this.Logout_Executed); }
        }

        public ICommand CreateAssetsCommand
        {
            get { return new RelayCommand(this.CreateAssets_Executed); }
        }

        public ICommand BrowseCommand
        {
            get { return new RelayCommand(this.Browse_Executed); }
        }

        public ICommand DownloadCommand
        {
            get { return new RelayCommand(this.Download_Executed); }
        }

        public ICommand UploadCommand
        {
            get { return new RelayCommand(this.Upload_Executed); }
        }

        public ICommand ChangesCommand
        {
            get { return new RelayCommand(this.Changes_Executed); }
        }

        private async void Login_Executed()
        {
            this.IsBusy = true;

            // Login
            await MyOneDrive.Login();

            // Connect to root folder
            await MyOneDrive.SetRootFolderAsCurrent();

            this.IsBusy = false;
        }

        private async void Logout_Executed()
        {
            // Logout
            await MyOneDrive.Logout();
        }

        private async void CreateAssets_Executed()
        {
            this.IsBusy = true;

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

            this.IsBusy = false;
        }

        private async void Browse_Executed()
        {
            this.IsBusy = true;

            // Connect to root folder
            await MyOneDrive.SetRootFolderAsCurrent();

            // Connect to working folder (TODO: refactor API)
            await MyOneDrive.CreateChildFolderInCurrentFolder("OneDrive SDK Test");

            // Get files
            this.Files = await MyOneDrive.GetFilesFromCurrentFolder();

            this.IsBusy = false;
        }

        private async void Download_Executed()
        {
            this.IsBusy = true;

            var stream = await MyOneDrive.DownloadFile(this.SelectedFile);
            this.SelectedContent = stream.AsString();

            this.isBusy = false;
        }

        private async void Upload_Executed()
        {
            this.IsBusy = true;

            // Connect to root folder
            await MyOneDrive.SetRootFolderAsCurrent();

            // Create Assets (TODO: change api)
            // This is a temporary hack to connect to the working folder
            await MyOneDrive.CreateChildFolderInCurrentFolder("OneDrive SDK Test");

            await MyOneDrive.PutNewFileToParentItemAsync(
                MyOneDrive.CurrentFolder.ItemReference(),
                this.selectedName,
                this.selectedContent.AsStream(),
                ItemUploadOptions.Default);

            this.IsBusy = false;
        }

        private async void Changes_Executed()
        {
            this.IsBusy = true;

            // Connect to root folder
            await MyOneDrive.SetRootFolderAsCurrent();

            // Create Assets (TODO: change api)
            // This is a temporary hack to connect to the working folder
            await MyOneDrive.CreateChildFolderInCurrentFolder("OneDrive SDK Test");

            var changesResult = await MyOneDrive.ViewChangesAsync();

            foreach (var item in changesResult.Collection)
            {
                Debug.WriteLine(item.Name + ":");
                if (item.Deleted != null)
                {
                    Debug.WriteLine("Deleted: " + item.Deleted.IsDeleted);
                }

                Debug.WriteLine("Modified or added at " + item.LastModifiedDateTime.ToLocalTime());
            }

            this.Changes = changesResult.Collection;

            this.IsBusy = false;
        }
    }
}
