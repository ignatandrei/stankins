using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Stankins.AzureDevOps;
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
                res.FromType().Should().HaveFlag(what);
            });
        }

        [Scenario]
        [Example(typeof(ReceiverCSVFile), new string[]{"a.csv"})]
        [Example(typeof(SenderYamlAzurePipelineToDot),null)]
        public void TestCreation(Type t, params string[] arguments)
        {
            FindAssembliesToExecute f = null;
            ResultTypeStankins r = null;
            $"when find types in the assembly {t.Assembly.FullName}".w(() =>
            {
                f = new FindAssembliesToExecute(t.Assembly);
            });
            $"can find {t.Name}".w(() =>
            {
                r = f.FindTypes().FirstOrDefault(it => it.Type == t);
                r.Should().NotBeNull();
            });
            $"and can construct {t.Name}".w(() =>
            {
                var b = r.Create(arguments);
                b.GetType().Should().Be(t);
            });
        }

        [Scenario]
        public void TestFindingAll()
        {
            
            ResultTypeStankins[] r = null;
            $"when find types in the assembly ".w(() =>
            {
                r = FindAssembliesToExecute.AddReferences();
            });
            $"can find data".w(() =>
            {
                r.Should().NotBeNull();
                r.Length.Should().BeGreaterThan(20);
            });
            
        }
    }
}
