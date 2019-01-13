using FluentAssertions;
using Stankins.AzureDevOps;
using System;
using System.Collections.Generic;
using System.Data;
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
            var visit = new YamlReader(nameFile, Encoding.UTF8);
            var dt = await visit.TransformData(null);
            dt.Should().NotBeNull();
            dt.DataToBeSentFurther.Should().NotBeNull();
            var jobs = dt.FindAfterName("jobs").Value;
            jobs.Should().NotBeNull();
            jobs.Rows.Count.Should().Be(1);
            var steps = dt.FindAfterName("steps").Value;
            steps.Should().NotBeNull();
            steps.Rows.Count.Should().Be(1);
            steps.Rows[0]["name"].Should().Be("task");
            steps.Rows[0]["displayName"].Should().Be("Xcode@5");


        }
        [Fact]
        public async Task TestXamarin3()
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
            string nameFile = $"{nameof(TestXamarin3)}.txt";
            File.WriteAllText(nameFile, data);
            var visit = new YamlReader(nameFile, Encoding.UTF8);
            var dt = await visit.TransformData(null);
            dt.Should().NotBeNull();
            dt.DataToBeSentFurther.Should().NotBeNull();
            var jobs = dt.FindAfterName("jobs").Value;
            jobs.Should().NotBeNull();
            jobs.Rows.Count.Should().Be(2);
            jobs.Rows[0]["name"].Should().Be("Android");
            jobs.Rows[1]["name"].Should().Be("iOS");
            var steps = dt.FindAfterName("steps");
            steps.Value.Rows.Count.Should().Be(9);
            var dv = new DataView(steps.Value);
            dv.RowFilter = "jobName='Android'";
            dv.Count.Should().Be(5);
            dv.RowFilter = "jobName='iOS'";
            dv.Count.Should().Be(4);


            //var steps = dt.FindAfterName("steps").Value;
            //steps.Should().NotBeNull();
            //steps.Rows.Count.Should().Be(1);
            //steps.Rows[0]["name"].Should().Be("task");
            //steps.Rows[0]["displayName"].Should().Be("Xcode@5");
        }
    }
}
