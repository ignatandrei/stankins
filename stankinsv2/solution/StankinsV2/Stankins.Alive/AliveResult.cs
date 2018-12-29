using System;

namespace Stankins.Alive
{
    public class AliveResult
    {
        public DateTime StartedDate { get; set; }
        public string Process { get; set; }
        public string Arguments { get; set; }
        public string To { get; set; }
        public bool IsSuccess { get; set; }
        public string Result { get; set; }
        public long? Duration { get; set; }
        public string DetailedResult { get; set; }
        public string Exception { get; set; }
        

    }
}
