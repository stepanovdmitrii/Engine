using System;
using Engine.Core.Concurrent;
using NUnit.Framework;

namespace Engine.Core.UnitTests
{
    public sealed class ConcurrentStackTests
    {
        private class Stub
        {

        }

        [Test]
        public void ReturnsNullWhenEmpty()
        {
            var stack = new ConcurrentStack<String>();
            Assert.IsNull(stack.Pop());
            Assert.IsNull(stack.Pop());
            Assert.IsNull(stack.Pop());
        }

        [Test]
        public void ReturnsCorrectValues()
        {
            Stub first = new Stub();
            Stub second = new Stub();
            Stub third = new Stub();

            var stack = new ConcurrentStack<Stub>();
            stack.Push(first);
            Assert.AreEqual(first, stack.Pop());
            Assert.AreEqual(null, stack.Pop());

            stack.Push(first);
            stack.Push(second);
            Assert.AreEqual(second, stack.Pop());
            Assert.AreEqual(first, stack.Pop());
            Assert.AreEqual(null, stack.Pop());

            stack.Push(first);
            stack.Push(second);
            stack.Push(third);
            Assert.AreEqual(third, stack.Pop());
            Assert.AreEqual(second, stack.Pop());
            Assert.AreEqual(first, stack.Pop());
            Assert.AreEqual(null, stack.Pop());
        }

        [Test]
        public void ThrowsExceptionOnNullValue()
        {
            var stack = new ConcurrentStack<Stub>();
            Assert.Throws<ArgumentNullException>(() => stack.Push(null));
        }
    }
}
