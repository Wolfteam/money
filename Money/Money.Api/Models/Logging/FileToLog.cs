using System;

namespace Money.Api.Models.Logging
{
    public class FileToLog
    {
        public string FileName { get; }
        public string AssemblyFullName { get; }
        public bool Filtered { get; }

        public FileToLog(Type type, string filename)
        {
            AssemblyFullName = type.FullName;
            FileName = filename;
            Filtered = true;
        }

        public FileToLog(string filename)
        {
            FileName = filename;
        }
    }
}
