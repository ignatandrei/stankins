using Stankins.Interfaces;
using StankinsCommon;
using StankinsObjects ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StankinsObjects 
{
    public class ChangeTableNamesRegex : BaseObject, ITransformer
    {
        public string TableName { get; }
        public string Expression { get; }

        public ChangeTableNamesRegex( string expression) : this(new CtorDictionary()
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
            var names = regex.GetGroupNames().
               Where(it => !int.TryParse(it, out var _)).
               FirstOrDefault();
            foreach (var item in receiveData.DataToBeSentFurther)
            {
                var g = regex.Match(item.Value.TableName);
                if (!g.Success)
                    continue;

                var groups = g.Groups;
                if (groups[names].Success)
                {
                    item.Value.TableName = groups[names].Value;
                    receiveData.Metadata.Tables[item.Key].Name = groups[names].Value;
                }
                
            }
            return receiveData;
        }

        public override Task<IMetadata> TryLoadMetadata()
        {
            throw new NotImplementedException();
        }
    }
}
