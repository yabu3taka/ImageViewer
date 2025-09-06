using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer.Util
{
    static class DataObjectUtil
    {
        #region NativeMethods
        class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr GlobalLock(IntPtr hMem);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GlobalUnlock(IntPtr hMem);

            [DllImport("kernel32.dll")]
            public static extern UIntPtr GlobalSize(IntPtr hMem);

            [DllImport("ole32.dll", PreserveSig = false)]
            public static extern ILockBytes CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease);

            [DllImport("OLE32.DLL", CharSet = CharSet.Auto, PreserveSig = false)]
            public static extern IntPtr GetHGlobalFromILockBytes(ILockBytes pLockBytes);

            [DllImport("OLE32.DLL", CharSet = CharSet.Unicode, PreserveSig = false)]
            public static extern IStorage StgCreateDocfileOnILockBytes(ILockBytes plkbyt, uint grfMode, uint reserved);

            [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000B-0000-0000-C000-000000000046")]
            public interface IStorage
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                IStream CreateStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName,
                    [In] int grfMode, [In, Optional] int reserved1, [In, Optional] int reserved2);

                [return: MarshalAs(UnmanagedType.Interface)]
                IStream OpenStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, Optional] IntPtr reserved1,
                    [In] int grfMode, [In, Optional] int reserved2);

                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage CreateStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] int grfMode,
                    [In, Optional] int reserved1, [In, Optional] int reserved2);

                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage OpenStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, Optional, MarshalAs(UnmanagedType.Interface)] IStorage pstgPriority,
                    [In] int grfMode, [In, Optional]  IntPtr snbExclude, [In, Optional] int reserved);

                void CopyTo([In, Optional] int ciidExclude, [In, Optional, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] Guid[] pIIDExclude,
                    [In, Optional] IntPtr snbExclude, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest);

                void MoveElementTo([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest,
                    [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName, [In] int grfFlags);

                void Commit([In] int grfCommitFlags);

                void Revert();

                [return: MarshalAs(UnmanagedType.Interface)]
                object EnumElements([In, Optional] int reserved1, [In, Optional] IntPtr reserved2, [In, Optional] int reserved3);

                void DestroyElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsName);

                void RenameElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName);

                void SetElementTimes([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] FILETIME pctime, [In] FILETIME patime, [In] FILETIME pmtime);

                void SetClass([In] ref Guid clsid);

                void SetStateBits([In] int grfStateBits, [In] int grfMask);

                void Stat([Out] out STATSTG pStatStg, [In] STATFLAG grfStatFlag);
            }

            [ComImport, Guid("0000000A-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface ILockBytes
            {
                void ReadAt(ulong ulOffset, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, uint cb, out uint pcbRead);
                void WriteAt(ulong ulOffset, [In] IntPtr pv, uint cb, out uint pcbWritten);
                void Flush();
                void SetSize(ulong cb);
                void LockRegion(ulong libOffset, ulong cb, int dwLockType);
                void UnlockRegion(ulong libOffset, ulong cb, int dwLockType);
                void Stat(out STATSTG pstatstg, STATFLAG grfStatFlag);
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct POINTL
            {
                public int x;
                public int y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SIZEL
            {
                public int cx;
                public int cy;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct FILEGROUPDESCRIPTORA
            {
                public uint cItems;
                public FILEDESCRIPTORA[] fgd;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct FILEDESCRIPTORA
            {
                public uint dwFlags;
                public Guid clsid;
                public SIZEL sizel;
                public POINTL pointl;
                public uint dwFileAttributes;
                public FILETIME ftCreationTime;
                public FILETIME ftLastAccessTime;
                public FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct FILEGROUPDESCRIPTORW
            {
                public uint cItems;
                public FILEDESCRIPTORW[] fgd;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct FILEDESCRIPTORW
            {
                public uint dwFlags;
                public Guid clsid;
                public SIZEL sizel;
                public POINTL pointl;
                public uint dwFileAttributes;
                public FILETIME ftCreationTime;
                public FILETIME ftLastAccessTime;
                public FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
            }

            public enum STATFLAG
            {
                STATFLAG_DEFAULT = 0,
                STATFLAG_NONAME = 1,
                STATFLAG_NOOPEN = 2
            }
        }
        #endregion

        public static string[] GetFileGroupDescriptor(System.Windows.Forms.IDataObject data, bool autoConvert)
        {
            IntPtr fileGroupDescriptorAPointer = IntPtr.Zero;
            try
            {
                MemoryStream fileGroupDescriptorStream = (MemoryStream)data.GetData("FileGroupDescriptor", autoConvert);
                byte[] fileGroupDescriptorBytes = new byte[fileGroupDescriptorStream.Length];
                fileGroupDescriptorStream.Read(fileGroupDescriptorBytes, 0, fileGroupDescriptorBytes.Length);
                fileGroupDescriptorStream.Close();

                fileGroupDescriptorAPointer = Marshal.AllocHGlobal(fileGroupDescriptorBytes.Length);
                Marshal.Copy(fileGroupDescriptorBytes, 0, fileGroupDescriptorAPointer, fileGroupDescriptorBytes.Length);

                var fileGroupDescriptor = Marshal.PtrToStructure<NativeMethods.FILEGROUPDESCRIPTORA>(fileGroupDescriptorAPointer);
                string[] fileNames = new string[fileGroupDescriptor.cItems];

                IntPtr fileDescriptorPointer = IntPtr.Add(fileGroupDescriptorAPointer, sizeof(uint));
                for (int fileDescriptorIndex = 0; fileDescriptorIndex < fileGroupDescriptor.cItems; fileDescriptorIndex++)
                {
                    var fileDescriptor = Marshal.PtrToStructure<NativeMethods.FILEDESCRIPTORA>(fileDescriptorPointer);
                    fileNames[fileDescriptorIndex] = fileDescriptor.cFileName;
                    fileDescriptorPointer = IntPtr.Add(fileDescriptorPointer, Marshal.SizeOf(fileDescriptor));
                }

                return fileNames;
            }
            finally
            {
                Marshal.FreeHGlobal(fileGroupDescriptorAPointer);
            }
        }

        public static string[] GetFileGroupDescriptorW(System.Windows.Forms.IDataObject data)
        {
            IntPtr fileGroupDescriptorWPointer = IntPtr.Zero;
            try
            {
                MemoryStream fileGroupDescriptorStream = (MemoryStream)data.GetData("FileGroupDescriptorW");
                byte[] fileGroupDescriptorBytes = new byte[fileGroupDescriptorStream.Length];
                fileGroupDescriptorStream.Read(fileGroupDescriptorBytes, 0, fileGroupDescriptorBytes.Length);
                fileGroupDescriptorStream.Close();

                fileGroupDescriptorWPointer = Marshal.AllocHGlobal(fileGroupDescriptorBytes.Length);
                Marshal.Copy(fileGroupDescriptorBytes, 0, fileGroupDescriptorWPointer, fileGroupDescriptorBytes.Length);

                var fileGroupDescriptor = Marshal.PtrToStructure<NativeMethods.FILEGROUPDESCRIPTORW>(fileGroupDescriptorWPointer);
                string[] fileNames = new string[fileGroupDescriptor.cItems];

                IntPtr fileDescriptorPointer = IntPtr.Add(fileGroupDescriptorWPointer, sizeof(uint));
                for (int fileDescriptorIndex = 0; fileDescriptorIndex < fileGroupDescriptor.cItems; fileDescriptorIndex++)
                {
                    var fileDescriptor = Marshal.PtrToStructure<NativeMethods.FILEDESCRIPTORW>(fileDescriptorPointer);
                    fileNames[fileDescriptorIndex] = fileDescriptor.cFileName;

                    fileDescriptorPointer = IntPtr.Add(fileDescriptorPointer, Marshal.SizeOf(fileDescriptor));
                }

                return fileNames;
            }
            finally
            {
                Marshal.FreeHGlobal(fileGroupDescriptorWPointer);
            }
        }

        public static MemoryStream GetFileContents(System.Windows.Forms.IDataObject data, int index)
        {
            string format = "FileContents";

            FORMATETC formatetc = new()
            {
                cfFormat = (short)DataFormats.GetFormat(format).Id,
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = index,
                ptd = new IntPtr(0),
                tymed = TYMED.TYMED_ISTREAM | TYMED.TYMED_ISTORAGE | TYMED.TYMED_HGLOBAL
            };

            STGMEDIUM medium = new();

            var comdata = (System.Runtime.InteropServices.ComTypes.IDataObject)data;
            comdata.GetData(ref formatetc, out medium);

            switch (medium.tymed)
            {
                case TYMED.TYMED_ISTORAGE:
                    NativeMethods.IStorage iStorage = null;
                    NativeMethods.IStorage iStorage2 = null;
                    NativeMethods.ILockBytes iLockBytes = null;
                    STATSTG iLockBytesStat;
                    try
                    {
                        iStorage = (NativeMethods.IStorage)Marshal.GetObjectForIUnknown(medium.unionmember);
                        Marshal.Release(medium.unionmember);

                        iLockBytes = NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true);
                        iStorage2 = NativeMethods.StgCreateDocfileOnILockBytes(iLockBytes, 0x00001012, 0);

                        iStorage.CopyTo(0, null, IntPtr.Zero, iStorage2);
                        iLockBytes.Flush();
                        iStorage2.Commit(0);

                        iLockBytesStat = new STATSTG();
                        iLockBytes.Stat(out iLockBytesStat, NativeMethods.STATFLAG.STATFLAG_NONAME);
                        int iLockBytesSize = (int)iLockBytesStat.cbSize;

                        byte[] iLockBytesContent = new byte[iLockBytesSize];
                        iLockBytes.ReadAt(0, iLockBytesContent, (uint)iLockBytesContent.Length, out var _);

                        return new MemoryStream(iLockBytesContent);
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(iStorage2);
                        Marshal.ReleaseComObject(iLockBytes);
                        Marshal.ReleaseComObject(iStorage);
                    }

                case TYMED.TYMED_ISTREAM:
                    IStream iStream = null;
                    STATSTG iStreamStat;
                    try
                    {
                        iStream = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
                        Marshal.Release(medium.unionmember);

                        iStreamStat = new STATSTG();
                        iStream.Stat(out iStreamStat, (int)NativeMethods.STATFLAG.STATFLAG_NONAME);
                        int iStreamSize = (int)iStreamStat.cbSize;

                        byte[] iStreamContent = new byte[iStreamSize];
                        iStream.Read(iStreamContent, iStreamContent.Length, IntPtr.Zero);

                        return new MemoryStream(iStreamContent);
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(iStream);
                    }

                case TYMED.TYMED_HGLOBAL:
                    return ReadByteStreamFromHGLOBAL(medium.unionmember);
            }

            return null;
        }

        private static unsafe MemoryStream ReadByteStreamFromHGLOBAL(IntPtr hglobal)
        {
            IntPtr buffer = NativeMethods.GlobalLock(hglobal);
            if (((void*)buffer) is null)
            {
                throw new ExternalException();
            }

            try
            {
                int size = (int)NativeMethods.GlobalSize(hglobal);
                byte[] bytes = new byte[size];
                Marshal.Copy((nint)buffer, bytes, 0, size);
                return new MemoryStream(bytes);
            }
            finally
            {
                NativeMethods.GlobalUnlock(hglobal);
            }
        }
    }
}
