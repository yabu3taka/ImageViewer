using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using System.Threading;

using static Vanara.PInvoke.Ole32;
using static Vanara.PInvoke.PortableDeviceApi;
using PropertyKey = Vanara.PInvoke.Ole32.PROPERTYKEY;
using PropVariant = Vanara.PInvoke.Ole32.PROPVARIANT;

using ImageConverter.Util;

namespace ImageConverter.PortableDevices
{
    public class PortableDevice(string deviceId)
    {
        public string DeviceId { get; } = deviceId;

        public PortableDeviceConnector Connect()
        {
            return new(DeviceId);
        }

        public static List<PortableDevice> GetDeviceList()
        {
            List<PortableDevice> ret = [];

            IPortableDeviceManager man = new();
            man.RefreshDeviceList();

            uint count = 1;
            man.GetDevices(null, ref count);
            if (count == 0)
            {
                return ret;
            }

            var deviceIds = new string[count];
            man.GetDevices(deviceIds, ref count);
            ret.AddRange(deviceIds.Select(deviceId => new PortableDevice(deviceId)));
            return ret;
        }
    }

    public class PortableDeviceConnector : IDisposable
    {
        private readonly IPortableDevice _devClass;
        private readonly IPortableDeviceContent _content;
        private readonly IPortableDeviceProperties _properties;
        private string eventCookie;
        private readonly Callback eventCallback;

        public PortableDeviceConnector(string deviceId)
        {
            DeviceId = deviceId;

            _devClass = new();
            IPortableDeviceValues clientInfo = new();
            _devClass.Open(DeviceId, clientInfo);
            _content = _devClass.Content();
            _properties = _content.Properties();

            eventCallback = new Callback(this);
            _devClass.Advise(0, this.eventCallback, null, out this.eventCookie);
        }

        public string DeviceId { get; }

        #region Const
        public const string DEVICE_ROOT = "DEVICE";

        // DEVICE_PROPERTIES_V1 -------------------------------------------------------------
        private static readonly Guid DEVICE_PROPERTIES_V1 =
            new(0x26D4979A, 0xE643, 0x4626, 0x9E, 0x2B, 0x73, 0x6D, 0xC0, 0xC9, 0x2F, 0xDC);

        private static readonly PropertyKey DEVICE_FRIENDLY_NAME =
            new() { fmtid = DEVICE_PROPERTIES_V1, pid = 12 };

        // PROPERTY_ATTRIBUTES_V1 -------------------------------------------------------------
        private static readonly Guid PROPERTY_ATTRIBUTES_V1 =
            new(0xAB7943D8, 0x6332, 0x445F, 0xA0, 0x0D, 0x8D, 0x5E, 0xF1, 0xE9, 0x6F, 0x37);

        private static readonly PropertyKey PROPERTY_ATTRIBUTE_CAN_WRITE =
            new() { fmtid = PROPERTY_ATTRIBUTES_V1, pid = 4 };

        // OBJECT_PROPERTIES_V1 -------------------------------------------------------------
        private static readonly Guid OBJECT_PROPERTIES_V1 =
            new(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);

        public static readonly PropertyKey OBJECT_ID =
            new() { fmtid = OBJECT_PROPERTIES_V1, pid = 2 };
        public static readonly PropertyKey OBJECT_PARENT_ID =
            new() { fmtid = OBJECT_PROPERTIES_V1, pid = 3 };
        public static readonly PropertyKey OBJECT_NAME =
            new() { fmtid = OBJECT_PROPERTIES_V1, pid = 4 };
        public static readonly PropertyKey OBJECT_CONTENT_TYPE =
            new() { fmtid = OBJECT_PROPERTIES_V1, pid = 7 };
        public static readonly PropertyKey OBJECT_SIZE =
            new() { fmtid = OBJECT_PROPERTIES_V1, pid = 11 };
        public static readonly PropertyKey OBJECT_ORIGINAL_FILE_NAME =
            new() { fmtid = OBJECT_PROPERTIES_V1, pid = 12 };

        // RESOURCE_FMT -------------------------------------------------------------
        private static readonly Guid RESOURCE_FMT =
            new(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F, 0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);

        private static readonly PropertyKey RESOURCE_DEFAULT =
            new() { fmtid = RESOURCE_FMT, pid = 0 };

        // EVENT_PROPERTIES_V1 -------------------------------------------------------------
        private static readonly Guid EVENT_PROPERTIES_V1 =
            new(0x15ab1953, 0xf817, 0x4fef, 0xa9, 0x21, 0x56, 0x76, 0xe8, 0x38, 0xf6, 0xe0);

