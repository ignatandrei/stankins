using FluentAssertions;
using Stankins.AzureDevOps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("yaml", "testYamlStankins")]
    [Trait("AfterPublish", "0")]
    public class TestYAMLStankins
    {
        [Fact]
        public async Task TestXCode1()
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

            string nameFile = $"{nameof(TestXCode1)}.txt";
            File.WriteAllText(nameFile, data);
            var visit = new YamlReader(nameFile,Encoding.UTF8);
            var dt = await visit.TransformData(null);
            dt.Should().NotBeNull();
            dt.DataToBeSentFurther.Should().NotBeNull();
            var jobs = dt.FindAfterName("jobs").Value;
            jobs.Should().NotBeNull();
            jobs.Rows.Count.Should().Be(1);





        }
    }
}
