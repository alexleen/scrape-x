using NUnit.Framework;
using System;

namespace ScrapeX.Test
{
    public static class Extensions
    {
        public static void AndHasMessage(this Exception exception, string expectedMessage)
        {
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}