        private static readonly PropertyKey EVENT_PARAMETER_EVENT_ID =
            new() { fmtid = EVENT_PROPERTIES_V1, pid = 3 };
        public static readonly PropertyKey EVENT_PARAMETER_OPERATION_STATE =
            new() { fmtid = EVENT_PROPERTIES_V1, pid = 4 };
        public static readonly PropertyKey EVENT_PARAMETER_OPERATION_PROGRESS =
            new() { fmtid = EVENT_PROPERTIES_V1, pid = 5 };
        public static readonly PropertyKey EVENT_PARAMETER_OBJECT_PARENT_PERSISTENT_UNIQUE_ID =
            new() { fmtid = EVENT_PROPERTIES_V1, pid = 6 };
        public static readonly PropertyKey EVENT_PARAMETER_OBJECT_CREATION_COOKIE =
            new() { fmtid = EVENT_PROPERTIES_V1, pid = 7 };
        public static readonly PropertyKey EVENT_PARAMETER_CHILD_HIERARCHY_CHANGED =
            new() { fmtid = EVENT_PROPERTIES_V1, pid = 8 };
        #endregion

        #region Contection
        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        public void Disconnect()
        {
            if (!string.IsNullOrEmpty(this.eventCookie))
            {
                this._devClass.Unadvise(this.eventCookie);
                this.eventCookie = null;
            }
            _devClass.Close();
        }
        #endregion

        #region Property
        public string FriendlyName
        {
            get
            {
                var propertyValues = _properties.GetValues(DEVICE_ROOT, null);

                var property = DEVICE_FRIENDLY_NAME;
                string propertyValue = propertyValues.GetStringValue(property);
                return propertyValue;
            }
        }
        #endregion

        #region GetContents/Filter
        public enum FilterResult
        {
            EXCLUDE = 0,
            INCLUDE = 1,
            STOP = 4
        }

        public delegate FilterResult Filter(PortableDeviceObject obj);

        public Filter CreateExtFilter(string ext)
        {
            FilterResult filter(PortableDeviceObject obj)
            {
                return PortableDeviceResult.ContainExt(obj.Name, ext) ? FilterResult.INCLUDE : FilterResult.EXCLUDE;
            }
            return filter;
        }

        public Filter CreatePathFilter(string name)
        {
            FilterResult filter(PortableDeviceObject obj)
            {
                if (!PortableDeviceResult.EqualsFileName(name, obj.Name))
                {
                    return FilterResult.EXCLUDE;
                }
                return FilterResult.INCLUDE | FilterResult.STOP;
            }
            return filter;
        }
        #endregion

        #region GetContents
        public PortableDeviceResult GetContents(Filter filter, PortableDeviceFolder folder = null)
        {
            folder ??= new PortableDeviceRootFolder(DeviceId);
            if (filter is null)
            {
                static FilterResult filter1(PortableDeviceObject obj)
                {
                    return FilterResult.INCLUDE;
                }
                filter = filter1;
            }
            return EnumerateContentsFirst(folder, filter);
        }

        public PortableDeviceResult GetContents(PortableDeviceFolder folder = null)
        {
            return GetContents(null, folder);
        }

        public PortableDeviceObject GetContent(string path, PortableDeviceFolder folder = null)
        {
            folder ??= new PortableDeviceRootFolder(DeviceId);

            PortableDeviceResult res = null;
            var elms = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dir in elms)
            {
                if (folder is null)
                {
                    return null;
                }
                res = EnumerateContentsFirst(folder, CreatePathFilter(dir));
                folder = res.FirstFolder();
            }
            return res.FirstContent();
        }

        public PortableDeviceFile GetFile(string path, PortableDeviceFolder folder = null)
        {
            return GetContent(path, folder) as PortableDeviceFile;
        }

        public PortableDeviceFolder GetFolder(string path, PortableDeviceFolder folder = null)
        {
            return GetContent(path, folder) as PortableDeviceFolder;
        }

        private PortableDeviceResult EnumerateContentsFirst(PortableDeviceFolder folder, Filter filter)
        {
            PortableDeviceResult result = new(folder);
            EnumerateContents(result, filter);
            return result;
        }

        private void EnumerateContents(PortableDeviceResult result, Filter filter)
        {
            SimpleLogUtil.D(GetType(), @"EnumerateContents {1}@{0}", result.Me.Id, result.Me.Name);

            var objectIds = _content.EnumObjects(0, result.Me.Id, null);
            uint fetched;
            string[] objectIdAry = new string[1];
            do
            {
                objectIds.Next(1, objectIdAry, out fetched);
                if (fetched <= 0) continue;

                var obj = WrapObject(objectIdAry[0]);
                var res = filter(obj);
                if (res.HasFlag(FilterResult.INCLUDE))
                {
                    result.Add(obj);
                }
                if (res.HasFlag(FilterResult.STOP))
                {
                    break;
                }
            } while (fetched > 0);
        }

        private static bool IsFolder(IPortableDeviceValues values)
        {
            var property = OBJECT_CONTENT_TYPE;
            Guid contentType = values.GetGuidValue(property);
            return contentType == PortableDeviceContentType.Folder || contentType == PortableDeviceContentType.FunctionalObject;
        }

