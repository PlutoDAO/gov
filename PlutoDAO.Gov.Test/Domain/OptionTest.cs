using System;
using PlutoDAO.Gov.Domain;
using Xunit;

namespace PlutoDAO.Gov.Test
{
    public class OptionTest
    {
        [Fact]
        public void TestCreateOption()
        {
            var option = new Option("FOR");
            
            Assert.Equal("FOR", option.Name);
            Assert.Throws<ArgumentNullException>(() => new Option(""));
            Assert.Throws<ArgumentNullException>(() => new Option("      "));
            Assert.Throws<ArgumentNullException>(() => new Option(null));
        }
    }
}
