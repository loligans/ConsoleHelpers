using System;
using System.IO;
using ConsoleHelpers;
using Xunit;
using Xunit.Abstractions;

namespace ConsoleHelperTests
{
    /// <summary>
    ///
    /// </summary>
    public class ConsoleHelperTests
    {
        private readonly ITestOutputHelper _testOutput;
        public ConsoleHelperTests(ITestOutputHelper output) => _testOutput = output;

        /// <summary>
        /// Puts data into <see cref="Console.In"/>.
        /// </summary>
        /// <remarks>Redirects <see cref="Console.Out"/> to null. All output gets written to <see cref="_testOutput"/>.</remarks>
        /// <param name="userInput">The input to simulate.</param>
        private static void SetupTest(string userInput)
        {
            var input = new StringReader(userInput);
            Console.SetIn(input);
            Console.SetOut(TextWriter.Null);
        }

        /// <summary>
        /// Prints expectations for each test.
        /// </summary>
        private void PrintExpectations(bool parsed, bool expectedParsed, object output, object expectedOutput)
        {
            _testOutput.WriteLine($"Expected Parsed Result: {expectedParsed}");
            _testOutput.WriteLine($"  Actual Parsed Result: {parsed}");
            _testOutput.WriteLine($"Expected Output: {expectedOutput ?? "<null>"}");
            _testOutput.WriteLine($"  Actual Output: {output ?? "<null>"}");
        }