        private PortableDeviceObject WrapObject(string objectId)
        {
            var keys = _properties.GetSupportedProperties(objectId);
            var values = _properties.GetValues(objectId, keys);
            if (IsFolder(values))
            {
                var property = OBJECT_NAME;
                string name = values.GetStringValue(property);
                return new PortableDeviceFolder(objectId, name);
            }
            else
            {
                var property = OBJECT_ORIGINAL_FILE_NAME;
                string name = values.GetStringValue(property);
                property = OBJECT_SIZE;
                ulong size = values.GetUnsignedLargeIntegerValue(property);
                return new PortableDeviceFile(objectId, name, size);
            }
        }
        #endregion

        #region Download
        public void DownloadFileToStream(PortableDeviceFile file, Stream targetStream)
        {
            var resources = _content.Transfer();

            SimpleLogUtil.D(GetType(), @"DownloadFileToStream {1}@{0}", file.Id, file.Name);

            var property = RESOURCE_DEFAULT;
            var wpdStream = resources.GetStream(file.Id, property, 0, out uint optimalTransferSize);
            var sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

            unsafe
            {
                const int size = 1024;
                var buffer = new byte[size];
                int bytesRead;
                do
                {
                    sourceStream.Read(buffer, size, new IntPtr(&bytesRead));
                    targetStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);
            }
        }

        public void DownloadFile(PortableDeviceFile file, string saveToPath)
        {
            var filename = Path.GetFileName(file.Name);
            using FileStream stream = new(Path.Combine(saveToPath, filename), FileMode.Create, FileAccess.Write);
            DownloadFileToStream(file, stream);
        }

        public unsafe byte[] DownloadFileToByte(PortableDeviceFile file)
        {
            using var stream = new MemoryStream();
            DownloadFileToStream(file, stream);
            return stream.ToArray();
        }

