using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StankinsHelperCommands;
using StankinsObjects;

namespace Stankins.Interpreter
{
    public class InterpretFromType:IInterpreter
    {
        List<ValidationResult> valid=new List<ValidationResult>();
        
        public bool CanInterpretString(string data)
        {
            var all = FindAssembliesToExecute.AddReferences();
            var instr = data.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var name = instr[0];
            ObjectType = all.FirstOrDefault(it => string.Equals(it.Name, name, StringComparison.InvariantCultureIgnoreCase));
            if (ObjectType == null)
            {
                valid.Add(new ValidationResult($"cannot find object {name}"));
                return false;
            }

           if(instr.Length -1 != ObjectType.ConstructorParam.Keys.Count)
           {
               valid.Add(new ValidationResult($"constructor items length different:{instr.Length -1} {ObjectType.ConstructorParam.Keys.Count}"));
           }
           var keys = ObjectType.ConstructorParam.Keys;
           for (int i = 1; i < instr.Length; i++)
           {
               var first = instr[i].IndexOf("=");
               var argumentName = instr[i].Substring(0, first );
               var argumentValue = instr[i].Substring(first + 1);
               var key = keys.FirstOrDefault(it =>
                   string.Equals(it, argumentName, StringComparison.InvariantCultureIgnoreCase));

               if(key==null)
               {
                   
                   valid.Add(new ValidationResult($"key not found:{argumentName}"));
               }
               else
               {
                   ObjectType.ConstructorParam[key] = argumentValue;
               }

           }

           return (valid.Count == 0);

        }

        public ResultTypeStankins ObjectType { get; private set; }

        public BaseObject ObjectToRun()
        {
            return ObjectType.Create(ObjectType.ConstructorParam.Values.Select(it => it).ToArray());
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return valid.ToArray();
        }
    }
}