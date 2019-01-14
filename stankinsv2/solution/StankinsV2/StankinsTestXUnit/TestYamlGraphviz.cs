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
            var res = graph.Result().Replace(Environment.NewLine, "");
            res.Should().ContainAll("Xcode@5", "->");
            res.Should().NotContain("2");// not 2 jobs, not 2 tasks

        }

        [Fact]
        public async Task TestYamlGrphviz2Jobs()
        {
            var data = (@"# Xamarin.Android and Xamarin.iOS
# Build a Xamarin.Android and Xamarin.iOS app.
# Add steps that test, sign, and distribute the app, save build artifacts, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/xamarin

jobs:

- job: Android
  pool:
    vmImage: 'VS2017-Win2016'

  variables:
    buildConfiguration: 'Release'
    outputDirectory: '$(build.binariesDirectory)/$(buildConfiguration)'

  steps:
  - task: NuGetToolInstaller@0

  - task: NuGetCommand@2
    inputs:
      restoreSolution: '**/*.sln'

  - task: XamarinAndroid@1
    inputs:
      projectFile: '**/*droid*.csproj'
      outputDirectory: '$(outputDirectory)'
      configuration: '$(buildConfiguration)'

  - task: AndroidSigning@3
    inputs:
      apksign: false
      zipalign: false
      apkFiles: '$(outputDirectory)/*.apk'

  - task: PublishBuildArtifacts@1
    inputs:
      pathtoPublish: '$(outputDirectory)'

- job: iOS
  pool:
    vmImage: 'macOS 10.13'

  steps:
  # To manually select a Xamarin SDK version on the Hosted macOS agent, enable this script with the SDK version you want to target
  # https://go.microsoft.com/fwlink/?linkid=871629
  - script: sudo $AGENT_HOMEDIRECTORY/scripts/select-xamarin-sdk.sh 5_4_1 
    displayName: 'Select Xamarin SDK version'
    enabled: false

  - task: NuGetToolInstaller@0

  - task: NuGetCommand@2
    inputs:
      restoreSolution: '**/*.sln'

  - task: XamariniOS@2
    inputs:
      solutionFile: '**/*.sln'
      configuration: 'Release'
      buildForSimulator: true
      packageApp: false");
            string nameFile = $"{nameof(TestYamlGrphviz2Jobs)}.txt";
            File.WriteAllText(nameFile, data);
            var visit = new YamlReader(nameFile, Encoding.UTF8);
            var dt = await visit.TransformData(null);
            var graph = new YamlToGraphviz();
            dt = await graph.TransformData(dt);
            //await File.WriteAllTextAsync("a.txt", graph.Result());
            //Process.Start("notepad.exe", "a.txt");
            var res = graph.Result().Replace(Environment.NewLine, "");
            res.Should().ContainAll("Android", "iOS", "XamarinAndroid@1", "XamariniOS@2");
            
        }
    }
}