        public string DownloadFileToString(PortableDeviceFile file)
        {
            using var stream = new MemoryStream();
            DownloadFileToStream(file, stream);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        #endregion

        #region Delete
        public void DeleteContent(PortableDeviceObject obj)
        {
            DeleteInternal(obj, false);
        }

        public void DeleteFile(PortableDeviceFile file)
        {
            DeleteInternal(file, false);
        }

        public void DeleteFolder(PortableDeviceFolder file, bool recursive = true)
        {
            DeleteInternal(file, recursive);
        }

        private volatile bool _doneDelete;

        private void StartDelete()
        {
            _doneDelete = false;
            ObjectRemoved = (sender, e) =>
            {
                _doneDelete = true;
            };
        }

        private void WaitDeleteDone()
        {
            while (!_doneDelete)
            {
                Thread.Sleep(1000);
            }
        }

        private void DeleteInternal(PortableDeviceObject file, bool recursive)
        {
            GetPropVariant(file.Id, out PropVariant variant);

            IPortableDevicePropVariantCollection objectIds = new();
            objectIds.Add(variant);

            SimpleLogUtil.D(GetType(), @"DeleteInternal {1}@{0}", file.Id, file.Name);
            DELETE_OBJECT_OPTIONS option = recursive ? DELETE_OBJECT_OPTIONS.PORTABLE_DEVICE_DELETE_WITH_RECURSION : DELETE_OBJECT_OPTIONS.PORTABLE_DEVICE_DELETE_NO_RECURSION;

            StartDelete();
            _content.Delete(option, objectIds);
            WaitDeleteDone();
        }

        private static void GetPropVariant(string id, out PropVariant propvarValue)
        {
            propvarValue = new PropVariant(id, VarEnum.VT_LPWSTR);
        }
        #endregion

        #region Delete Batch
        private readonly List<PortableDeviceObject> _batchList = [];

        public void DeleteBatch(PortableDeviceObject file)
        {
            SimpleLogUtil.D(GetType(), @"DeleteBatch {1}@{0}", file.Id, file.Name);
            _batchList.Add(file);
        }

        public void DoneDelete()
        {
            SimpleLogUtil.D(GetType(), @"DoDelete {0}", _batchList.Count);
            foreach (var list in _batchList.Chunk(50))
            {
                DoDelete(list);
                SimpleLogUtil.D(GetType(), @"DoDelete Done 50");
            }
            _batchList.Clear();
        }

        private void DoDelete(IEnumerable<PortableDeviceObject> list)
        {
            IPortableDevicePropVariantCollection objectIds = new();
            foreach (var file in list)
            {
                GetPropVariant(file.Id, out PropVariant variant);
                objectIds.Add(variant);
            }

            StartDelete();
            _content.Delete(DELETE_OBJECT_OPTIONS.PORTABLE_DEVICE_DELETE_NO_RECURSION, objectIds);
            WaitDeleteDone();
        }
        #endregion

        #region Create File
        private void CreateFileFromStream(PortableDeviceFolder parent, string fileName, Stream inputStream, long len)
        {
            // Property
            IPortableDeviceValues values = new();

            var property = OBJECT_PARENT_ID;
            values.SetStringValue(property, parent.Id);

            property = OBJECT_SIZE;
            values.SetUnsignedLargeIntegerValue(property, (ulong)len);

            property = OBJECT_ORIGINAL_FILE_NAME;
            values.SetStringValue(property, fileName);

            property = OBJECT_NAME;
            values.SetStringValue(property, fileName);

            SimpleLogUtil.D(GetType(), @"CreateFileFromStream {1}@{0}\\{2} len={3}", parent.Id, parent.Name, fileName, len);

            // Create
            uint optimalTransferSizeBytes = 0;
            _content.CreateObjectWithPropertiesAndData(values, out var wpdStream, ref optimalTransferSizeBytes);
            Transfer(inputStream, wpdStream, optimalTransferSizeBytes);
        }

        private static void Transfer(Stream inputStream, IStream wpdStream, uint optimalTransferSizeBytes)
        {
            var targetStream = wpdStream;
            var buffer = new byte[optimalTransferSizeBytes];
            do
            {
                int bytesRead = inputStream.Read(buffer, 0, (int)optimalTransferSizeBytes);
                if (bytesRead == 0)
                {
                    break;
                }
                var pcbWritten = IntPtr.Zero;
                targetStream.Write(buffer, bytesRead, pcbWritten);
            } while (true);
            targetStream.Commit(0);
        }

        public void CopyFileToDevice(PortableDeviceFolder parent, string inputFile)
        {
            var fileInfo = new FileInfo(inputFile);
            using var sourceStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            CreateFileFromStream(parent, Path.GetFileName(inputFile), sourceStream, fileInfo.Length);
        }

        public void CreateFileFromMemory(PortableDeviceFolder parent, string fileName, MemoryStream inputStream)
        {
            CreateFileFromStream(parent, fileName, inputStream, inputStream.Length);
        }
        #endregion

        #region Update File
        public void UpdateFileFromStream(PortableDeviceFile file, Stream inputStream, long len)
        {
            IPortableDeviceContent2 content2 = (IPortableDeviceContent2)_content;

            // Property
            IPortableDeviceValues values = new();

            var property = OBJECT_SIZE;
            values.SetUnsignedLargeIntegerValue(property, (ulong)len);

            property = OBJECT_ORIGINAL_FILE_NAME;
            values.SetStringValue(property, file.Name);

            property = OBJECT_NAME;
            values.SetStringValue(property, file.Name);

            SimpleLogUtil.D(GetType(), @"UpdateFileFromStream {1}@{0}", file.Id, file.Name);

            // Create
            content2.UpdateObjectWithPropertiesAndData(file.Id, values, out var wpdStream, out uint optimalTransferSizeBytes);
            Transfer(inputStream, wpdStream, optimalTransferSizeBytes);
        }

        public void UpdateFileFromFile(PortableDeviceFile file, string inputFile)
        {
            var fileInfo = new FileInfo(inputFile);
            using var sourceStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            UpdateFileFromStream(file, sourceStream, fileInfo.Length);
        }

        public void UpdateFileFromMemory(PortableDeviceFile file, MemoryStream inputStream)
        {
            UpdateFileFromStream(file, inputStream, inputStream.Length);
        }
        #endregion

        #region Rename
        public bool Rename(PortableDeviceObject file, string fileName)
        {
            IPortableDeviceKeyCollection keys = _properties.GetSupportedProperties(file.Id);
            IPortableDeviceValues values = _properties.GetValues(file.Id, keys);

            var property = OBJECT_ORIGINAL_FILE_NAME;
            if (IsFolder(values))
            {
                property = OBJECT_NAME;
            }

            if (!CanWrite(file, property))
            {
                //return false;
            }

            SimpleLogUtil.D(GetType(), @"Rename {1}@{0} to {2}", file.Id, file.Name, fileName);

            IPortableDeviceValues newValues = new();
            newValues.SetStringValue(property, fileName);
            _properties.SetValues(file.Id, newValues);

            return true;
        }

        private bool CanWrite(PortableDeviceObject file, PropertyKey tp)
        {
            IPortableDeviceValues propertyValues = _properties.GetPropertyAttributes(file.Id, tp);

            var property = PROPERTY_ATTRIBUTE_CAN_WRITE;
            bool canWrite = propertyValues.GetBoolValue(property);
            return canWrite;
        }
        #endregion

        #region Create Folder
        public PortableDeviceFolder CreateFolder(PortableDeviceFolder parent, string fileName)
        {
            IPortableDeviceValues values = new();

            var property = OBJECT_PARENT_ID;
            values.SetStringValue(property, parent.Id);

            property = OBJECT_NAME;
            values.SetStringValue(property, fileName);

            property = OBJECT_CONTENT_TYPE;
            values.SetGuidValue(property, PortableDeviceContentType.Folder);

            SimpleLogUtil.D(GetType(), @"CreateFolder {1}@{0} {2}", parent.Id, parent.Name, fileName);
            string objectId = _content.CreateObjectWithPropertiesOnly(values);

            return (PortableDeviceFolder)WrapObject(objectId);
        }
        #endregion

        #region Events
        public event EventHandler<ObjectAddedEventArgs> ObjectAdded;
        public event EventHandler<PortableDeviceEventArgs> ObjectRemoved;
        public event EventHandler<PortableDeviceEventArgs> ObjectUpdated;
        public event EventHandler<PortableDeviceEventArgs> DeviceReset;
        public event EventHandler<PortableDeviceEventArgs> DeviceCapabilitiesUpdated;
        public event EventHandler<PortableDeviceEventArgs> StorageFormat;
        public event EventHandler<PortableDeviceEventArgs> ObjectTransferRequest;
        public event EventHandler<PortableDeviceEventArgs> DeviceRemoved;
        public event EventHandler<PortableDeviceEventArgs> ServiceMethodComplete;

        private class Callback(PortableDeviceConnector device) : IPortableDeviceEventCallback
        {
            public void OnEvent(IPortableDeviceValues pEventParameters)
            {
                device.CallEvent(pEventParameters);
            }
        }

        internal void CallEvent(IPortableDeviceValues eventParameters)
        {
            var propery = EVENT_PARAMETER_EVENT_ID;
            Guid eventGuid = eventParameters.GetGuidValue(propery);

            if (eventGuid == PortableDeviceEvents.ObjectAdded)
            {
                this.ObjectAdded?.Invoke(this, new ObjectAddedEventArgs(eventGuid, this, eventParameters));
            }
            else if (eventGuid == PortableDeviceEvents.ObjectRemoved)
            {
                this.ObjectRemoved?.Invoke(this, new PortableDeviceEventArgs(eventGuid, this, eventParameters));
            }
            else if (eventGuid == PortableDeviceEvents.ObjectUpdated)
            {
                this.ObjectUpdated?.Invoke(this, new PortableDeviceEventArgs(eventGuid, this, eventParameters));
            }
            else if (eventGuid == PortableDeviceEvents.DeviceReset)
            {
                this.DeviceReset?.Invoke(this, new PortableDeviceEventArgs(eventGuid, this, eventParameters));
            }
            else if (eventGuid == PortableDeviceEvents.DeviceCapabilitiesUpdated)
            {
                this.DeviceCapabilitiesUpdated?.Invoke(this, new PortableDeviceEventArgs(eventGuid, this, eventParameters));
            }
            else if (eventGuid == PortableDeviceEvents.StorageFormat)
            {
                this.StorageFormat?.Invoke(this, new PortableDeviceEventArgs(eventGuid, this, eventParameters));
            }
            else if (eventGuid == PortableDeviceEvents.ObjectTransferRequest)
            {
                this.ObjectTransferRequest?.Invoke(this, new PortableDeviceEventArgs(eventGuid, this, eventParameters));
            }
            else if (eventGuid == PortableDeviceEvents.DeviceRemoved)
            {
                this.DeviceRemoved?.Invoke(this, new PortableDeviceEventArgs(eventGuid, this, eventParameters));
            }
            else if (eventGuid == PortableDeviceEvents.ServiceMethodComplete)
            {
                this.ServiceMethodComplete?.Invoke(this, new PortableDeviceEventArgs(eventGuid, this, eventParameters));
            }
        }
        #endregion
    }

