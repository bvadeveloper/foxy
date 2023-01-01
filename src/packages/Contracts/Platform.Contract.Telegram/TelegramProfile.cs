﻿using Platform.Contract.Reporter;
using Platform.Contract.Reporter.Abstractions;
using Platform.Primitive;

namespace Platform.Contract.Telegram
{
    public class TelegramProfile : IReportProfile
    {
        public TraceContext TraceContext { get; set; }

        public string Target { get; set; }

        public ReportModel[] Reports { get; set; }
        
        public byte[] FileBody { get; set; }

        public string FileName { get; set; }
    }
}