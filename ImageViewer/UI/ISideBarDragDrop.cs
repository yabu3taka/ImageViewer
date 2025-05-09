using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ImageViewer.UI
{
    interface ISideBarDragDrop : ISideBarUI
    {
        void SideBar_DragEnter(object sender, DragEventArgs e);
        void SideBar_DragDrop(object sender, DragEventArgs e);
    }
}
