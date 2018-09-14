using System;

namespace Logging

{
    public struct DebugInfo
    {
        public DateTime StartTime;
        public string ClassName;
        public string MethodName;
        public int LineNumber;
        public TimeSpan duration;
        public string Key()
        {
            return this.ClassName + "_" + this.MethodName + "_" + this.LineNumber;
        }
    }
}