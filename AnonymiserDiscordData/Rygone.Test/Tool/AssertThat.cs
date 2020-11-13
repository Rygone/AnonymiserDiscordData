using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rygone.Test.Tool
{
    class AssertThat
    {
        public static void AreEqual(object actual, IS expected) {
            if (!expected.Equals(actual))
            {
                Assert.Fail($"Expected {expected} but was {actual}");
            }
        }
        public static void AreNotEqual(object actual, IS expected)
        {
            if (expected.Equals(actual))
            {
                Assert.Fail($"Expected not {expected} but was {actual}");
            }
        }
        public static void IsTrue(bool condition) => Assert.IsTrue(condition);
        public static void IsFalse(bool condition) => Assert.IsFalse(condition);
        public static void IsNotNull(object value) => Assert.IsNotNull(value);
        public static void IsNull(object value) => Assert.IsNull(value);
        public static T ThrowsException<T>(Action action) where T : Exception => Assert.ThrowsException<T>(action);
        public static void DoesNotThrowException(Action action)
        {
            try { action(); }
            catch (Exception e) { Assert.Fail($"Expected no {e.GetType()} to be thrown"); }
        }
    }
}