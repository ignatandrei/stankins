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
    public class TestYamlVisitor
    {
        [Fact]
        public void TestXCode1()
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


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("macOS 10.13");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(1);
            var t = j.Steps[0];
            t.Should().BeOfType<TaskYaml>();
            var t1 = t as TaskYaml;
            t1.Inputs.Should().NotBeNull();
            t1.Inputs.Count.Should().Be(4);
            t1.Inputs[0].Should().Be(new KeyValuePair<string,string>("scheme", ""));
            t1.Inputs[1].Should().Be(new KeyValuePair<string, string>("sdk", "iphoneos"));
            t1.Inputs[2].Should().Be(new KeyValuePair<string, string>("configuration", "Release"));
            t1.Inputs[3].Should().Be(new KeyValuePair<string, string>("xcodeVersion", "default"));

        }
    }
}
