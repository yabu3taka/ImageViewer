namespace ImageConverter.Work
{
    /// <summary>
    /// 逆変換
    /// </summary>
    class RevertTextWorkProcessor(string toDir, MaskConverter converter) : LocalWorkProcessor(toDir)
    {
        private readonly MaskConverter _converter = converter;

        #region SubFolderWorkData
        public override void SyncSubFolder(WorkData work)
        {
        }
        #endregion

        #region SubFolderWorkData
        public override bool ScanSubFolder(SubFolderWorkData target)
        {
            target.AddConvertRule(".tsd", ".txt");
            return true;
        }

        public override bool IsSameImageOrTextFile(FilePair pair)
        {
            return pair.ToFile.Exists();
        }

        public override void CopyImageOrTextFile(FilePair pair)
        {
            pair.Convert(_converter.ConvertStream);
        }

        public override void CopyIndexFile(FilePair pair)
        {
        }

        public override void CopySettingFile(FilePair pair)
        {
        }
        #endregion
    }
}
