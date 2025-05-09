using System;
using System.IO;
using System.Reflection;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using ImageConverter.Util;

namespace ImageConverter.GDrive
{
    class GoogleDriveManager
    {
        private static readonly string[] Scopes = [DriveService.Scope.Drive];
        private const string ApplicationName = "Drive API .NET Quickstart";

        public static GoogleDrive GetDrive()
        {
            try
            {
                Assembly myAssembly = Assembly.GetEntryAssembly();
                string path = Path.GetDirectoryName(myAssembly.Location);

                string credFile = Path.Combine(path, "credentials.json");
                string credPath = Path.Combine(path, "token");

                UserCredential credential;
                using (var stream = new FileStream(credFile, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                return new GoogleDrive(service);
            }
            catch (Exception ex)
            {
                SimpleLogUtil.Ex(typeof(GoogleDriveManager), @"GetDrive", ex);
                throw;
            }
        }
    }
}
