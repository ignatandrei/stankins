using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StankinsObjects 
{
    public class ChangeTableNamesRegex : BaseObject, ITransformer
    {
        public string TableName { get; }
        public string Expression { get; }

        public ChangeTableNamesRegex( string expression) : base(new CtorDictionary()
        {
           
            {nameof(expression),expression }
        })
        {
           
        }
        public ChangeTableNamesRegex(CtorDictionary dataNeeded) : base(dataNeeded)
        {
            
            Expression = GetMyDataOrThrow<string>(nameof(Expression));
        }

        public override async Task<IDataToSent> TransformData(IDataToSent receiveData)
        {
            var regex = new Regex(Expression);
            foreach (var item in receiveData.DataToBeSentFurther)
            {
                var g = regex.Match(item.Value.TableName);
                if (!g.Success)
                    continue;

                var s = g.Groups;
                
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
