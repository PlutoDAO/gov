using System;
using Answap.Gov.Domain;
using Xunit;

namespace Answap.Gov.Test
{
    public class OptionTest
    {
        [Fact]
        public void TestCreateOption()
        {
            var option = new Option("FOR");
            
            Assert.Equal("FOR", option.Name);
            Assert.Throws<ArgumentNullException>(() => new Option( ""));
            Assert.Throws<ArgumentNullException>(() => new Option("      "));
            Assert.Throws<ArgumentNullException>(() => new Option(null));
        }
    }
}