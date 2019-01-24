using FluentAssertions;
using Stankins.AzureDevOps;
using StankinsObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("objects", "transformer")]
    [Trait("ExternalDependency", "0")]

    public class TestTransformHierar
    {
        [Fact]
        public async Task TestSimpleHierar()
        {
            var data = (@"# Xcode
# Build, test, and archive an Xcode workspace on macOS.
# Add steps that install certificates, test, sign, and distribute the app, save build artifacts, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/xcode

pool:
  vmImage: 'macOS 10.13'

steps:
- task: Xcode@5
  inputs:
    scheme: ''
    sdk: 'iphoneos'
    configuration: 'Release'
    xcodeVersion: 'default' # Options: 8, 9, default, specifyPath");

            string nameFile = $"{nameof(TestSimpleHierar)}.txt";
            File.WriteAllText(nameFile, data);
            var visit = new YamlReader(nameFile, Encoding.UTF8);
            var dt = await visit.TransformData(null);
            var transform = new TransformerToOneTableHierarchical("jobs", "name", "steps", "jobName","all");
            dt = await transform.TransformData(dt);

            dt.Should().NotBeNull();
            dt.DataToBeSentFurther.Should().NotBeNull();
            var jobs = dt.FindAfterName("all").Value;
            jobs.Should().NotBeNull();
            jobs.Rows.Count.Should().Be(1);
            
        }
    }
}