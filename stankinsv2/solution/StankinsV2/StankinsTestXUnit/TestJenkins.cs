using FluentAssertions;
using Stankins.Alive;
using Stankins.Interfaces;
using Stankins.Interpreter;
using Stankins.Jenkins;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbehave;
using Xunit;

namespace StankinsTestXUnit
{
    [Trait("Jenkins", "")]
    [Trait("ExternalDependency", "Jenkins")]
    public class TestJenkins
    {
        static TestJenkins()
        {
            JenkinsConnection = Environment.GetEnvironmentVariable("jenkins");
            if (string.IsNullOrWhiteSpace(JenkinsConnection))
            {
                JenkinsConnection = "localhost";
            }
            JenkinsConnection = "http://"+ JenkinsConnection+":8080";

        }
        public static IEnumerable<object[]> TestSimpleJSONData()
        {

            return new List<object[]>
            {
                new object[] { JenkinsConnection ,1 }
            };
        }
        public static string JenkinsConnection;
        [Scenario]
        [MemberData(nameof(TestSimpleJSONData))]
        public void TestSimpleJSON(string url, int numberTables)
        {
            IReceive receiver = null;

            IDataToSent data = null;
            string nl = Environment.NewLine;

            $"When I create the {nameof(JenkinsJson)} for the {url}".w(() => receiver = new JenkinsJson(url));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().BeGreaterOrEqualTo(numberTables);
            });

        }

        public static IEnumerable<object[]> TestAliveData()
        {

            return new List<object[]>
            {
                new object[] { JenkinsConnection ,3 }
            };
        }
        
        [Scenario]
        [MemberData(nameof(TestAliveData))]
        public void TestAlive(string url, int numberTables)
        {
            IReceive receiver = null;

            IDataToSent data = null;
            string nl = Environment.NewLine;

            $"When I create the {nameof(ReceiverJenkins)} for the {url}".w(() => receiver = new ReceiverJenkins(url));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().BeGreaterOrEqualTo(numberTables);
            });
            $"and should have data success".w(() =>
            {
                var res = AliveStatus.FromTable(data.DataToBeSentFurther.First().Value);
                res.Should().NotBeNull();
                res.Count().Should().Be(1);
                res.First().IsSuccess.Should().Be(true);
            });

        }
        public static IEnumerable<object[]> TestRecipeWithJenkinsDataData()
        {

            return new List<object[]>
            {
                new object[] { JenkinsConnection }
            };
        }
        
        [Scenario]
        [MemberData(nameof(TestRecipeWithJenkinsDataData))]
        public void TestRecipeWithJenkinsData(string url)
        {
            var rec = @"Stankins.Alive.ReceiverJenkins url=""" + TestJenkins.JenkinsConnection + @"""
StankinsObjects.FilterTablesWithColumn namecolumntokeep=Process
StankinsObjects.ChangeColumnName oldname=To newname=JenkinsTo
StankinsObjects.FilterRemoveColumn namecolumn=To
StankinsObjects.AddColumnRegex columnname=JenkinsTo expression=(?:http://)((?<Domain>.+):)
StankinsObjects.ChangeColumnName oldname=Domain newname=To
StankinsObjects.FilterRemoveColumnDataLessThan nameColumn=Duration value=10";
            RecipeFromString r=null;
            $"when I create a recipe with jenkins".w(() => r = new RecipeFromString(rec));
            $"and I transform".w(async () => await r.TransformData(null));

        }
        public static IEnumerable<object[]> TestLastBuildJobData()
        {

            return new List<object[]>
            {
                new object[] { JenkinsConnection, "newmyjob", 1 }
            };
        }
        [Scenario]
        [MemberData(nameof(TestLastBuildJobData))]
        public void TestLastBuildJob(string url,string job, int numberTables)
        {

            //if this does not work and you have already a jenkins running in docker-compose
            //run this 
            //docker exec docker_jenkins_1 bash -c "cat /var/jenkins_home/jenkinsjob.xml | java -jar /var/jenkins_home/war/WEB-INF/jenkins-cli.jar -s http://localhost:8080 create-job newmyjob"
        
            IReceive receiver = null;

            IDataToSent data = null;
            string nl = Environment.NewLine;

            $"When I create the {nameof(ReceiverJenkinsJobLastBuild)} for the {url}".w(() => receiver = new ReceiverJenkinsJobLastBuild(url,job));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().BeGreaterOrEqualTo(numberTables);
            });
            $"and should have data success".w(() =>
            {
                var res = AliveStatus.FromTable(data.DataToBeSentFurther.First().Value);
                res.Should().NotBeNull();
                res.Count().Should().Be(1);
                res.First().IsSuccess.Should().Be(true);
            });

        }

        public static IEnumerable<object[]> TestAliveJobData()
        {

            return new List<object[]>
            {
                new object[] { JenkinsConnection, "newmyjob", 1 }
            };
        }
        [Scenario]
        [MemberData(nameof(TestAliveJobData))]        
        public void TestAliveJob(string url,string job, int numberTables)
        {

            //if this does not work and you have already a jenkins running in docker-compose
            //run this 
            //docker exec docker_jenkins_1 bash -c "cat /var/jenkins_home/jenkinsjob.xml | java -jar /var/jenkins_home/war/WEB-INF/jenkins-cli.jar -s http://localhost:8080 create-job newmyjob"
        
            IReceive receiver = null;

            IDataToSent data = null;
            string nl = Environment.NewLine;

            $"When I create the {nameof(ReceiverJenkinsJob)} for the {url}".w(() => receiver = new ReceiverJenkinsJob(url,job));
            $"And I read the data".w(async () => data = await receiver.TransformData(null));
            $"Then should be a data".w(() => data.Should().NotBeNull());
            $"With {numberTables} table".w(() =>
            {
                data.DataToBeSentFurther.Should().NotBeNull();
                data.DataToBeSentFurther.Count.Should().BeGreaterOrEqualTo(numberTables);
            });
            $"and should have data success".w(() =>
            {
                var res = AliveStatus.FromTable(data.DataToBeSentFurther.First().Value);
                res.Should().NotBeNull();
                res.Count().Should().Be(1);
                res.First().IsSuccess.Should().Be(true);
            });

        }

    }
}
