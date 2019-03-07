using FluentAssertions;
using Stankins.Interfaces;
using Stankins.Interpreter;
using Stankins.Rest;
using StankinsHelperCommands;
using System.Linq;
using Xbehave;
using Xunit;
namespace StankinsTestXUnit
{
    [Trait("TestRecipeFromType", "")]
    [Trait("ExternalDependency", "0")]
    public class TestReceiverRestFromText 
    {
        [Scenario]
        [Example("{id:1,name:'andrei'}",2,1)]
        public void TestSimple(string text,int nrCols,int nrRows)
        {
            IReceive r=null;
            IDataToSent data=null;
            $"when creating receiving ReceiverRestFromText ".w(async ()=>
            { 
                r=new ReceiverRestFromText(text ); 
                data=await r.TransformData(null);
            }
            );
            $"then should be 1 table".w(()=>
                data.DataToBeSentFurther.Count().Should().Be(1)
            );
            $"then should be {nrCols} cols ".w(()=>
                data.DataToBeSentFurther[0].Columns.Count.Should().Be(nrCols)
            );
            $"then should be {nrRows} cols ".w(()=>
                data.DataToBeSentFurther[0].Rows.Count.Should().Be(nrRows)
            );
        }
    }
    [Trait("TestRecipeFromType", "")]
    [Trait("ExternalDependency", "0")]
    public class TestRecipeFromType
    {
        [Scenario]
        public void TestSimpleAll()
        {
            ResultTypeStankins[] rs=null;
            RecipeFromType[] rec=null;
            $"when finding all references".w(()=>{
            rs=FindAssembliesToExecute.AddReferences();

                });
            $"then it should construct RecipeFromType".w(() =>
            {
                rec=rs.Select(it=>new RecipeFromType(it)).ToArray();
            });
            $"and should have all kind of recipes".w(() =>
            {
                rec.Count(it=>it.WhatToList.Value == WhatToList.Filters).Should().BeGreaterThan(1);
            
                rec.Count(it=>it.WhatToList.Value == WhatToList.Receivers).Should().BeGreaterThan(1);
                
                rec.Count(it=>it.WhatToList.Value == WhatToList.Senders).Should().BeGreaterThan(1);
                rec.Count(it=>it.WhatToList.Value == WhatToList.Transformers).Should().BeGreaterThan(1);
            
               });
            
        }
    }
}
