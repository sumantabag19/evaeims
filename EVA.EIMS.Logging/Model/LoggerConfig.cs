using System;
using System.Collections.Generic;
using System.Text;

namespace EVA.EIMS.Logging
{
    public class LoggerConfig
    {
        public string LogWriteType { get; set; }
        public string ServiceName { get; set; }
        public LoggerStorage LoggerStorage { get; set; }
        public AzureBlobSettings AzureBlobSettings { get; set; }
        public LogWriteTypeEnum LogWriteTypeEnum
        {
            get
            {
                if (LogWriteType == LogConstants.Blob)
                    return LogWriteTypeEnum.BlobLog;
                else if (LogWriteType == LogConstants.File)
                    return LogWriteTypeEnum.FileLog;
                else if (LogWriteType == LogConstants.Elastic)
                    return LogWriteTypeEnum.ElsaticLog;
                if (LogWriteType == LogConstants.Console)
                    return LogWriteTypeEnum.Console;
                else
                    return LogWriteTypeEnum.ElsaticLog;
            }
        }
    }
}
