using System;
using System.Linq;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.Interpreter
{
    public class RecipeFromString : BaseObjectInSerial, IReceive
    {
        private readonly string content;

        public RecipeFromString(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            this.content = GetMyDataOrThrow<string>(nameof(content));
        }

        public RecipeFromString(string content) : this(new CtorDictionary()
        {
            {nameof(content), content}
        })
        {
           
        }

        public override Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var lines = content.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                if(line.Trim().StartsWith("#"))
                    continue;
                IInterpreter i=new InterpretFromType();
                if (!i.CanInterpretString(line.Trim()))
                {
                    var v = i.Validate(null).First();
                    throw new ArgumentException(v.ErrorMessage);
                }
                base.AddType(i.ObjectType.Type, i.ObjectType.ConstructorParam);
            }
            return base.TransformData(receiveData);
        }
    }
}