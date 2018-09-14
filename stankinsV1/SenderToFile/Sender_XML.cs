using MediaTransform;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenderToFile
{
    public class Sender_XML : SenderMediaToFile
    {
        public string RootName { get; set; }

        public Sender_XML(string fileName, string rootName) :base( fileName, new MediaTransformXML(rootName))
        {
            this.RootName = rootName;
        }

    }
}
