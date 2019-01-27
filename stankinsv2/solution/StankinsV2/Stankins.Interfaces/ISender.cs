using System.Collections.Generic;

namespace Stankins.Interfaces
{
    public interface ISender : IBaseObject
    {

    }

    public interface ISenderToOutput : ISender
    {
        string InputContents { get; set; }
        KeyValuePair<string,string>[] OutputContents { get; set; }

    }
}
