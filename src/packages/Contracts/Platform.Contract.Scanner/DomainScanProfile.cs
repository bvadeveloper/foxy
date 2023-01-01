using System.Collections.Generic;
using Platform.Contract.Enums;

namespace Platform.Contract.Scanner
{
    public class DomainScanProfile : ScanProfile
    {
        public Dictionary<TargetType, List<string>> Tags { get; set; }
    }
}