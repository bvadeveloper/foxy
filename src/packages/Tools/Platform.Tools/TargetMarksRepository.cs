using System;
using System.Collections.Generic;
using System.Linq;
using Platform.Contract.Enums;

namespace Platform.Tools
{
    public class TargetMarksRepository
    {
        /// <summary>
        /// Init target marks
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static IEnumerable<string> InitMarks(TargetType targetType) =>
            targetType switch
            {
                TargetType.Framework => new List<string>
                {
                    "joomla",
                    "wordpress"
                },
                TargetType.WebServer => new List<string>
                {
                    "nginx",
                    "iis",
                    "kestrel",
                    "tomcat",
                    "apache"
                },
                TargetType.Database => new List<string>
                {
                    "mysql",
                    "maria",
                    "mssql",
                    "postgres",
                    "mongo",
                    "dynamo"
                },
                TargetType.Port => new List<string>
                {
                    "21",
                    "22",
                    "80",
                    "443",
                    "1433",
                    "8080",
                    "8081",
                    "15672",
                    "5672",
                    "6379",
                    "3306"
                },
                TargetType.Server => new List<string>
                {
                    "linux",
                    "windows",
                    "ubuntu",
                    "fedora",
                    "redhat",
                    "centos",
                    "redhat"
                },
                TargetType.NotAvailable => new List<string>
                {
                    "Nmap done: 0 IP addresses (0 hosts up)"
                },
                _ => throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null)
            };

        public static IEnumerable<string> FindMarks(TargetType targetType, string text) => 
            InitMarks(targetType).Where(mark => text.Contains(mark, StringComparison.InvariantCultureIgnoreCase));
    }
}