using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace ConsoleHelpers.Tests
{
    public static class TestSetup
    {
        /// <summary>
        /// Puts data into <see cref="Console.In"/>.
        /// </summary>
        /// <remarks>Redirects <see cref="Console.Out"/> to null. All output gets written to <see cref="_testOutput"/>.</remarks>
        /// <param name="userInput">The input to simulate.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Transfering ownership to System.Console")]
        public static void InjectConsoleInput(string userInput)
        {
            var input = new StringReader(userInput);
            Console.SetIn(input);
            Console.SetOut(TextWriter.Null);
        }
    }

    public class Bools
    {
        [Theory]
        [TestCase("y", true, true)]
        [TestCase("n", true, false)]
        [TestCase("yes", true, true)]
        [TestCase("no", true, false)]
        [TestCase("true", true, true)]
        [TestCase("false", true, false)]
        [TestCase("invalidOption", false, null)]
        [TestCase("w31rdch@rs", false, null)]
        [TestCase("`~!@#$%^&*()-_=+,<.>/?;:'\"[{]}\\|", false, null)]
        [TestCase("invalidOptionWithSpecialChar`~!@#$%^&*()-_=+,<.>/?;:'\"[{]}\\|", false, null)]
        [TestCase("emojis🚪🏃💨", false, null)]
        [TestCase("yes`~!@#$%^&*()-_=+,<.>/?;:", false, null)]
        public void TryGetInput_Bool(string userInput, bool expectedParseResult, bool? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<bool>(out var output, "Enter a boolean value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }
    }

    public class Chars
    {
        [Theory]
        [TestCase("a", true, 'a')]
        [TestCase("1", true, '1')]
        [TestCase("!", true, '!')]
        [TestCase("💨", false, null)] // Don't accept emojis
        [TestCase("word", false, null)]
        public void TryGetInput_Char(string userInput, bool expectedParseResult, char? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<char>(out var output, "Enter a char value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }
    }

    public class Bytes
    {
        [Theory]
        [TestCase("0", true, (sbyte)0)]
        [TestCase("1", true, (sbyte)1)]
        [TestCase("-128", true, sbyte.MinValue)]
        [TestCase("127", true, sbyte.MaxValue)]
        [TestCase("-129", false, null)]
        [TestCase("128", false, null)]
        [TestCase("0xC9", false, null)] // Don't accept hex for now
        [TestCase("C9", false, null)] // Don't accept hex for now
        [TestCase("0b1100_1001", false, null)] // Don't accept binary for now
        [TestCase("one", false, null)]
        public void TryGetInput_SignedByte(string userInput, bool expectedParseResult, sbyte? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<sbyte>(out var output, "Enter a signed byte value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [TestCase("0", true, byte.MinValue)]
        [TestCase("1", true, (byte)1)]
        [TestCase("255", true, byte.MaxValue)]
        [TestCase("256", false, null)]
        [TestCase("-1", false, null)]
        [TestCase("0xC9", false, null)] // Don't accept hex for now
        [TestCase("C9", false, null)] // Don't accept hex for now
        [TestCase("0b1100_1001", false, null)] // Don't accept binary for now
        [TestCase("one", false, null)]
        public void TryGetInput_UnsignedByte(string userInput, bool expectedParseResult, byte? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<byte>(out var output, "Enter a unsigned byte value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }
    }

    public class Shorts
    {
        [Theory]
        [TestCase("-32768", true, short.MinValue)]
        [TestCase("-1" , true, (short)-1)]
        [TestCase("0", true, (short)0)]
        [TestCase("1", true, (short)1)]
        [TestCase("32767", true, short.MaxValue)]
        [TestCase("", false, null)]
        [TestCase("32768", false, null)]
        [TestCase("-32769", false, null)]
        [TestCase("one", false, null)]
        public void TryGetInput_SignedShort(string userInput, bool expectedParseResult, short? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<short>(out var output, "Enter a signed short value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [TestCase("0", true, ushort.MinValue)]
        [TestCase("1", true, (ushort)1)]
        [TestCase("65535", true, ushort.MaxValue)]
        [TestCase("", false, null)]
        [TestCase("65536", false, null)]
        [TestCase("-1", false, null)]
        [TestCase("one", false, null)]
        public void TryGetInput_UnsignedShort(string userInput, bool expectedParseResult, ushort? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<ushort>(out var output, "Enter an unsigned short value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }
    }

    public class Ints
    {
        [Theory]
        [TestCase("-2147483648", true, int.MinValue)]
        [TestCase("-1" , true, -1)]
        [TestCase("0", true, 0)]
        [TestCase("1", true, 1)]
        [TestCase("2147483647", true, int.MaxValue)]
        [TestCase("", false, null)]
        [TestCase("2147483648", false, null)]
        [TestCase("-2147483649", false, null)]
        [TestCase("one", false, null)]
        [TestCase("-two", false, null)]
        [TestCase("8.0", false, null)]
        public void TryGetInput_SignedInt(string userInput, bool expectedParseResult, int? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<int>(out var output, "Enter a signed integer value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [TestCase("0", true, uint.MinValue)]
        [TestCase("1", true, 1U)]
        [TestCase("4294967295", true, uint.MaxValue)]
        [TestCase("", false, null)]
        [TestCase("4294967296", false, null)]
        [TestCase("-1", false, null)]
        [TestCase("one", false, null)]
        public void TryGetInput_UnsignedInt(string userInput, bool expectedParseResult, uint? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<uint>(out var output, "Enter an unsigned int value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }
    }

    public class Longs
    {
        [Theory]
        [TestCase("-9223372036854775808", true, long.MinValue)]
        [TestCase("-1" , true, -1L)]
        [TestCase("0", true, 0L)]
        [TestCase("1", true, 1L)]
        [TestCase("9223372036854775807", true, long.MaxValue)]
        [TestCase("", false, null)]
        [TestCase("9223372036854775808", false, null)]
        [TestCase("-9223372036854775809", false, null)]
        [TestCase("one", false, null)]
        [TestCase("-two", false, null)]
        [TestCase("8.0", false, null)]
        public void TryGetInput_SignedLong(string userInput, bool expectedParseResult, long? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<long>(out var output, "Enter a signed long value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }

        [Theory]
        [TestCase("0", true, ulong.MinValue)]
        [TestCase("1", true, 1UL)]
        [TestCase("18446744073709551615", true, ulong.MaxValue)]
        [TestCase("", false, null)]
        [TestCase("18446744073709551616", false, null)]
        [TestCase("-1", false, null)]
        [TestCase("one", false, null)]
        public void TryGetInput_UnsignedLong(string userInput, bool expectedParseResult, ulong? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<ulong>(out var output, "Enter an unsigned long value: ");
            Assert.True(parsed == expectedParseResult);
            Assert.True(output == expectedOutput);
        }
    }

    public class Singles
    {
        [Theory]
        [TestCase("-3.402823E+38", true, -3.402823E+38f)]
        [TestCase("-1", true, -1f)]
        [TestCase("0", true, 0f)]
        [TestCase("0.0", true, 0.0f)]
        [TestCase("1", true, 1f)]
        [TestCase("3.402823E+38", true, 3.402823E+38f)]
        [TestCase("", false, null)]
        [TestCase("-3.402824E38", true, float.NegativeInfinity)]
        [TestCase("3.402824E38", true, float.PositiveInfinity)]
        [TestCase("-3.402824E999", true, float.NegativeInfinity)]
        [TestCase("3.402824E999", true, float.PositiveInfinity)]
        public void TryGetInput_Single(string userInput, bool expectedParseResult, float? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<float>(out var output, "Enter a float value: ");
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
                var epsilon = output!.Value * .000001f;
                var difference = Math.Abs(output.Value - expectedOutput.Value);

                // So long as the difference is within the range of the calculated epsilon the test passes.
                Assert.True(difference <= epsilon);
            }
        }
    }

    public class Doubles
    {
        [Theory]
        [TestCase("-1.79769313486231E+308", true, -1.79769313486231E+308)]
        [TestCase("-1", true, -1d)]
        [TestCase("0", true, 0d)]
        [TestCase("0.0", true, 0.0d)]
        [TestCase("1", true, 1d)]
        [TestCase("1.79769313486231E+308", true, 1.79769313486231E+308d)]
        [TestCase("", false, null)]
        [TestCase("1.79769313486232E+308", true, double.PositiveInfinity)]
        [TestCase("-1.79769313486232E+308", true, double.NegativeInfinity)]
        [TestCase("1.79769313486232E+999", true, double.PositiveInfinity)]
        [TestCase("-1.79769313486232E+999", true, double.NegativeInfinity)]
        public void TryGetInput_Double(string userInput, bool expectedParseResult, double? expectedOutput)
        {
            TestSetup.InjectConsoleInput(userInput);
            var parsed = ConsoleHelper.TryGetInput<double>(out var output, "Enter a double value: ");
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
                var epsilon = output!.Value * .00000000000001d;
                var difference = Math.Abs(output.Value - expectedOutput.Value);

                // So long as the difference is within the range of the calculated epsilon the test passes.
                Assert.True(difference <= epsilon);
            }
        }
    }

    public class Decimals
    {
        [Theory]
        [TestCase("-79228162514264337593543950335", true, "-79228162514264337593543950335")]
        [TestCase("-1.0", true, "-1.0")]
        [TestCase("0.0", true, "0.0")]
        [TestCase("0", true, "0")]
        [TestCase("1.0", true, "1.0")]
        [TestCase("79228162514264337593543950335", true, "79228162514264337593543950335")]
        [TestCase("", false, null)]
        [TestCase("79228162514264337593543950336", false, null)]
        [TestCase("-79228162514264337593543950336", false, null)]
        public void TryGetInput_Decimal(string userInput, bool expectedParseResult, string strExpectedOutput)
        {
            // All attributes require a compile-time constant.
            // Decimal's are not a compile-time constant and thus cannot be placed within an XUnit test attribute.
            // To get around this limitation we manually parse expected output.
            var parsed = decimal.TryParse(strExpectedOutput, out var result);
            var expectedOutput = parsed ? result : (decimal?)null;

            // Begin test like normal
            TestSetup.InjectConsoleInput(userInput);
            parsed = ConsoleHelper.TryGetInput<decimal>(out var output, "Enter a decimal value: ");
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
                var epsilon = output!.Value * .000000000000000000000000001m;
                var difference = Math.Abs(output.Value - expectedOutput.Value);

                // So long as the difference is within the range of the calculated epsilon the test passes.
                Assert.True(difference <= epsilon);
            }
        }
    }

    public class UnsupportedTypes
    {
        [Test]
        public void TryGetInput_DateTime()
        {
            TestSetup.InjectConsoleInput($"{DateTime.Now.Ticks}");
            var parsed = ConsoleHelper.TryGetInput<DateTime>(out var output, "Enter a Date value: ");
            Assert.True(parsed == false);
            Assert.True(output == null);
        }
    }
}
