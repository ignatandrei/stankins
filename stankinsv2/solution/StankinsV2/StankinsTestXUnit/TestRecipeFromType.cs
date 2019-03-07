using FluentAssertions;
using Stankins.Interpreter;
using StankinsHelperCommands;
using System.Linq;
using Xbehave;
using Xunit;
namespace StankinsTestXUnit
{
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
