namespace XamlBrewer.Universal.OneDriveApi.Services
{
    using OneDrive;
    using System;
    using System.Threading.Tasks;

    public class OneDriveSdkAuthenticationInfo : IAuthenticationInfo
    {
        private string accessToken;

        public OneDriveSdkAuthenticationInfo(string token)
        {
            this.accessToken = token;
        }

        public string AccessToken
        {
            get { return this.accessToken; }
        }

        public string RefreshToken
        {
            get { throw new NotImplementedException(); }
        }

        public string TokenType
        {
            get { return "Bearer"; }
        }

        public DateTimeOffset TokenExpiration
        {
            get { return new DateTimeOffset(DateTime.MaxValue); }
        }

        public Task<bool> RefreshAccessTokenAsync()
        {
            throw new NotImplementedException();
        }

        public string AuthorizationHeaderValue
        {
            get { return string.Concat(TokenType, " ", this.accessToken); }
        }
    }
}
