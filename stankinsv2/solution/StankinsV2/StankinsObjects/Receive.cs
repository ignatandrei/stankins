using Stankins.Interfaces;
using StankinsCommon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StankinsObjects
{
    public abstract class Receive : BaseObject, IReceive
    {
        public Receive(CtorDictionary dataNeeded) : base(dataNeeded)
        {
        }
    }
}
