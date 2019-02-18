using System;
using System.Linq;
using System.Threading.Tasks;
using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects;

namespace Stankins.Interpreter
{
    public class RecipeFromString : BaseObjectInSerial,IReceive, ISender
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
                IInterpreter i=new InterpretFromType();
                if (!i.CanInterpretString(line))
                {
                    var v = i.Validate(null).First();
                    throw new ArgumentException(v.ErrorMessage);
                }
                base.AddType(i.ObjectType.Type);
                foreach (var ctorParam in i.ObjectType.ConstructorParam)
                {
                    
                    base.dataNeeded[ctorParam.Key]=ctorParam.Value;
                }
            }
            return base.TransformData(receiveData);
        }
    }
}