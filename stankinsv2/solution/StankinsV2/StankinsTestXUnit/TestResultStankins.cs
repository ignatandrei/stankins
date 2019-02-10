using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Stankins.File;
using Stankins.SimpleRecipes;
using StankinsHelperCommands;
using StankinsObjects;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("ReceiverHtmlSelector", "")]
    [Trait("ExternalDependency", "0")]
    public class TestResultStankins
    {
        [Scenario]
        [Example(typeof(ReceiverCSV), WhatToList.None)]
        [Example(typeof(ReceiverCSVFile), WhatToList.Receivers)]
        [Example(typeof(SenderAllTablesToFileCSV), WhatToList.Senders)]
        [Example(typeof(TransformSplitColumn), WhatToList.Transformers)]
        [Example(typeof(FilterColumnDataGreaterThanLength), WhatToList.Filters)]
        [Example(typeof(ExportAzurePipelinesToDot),WhatToList.RecipeSimple)]
        public void TestFindEnum(Type t, WhatToList what)
        {
            $"when find enum for {t.Name} it is {what.ToString()}".w(() =>
            {
                var res=new ResultTypeStankins(t,null);
                res.FromType().Should().Be(what);
            });
        }
    }
}
