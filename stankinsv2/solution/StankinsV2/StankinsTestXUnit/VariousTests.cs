using FluentAssertions;
using Stankins.Interfaces;
using Stankins.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    
    
    public class VariousTests
    {
        [Scenario]
        [Trait("ExternalDependency", "YouTube")]
        public void AndreiPlaylist()
        {
            IReceive r = null;
            IDataToSent data = null;
            $"when I create a recipe with jenkins".w(() => r = new RecipeFromFilePath("Assets/recipes/youtubeplaylist.txt"));
            $"and I transform".w(async () => data= await r.TransformData(null));
            $" there should be >50 links".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().Be(2);
                var sumRows = data.DataToBeSentFurther.Select(it => it.Value.Rows.Count).Sum();
                sumRows.Should().BeGreaterThan(50);
            });
        }
    }
}
