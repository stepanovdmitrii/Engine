using System;
using System.Collections.Generic;
using Engine.Core.Collections;
using NUnit.Framework;

namespace Engine.Core.UnitTests
{
    public sealed class BinaryTreeTests
    {
        [Test]
        public void CanInsertItems()
        {
            var tree = new BinaryTree<Guid, int>();
            Assert.That(tree.Count == 0);
            Guid key = Guid.NewGuid();
            tree.Insert(key, 0);
            Assert.That(tree.Count == 1);
            tree.Insert(key, 1);
            Assert.That(tree.Count == 2);
        }

        [Test]
        public void CanDeleteItems()
        {
            var tree = new BinaryTree<Guid, int>();
            Guid key = Guid.NewGuid();
            tree.Insert(key, 0);
            tree.Insert(key, 1);
            Assert.That(tree.Count == 2);
            Assert.IsFalse(tree.Delete(Guid.NewGuid()));
            Assert.That(tree.Count == 2);
            Assert.IsTrue(tree.Delete(key));
            Assert.That(tree.Count == 1);
            Assert.IsTrue(tree.Delete(key));
            Assert.That(tree.Count == 0);
            tree.Insert(key, 0);
            Assert.That(tree.Count == 1);
            Assert.IsFalse(tree.Delete(Guid.NewGuid()));
            Assert.IsTrue(tree.Delete(key));
            Assert.IsTrue(tree.Count == 0);
        }

        [Test]
        public void CanFindItems()
        {
            var tree = new BinaryTree<int, int>();
            int key1 = -100;
            int key2 = 100;
            int key3 = 50;

            tree.Insert(key1, key1 * 2);
            tree.Insert(key2, key2 * 2);

            Assert.IsTrue(tree.Find(key1, out int value));
            Assert.AreEqual(key1 * 2, value);
            Assert.IsTrue(tree.Find(key2, out value));
            Assert.AreEqual(key2 * 2, value);
            Assert.IsFalse(tree.Find(key3, out value));
            Assert.AreEqual(default(int), value);
            Assert.IsTrue(tree.FindMin(out value));
            Assert.AreEqual(key1 * 2, value);
            Assert.IsTrue(tree.FindMax(out value));
            Assert.AreEqual(key2 * 2, value);

            tree.Clear();
            value = key1;
            Assert.IsFalse(tree.FindMax(out value));
            Assert.AreEqual(default(int), value);
            value = key1;
            Assert.IsFalse(tree.FindMin(out value));
            Assert.AreEqual(default(int), value);
        }

        [Test]
        public void CanEnumerateItems()
        {
            var tree = new BinaryTree<int, int>();
            int key1 = -100;
            int key2 = 100;
            int key3 = 50;

            tree.Insert(key1, key1 * 2);
            tree.Insert(key2, key2 * 2);

            Assert.AreEqual(2, tree.Count);

            using(IEnumerator<int> enumerator = tree.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.Current == key1 * 2 || enumerator.Current == key2 * 2);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.Current == key1 * 2 || enumerator.Current == key2 * 2);
                Assert.IsFalse(enumerator.MoveNext());
                Assert.AreEqual(default(int), enumerator.Current);
            }

            using (IEnumerator<int> enumerator = tree.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.Current == key1 * 2 || enumerator.Current == key2 * 2);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.Current == key1 * 2 || enumerator.Current == key2 * 2);
                enumerator.Reset();
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.Current == key1 * 2 || enumerator.Current == key2 * 2);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.Current == key1 * 2 || enumerator.Current == key2 * 2);
                Assert.IsFalse(enumerator.MoveNext());
                Assert.AreEqual(default(int), enumerator.Current);
            }

            using (IEnumerator<int> enumerator = tree.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(enumerator.Current == key1 * 2 || enumerator.Current == key2 * 2);
                tree.Insert(key3, key3 * 2);
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => { var cur = enumerator.Current; });
            }
        }
    }
}