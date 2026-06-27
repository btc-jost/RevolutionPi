using IctBaden.RevolutionPi.Configuration;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace IctBaden.RevolutionPi.Test
{
    [TestFixture]
    public class ConfigurationTests
    {
        const string ConfigFileName = "config.json";
        private PiConfiguration _configuration;

        [SetUp]
        public void TestSetup()
        {
            var assembly = Assembly.GetAssembly(GetType());
            var json = ResourceLoader.LoadAsString(assembly, $"IctBaden.RevolutionPi.Test.{ConfigFileName}");
            var path = Path.GetDirectoryName(assembly.Location) ?? ".";
            var fullName = Path.Combine(path, ConfigFileName);
            File.WriteAllText(fullName, json);

            _configuration = new PiConfiguration
            {
                RevPiConfigFileName = fullName
            };
        }

        [Test]
        public void OpenConfigFileShouldSucceed()
        {
            var opened = _configuration.Open();
            Assert.That(opened);
        }

        [Test]
        public void ConfigShouldHaveDevices()
        {
            _configuration.Open();
            Assert.That(_configuration.Devices.Count, Is.EqualTo(1));
        }

        [Test]
        public void ConfigShouldHaveInputs()
        {
            _configuration.Open();
            Assert.That(_configuration.Devices[0].Inputs.Length, Is.EqualTo(5));
        }

        [Test]
        public void ConfigShouldHaveOutputs()
        {
            _configuration.Open();
            Assert.That(_configuration.Devices[0].Outputs.Length, Is.EqualTo(3));
        }
    }
}
