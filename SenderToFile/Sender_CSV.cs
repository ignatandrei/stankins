using MediaTransform;
using System;
using System.Collections.Generic;
using System.Text;

namespace SenderToFile
{
    /// <summary>
    /// TODO: CSV separator : ,  or any other
    /// </summary>
    public class Sender_CSV : SenderMediaToFile
    {
        public Sender_CSV(string fileName):base(new MediaTransformCSV(), fileName)
        {

        }

    }
}
