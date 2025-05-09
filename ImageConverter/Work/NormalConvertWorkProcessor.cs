namespace ImageConverter.Work
{
    /// <summary>
    /// 通常変換
    /// </summary>
    class NormalConvertWorkProcessor(string toDir, MaskConverter c) : MaskConvertWorkProcessor(toDir, c)
    {
        public override void SetupSetting(WorkData work)
        {
            work.Setting.UpdateModifedDate = true;
        }

        #region SubFolderWorkData
        public override bool IsSameImageOrTextFile(FilePair pair)
        {
            return pair.IsSameBySize();
        }

        public override void CopyImageOrTextFile(FilePair pair)
        {
            pair.Convert(_converter.ConvertStream);
        }
        #endregion
    }
}
