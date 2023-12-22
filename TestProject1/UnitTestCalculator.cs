namespace TestCalculator;

using Calculator;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    //[Test, Category("Positive scenario")]
    //public void ComputesExpressionWithSingleNum()
    //{
    //    string expression = "2";

    //    var actual = Execution.Exec(expression); //Calculator.Compute(expression);

    //    //double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    //    double expected = 2;

    //    Assert.That(actual, Is.EqualTo(expected));
    //}
    private static IEnumerable<string> NumberTestCases()
    {
        yield return @"
        var a = 10;
        var b = 20;
        return a + b;
    ";
        yield return @"
        var x = 3.14;
        var y = 2.5;
        return x * y;
    ";
        yield return @"
        var c = 10;
        var d = 3;
        return c / d;        // Деление целых чисел (результат - целое число)
    ";
        yield return @"
        var e = 5;
        var f = 2;
        return e % f;        // Остаток от деления (5 % 2 = 1)
    ";
    }

    private static IEnumerable<string> ObjectTestCases()
    {
        yield return @"
        var obj = {
            name: 'John',
            age: 30,
            city: 'New York'
        };
        return obj.name;
    ";
        yield return @"
        var person = {
            name: 'Alice',
            age: 25,
            city: 'London'
        };
        return person.age;
    ";
        yield return @"
        var car = {
            brand: 'Tesla',
            model: 'Model S',
            year: 2022
        };
        car.year = 2023;     // Изменение значения свойства объекта
        return car.year;
    ";
        yield return @"
        var book = {
            title: 'Harry Potter',
            author: 'J.K. Rowling',
            price: 29.99
        };
        return book.pages;   // Обращение к несуществующему свойству объекта
    ";
    }

    [TestCase(".5", .5)]
    [TestCase("5.0", 5)]
    [TestCase("5.0e0", 5)]
    [TestCase("5.0e-0", 5)]
    [TestCase("5.0e+0", 5)]
    [TestCase("50e-1", 5)]
    [TestCase("500E-2", 5)]
    [TestCase("0.500E1", 5)]
    [TestCase("0.500E+1", 5)]
    [TestCase("0.5e+1", 5)]
    [TestCase(".5e+1", 5)]
    [TestCase("2.0+3.0", 5)]
    [TestCase("2+3.0", 5)]
    [TestCase("2.0+3", 5)]
    [TestCase("2.1+2.9", 5)]
    [TestCase("  2.1    +   2.9 ", 5)]
    [TestCase("7.0 - 2", 5)]
    [TestCase("7 - 2.0", 5)]
    [TestCase("7.0 - 2.0", 5)]
    [TestCase(" - 2.0 + 7.0", 5)]
    [TestCase(" -2.0 + 7.000", 5)]
    [Test, Category("Positive scenario")]
    public void ComputesDoubleExpression(string expression, double expected)
    {
        Token actual = Execution.CalcExpression(expression); //Calculator.Compute(expression);
        var actualDouble = (actual as TokenConstant<double>).value;

        const double tolerance = 1e-200;


             Assert.That(Math.Abs(actualDouble/expected-1), Is.LessThanOrEqualTo(tolerance));
    }

    [TestCase(" 2 +3 ", 5)]
    [TestCase(" +2 +3 ", 5)]
    [TestCase("7 - 2", 5)]
    [TestCase(" - 2 + 7", 5)]
    [TestCase(" -20 + 25  ", 5)]
    [Test, Category("Positive scenario")]
    public void ComputesIntExpression(string expression, int expected)
    {
        Token actual = Execution.CalcExpression(expression); //Calculator.Compute(expression);
        var actualInt = (actual as TokenConstant<int>).value;

        Assert.That(actualInt, Is.EqualTo(expected));
    }

    //[Test, Category("Positive scenario")]
    //public void ComputesWithPriority()
    //{
    //    string expression = "1+2*3";
    //    double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    //    double expected = 7;

    //    Assert.That(actual, Is.EqualTo(expected));
    //}

    //[Test, Category("Positive scenario")]
    //public void ComputeMultiplicationFirst()
    //{
    //    string expression = "2*4-4";
    //    double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    //    double expected = 4;

    //    Assert.That(actual, Is.EqualTo(expected));
    //}

    //[Test, Category("Positive scenario")]
    //public void ComputesWithSpaces()
    //{
    //    string expression = "2 * 4 - 5";
    //    double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    //    double expected = 3;

    //    Assert.That(actual, Is.EqualTo(expected));
    //}

    //[TestCase("2+3", 5)]
    //[TestCase("1+2*3", 7)]
    //[TestCase("2*4-4", 4)]
    //[TestCase("2 * 4 - 5", 3)]
    //[TestCase("(2 * 4) * 4 - 5 * (4 - 1)", 17)]
    //[TestCase("2 * (2 + 3)", 10)]
    //[TestCase("2 + (1 + 2 * 3)", 9)]
    //[TestCase("2 * (1 + 3)", 8)]
    //[TestCase("4 - (2 + 3 * 5)", -13)]
    //[TestCase("4 - ((2 + 12) / (3 + 4))", 2)]
    //[Category("Positive scenario")]
    //public void Computes(string expression, double expected)
    //{
    //    double actual = Execution.Exec(expression); //Calculator.Compute(expression);

    //    Assert.That(actual, Is.EqualTo(expected));
    //}


    ////[TestCase("2 * (1 + 3) *    ")]
    ////[TestCase("4 - (2 + 3 * 5   ")]
    ////[Test, Category("Negative scenario")]
    ////public void CalculatorReturnNaN(string expression)
    ////{
    ////    double actual = Execution.Exec(expression); //Calculator.Compute(expression);
    ////    Assert.That(actual, Is.EqualTo(double.NaN));
    ////}

    //[TestCase("2 * (1 + 3) *   ")]
    //[TestCase("4 - (2 + 3 * 5  ")]
    //[TestCase("2 & 4 - 5       ")]
    //[TestCase("+ - * /         ")]
    ////[TestCase("1 2 +           ")]
    //[TestCase("1 + ( ) *)      ")]
    //[Test, Category("Negative scenario")]
    //public void ThrowsWithInvalidOperator(string expression)
    //{
    // Assert.Throws<InvalidOperationException>(() =>
    //            Execution.Exec(expression)  //Calculator.Compute(expression)
    //              , "Operators must be +, -, * or / only."); 
    //}

    //[Test, Category("Negative scenario")]
    //public void ThrowsWithDivisionByZero()
    //{
    //    string expression = "2 / 0 - 5";

    //    Assert.Throws<DivideByZeroException>(() =>
    //            Execution.Exec(expression)  //Calculator.Compute(expression)
    //                , "Division by zero is not allowed."); 
    //}
}