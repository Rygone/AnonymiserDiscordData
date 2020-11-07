using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnonymiserDiscordData.Test.Tool
{
    class AssertThat
    {
        public static void AreEqual(object expected, IS actual) {
            if (!actual.Equals(expected))
            {
                Assert.Fail($"Expected {expected} but was {actual}");
            }
        }
        public static void AreNotEqual(object expected, IS actual)
        {
            if (actual.Equals(expected))
            {
                Assert.Fail($"Expected not {expected} but was {actual}");
            }
        }
        public static void IsTrue(bool condition) => IsTrue(IS.a(condition));
        public static void IsTrue(IS condition)
        {
            if (condition.Object is bool)
                Assert.IsTrue((bool)condition.Object);
            else
                Assert.Fail($"Condition is not a bool but a {condition.Object.GetType()}");
        }
        public static void IsFalse(bool condition) => IsFalse(IS.a(condition));
        public static void IsFalse(IS condition)
        {
            if (condition.Object is bool)
                Assert.IsFalse((bool)condition.Object);
            else
                Assert.Fail($"Condition is not a bool but a {condition.Object.GetType()}");
        }
        public static void IsNotNull(IS value) => Assert.IsNotNull(value.Object);
        public static void IsNull(IS value) => Assert.IsNull(value.Object);
        public static T ThrowsException<T>(Action action) where T : Exception => Assert.ThrowsException<T>(action);
        public static void DoesNotThrowException(Action action)
        {
            try { action(); }
            catch (Exception e) { Assert.Fail($"Expected no {e.GetType()} to be thrown"); }
        }
    }
}