using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using StankinsHelperCommands;
using StankinsObjects;

namespace Stankins.Interpreter
{
    public interface IInterpreter:IValidatableObject
    {
        
        bool CanInterpretString(string data);
        ResultTypeStankins ObjectType { get; }
        
        BaseObject ObjectToRun();
    }
}
