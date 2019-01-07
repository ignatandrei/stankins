using FluentAssertions;
using Stankins.AzureDevOps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        [Fact]
        public void TestXamarin2()
        {
            var data = (@"# Xamarin.iOS
# Build a Xamarin.iOS app and Xamarin.UITest assembly.
# Add steps that install certificates, test, sign, and distribute the app, save build artifacts, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/xamarin

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
            visit.jobs.Count.Should().Be(1);
            var android = visit.jobs[0];
            android.pool.Value.Should().Be("macOS 10.13");
            android.Steps.Should().NotBeNull();
            android.Steps.Count.Should().Be(4);
        }

        [Fact]
        public void TestXamarin1()
        {
            var data = (@"# Xamarin.Android
# Build a Xamarin.Android app and Xamarin.UITest assembly.
# Add steps that test, sign, and distribute the app, save build artifacts, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/xamarin

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
    pathtoPublish: '$(outputDirectory)'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var android = visit.jobs[0];
            android.Steps.Should().NotBeNull();
            android.Steps.Count.Should().Be(5);
        }
        [Fact]
        public void TestRuby()
        {
            var data = (@"# Ruby
# Package your Ruby application.
# Add steps that install rails, analyze code, save build artifacts, deploy, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/ruby

pool:
  vmImage: 'Ubuntu 16.04'

steps:
- task: UseRubyVersion@0
  inputs:
    versionSpec: '>= 2.5'

- script: |
    gem install bundler
    bundle install --retry=3 --jobs=4
  displayName: 'bundle install'

- script: bundle exec rake
  displayName: 'bundle exec rake'

# - script: bundle exec rspec spec --format RspecJunitFormatter --out test_results/TEST-rspec.xml

- task: PublishTestResults@2
  condition: succeededOrFailed()
  inputs:
    testResultsFiles: '**/test-*.xml'
    testRunTitle: 'Ruby tests'
    
- task: PublishCodeCoverageResults@1
  inputs:
    codeCoverageTool: Cobertura
    summaryFileLocation: '$(System.DefaultWorkingDirectory)/**/coverage.xml'
    reportDirectory: '$(System.DefaultWorkingDirectory)/**/coverage'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(5);
            
        }

        [Fact]
        public void TestJavascript3()
        {
            var data = (@"# Build NodeJS Express app using Azure Pipelines
# https://docs.microsoft.com/azure/devops/pipelines/languages/javascript?view=vsts
pool:
  vmImage: 'Ubuntu 16.04'
  
steps:
- task: NodeTool@0
  inputs:
    versionSpec: '8.x'
  
- task: Npm@1
  displayName: 'npm install'
  inputs:
    command: install

- task: Npm@1
  displayName: 'npm test'
  inputs:
    command: custom
    customCommand: 'test'

- task: PublishTestResults@2
  inputs:
    testResultsFiles: '**/TEST-RESULTS.xml'
    testRunTitle: 'Test results for JavaScript'
  condition: succeededOrFailed()

- task: PublishCodeCoverageResults@1
  inputs: 
    codeCoverageTool: Cobertura
    summaryFileLocation: '$(System.DefaultWorkingDirectory)/**/*coverage.xml'
    reportDirectory: '$(System.DefaultWorkingDirectory)/**/coverage'
    
- task: ArchiveFiles@2
  inputs:
    rootFolderOrFile: '$(System.DefaultWorkingDirectory)'
    includeRootFolder: false

- task: PublishBuildArtifacts@1");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(7);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishBuildArtifacts@1");
            t.Inputs.Count.Should().Be(0);

        }

        [Fact]
        public void TestPHP()
        {
            var data = (@"# PHP
# Test and package your PHP application.
# Add steps that run tests, save build artifacts, deploy, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/php

pool:
  vmImage: 'Ubuntu 16.04'

variables:
  phpVersion: 7.2

steps:
- script: |
    sudo update-alternatives --set php /usr/bin/php$(phpVersion)
    sudo update-alternatives --set phar /usr/bin/phar$(phpVersion)
    sudo update-alternatives --set phpdbg /usr/bin/phpdbg$(phpVersion)
    sudo update-alternatives --set php-cgi /usr/bin/php-cgi$(phpVersion)
    sudo update-alternatives --set phar.phar /usr/bin/phar.phar$(phpVersion)
    php -version
  displayName: 'Use PHP version $(phpVersion)'

- script: composer install --no-interaction --prefer-dist
  displayName: 'composer install'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(2);

        }

        [Fact]
        public void TestJavascript2()
        {
            var data = (@"# Build Docker image for this app using Azure Pipelines
# https://docs.microsoft.com/azure/devops/pipelines/languages/docker?view=vsts
pool:
  vmImage: 'Ubuntu 16.04'

variables:
  imageName: 'nodejssample:$(Build.BuildId)'
  # define two more variables dockerId and dockerPassword in the build pipeline in UI

steps:
- script: |
    npm install
    npm test
    docker build -f Dockerfile -t $(dockerId).azurecr.io/$(imageName) .
    docker login -u $(dockerId) -p $pswd $(dockerId).azurecr.io
    docker push $(dockerId).azurecr.io/$(imageName)
  env:
    pswd: $(dockerPassword)

- task: PublishTestResults@2
  condition: succeededOrFailed()
  inputs:
    testResultsFiles: '**/TEST-RESULTS.xml'
    testRunTitle: 'Test results for JavaScript'

- task: PublishCodeCoverageResults@1
  inputs: 
    codeCoverageTool: Cobertura
    summaryFileLocation: '$(System.DefaultWorkingDirectory)/**/*coverage.xml'
    reportDirectory: '$(System.DefaultWorkingDirectory)/**/coverage'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(3);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishCodeCoverageResults@1");
            t.Inputs.Count.Should().Be(3);

        }

        [Fact]
        public void TestJavascript1()
        {
            var data = (@"# Build Docker image for this app using Azure Pipelines
# https://docs.microsoft.com/azure/devops/pipelines/languages/docker?view=vsts
pool:
  vmImage: 'Ubuntu 16.04'
  
variables:
  imageName: 'nodejssample:$(Build.BuildId)'
  # define two more variables dockerId and dockerPassword in the build pipeline in UI

steps:
- script: |
    npm install
    npm test
    docker build -f Dockerfile -t $(dockerId)/$(imageName) .
    docker login -u $(dockerId) -p $pswd
    docker push $(dockerId)/$(imageName)
  env:
    pswd: $(dockerPassword)

- task: PublishTestResults@2
  condition: succeededOrFailed()
  inputs:
    testResultsFiles: '**/TEST-RESULTS.xml'
    testRunTitle: 'Test results for JavaScript'

- task: PublishCodeCoverageResults@1
  inputs: 
    codeCoverageTool: Cobertura
    summaryFileLocation: '$(System.DefaultWorkingDirectory)/**/*coverage.xml'
    reportDirectory: '$(System.DefaultWorkingDirectory)/**/coverage'
    ");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(3);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishCodeCoverageResults@1");
            t.Inputs.Count.Should().Be(3);

        }
        [Fact]
        public void TestJava2()
        {
            var data = (@"# Gradle
# Build your Java projects and run tests with Gradle using a Gradle wrapper script.
# Add steps that analyze code, save build artifacts, deploy, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/java

pool:
  vmImage: 'Ubuntu 16.04'

steps:
- task: Gradle@2
  inputs:
    workingDirectory: ''
    gradleWrapperFile: 'gradlew'
    gradleOptions: '-Xmx3072m'
    javaHomeOption: 'JDKVersion'
    jdkVersionOption: '1.10'
    jdkArchitectureOption: 'x64'
    publishJUnitResults: true
    testResultsFiles: '**/TEST-*.xml'
    tasks: 'build'

# Publish Cobertura or JaCoCo code coverage results from a build
- task: PublishCodeCoverageResults@1
  inputs:
    codeCoverageTool: 'JaCoCo' 
    summaryFileLocation: '$(System.DefaultWorkingDirectory)/**/reports/code-cov-report.xml'
    reportDirectory: '$(System.DefaultWorkingDirectory)/**/reports/code-cov-report.html'
    failIfCoverageEmpty: true
    
- task: CopyFiles@2
  inputs:
    contents: '**/helloworld*.jar'
    targetFolder: '$(build.artifactStagingDirectory)'

- task: PublishBuildArtifacts@1
  inputs:
    artifactName: 'jars'
    pathToPublish: '$(build.artifactStagingDirectory)'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(4);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishBuildArtifacts@1");
            t.Inputs.Count.Should().Be(2);

        }

        [Fact]
        public void TestJava1()
        {
            var data = (@"# Maven
# Build your Java projects and run tests with Apache Maven.
# Add steps that analyze code, save build artifacts, deploy, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/java

pool:
  vmImage: 'Ubuntu 16.04'

steps:
- task: Maven@3
  inputs:
    mavenPomFile: 'pom.xml'
    mavenOptions: '-Xmx3072m'
    javaHomeOption: 'JDKVersion'
    jdkVersionOption: '1.10'
    jdkArchitectureOption: 'x64'
    publishJUnitResults: true
    testResultsFiles: '**/TEST-*.xml'
    goals: 'package'

# Publish Cobertura or JaCoCo code coverage results from a build
- task: PublishCodeCoverageResults@1
  inputs:
    codeCoverageTool: 'JaCoCo' 
    summaryFileLocation: '$(System.DefaultWorkingDirectory)/**/site/jacoco/jacoco.xml'
    reportDirectory: '$(System.DefaultWorkingDirectory)/**/site/jacoco'
    failIfCoverageEmpty: true

- task: CopyFiles@2
  inputs:
    contents: '**/*.war'
    targetFolder: '$(build.artifactStagingDirectory)'

- task: PublishBuildArtifacts@1
  inputs:
    artifactName: 'war'
    pathToPublish: '$(build.artifactStagingDirectory)'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(4);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishBuildArtifacts@1");
            t.Inputs.Count.Should().Be(2);

        }

        [Fact]
        public void TestGO1()
        {
            var data = (@"# Go
# Build and test your Go application.
# Add steps that save build artifacts, deploy, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/go

pool:
  vmImage: 'Ubuntu 16.04'

variables:
  GOBIN:  '$(GOPATH)/bin' # Go binaries path
  GOROOT: '/usr/local/go1.11' # Go installation path
  GOPATH: '$(system.defaultWorkingDirectory)/gopath' # Go workspace path
  modulePath: '$(GOPATH)/src/github.com/$(build.repository.name)' # Path to the module's code

steps:
- script: |
    mkdir -p '$(GOBIN)'
    mkdir -p '$(GOPATH)/pkg'
    mkdir -p '$(modulePath)'
    shopt -s extglob
    mv !(gopath) '$(modulePath)'
    echo '##vso[task.prependpath]$(GOBIN)'
    echo '##vso[task.prependpath]$(GOROOT)/bin'
  displayName: 'Set up the Go workspace'

- script: |
    go version
    go get -v -t -d ./...
    if [ -f Gopkg.toml ]; then
        curl https://raw.githubusercontent.com/golang/dep/master/install.sh | sh
        dep ensure
    fi
    go get github.com/jstemmer/go-junit-report
    go get github.com/axw/gocov/gocov
    go get github.com/AlekSi/gocov-xml
    go get -u gopkg.in/matm/v1/gocov-html
  workingDirectory: '$(modulePath)'
  displayName: 'Get dependencies'
  
- script: |
    go test -v -coverprofile=coverage.txt -covermode count 2>&1 | go-junit-report > junit.xml
    gocov convert coverage.txt > coverage.json    
    gocov-xml < coverage.json > coverage.xml
    gocov-html < coverage.json > index.html
    mkdir reports
    cp junit.xml ./reports/junit.xml
    cp coverage.xml ./reports/coverage.xml
    cp index.html ./reports/index.html
  continueOnError: 'true'
  workingDirectory: '$(modulePath)'
  displayName: 'Run unit tests'
  
- script: |
    go build -v
  workingDirectory: '$(modulePath)'
  continueOnError: 'true'
  displayName: 'Build app'

- task: PublishTestResults@2
  inputs:
    testRunner: JUnit
    testResultsFiles: '$(modulePath)/reports/junit.xml'

- task: PublishCodeCoverageResults@1
  inputs:
    codeCoverageTool: 'cobertura'
    summaryFileLocation: '$(modulePath)/reports/coverage.xml'
    reportDirectory: '$(modulePath)/reports/'

- task: PublishPipelineArtifact@0");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(7);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishPipelineArtifact@0");


        }
        [Fact]
        public void TestGO2()
        {
            var data = (@"# Docker image
# Build a Docker image to run, deploy, or push to a container registry.
# Add steps that use Docker Compose, tag images, push to a registry, run an image, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/docker

pool:
  vmImage: 'Ubuntu 16.04'

variables:
  imageName: 'pipelines-go:$(build.buildId)'

steps:
- script: docker build -f Dockerfile -t $(imageName) .
  displayName: 'docker build'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(1);
            var s = j.Steps.Last();
            s.Should().BeOfType<Script>();
            var t = s as Script;
            t.DisplayName.Should().Be("docker build");
            

        }
        [Fact]
        public void TestAndroid()
        {
            var data = (@"# Android
# Build your Android project with Gradle.
# Add steps that test, sign, and distribute the APK, save build artifacts, and more:
# https://docs.microsoft.com/azure/devops/pipelines/languages/android

pool:
  vmImage: 'macOS 10.13'

steps:
- task: Gradle@2
  inputs:
    workingDirectory: ''
    gradleWrapperFile: 'gradlew'
    gradleOptions: '-Xmx3072m'
    publishJUnitResults: false
    testResultsFiles: '**/TEST-*.xml'
    tasks: 'assembleDebug'

- task: CopyFiles@2
  inputs:
    contents: '**/*.apk'
    targetFolder: '$(build.artifactStagingDirectory)'

- task: PublishBuildArtifacts@1
  inputs:
    pathToPublish: '$(build.artifactStagingDirectory)'
    artifactName: 'drop'
    artifactType: 'container'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("macOS 10.13");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(3);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishBuildArtifacts@1");


        }

        [Fact]
        public void TestAzureDocker()
        {
            var data = (@"# Build Docker image for this app using Azure Pipelines
# http://docs.microsoft.com/azure/devops/pipelines/languages/docker?view=vsts
pool:
  vmImage: 'Ubuntu 16.04'

variables:
  buildConfiguration: 'Release'
  imageName: 'dotnetcore:$(Build.BuildId)'
  # define two more variables dockerId and dockerPassword in the build pipeline in UI

steps:
- script: |
    dotnet build --configuration $(buildConfiguration)
    dotnet test dotnetcore-tests --configuration $(buildConfiguration) --logger trx
    dotnet publish --configuration $(buildConfiguration) --output out
    docker build -f Dockerfile -t $(dockerId)/$(imageName) .
    docker login -u $(dockerId) -p $pswd
    docker push $(dockerId)/$(imageName)
  env:
    pswd: $(dockerPassword)

- task: PublishTestResults@2
  condition: succeededOrFailed()
  inputs:
    testRunner: VSTest
    testResultsFiles: '**/*.trx'");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(2);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishTestResults@2");
            //TODO: add condition


        }

        [Fact]
        public void TestAzureACR()
        {
            var data = (@"# Build Docker image for this app using Azure Pipelines
# http://docs.microsoft.com/azure/devops/pipelines/languages/docker?view=vsts
pool:
  vmImage: 'Ubuntu 16.04'

variables:
  buildConfiguration: 'Release'
  imageName: 'dotnetcore:$(Build.BuildId)'
  # define two more variables dockerId and dockerPassword in the build pipeline in UI

steps:
- script: |
    dotnet build --configuration $(buildConfiguration)
    dotnet test dotnetcore-tests --configuration $(buildConfiguration) --logger trx
    dotnet publish --configuration $(buildConfiguration) --output out
    docker build -f Dockerfile -t $(dockerId).azurecr.io/$(imageName) .
    docker login -u $(dockerId) -p $pswd $(dockerid).azurecr.io
    docker push $(dockerId).azurecr.io/$(imageName)
  env:
    pswd: $(dockerPassword)

- task: PublishTestResults@2
  condition: succeededOrFailed()
  inputs:
    testRunner: VSTest
    testResultsFiles: '**/*.trx'

#- script: |
#    docker-compose -f docs/docker-compose.yml --project-directory . -p docs up -d |
#    docker wait docs_sut_1 |
#    docker-compose -f docs/docker-compose.yml --project-directory . down");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(2);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishTestResults@2");
            //TODO: add condition


        }

        [Fact]
        public void TestDotNetCore()
        {
            var data = (@"# Build ASP.NET Core project using Azure Pipelines
# https://docs.microsoft.com/azure/devops/pipelines/languages/dotnet-core?view=vsts

pool:
  vmImage: 'Ubuntu 16.04'
  
variables:
  buildConfiguration: 'Release'

steps:
- script: |
    dotnet build --configuration $(buildConfiguration)
    dotnet test dotnetcore-tests --configuration $(buildConfiguration) --logger trx
    dotnet publish --configuration $(buildConfiguration) --output $BUILD_ARTIFACTSTAGINGDIRECTORY
- task: PublishTestResults@2
  condition: succeededOrFailed()
  inputs:
    testRunner: VSTest
    testResultsFiles: '**/*.trx'

- task: PublishBuildArtifacts@1");


            var visit = new YamlDevOpsVisitor();
            visit.LoadFromString(data);

            visit.jobs.Should().NotBeNull();
            visit.jobs.Count.Should().Be(1);
            var j = visit.jobs[0];
            j.pool.Value.Should().Be("Ubuntu 16.04");
            j.Steps.Should().NotBeNull();
            j.Steps.Count.Should().Be(3);
            var s = j.Steps.Last();
            s.Should().BeOfType<TaskYaml>();
            var t = s as TaskYaml;
            t.Value.Should().Be("PublishBuildArtifacts@1");
            //TODO: add condition


        }

    }

}
