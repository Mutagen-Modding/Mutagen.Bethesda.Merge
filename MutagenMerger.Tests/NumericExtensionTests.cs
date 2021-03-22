﻿using System;
using MutagenMerger.Pex.Extensions;
using Xunit;

namespace MutagenMerger.Tests
{
    public class NumericExtensionTests
    {
        [Fact]
        public void TestTimeConversionToUInt64()
        {
            var expected = new DateTime(2021, 03, 22, 19, 54, 23);

            var numericValue = expected.ToUInt64();
            var actual = numericValue.ToDateTime();
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestTimeConversionFromUInt64()
        {
            const ulong expected = 1596043501;

            var dateTimeValue = expected.ToDateTime();
            var actual = dateTimeValue.ToUInt64();
            
            Assert.Equal(expected, actual);
        }
    }
}
