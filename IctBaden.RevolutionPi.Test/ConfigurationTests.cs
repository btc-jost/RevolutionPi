using System.IO;
using System.Reflection;
using IctBaden.RevolutionPi.Configuration;
using IctBaden.RevolutionPi.Model;
using NUnit.Framework;

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

        [Test]
        public void DeviceScalarFieldsAreParsed()
        {
            _configuration.Open();
            var device = _configuration.Devices[0];

            // productType ("95") and position ("0") are JSON strings in config.rsc.
            Assert.That(device.Name, Is.EqualTo("RevPi Core V1.2"));
            Assert.That(device.ProductType, Is.EqualTo(95));
            Assert.That(device.Position, Is.EqualTo(0));
            Assert.That(device.Offset, Is.EqualTo(0));
        }

        [Test]
        public void VariableFieldsAreParsed()
        {
            _configuration.Open();

            var status = _configuration.GetVariable("RevPiStatus");
            Assert.That(status, Is.Not.Null);
            Assert.That(status!.Type, Is.EqualTo(VariableType.Input));
            Assert.That(status.Length, Is.EqualTo(8));
            Assert.That(status.LengthText, Is.EqualTo("BYTE"));
            Assert.That(status.Address, Is.EqualTo(0));

            // 16-bit output, address parsed from the JSON string "9".
            var limit = _configuration.GetVariable("RS485ErrorLimit2");
            Assert.That(limit, Is.Not.Null);
            Assert.That(limit!.Type, Is.EqualTo(VariableType.Output));
            Assert.That(limit.Length, Is.EqualTo(16));
            Assert.That(limit.LengthText, Is.EqualTo("WORD"));
            Assert.That(limit.Address, Is.EqualTo(9));
        }
    }
}
