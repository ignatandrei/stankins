using System;
using System.Threading.Tasks;
using StankinsObjects;

namespace Stankins.Interpreter
{
    public interface IInterpreter
    {
        bool CanInterpretString(string data);
        BaseObject ObjectToRun();
    }


}
