using System;
using System.Collections.Generic;
using System.Text;

namespace Transformers
{
    public class TransformerChangeType<T, U> : TransformAddField<T, U>
    {
        public TransformerChangeType(string oldField, string newField)
            :base((t)=>(U)(Convert.ChangeType(t, typeof(U))),oldField,newField)
        {
                        
        }
    }
}
