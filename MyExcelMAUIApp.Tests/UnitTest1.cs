using Xunit;
using MyExcelMAUIApp;

namespace MyExcelMAUIApp.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test_Column1_Is_A()
        {
            // Перевіряємо, чи 1-й стовпець правильно перетворюється на "A"
            string result = MainPage.GetColumnName(1);
            Assert.Equal("A", result);
        }

        [Fact]
        public void Test_Column26_Is_Z()
        {
            // Перевіряємо, чи 26-й стовпець перетворюється на "Z" (кінець алфавіту)
            string result = MainPage.GetColumnName(26);
            Assert.Equal("Z", result);
        }

        [Fact]
        public void Test_Column27_Is_AA()
        {
            // Перевіряємо, чи 27-й стовпець перетворюється на "AA" (перехід на 2 букви)
            string result = MainPage.GetColumnName(27);
            Assert.Equal("AA", result);
        }
    }
}