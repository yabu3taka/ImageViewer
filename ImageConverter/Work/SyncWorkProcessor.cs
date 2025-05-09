using System.IO;

namespace ImageConverter.Work
{
    /// <summary>
    /// 同期
    /// </summary>
    class SyncWorkProcessor(string toDir) : LocalWorkProcessor(toDir)
    {
        public override void SyncSubFolder(WorkData work)
        {
            File.Copy(work.FromProc.PassFile, this.PassFile, true);

            work.DeleteSubFolderIfNoOrig(ToRoot);
        }

        public override bool ScanSubFolder(SubFolderWorkData target)
        {
            target.AddSyncConvertRule();
            target.DeleteNoOrigMode = true;
            return true;
        }

        public override bool IsSameImageOrTextFile(FilePair pair)
        {
            return pair.IsSameBySize();
        }

        public override void CopyImageOrTextFile(FilePair pair)
        {
            pair.Copy();
        }

        public override void CopyIndexFile(FilePair pair)
        {
            pair.Copy();
        }

        public override void CopySettingFile(FilePair pair)
        {
            pair.Copy();
        }
    }
}
