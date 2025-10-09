using Xunit;
using ClassLibraryWPCalculator;

namespace TestProjectWPCalculator
{
    public class UnitTest1
    {
        [Fact]
        public void TestAssignment_Simple()
        {
            // Arrange
            var calculator = new WPCalculator();
            string assignment = "x := 2*x + 5";
            string postcondition = "x > 15";
            string expectedPrecondition = "(2*x + 5) > 15";

            // Act
            string actualPrecondition = calculator.CalculateForAssignment(assignment, postcondition);

            // Assert
            Assert.Equal(expectedPrecondition, actualPrecondition);
        }
    }
}