    #region Content
    public abstract class PortableDeviceObject(string id, string name)
    {
        public string Id { get; } = id;
        public string Name { get; } = name;
    }

    public class PortableDeviceFile(string id, string name, ulong size) : PortableDeviceObject(id, name)
    {
        public long Length { get; } = (long)size;
    }

    public class PortableDeviceFolder(string id, string name) : PortableDeviceObject(id, name)
    {
    }

    public class PortableDeviceRootFolder(string deviceId) :
        PortableDeviceFolder(PortableDeviceConnector.DEVICE_ROOT, "root")
    {
        public string DeviceId { get; } = deviceId;
}

    public class PortableDeviceResult(PortableDeviceFolder folder)
    {
        private readonly List<PortableDeviceFile> _files = [];
        private readonly List<PortableDeviceFolder> _folders = [];
        private readonly PortableDeviceFolder _folder = folder;

        public PortableDeviceFolder Me => _folder;
        public IReadOnlyList<PortableDeviceFile> Files => _files;
        public IReadOnlyList<PortableDeviceFolder> Folders => _folders;

        public void Add(PortableDeviceObject obj)
        {
            if (obj is PortableDeviceFile file)
            {
                _files.Add(file);
            }
            else if (obj is PortableDeviceFolder folder)
            {
                _folders.Add(folder);
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

        public PortableDeviceFile FindFile(string name)
        {
            return _files.Find(file => EqualsFileName(file.Name, name));
        }

        public IEnumerable<PortableDeviceFile> FindFilesExt(string ext)
        {
            return _files.Where(file => ContainExt(file.Name, ext));
        }

        public PortableDeviceFolder FindFolder(string name)
        {
            return _folders.Find(file => EqualsFileName(file.Name, name));
        }

        public PortableDeviceFolder FirstFolder()
        {
            return _folders.FirstOrDefault();
        }

        public PortableDeviceObject FirstContent()
        {
            return (PortableDeviceObject)_files.FirstOrDefault() ?? (PortableDeviceObject)_folders.FirstOrDefault();
        }
    }

    public static class PortableDeviceContentType
    {
        public static readonly Guid FunctionalObject =
            new(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98, 0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);
        public static readonly Guid Folder =
            new(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C, 0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
        public static readonly Guid Image =
            new(0xef2107d5, 0xa52a, 0x4243, 0xa2, 0x6b, 0x62, 0xd4, 0x17, 0x6d, 0x76, 0x03);
        public static readonly Guid Document =
            new(0x680ADF52, 0x950A, 0x4041, 0x9B, 0x41, 0x65, 0xE3, 0x93, 0x64, 0x81, 0x55);
        public static readonly Guid Contact =
            new(0xEABA8313, 0x4525, 0x4707, 0x9F, 0x0E, 0x87, 0xC6, 0x80, 0x8E, 0x94, 0x35);
        public static readonly Guid ContactGroup =
            new(0x346B8932, 0x4C36, 0x40D8, 0x94, 0x15, 0x18, 0x28, 0x29, 0x1F, 0x9D, 0xE9);
        public static readonly Guid Audio =
            new(0x4AD2C85E, 0x5E2D, 0x45E5, 0x88, 0x64, 0x4F, 0x22, 0x9E, 0x3C, 0x6C, 0xF0);
        public static readonly Guid Video =
            new(0x9261B03C, 0x3D78, 0x4519, 0x85, 0xE3, 0x02, 0xC5, 0xE1, 0xF5, 0x0B, 0xB9);
        public static readonly Guid Television =
            new(0x60A169CF, 0xF2AE, 0x4E21, 0x93, 0x75, 0x96, 0x77, 0xF1, 0x1C, 0x1C, 0x6E);
        public static readonly Guid Playlist =
            new(0x1A33F7E4, 0xAF13, 0x48F5, 0x99, 0x4E, 0x77, 0x36, 0x9D, 0xFE, 0x04, 0xA3);
        public static readonly Guid MixedContentAlbum =
            new(0x00F0C3AC, 0xA593, 0x49AC, 0x92, 0x19, 0x24, 0xAB, 0xCA, 0x5A, 0x25, 0x63);
        public static readonly Guid AudioAlbum =
            new(0xAA18737E, 0x5009, 0x48FA, 0xAE, 0x21, 0x85, 0xF2, 0x43, 0x83, 0xB4, 0xE6);
        public static readonly Guid ImageAlbum =
            new(0x75793148, 0x15F5, 0x4A30, 0xA8, 0x13, 0x54, 0xED, 0x8A, 0x37, 0xE2, 0x26);
        public static readonly Guid VideoAlbum =
            new(0x012B0DB7, 0xD4C1, 0x45D6, 0xB0, 0x81, 0x94, 0xB8, 0x77, 0x79, 0x61, 0x4F);
        public static readonly Guid Memo =
            new(0x9CD20ECF, 0x3B50, 0x414F, 0xA6, 0x41, 0xE4, 0x73, 0xFF, 0xE4, 0x57, 0x51);
        public static readonly Guid EMail =
            new(0x8038044A, 0x7E51, 0x4F8F, 0x88, 0x3D, 0x1D, 0x06, 0x23, 0xD1, 0x45, 0x33);
        public static readonly Guid Appointment =
            new(0x0FED060E, 0x8793, 0x4B1E, 0x90, 0xC9, 0x48, 0xAC, 0x38, 0x9A, 0xC6, 0x31);
        public static readonly Guid Task =
            new(0x63252F2C, 0x887F, 0x4CB6, 0xB1, 0xAC, 0xD2, 0x98, 0x55, 0xDC, 0xEF, 0x6C);
        public static readonly Guid Program =
            new(0xD269F96A, 0x247C, 0x4BFF, 0x98, 0xFB, 0x97, 0xF3, 0xC4, 0x92, 0x20, 0xE6);
        public static readonly Guid GenericFile =
            new(0x0085E0A6, 0x8D34, 0x45D7, 0xBC, 0x5C, 0x44, 0x7E, 0x59, 0xC7, 0x3D, 0x48);
        public static readonly Guid Calendar =
            new(0xA1FD5967, 0x6023, 0x49A0, 0x9D, 0xF1, 0xF8, 0x06, 0x0B, 0xE7, 0x51, 0xB0);
        public static readonly Guid GenericMessage =
            new(0xE80EAAF8, 0xB2DB, 0x4133, 0xB6, 0x7E, 0x1B, 0xEF, 0x4B, 0x4A, 0x6E, 0x5F);
        public static readonly Guid NetworkAssociation =
            new(0x031DA7EE, 0x18C8, 0x4205, 0x84, 0x7E, 0x89, 0xA1, 0x12, 0x61, 0xD0, 0xF3);
        public static readonly Guid Certificate =
            new(0xDC3876E8, 0xA948, 0x4060, 0x90, 0x50, 0xCB, 0xD7, 0x7E, 0x8A, 0x3D, 0x87);
        public static readonly Guid WirelessProfile =
            new(0x0BAC070A, 0x9F5F, 0x4DA4, 0xA8, 0xF6, 0x3D, 0xE4, 0x4D, 0x68, 0xFD, 0x6C);
        public static readonly Guid MediaCast =
            new(0x5E88B3CC, 0x3E65, 0x4E62, 0xBF, 0xFF, 0x22, 0x94, 0x95, 0x25, 0x3A, 0xB0);
        public static readonly Guid Section =
            new(0x821089F5, 0x1D91, 0x4DC9, 0xBE, 0x3C, 0xBB, 0xB1, 0xB3, 0x5B, 0x18, 0xCE);
        public static readonly Guid Unspecified =
            new(0x28D8D31E, 0x249C, 0x454E, 0xAA, 0xBC, 0x34, 0x88, 0x31, 0x68, 0xE6, 0x34);
        public static readonly Guid All =
            new(0x80E170D2, 0x1055, 0x4A3E, 0xB9, 0x52, 0x82, 0xCC, 0x4F, 0x8A, 0x86, 0x89);
    }
    #endregion

    #region Event
    public static class PortableDeviceEvents
    {
        public static readonly Guid Notification =
            new(0x2BA2E40A, 0x6B4C, 0x4295, 0xBB, 0x43, 0x26, 0x32, 0x2B, 0x99, 0xAE, 0xB2);
        public static readonly Guid ObjectAdded =
            new(0xA726DA95, 0xE207, 0x4B02, 0x8D, 0x44, 0xBE, 0xF2, 0xE8, 0x6C, 0xBF, 0xFC);
        public static readonly Guid ObjectRemoved =
            new(0xBE82AB88, 0xA52C, 0x4823, 0x96, 0xE5, 0xD0, 0x27, 0x26, 0x71, 0xFC, 0x38);
        public static readonly Guid ObjectUpdated =
            new(0x1445A759, 0x2E01, 0x485D, 0x9F, 0x27, 0xFF, 0x07, 0xDA, 0xE6, 0x97, 0xAB);
        public static readonly Guid DeviceReset =
            new(0x7755CF53, 0xC1ED, 0x44F3, 0xB5, 0xA2, 0x45, 0x1E, 0x2C, 0x37, 0x6B, 0x27);
        public static readonly Guid DeviceCapabilitiesUpdated =
            new(0x36885AA1, 0xCD54, 0x4DAA, 0xB3, 0xD0, 0xAF, 0xB3, 0xE0, 0x3F, 0x59, 0x99);
        public static readonly Guid StorageFormat =
            new(0x3782616B, 0x22BC, 0x4474, 0xA2, 0x51, 0x30, 0x70, 0xF8, 0xD3, 0x88, 0x57);
        public static readonly Guid ObjectTransferRequest =
            new(0x8D16A0A1, 0xF2C6, 0x41DA, 0x8F, 0x19, 0x5E, 0x53, 0x72, 0x1A, 0xDB, 0xF2);
        public static readonly Guid DeviceRemoved =
            new(0xE4CBCA1B, 0x6918, 0x48B9, 0x85, 0xEE, 0x02, 0xBE, 0x7C, 0x85, 0x0A, 0xF9);
        public static readonly Guid ServiceMethodComplete =
            new(0x8A33F5F8, 0x0ACC, 0x4D9B, 0x9C, 0xC4, 0x11, 0x2D, 0x35, 0x3B, 0x86, 0xCA);
    }

    public enum OperationState
    {
        Unspecified = 0,
        Started = 1,
        Running = 2,
        Paused = 3,
        Cancelled = 4,
        Finished = 5,
        Aborted = 6
    }

    public class PortableDeviceEventArgs : EventArgs
    {
        internal PortableDeviceEventArgs(Guid eventGuid, PortableDeviceConnector device, IPortableDeviceValues eventParameters)
        {
            this.Device = device;
            this.Event = eventGuid;

            eventParameters.TryGetUnsignedIntegerValue(PortableDeviceConnector.EVENT_PARAMETER_OPERATION_STATE, out uint operationState);
            this.OperationState = (OperationState)operationState;

            eventParameters.TryGetUnsignedIntegerValue(PortableDeviceConnector.EVENT_PARAMETER_OPERATION_PROGRESS, out uint operationProgress);
            this.OperationProgress = operationProgress;

            eventParameters.TryGetStringValue(PortableDeviceConnector.EVENT_PARAMETER_OBJECT_PARENT_PERSISTENT_UNIQUE_ID, out string objectParentPersistanceUniqueId);
            this.ObjectParentPersistanceUniqueId = objectParentPersistanceUniqueId;

            eventParameters.TryGetStringValue(PortableDeviceConnector.EVENT_PARAMETER_OBJECT_CREATION_COOKIE, out string objectCreationCookie);
            this.ObjectCreationCookie = objectCreationCookie;

            eventParameters.TryGetBoolValue(PortableDeviceConnector.EVENT_PARAMETER_CHILD_HIERARCHY_CHANGED, out bool childHierarchyChanged);
            this.ChildHierarchyChanged = childHierarchyChanged;
        }

        public PortableDeviceConnector Device { get; }
        public Guid Event { get; }
        public string PnpDeviceId { get; }
        public OperationState OperationState { get; }
        public uint OperationProgress { get; }
        public string ObjectParentPersistanceUniqueId { get; }
        public string ObjectCreationCookie { get; }
        public bool ChildHierarchyChanged { get; }
        public string ServiceMethodContext { get; }
    }

    public class ObjectAddedEventArgs : PortableDeviceEventArgs
    {
        internal ObjectAddedEventArgs(Guid eventGuid, PortableDeviceConnector device, IPortableDeviceValues eventParameters)
            : base(eventGuid, device, eventParameters)
        {
            eventParameters.TryGetStringValue(PortableDeviceConnector.OBJECT_ID, out string objectId);
            this.ObjectId = objectId;

            eventParameters.TryGetStringValue(PortableDeviceConnector.OBJECT_NAME, out string objectName);
            this.ObjectName = objectName;

            if (eventParameters.TryGetGuidValue(PortableDeviceConnector.OBJECT_CONTENT_TYPE, out Guid objectContentType))
            {
                this.ObjectContentType = objectContentType;
            }

            eventParameters.TryGetStringValue(PortableDeviceConnector.OBJECT_ORIGINAL_FILE_NAME, out string objectOriginalFileName);
            this.ObjectOriginalFileName = objectOriginalFileName;

            eventParameters.TryGetStringValue(PortableDeviceConnector.OBJECT_PARENT_ID, out string objectParentId);
            this.ObjectParentId = objectParentId;
        }

        public string ObjectId { get; }
        public string ObjectName { get; }
        public Guid ObjectContentType { get; }
        public string ObjectOriginalFileName { get; }
        public string ObjectParentId { get; private set; }
    }
    #endregion

    #region Util
    internal static class ComHelper
    {
        private static bool EqualKey(PropertyKey obj1, PropertyKey obj2)
        {
            return obj1.fmtid == obj2.fmtid && obj1.pid == obj2.pid;
        }

        public static bool HasKeyValue(this IPortableDeviceValues values, PropertyKey findKey)
        {
            uint num = values?.GetCount() ?? 0;
            for (uint i = 0; i < num; i++)
            {
                PropertyKey key;
                PropVariant val = new();
                try
                {
                    values.GetAt(i, out key, val);
                    if (EqualKey(key, findKey))
                    {
                        return val.vt != VARTYPE.VT_ERROR;
                    }
                }
                finally
                {
                    PropVariantClear(val);
                }
            }
            return false;
        }

        public static bool TryGetStringValue(this IPortableDeviceValues values, PropertyKey key, out string value)
        {
            if (values.HasKeyValue(key))
            {
                value = values.GetStringValue(key);
                return true;
            }
            value = string.Empty;
            return false;
        }

        public static bool TryGetGuidValue(this IPortableDeviceValues values, PropertyKey key, out Guid value)
        {
            if (values.HasKeyValue(key))
            {
                value = values.GetGuidValue(key);
                return true;
            }
            value = Guid.Empty;
            return false;
        }

        public static bool TryGetBoolValue(this IPortableDeviceValues values, PropertyKey key, out bool value)
        {
            if (values.HasKeyValue(key))
            {
                value = values.GetBoolValue(key);
                return true;
            }
            value = false;
            return false;
        }

        public static bool TryGetUnsignedIntegerValue(this IPortableDeviceValues values, PropertyKey key, out uint value)
        {
            if (values.HasKeyValue(key))
            {
                value = values.GetUnsignedIntegerValue(key);
                return true;
            }
            value = 0;
            return false;
        }

        public static bool TryGetUnsignedLargeIntegerValue(this IPortableDeviceValues values, PropertyKey key, out ulong value)
        {
            if (values.HasKeyValue(key))
            {
                value = values.GetUnsignedLargeIntegerValue(key);
                return true;
            }
            value = 0;
            return false;
        }

        public static bool TryGetSignedIntegerValue(this IPortableDeviceValues values, PropertyKey key, out int value)
        {
            if (values.HasKeyValue(key))
            {
                value = values.GetSignedIntegerValue(key);
                return true;
            }
            value = 0;
            return false;
        }
    }
    #endregion
}
