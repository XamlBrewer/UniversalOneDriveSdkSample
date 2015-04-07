namespace XamlBrewer.Universal.OneDriveApi.Services
{
    using OneDrive;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the user's OneDrive.
    /// </summary>
    public static class MyOneDrive
    {
        private static string Token = string.Empty;

        private static ODConnection Connection { get; set; }

        public static ODItem WorkingFolder { get; private set; }

        /// <summary>
        /// Login and create the connection, if necessary.
        /// </summary>
        public static async Task Login()
        {
            if (Connection == null || !LiveOAuth.IsSignedIn)
            {
                // Get an OAuth2 access token through REST.           
                var token = await LiveOAuth.GetAuthToken();

                // Initialize connection
                Connection = new ODConnection("https://api.onedrive.com/v1.0", new OneDriveSdkAuthenticationInfo(token));
            }
        }

        /// <summary>
        /// Sign out. No effect in universal app.
        /// </summary>
        public static async Task Logout()
        {
            await LiveOAuth.SignOut();
        }

        /// <summary>
        /// Creates (if necessary) and returns a Working folder.
        /// </summary>
        public static async Task<ODItem> SetWorkingFolder(string folderName)
        {
            EnsureConnection();

            // Connect to OneDrive root.
            var rootFolder = await Connection.GetRootItemAsync(ItemRetrievalOptions.DefaultWithChildren);

            // Search working folder
            var existingItem = (from f in rootFolder.Children
                                where f.Name == folderName
                                select f).FirstOrDefault();

            if (existingItem != null)
            {
                if (existingItem.Folder == null)
                {
                    // There is already a file with this name.
                    throw new Exception("There is already a file with this name.");
                }
                else
                {
                    // The folder already exists.
                    WorkingFolder = existingItem;
                    return WorkingFolder;
                }
            }

            var newFolder = await Connection.CreateFolderAsync(rootFolder.ItemReference(), folderName);
            WorkingFolder = newFolder;
            return WorkingFolder;
        }

        /// <summary>
        /// Saves a file in the specified folder.
        /// </summary>
        public static async Task<ODItem> SaveFile(ODItem parentItem, string filename, Stream fileContent)
        {
            EnsureConnection();

            return await Connection.PutNewFileToParentItemAsync(parentItem.ItemReference(), filename, fileContent, ItemUploadOptions.Default);
        }

        /// <summary>
        /// Deletes a file or a folder.
        /// </summary>
        public static async Task<bool> DeleteItem(ODItem item)
        {
            EnsureConnection();

            return await Connection.DeleteItemAsync(item.ItemReference(), ItemDeleteOptions.Default);
        }

        /// <summary>
        /// Fetches the files in a folder.
        /// </summary>
        public static async Task<IEnumerable<ODItem>> GetFiles(ODItem parentItem)
        {
            EnsureConnection();

            List<ODItem> result = new List<ODItem>();
            var page = await parentItem.PagedChildrenCollectionAsync(Connection, ChildrenRetrievalOptions.Default);

            var list = from f in page.Collection
                       where f.File != null
                       select f;

            result.AddRange(list);

            while (page.MoreItemsAvailable())
            {
                list = from f in page.Collection
                       where f.File != null
                       select f;

                result.AddRange(list);

                page = await page.GetNextPage(Connection);
            }

            return result;
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        public static async Task<Stream> DownloadFile(ODItem item)
        {
            EnsureConnection();

            return await item.GetContentStreamAsync(Connection, StreamDownloadOptions.Default);
        }

        /// <summary>
        /// Returns the list of changes to the working folder since the previous call.
        /// </summary>
        /// <param name="reset">Whether or not to reset the token.</param>
        /// <remarks>In the first call and whenever the token is reset, the whole folder's content is returned.</remarks>
        public static async Task<ODViewChangesResult> ViewChanges(bool reset = false)
        {
            EnsureConnection();

            if (reset)
            {
                Token = null;
            }

            var options = string.IsNullOrEmpty(Token) ? ViewChangesOptions.Default : new ViewChangesOptions() { StartingToken = Token };

            var result = await Connection.ViewChangesAsync(WorkingFolder.ItemReference(), options);
            Token = result.NextToken;

            return result;
        }

        /// <summary>
        /// Verifies if the user is authenticated and connected.
        /// </summary>
        private static void EnsureConnection()
        {
            if (Connection == null || !LiveOAuth.IsSignedIn)
            {
                throw new Exception("You're not logged in.");
            }
        }
    }
}
