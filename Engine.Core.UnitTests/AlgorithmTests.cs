using Engine.Core.Algorithms;
using NUnit.Framework;

namespace Engine.Core.UnitTests
{
    public sealed class AlgorithmTests
    {
        [Test]
        public void LowerBound()
        {
            int[] data = new[] { 1, 2, 4, 5, 5, 6 };
            var lowerBound = new LowerBound<int>(data);
            int index = -1;
            Assert.IsTrue(lowerBound.Find(0, out index) && index == 0);
            Assert.IsTrue(lowerBound.Find(1, out index) && index == 0);
            Assert.IsTrue(lowerBound.Find(2, out index) && index == 1);
            Assert.IsTrue(lowerBound.Find(3, out index) && index == 2);
            Assert.IsTrue(lowerBound.Find(4, out index) && index == 2);
            Assert.IsTrue(lowerBound.Find(5, out index) && index == 3);
            Assert.IsTrue(lowerBound.Find(6, out index) && index == 5);
            Assert.IsTrue(false == lowerBound.Find(7, out index) && index == -1);
        }
    }
}