        [Theory]
        [InlineData("y", true, true)]
        [InlineData("n", true, false)]
        [InlineData("yes", true, true)]
        [InlineData("no", true, false)]
        [InlineData("true", true, true)]
        [InlineData("false", true, false)]
        [InlineData("invalidOption", false, null)]
        [InlineData("w31rdch@rs", false, null)]
        [InlineData("`~!@#$%^&*()-_=+,<.>/?;:'\"[{]}\\|", false, null)]
        [InlineData("invalidOptionWithSpecialChar`~!@#$%^&*()-_=+,<.>/?;:'\"[{]}\\|", false, null)]
        [InlineData("emojis🚪🏃💨", false, null)]
        [InlineData("yes`~!@#$%^&*()-_=+,<.>/?;:", false, null)]
        public void TryGetInput_Bool(string userInput, bool expectedParseResult, bool? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<bool>(out var output, "Enter a boolean value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("0", true, byte.MinValue)]
        [InlineData("1", true, (byte)1)]
        [InlineData("255", true, byte.MaxValue)]
        [InlineData("256", false, null)]
        [InlineData("-1", false, null)]
        [InlineData("0xC9", false, null)] // Don't accept hex for now
        [InlineData("C9", false, null)] // Don't accept hex for now
        [InlineData("0b1100_1001", false, null)] // Don't accept binary for now
        [InlineData("one", false, null)]
        public void TryGetInput_Byte(string userInput, bool expectedParseResult, byte? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<byte>(out var output, "Enter a unsigned byte value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("0", true, (sbyte)0)]
        [InlineData("1", true, (sbyte)1)]
        [InlineData("-128", true, sbyte.MinValue)]
        [InlineData("127", true, sbyte.MaxValue)]
        [InlineData("-129", false, null)]
        [InlineData("128", false, null)]
        [InlineData("0xC9", false, null)] // Don't accept hex for now
        [InlineData("C9", false, null)] // Don't accept hex for now
        [InlineData("0b1100_1001", false, null)] // Don't accept binary for now
        [InlineData("one", false, null)]
        public void TryGetInput_SByte(string userInput, bool expectedParseResult, sbyte? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<sbyte>(out var output, "Enter a signed byte value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("a", true, 'a')]
        [InlineData("1", true, '1')]
        [InlineData("!", true, '!')]
        [InlineData("💨", false, null)] // Don't accept emojis
        [InlineData("word", false, null)]
        public void TryGetInput_Char(string userInput, bool expectedParseResult, char? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<char>(out var output, "Enter a char value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("-3.402823E+38", true, -3.402823E+38f)]
        [InlineData("-1", true, -1f)]
        [InlineData("0", true, 0f)]
        [InlineData("0.0", true, 0.0f)]
        [InlineData("1", true, 1f)]
        [InlineData("3.402823E+38", true, 3.402823E+38f)]
        [InlineData("", false, null)]
        [InlineData("-3.402824E38", false, null)]
        [InlineData("3.402824E38", false, null)]
        public void TryGetInput_Single(string userInput, bool expectedParseResult, float? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<float>(out var output, "Enter a float value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);

            // If we expect null we better be null
            if (expectedOutput == null)
            {
                Assert.True(output == null);
            }
            else
            {
                Assert.True(output != null);
                if (output.Equals(expectedOutput)) return;

                // An epsilon is the relative error due to rounding in floating point arithmetic. float precision goes up to ~6-9 digits
                var epsilon = output.Value * .000001f;
                var difference = Math.Abs(output.Value - expectedOutput.Value);

                // So long as the difference is within the range of the calculated epsilon the test passes.
                Assert.True(difference <= epsilon);
            }
        }

        [Theory]
        [InlineData("-1.79769313486231E+308", true, -1.79769313486231E+308)]
        [InlineData("-1", true, -1d)]
        [InlineData("0", true, 0d)]
        [InlineData("0.0", true, 0.0d)]
        [InlineData("1", true, 1d)]
        [InlineData("1.79769313486231E+308", true, 1.79769313486231E+308d)]
        [InlineData("", false, null)]
        [InlineData("1.79769313486232E+308", false, null)]
        [InlineData("-1.79769313486232E+308", false, null)]
        public void TryGetInput_Double(string userInput, bool expectedParseResult, double? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<double>(out var output, "Enter a double value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);

            // If we expect null we better be null
            if (expectedOutput == null)
            {
                Assert.True(output == null);
            }
            else
            {
                Assert.True(output != null);
                if (output.Equals(expectedOutput)) return;

                // An epsilon is the relative error due to rounding in floating point arithmetic. double precision goes up to ~14-17 digits
                var epsilon = output.Value * .00000000000001d;
                var difference = Math.Abs(output.Value - expectedOutput.Value);

                // So long as the difference is within the range of the calculated epsilon the test passes.
                Assert.True(difference <= epsilon);
            }
        }

        [Theory]
        [InlineData("-79228162514264337593543950335", true, "-79228162514264337593543950335")]
        [InlineData("-1.0", true, "-1.0")]
        [InlineData("0.0", true, "0.0")]
        [InlineData("0", true, "0")]
        [InlineData("1.0", true, "1.0")]
        [InlineData("79228162514264337593543950335", true, "79228162514264337593543950335")]
        [InlineData("", false, null)]
        [InlineData("79228162514264337593543950336", false, null)]
        [InlineData("-79228162514264337593543950336", false, null)]
        public void TryGetInput_Decimal(string userInput, bool expectedParseResult, string strExpectedOutput)
        {
            // All attributes require a compile-time constant.
            // Decimal's are not a compile-time constant and thus cannot be placed within an XUnit test attribute.
            // To get around this limitation we manually parse expected output.
            var parsed = decimal.TryParse(strExpectedOutput, out var result);
            var expectedOutput = parsed ? result : (decimal?)null;

            // Begin test like normal
            SetupTest(userInput);
            parsed = ConsoleHelper.TryGetInput<decimal>(out var output, "Enter a decimal value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);

            // If we expect null we better be null... Deja Vu
            if (expectedOutput == null)
            {
                Assert.True(output == null);
            }
            else
            {
                Assert.True(output != null);
                if (output.Equals(expectedOutput)) return;

                // An epsilon is the relative error due to rounding in floating point arithmetic. decimal precision goes up to ~27-29 digits
                var epsilon = output.Value * .000000000000000000000000001m;
                var difference = Math.Abs(output.Value - expectedOutput.Value);

                // So long as the difference is within the range of the calculated epsilon the test passes.
                Assert.True(difference <= epsilon);
            }
        }

        [Theory]
        [InlineData("-32768", true, short.MinValue)]
        [InlineData("-1" , true, (short)-1)]
        [InlineData("0", true, (short)0)]
        [InlineData("1", true, (short)1)]
        [InlineData("32767", true, short.MaxValue)]
        [InlineData("", false, null)]
        [InlineData("32768", false, null)]
        [InlineData("-32769", false, null)]
        [InlineData("one", false, null)]
        public void TryGetInput_Int16(string userInput, bool expectedParseResult, short? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<short>(out var output, "Enter a signed short value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("-2147483648", true, int.MinValue)]
        [InlineData("-1" , true, -1)]
        [InlineData("0", true, 0)]
        [InlineData("1", true, 1)]
        [InlineData("2147483647", true, int.MaxValue)]
        [InlineData("", false, null)]
        [InlineData("2147483648", false, null)]
        [InlineData("-2147483649", false, null)]
        [InlineData("one", false, null)]
        [InlineData("-two", false, null)]
        [InlineData("8.0", false, null)]
        public void TryGetInput_Int32(string userInput, bool expectedParseResult, int? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<int>(out var output, "Enter a signed integer value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("-9223372036854775808", true, long.MinValue)]
        [InlineData("-1" , true, -1L)]
        [InlineData("0", true, 0L)]
        [InlineData("1", true, 1L)]
        [InlineData("9223372036854775807", true, long.MaxValue)]
        [InlineData("", false, null)]
        [InlineData("9223372036854775808", false, null)]
        [InlineData("-9223372036854775809", false, null)]
        [InlineData("one", false, null)]
        [InlineData("-two", false, null)]
        [InlineData("8.0", false, null)]
        public void TryGetInput_Int64(string userInput, bool expectedParseResult, long? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<long>(out var output, "Enter a signed long value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("0", true, ushort.MinValue)]
        [InlineData("1", true, (ushort)1)]
        [InlineData("65535", true, ushort.MaxValue)]
        [InlineData("", false, null)]
        [InlineData("65536", false, null)]
        [InlineData("-1", false, null)]
        [InlineData("one", false, null)]
        public void TryGetInput_UInt16(string userInput, bool expectedParseResult, ushort? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<ushort>(out var output, "Enter an unsigned short value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("0", true, uint.MinValue)]
        [InlineData("1", true, 1U)]
        [InlineData("4294967295", true, uint.MaxValue)]
        [InlineData("", false, null)]
        [InlineData("4294967296", false, null)]
        [InlineData("-1", false, null)]
        [InlineData("one", false, null)]
        public void TryGetInput_UInt32(string userInput, bool expectedParseResult, uint? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<uint>(out var output, "Enter an unsigned int value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [InlineData("0", true, ulong.MinValue)]
        [InlineData("1", true, 1UL)]
        [InlineData("18446744073709551615", true, ulong.MaxValue)]
        [InlineData("", false, null)]
        [InlineData("18446744073709551616", false, null)]
        [InlineData("-1", false, null)]
        [InlineData("one", false, null)]
        public void TryGetInput_Uint64(string userInput, bool expectedParseResult, ulong? expectedOutput)
        {
            SetupTest(userInput);
            var parsed = ConsoleHelper.TryGetInput<ulong>(out var output, "Enter an unsigned long value: ");
            PrintExpectations(parsed, expectedParseResult, output, expectedOutput);
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Fact]
        public void TryGetInput_UnsupportedTypes()
        {
            SetupTest($"{DateTime.Now.Ticks}");
            var parsed = ConsoleHelper.TryGetInput<DateTime>(out var output, "Enter a Date value: ");
            PrintExpectations(parsed, false, output, null);
            Assert.True(parsed == false);
            Assert.True(output == null);
        }
    }
}
