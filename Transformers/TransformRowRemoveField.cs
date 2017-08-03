using System;
using System.Collections.Generic;
using System.Text;

namespace Transformers
{
    public class TransformRowRemoveField: TransformRow
    {
        public TransformRowRemoveField(string nameField)
            : base($"Values.Remove(\"{nameField}\");Values")
        {

        }
    }
}
