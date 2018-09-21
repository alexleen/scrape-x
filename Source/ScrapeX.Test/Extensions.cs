// Copyright © 2018 Alex Leendertsen

using System;
using NUnit.Framework;

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
