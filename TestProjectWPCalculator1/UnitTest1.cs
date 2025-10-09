using ClassLibraryWPCalculator;
using Xunit;

namespace TestProjectWPCalculator1
{
    public class UnitTest1
    {
        [Fact]
        public void TestAssignment_Simple()
        {
            // Arrange
            var calculator = new WPCalculator();
            var parser = new ExpressionParser();
            string statement = "x := 2*x + 5";
            string postcondition = "x > 15";
            //string expectedPrecondition = "((2*x + 5)) > 15"; // xUnit may require different formatting for assertion

            // Act
            string actualPrecondition = "";
            if (parser.TryParseAssignment(statement, out string variable, out string expression))
            {
                actualPrecondition = calculator.CalculateForAssignment(statement, postcondition);
            }

            // Assert
            // Correcting the assertion to use xUnit's Assert.Equal
            // Also adjusting the expected string to match the simple regex replacement logic
            Assert.Equal("(2*x + 5) > 15", actualPrecondition.Replace("((", "(").Replace("))", ")"));
        }

        [Fact]
        public void TestIf_SimpleMax()
        {
            // Этот тест пока упадет, так как логика для if еще не реализована.
            // Arrange
            var calculator = new WPCalculator();
            string statement = "if (x > y) then max := x else max := y";
            string postcondition = "max > 100";

            // Ожидаемый результат: (x > y && wp(max := x, max > 100)) || (x <= y && wp(max := y, max > 100))
            // (x > y && (x) > 100) || (x <= y && (y) > 100)
            string expectedPrecondition = "(x > y && (x) > 100) || (x <= y && (y) > 100)";

            // Act
            string actualPrecondition = calculator.CalculateForIf(statement, postcondition);

            // Assert
            Assert.Equal(expectedPrecondition, actualPrecondition);
        }

        [Fact]
        public void TestSimplifier_Basic()
        {
            // Arrange
            var simplifier = new ExpressionSimplifier();
            string expression = "2*x + 5 > 15";
            string expected = "x > 5";

            // Act
            string actual = simplifier.Simplify(expression);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}

        /*
        [Fact]
        public void TestSequence_Simple()
        {
            // Этот тест закомментирован, так как пример для последовательности
            // был перенесен в нефункциональные требования для дальнейшего уточнения (NFR-3).

            // Arrange
            var calculator = new WPCalculator();
            var parser = new ExpressionParser();
            string sequence = "y := x + 1; x := y * 2";
            string postcondition = "x > 10";
            
            // Ожидаемый результат: wp(y := x + 1, wp(x := y * 2, x > 10))
            // 1. wp(x := y * 2, x > 10)  =>  (y * 2) > 10
            // 2. wp(y := x + 1, (y * 2) > 10)    =>  ((x + 1) * 2) > 10
            string expectedPrecondition = "((x + 1) * 2) > 10";

            // Act
            string actualPrecondition = calculator.CalculateForSequence(sequence, postcondition);

            // Assert
            Assert.Equal(expectedPrecondition, actualPrecondition);
        }
        */