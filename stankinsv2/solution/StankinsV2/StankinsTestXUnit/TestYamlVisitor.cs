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
    [Trait("yaml", "testYamlParser")]
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
        [Fact]
        
        public void TestXamarin3()
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


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(2);
            var android = visit.jobs[0];
            android.Name.Should().Be("Android");
            android.Steps.Should().NotBeNull();
            android.Steps.Count.Should().Be(5);
            var ios= visit.jobs[1];
            ios.Name.Should().Be("iOS");
            ios.Steps.Should().NotBeNull();
            ios.Steps.Count.Should().Be(4);
        }

    }
}
