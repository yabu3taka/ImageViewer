using System;
using System.Collections.Generic;
using ImageViewer.Manager;

namespace ImageViewer.UI
{
    enum SideBarUserInstruction
    {
        Closed,
        Opened
    }

    enum SideBarType
    {
        List,
        Compare,
        FileAdd,
        FileEdit
    }

    public enum SideBarOpenReason
    {
        SideBarUserInstruction,
        SideBarType,
        TabPageChange,
        TabPageCursorReplace,
        SideListType
    }

    public enum SideBarUpdateReason
    {
        ShowSideBar,
        ConditionChange,
        MarkChange,
        MarkReset,
        MarkSave,
        Reload,
        FileAdd,
        FileDelete,
        FileRename,
        FolderAdd,
        FolderRename
    }

    class SideBarUpdateInfo(SideBarUpdateReason reason)
    {
        public SideBarUpdateReason UpdateReason { get; } = reason;
        public SideBarOpenReason? OpenReason { get; set; }
        public string NewName { get; set; }

        public bool IsFileCountChanged()
        {
            return UpdateReason switch
            {
                SideBarUpdateReason.FileAdd or SideBarUpdateReason.FileDelete => true,
                _ => false,
            };
        }

        public bool IsMarkChanged()
        {
            return UpdateReason switch
            {
                SideBarUpdateReason.MarkReset or SideBarUpdateReason.MarkChange => true,
                _ => false,
            };
        }

        public void UpdateImageFileListView(ImageFileListView v, FilePosCursor c)
        {
            switch (UpdateReason)
            {
                case SideBarUpdateReason.MarkSave:
                    break;
                case SideBarUpdateReason.MarkReset:
                case SideBarUpdateReason.MarkChange:
                    v.UpdateMarkStatus();
                    break;
                default:
                    v.UpdateFileList(c);
                    break;
            }
        }
    }

    interface ISideBarUI
    {
        bool Visible { get; set; }

        bool NornmalControlFocused { get; }
        bool ListBoxFocused { get; }

        void UpdateOnOpening(SideBarOpenReason reason);
        void UpdateFileList(SideBarUpdateInfo info);
        void UpdateFileSelection();

        void NotifySettingChanged();

        void SetImageInfo(FilePosCursor c, IFileImageInfo imgInfo);
    }
}
