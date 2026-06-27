using IctBaden.RevolutionPi.Model;
using NUnit.Framework;

namespace IctBaden.RevolutionPi.Test
{
    [TestFixture]
    public class RevPiProductNamesTests
    {
        [TestCase(95, "RevPi Core")]
        [TestCase(136, "RevPi Connect 4")]
        [TestCase(0x6001, "ModbusTCP Slave Adapter")]
        public void KnownProductTypesResolveToName(int productType, string expected)
        {
            Assert.That(RevPiProductNames.GetProductName(productType), Is.EqualTo(expected));
        }

        [Test]
        public void UnknownProductTypeFallsBackToPlaceholder()
        {
            Assert.That(RevPiProductNames.GetProductName(99999), Is.EqualTo("Unknown product type (99999)"));
        }
    }
}
