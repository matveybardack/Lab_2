using ClassLibraryWPCalculator;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace TestProjectWPCalculator1
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAssignment_Simple()
        {
            // Arrange
            var calculator = new WPCalculator();
            var parser = new ExpressionParser();
            string statement = "x := 2*(x + 1) + 5";
            string postcondition = "x > 15";
            //string expectedPrecondition = "((2*x + 5)) > 15";

            // Act
            string actualPrecondition = "";
            if (parser.TryParseAssignment(statement))
            {
                actualPrecondition = calculator.CalculateForAssignment(statement, postcondition);
            }

            Assert.Equal("x > 4", actualPrecondition);
        }

        [Fact]
        public void TestIf_SimpleMax()
        {
            var calculator = new WPCalculator();
            string statement = "if (x > y) then max := x else max := y";
            string postcondition = "max > 100";

            // Ожидаемый результат: (x > y && wp(max := x, max > 100)) || (x <= y && wp(max := y, max > 100))
            // (x > y && x > 100) || (x <= y && y > 100)
            string expectedPrecondition = "(x > y && x > 100) || (x <= y && y > 100)";

            // Act
            string actualPrecondition = calculator.CalculateForIf(statement, postcondition);

            // Assert
            Assert.Equal(expectedPrecondition, actualPrecondition);
        }

        [Fact]
        public void TestSimplifier_Basic()
        {
            // Arrange
            var simplifier = new InequalitySimplifier();
            string expression = "(2*max + 5) > 15";
            string expected = "max > 5";

            // Act
            string actual = simplifier.SimplificateInequality(expression);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestSequence_Simple()
        {
            // Arrange
            var calc = new WPCalculator();
            var assignments = new Stack<string>();
            assignments.Push("x := 2*x + 10");
            assignments.Push("y := x + 2");
            string post = "y > 15";

            // Act
            string result = calc.CalculateForSequence(assignments, post);

            // Assert
            Assert.Equal("x > 1,5", result);

            foreach (var item in WpTrace.GetAll())
                _output.WriteLine(item);
        }
    }
}
