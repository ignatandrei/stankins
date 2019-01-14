using FluentAssertions;
using Stankins.AzureDevOps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("yaml", "testGraphVizStankins")]
    [Trait("AfterPublish", "0")]
    public class TestYamlGraphviz
    {
        [Fact]
        public async Task TestYamlGrphvizSimple()
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

            string nameFile = $"{nameof(TestYamlGrphvizSimple)}.txt";
            File.WriteAllText(nameFile, data);
            var visit = new YamlReader(nameFile, Encoding.UTF8);
            var dt = await visit.TransformData(null);
            var graph = new YamlToGraphviz();
            dt = await graph.TransformData(dt);
            //await File.WriteAllTextAsync("a.txt",graph.Result());
            //Process.Start("notepad.exe","a.txt");
            var res = graph.Result().Replace(Environment.NewLine,"");
            res.Should().ContainAll("Xcode@5","->");
            res.Should().NotContain("2");// not 2 jobs, not 2 tasks

        }
    }
}
