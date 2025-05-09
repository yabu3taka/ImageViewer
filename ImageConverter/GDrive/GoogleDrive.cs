using System;
using System.Collections.Generic;
using System.Linq;

using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using ImageConverter.Util;

namespace ImageConverter.GDrive
{
    class GoogleDrive(DriveService drive)
    {
        private const string FOLDER_MIME = "application/vnd.google-apps.folder";
        private const string COMMON_FIELDS = "files(id, name, modifiedTime, size)";

        private readonly DriveService _drive = drive;

        private static string GetMimeCond(bool findFolder)
        {
            if (findFolder)
            {
                return $"mimeType = '{FOLDER_MIME}'";
            }
            else
            {
                return $"mimeType != '{FOLDER_MIME}'";
            }
        }

        #region GetFileInfo/GetFolderInfo
        private File GetFileInfoInternal(GoogleDriveFolder folder, string name, bool? findFolder)
        {
            string cond = findFolder is null ? "" : $"{GetMimeCond(findFolder.Value)} and ";

            var request = _drive.Files.List();
            request.Q = $"{cond} name = '{name}' and '{folder.Id}' in parents and trashed = false";
            request.Spaces = "drive";
            request.Fields = COMMON_FIELDS;
            request.PageSize = 1;

            IList<File> list = request.Execute().Files;
            if (list.Count <= 0)
            {
                return null;
            }

            SimpleLogUtil.D(GetType(), @"GetFileInfo {0}\{1} => {2}", folder.Id, name, list[0].Id);
            return list[0];
        }

        public GoogleDriveFolder GetRoot()
        {
            return new(_drive.Files.Get("root").Execute());
        }

        public GoogleDriveFile GetFileInfo(string name, GoogleDriveFolder folder = null)
        {
            folder ??= GetRoot();
            return new GoogleDriveFile(GetFileInfoInternal(folder, name, false));
        }

        public GoogleDriveFolder GetFolderInfo(string name, GoogleDriveFolder folder = null)
        {
            folder ??= GetRoot();
            return new GoogleDriveFolder(GetFileInfoInternal(folder, name, true));
        }
        #endregion

        #region GetFileList/GetFolderList
        private IList<File> GetFileListInternal(GoogleDriveFolder folder, bool findFolder)
        {
            folder ??= GetRoot();
            SimpleLogUtil.D(GetType(), @"GetFileListInternal {0}", folder.Id);

            var request = _drive.Files.List();
            request.Q = $"{GetMimeCond(findFolder)} and '{folder.Id}' in parents and trashed = false";
            request.Spaces = "drive";
            request.Fields = COMMON_FIELDS;
            request.PageSize = 1000;
            return request.Execute().Files;
        }

        public IReadOnlyList<GoogleDriveFile> GetFileList(GoogleDriveFolder folder = null)
        {
            return GetFileListInternal(folder, false).Select((f) => new GoogleDriveFile(f)).ToList();
        }

        public IReadOnlyList<GoogleDriveFolder> GetFolderList(GoogleDriveFolder folder = null)
        {
            return GetFileListInternal(folder, true).Select((f) => new GoogleDriveFolder(f)).ToList();
        }
        #endregion

        #region Manipulation
        public GoogleDriveFolder CreateFolder(GoogleDriveFolder parent, string folderName)
        {
            SimpleLogUtil.D(GetType(), @"CreateFolder {0}\{1}", parent.Id, folderName);

            File fileMetadata = new()
            {
                Parents = [parent.Id],
                Name = folderName,
                MimeType = FOLDER_MIME
            };

            var request = _drive.Files.Create(fileMetadata);
            request.Fields = "id";
            return new(request.Execute());
        }

        public GoogleDriveFile UploadFile(GoogleDriveFolder parent, string fileName, System.IO.Stream stream, string mimeType)
        {
            SimpleLogUtil.D(GetType(), @"UploadFile {0}\{1}", parent.Id, fileName);

            File curFile = GetFileInfoInternal(parent, fileName, null);
            if (curFile is not null)
            {
                SimpleLogUtil.D(GetType(), @"UploadFile - Update {0}", curFile.Id);
                Delete(new GoogleDriveFile(curFile));
            }

            var fileMetadata = new File()
            {
                Parents = [parent.Id],
                Name = fileName
            };

            var request = _drive.Files.Create(fileMetadata, stream, mimeType);
            request.Fields = "id, parents";
            request.Upload();
            return new(request.ResponseBody);
        }

        public void Delete(GoogleDriveObject file)
        {
            SimpleLogUtil.D(GetType(), @"DeleteFile {0}", file.Id);

            _drive.Files.Delete(file.Id).Execute();
        }

        public void Rename(GoogleDriveObject file, string newName)
        {
            SimpleLogUtil.D(GetType(), @"RenameFile {0} {1}", file.Id, newName);

            File body = new()
            {
                Name = newName
            };
            _drive.Files.Update(body, file.Id).Execute();
        }
        #endregion
    }

    #region Content
    public abstract class GoogleDriveObject(File file)
    {
        protected readonly File _file = file;
        public string Id => _file.Id;
        public string Name => _file.Name;
    }

    public class GoogleDriveFile(File file) : GoogleDriveObject(file)
    {
        public long Length => _file.Size.Value;
    }

    public class GoogleDriveFolder(File file) : GoogleDriveObject(file)
    {
    }

    class GoogleDriveResult(GoogleDrive drive, GoogleDriveFolder folder)
    {
        private readonly GoogleDrive _drive = drive;
        private readonly GoogleDriveFolder _folder = folder;

        public GoogleDriveFolder Me => _folder;

        private IReadOnlyList<GoogleDriveFile> _filesResult = null;
        public IReadOnlyList<GoogleDriveFile> Files
        {
            get
            {
                _filesResult ??= _drive.GetFileList(_folder);
                return _filesResult;
            }
        }

        private IReadOnlyList<GoogleDriveFolder> _foldersResult = null;
        public IReadOnlyList<GoogleDriveFolder> Folders
        {
            get
            {
                _foldersResult ??= _drive.GetFolderList(_folder);
                return _foldersResult;
            }
        }

        public static bool EqualsFileName(string f1, string f2)
        {
            return string.Equals(f1, f2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainExt(string name, string ext)
        {
            return name.EndsWith(ext, StringComparison.OrdinalIgnoreCase);
        }

        public GoogleDriveFile FindFile(string name)
        {
            return Files.FirstOrDefault(file => EqualsFileName(file.Name, name));
        }

        public IEnumerable<GoogleDriveFile> FindFilesExt(string ext)
        {
            return Files.Where(file => ContainExt(file.Name, ext));
        }

        public GoogleDriveFolder FindFolder(string name)
        {
            return Folders.FirstOrDefault(file => EqualsFileName(file.Name, name));
        }

        public GoogleDriveFolder FirstFolder()
        {
            return Folders.FirstOrDefault();
        }

        public GoogleDriveObject FirstContent()
        {
            return (GoogleDriveObject)Files.FirstOrDefault() ?? (GoogleDriveObject)Folders.FirstOrDefault();
        }
    }
    #endregion
}
