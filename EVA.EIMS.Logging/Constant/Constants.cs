namespace EVA.EIMS.Logging
{
    public static class LogConstants
    {
        #region Log Type
        public const string Error = "Error";
        public const string Debug = "Debug";
        public const string Info = "Info";
        public const string Warn = "Warn";
        #endregion

        #region Blob
        public const string BlobFileExtension = ".txt";
        public const string BlobFileNameSuffix = "Log_";
        #endregion

        #region Log Write Type
        public const string Blob = "Blob";
        public const string File = "File";
        public const string Elastic = "Elastic";
        public const string Console = "Console";
        #endregion

    }
}
