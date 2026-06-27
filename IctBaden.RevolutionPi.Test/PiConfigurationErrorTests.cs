using System.Collections.Generic;
using System.IO;
using IctBaden.RevolutionPi.Configuration;
using NUnit.Framework;

namespace IctBaden.RevolutionPi.Test
{
    /// <summary>
    /// Error / edge-case paths of <see cref="PiConfiguration"/> (missing, empty, malformed,
    /// or device-less config). The happy path lives in <see cref="ConfigurationTests"/>.
    /// </summary>
    [TestFixture]
    public class PiConfigurationErrorTests
    {
        private readonly List<string> _tempFiles = new();

        [TearDown]
        public void Cleanup()
        {
            foreach (var file in _tempFiles)
            {
                if (File.Exists(file)) File.Delete(file);
            }
            _tempFiles.Clear();
        }

        private PiConfiguration ConfigWith(string content)
        {
            var path = Path.Combine(Path.GetTempPath(), $"revpi-test-{Path.GetRandomFileName()}.rsc");
            File.WriteAllText(path, content);
            _tempFiles.Add(path);
            return new PiConfiguration { RevPiConfigFileName = path };
        }

        [Test]
        public void MissingFileDoesNotOpen()
        {
            var config = new PiConfiguration
            {
                RevPiConfigFileName = Path.Combine(Path.GetTempPath(), "revpi-does-not-exist.rsc")
            };

            Assert.That(config.Open(), Is.False);
            Assert.That(config.IsOpen, Is.False);
            Assert.That(config.Devices, Is.Empty);
            Assert.That(config.GetVariable("RevPiStatus"), Is.Null);
        }

        [Test]
        public void EmptyFileDoesNotOpen()
        {
            var config = ConfigWith("   \n  ");
            Assert.That(config.Open(), Is.False);
            Assert.That(config.IsOpen, Is.False);
        }

        [Test]
        public void MalformedJsonDoesNotOpen()
        {
            var config = ConfigWith("{ this is not valid json");
            Assert.That(config.Open(), Is.False);
            Assert.That(config.Devices, Is.Empty);
        }

        [Test]
        public void ValidJsonWithoutDevicesYieldsNoDevices()
        {
            var config = ConfigWith("{ \"App\": { \"name\": \"x\" } }");

            Assert.That(config.Open(), Is.True);   // parsed fine...
            Assert.That(config.Devices, Is.Empty);  // ...but there is no Devices array
        }
    }
}
