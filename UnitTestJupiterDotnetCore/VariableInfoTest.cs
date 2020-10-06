using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jupiter;
using Jupiter.Converter;
using System;
using System.Collections.Generic;
using Opc.Ua;

namespace UnitTestJupiterDotnetCore
{
    [TestClass]
    public class VariableInfoTest
    {
        [TestMethod]
        public void TestBooleanVariableInfo()
        {
            var vi = new BooleanDataValue(new DataValue(new Variant(false)));
            Assert.AreEqual(vi.GetRawValue(), false);
            vi.ConvertValueBack(true);
            Assert.AreEqual(vi.GetRawValue(), true);
            vi.ConvertValueBack(false);
            Assert.AreEqual(vi.GetRawValue(), false);
        }

        [TestMethod]
        public void TestSByteeanVariableInfo()
        {
            var vi = new SByteDataValue(new DataValue(new Variant((sbyte)0)));
            Assert.AreEqual(vi.GetRawValue(), (sbyte)0);
            vi.UpdateDataValue(new DataValue(new Variant((sbyte)-128)));
            Assert.AreEqual(vi.GetRawValue(), (sbyte)-128);
        }

        [DataRow((sbyte)0, FormatType.DEC, "0")]
        [DataRow((sbyte)1, FormatType.DEC, "1")]
        [DataRow((sbyte)127, FormatType.DEC, "127")]
        [DataRow((sbyte)-1, FormatType.DEC, "-1")]
        [DataRow((sbyte)-128, FormatType.DEC, "-128")]
        [DataRow((sbyte)0, FormatType.HEX, "00")]
        [DataRow((sbyte)1, FormatType.HEX, "01")]
        [DataRow((sbyte)127, FormatType.HEX, "7F")]
        [DataRow((sbyte)-1, FormatType.HEX, "FF")]
        [DataRow((sbyte)-128, FormatType.HEX, "80")]
        [DataRow((sbyte)0, FormatType.OCT, "0")]
        [DataRow((sbyte)1, FormatType.OCT, "1")]
        [DataRow((sbyte)127, FormatType.OCT, "177")]
        [DataRow((sbyte)-1, FormatType.OCT, "377")]
        [DataRow((sbyte)-128, FormatType.OCT, "200")]
        [DataTestMethod]
        public void TestSByteCoverter(sbyte inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new SByteDataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow((byte)0, FormatType.DEC, "0")]
        [DataRow((byte)1, FormatType.DEC, "1")]
        [DataRow((byte)127, FormatType.DEC, "127")]
        [DataRow((byte)255, FormatType.DEC, "255")]
        [DataRow((byte)128, FormatType.DEC, "128")]
        [DataRow((byte)0, FormatType.HEX, "00")]
        [DataRow((byte)1, FormatType.HEX, "01")]
        [DataRow((byte)127, FormatType.HEX, "7F")]
        [DataRow((byte)255, FormatType.HEX, "FF")]
        [DataRow((byte)128, FormatType.HEX, "80")]
        [DataRow((byte)0, FormatType.OCT, "0")]
        [DataRow((byte)1, FormatType.OCT, "1")]
        [DataRow((byte)127, FormatType.OCT, "177")]
        [DataRow((byte)255, FormatType.OCT, "377")]
        [DataRow((byte)128, FormatType.OCT, "200")]
        [DataTestMethod]
        public void TestByteCoverter(byte inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new ByteDataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow((Int16)0, FormatType.DEC, "0")]
        [DataRow((Int16)1, FormatType.DEC, "1")]
        [DataRow((Int16)32767, FormatType.DEC, "32767")]
        [DataRow((Int16)(-1), FormatType.DEC, "-1")]
        [DataRow((Int16)(-32768), FormatType.DEC, "-32768")]
        [DataRow((Int16)0, FormatType.HEX, "0000")]
        [DataRow((Int16)1, FormatType.HEX, "0001")]
        [DataRow((Int16)32767, FormatType.HEX, "7FFF")]
        [DataRow((Int16)(-1), FormatType.HEX, "FFFF")]
        [DataRow((Int16)(-32768), FormatType.HEX, "8000")]
        [DataRow((Int16)0, FormatType.OCT, "0")]
        [DataRow((Int16)1, FormatType.OCT, "1")]
        [DataRow((Int16)32767, FormatType.OCT, "77777")]
        [DataRow((Int16)(-1), FormatType.OCT, "177777")]
        [DataRow((Int16)(-32768), FormatType.OCT, "100000")]
        [DataTestMethod]
        public void TestInt16Coverter(Int16 inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new Int16DataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow((UInt16)0, FormatType.DEC, "0")]
        [DataRow((UInt16)1, FormatType.DEC, "1")]
        [DataRow((UInt16)32767, FormatType.DEC, "32767")]
        [DataRow((UInt16)65535, FormatType.DEC, "65535")]
        [DataRow((UInt16)32768, FormatType.DEC, "32768")]
        [DataRow((UInt16)0, FormatType.HEX, "0000")]
        [DataRow((UInt16)1, FormatType.HEX, "0001")]
        [DataRow((UInt16)32767, FormatType.HEX, "7FFF")]
        [DataRow((UInt16)65535, FormatType.HEX, "FFFF")]
        [DataRow((UInt16)32768, FormatType.HEX, "8000")]
        [DataRow((UInt16)0, FormatType.OCT, "0")]
        [DataRow((UInt16)1, FormatType.OCT, "1")]
        [DataRow((UInt16)32767, FormatType.OCT, "77777")]
        [DataRow((UInt16)65535, FormatType.OCT, "177777")]
        [DataRow((UInt16)32768, FormatType.OCT, "100000")]
        [DataTestMethod]
        public void TestUInt16Coverter(UInt16 inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new UInt16DataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow((Int32)0, FormatType.DEC, "0")]
        [DataRow((Int32)1, FormatType.DEC, "1")]
        [DataRow((Int32)2147483647, FormatType.DEC, "2147483647")]
        [DataRow((Int32)(-1), FormatType.DEC, "-1")]
        [DataRow((Int32)(-2147483648), FormatType.DEC, "-2147483648")]
        [DataRow((Int32)0, FormatType.HEX, "00000000")]
        [DataRow((Int32)1, FormatType.HEX, "00000001")]
        [DataRow((Int32)2147483647, FormatType.HEX, "7FFFFFFF")]
        [DataRow((Int32)(-1), FormatType.HEX, "FFFFFFFF")]
        [DataRow((Int32)(-2147483648), FormatType.HEX, "80000000")]
        [DataRow((Int32)0, FormatType.OCT, "0")]
        [DataRow((Int32)1, FormatType.OCT, "1")]
        [DataRow((Int32)2147483647, FormatType.OCT, "17777777777")]
        [DataRow((Int32)(-1), FormatType.OCT, "37777777777")]
        [DataRow((Int32)(-2147483648), FormatType.OCT, "20000000000")]
        [DataTestMethod]
        public void TestInt32Coverter(Int32 inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new Int32DataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow((UInt32)0, FormatType.DEC, "0")]
        [DataRow((UInt32)1, FormatType.DEC, "1")]
        [DataRow((UInt32)2147483647, FormatType.DEC, "2147483647")]
        [DataRow((UInt32)(4294967295), FormatType.DEC, "4294967295")]
        [DataRow((UInt32)(2147483648), FormatType.DEC, "2147483648")]
        [DataRow((UInt32)0, FormatType.HEX, "00000000")]
        [DataRow((UInt32)1, FormatType.HEX, "00000001")]
        [DataRow((UInt32)2147483647, FormatType.HEX, "7FFFFFFF")]
        [DataRow((UInt32)(4294967295), FormatType.HEX, "FFFFFFFF")]
        [DataRow((UInt32)(2147483648), FormatType.HEX, "80000000")]
        [DataRow((UInt32)0, FormatType.OCT, "0")]
        [DataRow((UInt32)1, FormatType.OCT, "1")]
        [DataRow((UInt32)2147483647, FormatType.OCT, "17777777777")]
        [DataRow((UInt32)(4294967295), FormatType.OCT, "37777777777")]
        [DataRow((UInt32)(2147483648), FormatType.OCT, "20000000000")]
        [DataTestMethod]
        public void TestUInt32Coverter(UInt32 inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new UInt32DataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow((Int64)0, FormatType.DEC, "0")]
        [DataRow((Int64)1, FormatType.DEC, "1")]
        [DataRow((Int64)9223372036854775807, FormatType.DEC, "9223372036854775807")]
        [DataRow((Int64)(-1), FormatType.DEC, "-1")]
        [DataRow((Int64)(-9223372036854775808), FormatType.DEC, "-9223372036854775808")]
        [DataRow((Int64)0, FormatType.HEX, "0000000000000000")]
        [DataRow((Int64)1, FormatType.HEX, "0000000000000001")]
        [DataRow((Int64)9223372036854775807, FormatType.HEX, "7FFFFFFFFFFFFFFF")]
        [DataRow((Int64)(-1), FormatType.HEX, "FFFFFFFFFFFFFFFF")]
        [DataRow((Int64)(-9223372036854775808), FormatType.HEX, "8000000000000000")]
        [DataRow((Int64)0, FormatType.OCT, "0")]
        [DataRow((Int64)1, FormatType.OCT, "1")]
        [DataRow((Int64)9223372036854775807, FormatType.OCT, "777777777777777777777")]
        [DataRow((Int64)(-1), FormatType.OCT, "1777777777777777777777")]
        [DataRow((Int64)(-9223372036854775808), FormatType.OCT, "1000000000000000000000")]
        [DataTestMethod]
        public void TestInt64Coverter(Int64 inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new Int64DataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow((UInt64)0, FormatType.DEC, "0")]
        [DataRow((UInt64)1, FormatType.DEC, "1")]
        [DataRow((UInt64)9223372036854775807, FormatType.DEC, "9223372036854775807")]
        [DataRow((UInt64)(18446744073709551615), FormatType.DEC, "18446744073709551615")]
        [DataRow((UInt64)(9223372036854775808), FormatType.DEC, "9223372036854775808")]
        [DataRow((UInt64)0, FormatType.HEX, "0000000000000000")]
        [DataRow((UInt64)1, FormatType.HEX, "0000000000000001")]
        [DataRow((UInt64)9223372036854775807, FormatType.HEX, "7FFFFFFFFFFFFFFF")]
        [DataRow((UInt64)(18446744073709551615), FormatType.HEX, "FFFFFFFFFFFFFFFF")]
        [DataRow((UInt64)(9223372036854775808), FormatType.HEX, "8000000000000000")]
        [DataRow((UInt64)0, FormatType.OCT, "0")]
        [DataRow((UInt64)1, FormatType.OCT, "1")]
        [DataRow((UInt64)9223372036854775807, FormatType.OCT, "777777777777777777777")]
        [DataRow((UInt64)(18446744073709551615), FormatType.OCT, "1777777777777777777777")]
        [DataRow((UInt64)(9223372036854775808), FormatType.OCT, "1000000000000000000000")]
        [DataTestMethod]
        public void TestUInt64Coverter(UInt64 inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new UInt64DataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow(float.MinValue, FormatType.FLOAT, "-3.4028235E+38")]
        [DataRow(float.MaxValue, FormatType.FLOAT, "3.4028235E+38")]
        [DataRow(float.Epsilon, FormatType.FLOAT, "1E-45")] // Ç»ÇÒÇ©Ç®Ç©ÇµÇ¢
        [DataRow((float)0, FormatType.FLOAT, "0")]
        [DataRow((float)1.234, FormatType.FLOAT, "1.234")]
        [DataRow((float)-2.345, FormatType.FLOAT, "-2.345")]
        [DataRow(float.PositiveInfinity, FormatType.FLOAT, "Åá")]
        [DataRow(float.NegativeInfinity, FormatType.FLOAT, "-Åá")]
        [DataRow(float.NaN, FormatType.FLOAT, "NaN")]
        [DataTestMethod]
        public void TestFloatCoverter(float inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new FloatDataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [DataRow(double.MinValue, FormatType.FLOAT, "-1.7976931348623157E+308")]
        [DataRow(double.MaxValue, FormatType.FLOAT, "1.7976931348623157E+308")]
        [DataRow(double.Epsilon, FormatType.FLOAT, "5E-324")] // Ç»ÇÒÇ©Ç®Ç©ÇµÇ¢
        [DataRow((double)0, FormatType.FLOAT, "0")]
        [DataRow((double)1.234, FormatType.FLOAT, "1.234")]
        [DataRow((double)-2.345, FormatType.FLOAT, "-2.345")]
        [DataRow(double.PositiveInfinity, FormatType.FLOAT, "Åá")]
        [DataRow(double.NegativeInfinity, FormatType.FLOAT, "-Åá")]
        [DataRow(double.NaN, FormatType.FLOAT, "NaN")]
        [DataTestMethod]
        public void TestDoubleCoverter(double inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new DoubleDataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(expectedDisplayValue, convVal);
        }

        [DataRow("", FormatType.STRING, "")]
        [DataRow(null, FormatType.STRING, "")]
        [DataRow("a", FormatType.STRING, "a")]
        [DataRow("Ç†", FormatType.STRING, "Ç†")]
        [DataRow("abcdefg", FormatType.STRING, "abcdefg")]
        [DataTestMethod]
        public void TestStringCoverter(string inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new StringDataValue(new DataValue(new Variant(inputValue)));
            varInfo.FormatSelectedItem = format;
            var convVal = varInfo.ConvertValue();
            Assert.AreEqual(convVal, expectedDisplayValue);
        }

        [TestMethod]
        public void TestDateTimeCoverter()
        {
            var errMessages = new List<string>();

            var fn = new Action<DateTime, string, string>((inputValue, format, expectedDisplayValue) =>
            {
                var varInfo = new DateTimeDataValue(new DataValue(new Variant(inputValue)));
                varInfo.FormatSelectedItem = format;
                var convVal = varInfo.ConvertValue();
                try
                {
                    Assert.AreEqual(convVal, expectedDisplayValue);
                }
                catch (Exception e)
                {
                    errMessages.Add(e.Message);
                }
            });

            fn(DateTime.MinValue, FormatType.DATE_AND_TIME, "0001-01-01 00:00:00.000");
            fn(DateTime.MaxValue, FormatType.DATE_AND_TIME, "9999-12-31 23:59:59.999");

            if (errMessages.Count != 0)
            {
                var messages = "\n";
                foreach (var msg in errMessages)
                {
                    messages += string.Format(" - {0}\n", msg);
                }
                Assert.Fail(messages);
            }
        }

        [DataRow("0", FormatType.DEC, (sbyte)0)]
        [DataRow("1", FormatType.DEC, (sbyte)1)]
        [DataRow("127", FormatType.DEC, (sbyte)127)]
        [DataRow("-128", FormatType.DEC, (sbyte)-128)]
        [DataRow("-1", FormatType.DEC, (sbyte)-1)]
        [DataRow("000", FormatType.DEC, (sbyte)0)]
        [DataRow("00127", FormatType.DEC, (sbyte)127)]
        [DataRow("00", FormatType.HEX, (sbyte)0)]
        [DataRow("01", FormatType.HEX, (sbyte)1)]
        [DataRow("7F", FormatType.HEX, (sbyte)127)]
        [DataRow("80", FormatType.HEX, (sbyte)-128)]
        [DataRow("FF", FormatType.HEX, (sbyte)-1)]
        [DataRow("0", FormatType.HEX, (sbyte)0)]
        [DataRow("000", FormatType.HEX, (sbyte)0)]
        [DataRow("007F", FormatType.HEX, (sbyte)127)]
        [DataRow("00FF", FormatType.HEX, (sbyte)-1)]
        [DataRow("0", FormatType.OCT, (sbyte)0)]
        [DataRow("1", FormatType.OCT, (sbyte)1)]
        [DataRow("177", FormatType.OCT, (sbyte)127)]
        [DataRow("200", FormatType.OCT, (sbyte)-128)]
        [DataRow("377", FormatType.OCT, (sbyte)-1)]
        [DataRow("0", FormatType.OCT, (sbyte)0)]
        [DataRow("000", FormatType.OCT, (sbyte)0)]
        [DataRow("00177", FormatType.OCT, (sbyte)127)]
        [DataRow("00377", FormatType.OCT, (sbyte)-1)]
        [DataTestMethod]
        public void TestSByteBackCoverter(string inputValue, string format, sbyte expectedDisplayValue)
        {
            var varInfo = new SByteDataValue(new DataValue(new Variant((sbyte)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("128", FormatType.DEC)]
        [DataRow("-129", FormatType.DEC)]
        [DataRow("100", FormatType.HEX)]
        [DataRow("400", FormatType.OCT)]
        [DataTestMethod]
        public void TestSByteBackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new SByteDataValue(new DataValue(new Variant((sbyte)0)));
            varInfo.FormatSelectedItem = format;
            Assert.ThrowsException<OverflowException>(() => varInfo.ConvertValueBack(inputValue));
        }

        [DataRow("0", FormatType.DEC, (byte)0)]
        [DataRow("1", FormatType.DEC, (byte)1)]
        [DataRow("127", FormatType.DEC, (byte)127)]
        [DataRow("128", FormatType.DEC, (byte)128)]
        [DataRow("255", FormatType.DEC, (byte)255)]
        [DataRow("000", FormatType.DEC, (byte)0)]
        [DataRow("00127", FormatType.DEC, (byte)127)]
        [DataRow("00", FormatType.HEX, (byte)0)]
        [DataRow("01", FormatType.HEX, (byte)1)]
        [DataRow("7F", FormatType.HEX, (byte)127)]
        [DataRow("80", FormatType.HEX, (byte)128)]
        [DataRow("FF", FormatType.HEX, (byte)255)]
        [DataRow("0", FormatType.HEX, (byte)0)]
        [DataRow("000", FormatType.HEX, (byte)0)]
        [DataRow("007F", FormatType.HEX, (byte)127)]
        [DataRow("00FF", FormatType.HEX, (byte)255)]
        [DataRow("0", FormatType.OCT, (byte)0)]
        [DataRow("1", FormatType.OCT, (byte)1)]
        [DataRow("177", FormatType.OCT, (byte)127)]
        [DataRow("200", FormatType.OCT, (byte)128)]
        [DataRow("377", FormatType.OCT, (byte)255)]
        [DataRow("0", FormatType.OCT, (byte)0)]
        [DataRow("000", FormatType.OCT, (byte)0)]
        [DataRow("00177", FormatType.OCT, (byte)127)]
        [DataRow("00377", FormatType.OCT, (byte)255)]
        [DataTestMethod]
        public void TestByteBackCoverter(string inputValue, string format, byte expectedDisplayValue)
        {
            var varInfo = new ByteDataValue(new DataValue(new Variant((byte)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("-1", FormatType.DEC)]
        [DataRow("256", FormatType.DEC)]
        [DataRow("100", FormatType.HEX)]
        [DataRow("400", FormatType.OCT)]
        [DataTestMethod]
        public void TestByteBackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new ByteDataValue(new DataValue(new Variant((byte)0)));
            varInfo.FormatSelectedItem = format;
            Assert.ThrowsException<OverflowException>(() => varInfo.ConvertValueBack(inputValue));
        }

        [DataRow("0", FormatType.DEC, (Int16)0)]
        [DataRow("1", FormatType.DEC, (Int16)1)]
        [DataRow("32767", FormatType.DEC, (Int16)32767)]
        [DataRow("-32768", FormatType.DEC, (Int16)(-32768))]
        [DataRow("-1", FormatType.DEC, (Int16)(-1))]
        [DataRow("000", FormatType.DEC, (Int16)0)]
        [DataRow("0032767", FormatType.DEC, (Int16)32767)]
        [DataRow("0000", FormatType.HEX, (Int16)0)]
        [DataRow("0001", FormatType.HEX, (Int16)1)]
        [DataRow("7FFF", FormatType.HEX, (Int16)32767)]
        [DataRow("8000", FormatType.HEX, (Int16)(-32768))]
        [DataRow("FFFF", FormatType.HEX, (Int16)(-1))]
        [DataRow("0", FormatType.HEX, (Int16)0)]
        [DataRow("000", FormatType.HEX, (Int16)0)]
        [DataRow("007FFF", FormatType.HEX, (Int16)32767)]
        [DataRow("00FFFF", FormatType.HEX, (Int16)(-1))]
        [DataRow("0", FormatType.OCT, (Int16)0)]
        [DataRow("1", FormatType.OCT, (Int16)1)]
        [DataRow("77777", FormatType.OCT, (Int16)32767)]
        [DataRow("100000", FormatType.OCT, (Int16)(-32768))]
        [DataRow("177777", FormatType.OCT, (Int16)(-1))]
        [DataRow("0", FormatType.OCT, (Int16)0)]
        [DataRow("000", FormatType.OCT, (Int16)0)]
        [DataRow("0077777", FormatType.OCT, (Int16)32767)]
        [DataRow("00177777", FormatType.OCT, (Int16)(-1))]
        [DataTestMethod]
        public void TestInt16BackCoverter(string inputValue, string format, Int16 expectedDisplayValue)
        {
            var varInfo = new Int16DataValue(new DataValue(new Variant((Int16)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("32768", FormatType.DEC)]
        [DataRow("-32769", FormatType.DEC)]
        [DataRow("10000", FormatType.HEX)]
        [DataRow("200000", FormatType.OCT)]
        [DataTestMethod]
        public void TestInt16BackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new Int16DataValue(new DataValue(new Variant((Int16)0)));
            varInfo.FormatSelectedItem = format;
            Assert.ThrowsException<OverflowException>(() => varInfo.ConvertValueBack(inputValue));
        }

        [DataRow("0", FormatType.DEC, (UInt16)0)]
        [DataRow("1", FormatType.DEC, (UInt16)1)]
        [DataRow("32767", FormatType.DEC, (UInt16)32767)]
        [DataRow("32768", FormatType.DEC, (UInt16)32768)]
        [DataRow("65535", FormatType.DEC, (UInt16)65535)]
        [DataRow("000", FormatType.DEC, (UInt16)0)]
        [DataRow("0032767", FormatType.DEC, (UInt16)32767)]
        [DataRow("00", FormatType.HEX, (UInt16)0)]
        [DataRow("01", FormatType.HEX, (UInt16)1)]
        [DataRow("7FFF", FormatType.HEX, (UInt16)32767)]
        [DataRow("8000", FormatType.HEX, (UInt16)32768)]
        [DataRow("FFFF", FormatType.HEX, (UInt16)65535)]
        [DataRow("0", FormatType.HEX, (UInt16)0)]
        [DataRow("000", FormatType.HEX, (UInt16)0)]
        [DataRow("007FFF", FormatType.HEX, (UInt16)32767)]
        [DataRow("00FFFF", FormatType.HEX, (UInt16)65535)]
        [DataRow("0", FormatType.OCT, (UInt16)0)]
        [DataRow("1", FormatType.OCT, (UInt16)1)]
        [DataRow("77777", FormatType.OCT, (UInt16)32767)]
        [DataRow("100000", FormatType.OCT, (UInt16)32768)]
        [DataRow("177777", FormatType.OCT, (UInt16)65535)]
        [DataRow("0", FormatType.OCT, (UInt16)0)]
        [DataRow("000", FormatType.OCT, (UInt16)0)]
        [DataRow("0077777", FormatType.OCT, (UInt16)32767)]
        [DataRow("00177777", FormatType.OCT, (UInt16)65535)]
        [DataTestMethod]
        public void TestUInt16BackCoverter(string inputValue, string format, UInt16 expectedDisplayValue)
        {
            var varInfo = new UInt16DataValue(new DataValue(new Variant((UInt16)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("-1", FormatType.DEC)]
        [DataRow("65536", FormatType.DEC)]
        [DataRow("10000", FormatType.HEX)]
        [DataRow("200000", FormatType.OCT)]
        [DataTestMethod]
        public void TestUInt16BackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new UInt16DataValue(new DataValue(new Variant((UInt16)0)));
            varInfo.FormatSelectedItem = format;
            Assert.ThrowsException<OverflowException>(() => varInfo.ConvertValueBack(inputValue));
        }

        [DataRow("0", FormatType.DEC, (Int32)0)]
        [DataRow("1", FormatType.DEC, (Int32)1)]
        [DataRow("2147483647", FormatType.DEC, (Int32)2147483647)]
        [DataRow("-2147483648", FormatType.DEC, (Int32)(-2147483648))]
        [DataRow("-1", FormatType.DEC, (Int32)(-1))]
        [DataRow("000", FormatType.DEC, (Int32)0)]
        [DataRow("002147483647", FormatType.DEC, (Int32)2147483647)]
        [DataRow("0000", FormatType.HEX, (Int32)0)]
        [DataRow("0001", FormatType.HEX, (Int32)1)]
        [DataRow("7FFFFFFF", FormatType.HEX, (Int32)2147483647)]
        [DataRow("80000000", FormatType.HEX, (Int32)(-2147483648))]
        [DataRow("FFFFFFFF", FormatType.HEX, (Int32)(-1))]
        [DataRow("0", FormatType.HEX, (Int32)0)]
        [DataRow("000", FormatType.HEX, (Int32)0)]
        [DataRow("007FFFFFFF", FormatType.HEX, (Int32)2147483647)]
        [DataRow("00FFFFFFFF", FormatType.HEX, (Int32)(-1))]
        [DataRow("0", FormatType.OCT, (Int32)0)]
        [DataRow("1", FormatType.OCT, (Int32)1)]
        [DataRow("17777777777", FormatType.OCT, (Int32)2147483647)]
        [DataRow("20000000000", FormatType.OCT, (Int32)(-2147483648))]
        [DataRow("37777777777", FormatType.OCT, (Int32)(-1))]
        [DataRow("0", FormatType.OCT, (Int32)0)]
        [DataRow("000", FormatType.OCT, (Int32)0)]
        [DataRow("0017777777777", FormatType.OCT, (Int32)2147483647)]
        [DataRow("0037777777777", FormatType.OCT, (Int32)(-1))]
        [DataTestMethod]
        public void TestInt32BackCoverter(string inputValue, string format, Int32 expectedDisplayValue)
        {
            var varInfo = new Int32DataValue(new DataValue(new Variant((Int32)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("2147483648", FormatType.DEC)]
        [DataRow("-2147483649", FormatType.DEC)]
        [DataRow("100000000", FormatType.HEX)]
        [DataRow("40000000000", FormatType.OCT)]
        [DataTestMethod]
        public void TestInt32BackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new Int32DataValue(new DataValue(new Variant((Int32)0)));
            varInfo.FormatSelectedItem = format;
            Assert.ThrowsException<OverflowException>(() => varInfo.ConvertValueBack(inputValue));
        }

        [DataRow("0", FormatType.DEC, (UInt32)0)]
        [DataRow("1", FormatType.DEC, (UInt32)1)]
        [DataRow("2147483647", FormatType.DEC, (UInt32)2147483647)]
        [DataRow("2147483648", FormatType.DEC, (UInt32)2147483648)]
        [DataRow("4294967295", FormatType.DEC, (UInt32)4294967295)]
        [DataRow("000", FormatType.DEC, (UInt32)0)]
        [DataRow("002147483647", FormatType.DEC, (UInt32)2147483647)]
        [DataRow("00", FormatType.HEX, (UInt32)0)]
        [DataRow("01", FormatType.HEX, (UInt32)1)]
        [DataRow("7FFFFFFF", FormatType.HEX, (UInt32)2147483647)]
        [DataRow("80000000", FormatType.HEX, (UInt32)2147483648)]
        [DataRow("FFFFFFFF", FormatType.HEX, (UInt32)4294967295)]
        [DataRow("0", FormatType.HEX, (UInt32)0)]
        [DataRow("000", FormatType.HEX, (UInt32)0)]
        [DataRow("007FFFFFFF", FormatType.HEX, (UInt32)2147483647)]
        [DataRow("00FFFFFFFF", FormatType.HEX, (UInt32)4294967295)]
        [DataRow("0", FormatType.OCT, (UInt32)0)]
        [DataRow("1", FormatType.OCT, (UInt32)1)]
        [DataRow("17777777777", FormatType.OCT, (UInt32)2147483647)]
        [DataRow("20000000000", FormatType.OCT, (UInt32)2147483648)]
        [DataRow("37777777777", FormatType.OCT, (UInt32)4294967295)]
        [DataRow("0", FormatType.OCT, (UInt32)0)]
        [DataRow("000", FormatType.OCT, (UInt32)0)]
        [DataRow("0017777777777", FormatType.OCT, (UInt32)2147483647)]
        [DataRow("0037777777777", FormatType.OCT, (UInt32)4294967295)]
        [DataTestMethod]
        public void TestUInt32BackCoverter(string inputValue, string format, UInt32 expectedDisplayValue)
        {
            var varInfo = new UInt32DataValue(new DataValue(new Variant((UInt32)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("-1", FormatType.DEC)]
        [DataRow("4294967296", FormatType.DEC)]
        [DataRow("100000000", FormatType.HEX)]
        [DataRow("40000000000", FormatType.OCT)]
        [DataTestMethod]
        public void TestUInt32BackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new UInt32DataValue(new DataValue(new Variant((UInt32)0)));
            varInfo.FormatSelectedItem = format;
            Assert.ThrowsException<OverflowException>(() => varInfo.ConvertValueBack(inputValue));
        }


        [DataRow("0", FormatType.DEC, (Int64)0)]
        [DataRow("1", FormatType.DEC, (Int64)1)]
        [DataRow("9223372036854775807", FormatType.DEC, (Int64)9223372036854775807)]
        [DataRow("-9223372036854775808", FormatType.DEC, (Int64)(-9223372036854775808))]
        [DataRow("-1", FormatType.DEC, (Int64)(-1))]
        [DataRow("000", FormatType.DEC, (Int64)0)]
        [DataRow("009223372036854775807", FormatType.DEC, (Int64)9223372036854775807)]
        [DataRow("0000", FormatType.HEX, (Int64)0)]
        [DataRow("0001", FormatType.HEX, (Int64)1)]
        [DataRow("7FFFFFFFFFFFFFFF", FormatType.HEX, (Int64)9223372036854775807)]
        [DataRow("8000000000000000", FormatType.HEX, (Int64)(-9223372036854775808))]
        [DataRow("FFFFFFFFFFFFFFFF", FormatType.HEX, (Int64)(-1))]
        [DataRow("0", FormatType.HEX, (Int64)0)]
        [DataRow("000", FormatType.HEX, (Int64)0)]
        [DataRow("007FFFFFFFFFFFFFFF", FormatType.HEX, (Int64)9223372036854775807)]
        [DataRow("00FFFFFFFFFFFFFFFF", FormatType.HEX, (Int64)(-1))]
        [DataRow("0", FormatType.OCT, (Int64)0)]
        [DataRow("1", FormatType.OCT, (Int64)1)]
        [DataRow("777777777777777777777", FormatType.OCT, (Int64)9223372036854775807)]
        [DataRow("1000000000000000000000", FormatType.OCT, (Int64)(-9223372036854775808))]
        [DataRow("1777777777777777777777", FormatType.OCT, (Int64)(-1))]
        [DataRow("0", FormatType.OCT, (Int64)0)]
        [DataRow("000", FormatType.OCT, (Int64)0)]
        [DataRow("00777777777777777777777", FormatType.OCT, (Int64)9223372036854775807)]
        [DataRow("001777777777777777777777", FormatType.OCT, (Int64)(-1))]
        [DataTestMethod]
        public void TestInt64BackCoverter(string inputValue, string format, Int64 expectedDisplayValue)
        {
            var varInfo = new Int64DataValue(new DataValue(new Variant((Int64)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("9223372036854775808", FormatType.DEC)]
        [DataRow("-9223372036854775809", FormatType.DEC)]
        [DataRow("10000000000000000", FormatType.HEX)]
        [DataRow("2000000000000000000000", FormatType.OCT)]
        [DataTestMethod]
        public void TestInt64BackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new Int64DataValue(new DataValue(new Variant((Int64)0)));
            varInfo.FormatSelectedItem = format;
            Assert.ThrowsException<OverflowException>(() => varInfo.ConvertValueBack(inputValue));
        }

        [DataRow("0", FormatType.DEC, (UInt64)0)]
        [DataRow("1", FormatType.DEC, (UInt64)1)]
        [DataRow("9223372036854775807", FormatType.DEC, (UInt64)9223372036854775807)]
        [DataRow("9223372036854775808", FormatType.DEC, (UInt64)9223372036854775808)]
        [DataRow("18446744073709551615", FormatType.DEC, (UInt64)18446744073709551615)]
        [DataRow("000", FormatType.DEC, (UInt64)0)]
        [DataRow("009223372036854775807", FormatType.DEC, (UInt64)9223372036854775807)]
        [DataRow("00", FormatType.HEX, (UInt64)0)]
        [DataRow("01", FormatType.HEX, (UInt64)1)]
        [DataRow("7FFFFFFFFFFFFFFF", FormatType.HEX, (UInt64)9223372036854775807)]
        [DataRow("8000000000000000", FormatType.HEX, (UInt64)9223372036854775808)]
        [DataRow("FFFFFFFFFFFFFFFF", FormatType.HEX, (UInt64)18446744073709551615)]
        [DataRow("0", FormatType.HEX, (UInt64)0)]
        [DataRow("000", FormatType.HEX, (UInt64)0)]
        [DataRow("007FFFFFFFFFFFFFFF", FormatType.HEX, (UInt64)9223372036854775807)]
        [DataRow("00FFFFFFFFFFFFFFFF", FormatType.HEX, (UInt64)18446744073709551615)]
        [DataRow("0", FormatType.OCT, (UInt64)0)]
        [DataRow("1", FormatType.OCT, (UInt64)1)]
        [DataRow("777777777777777777777", FormatType.OCT, (UInt64)9223372036854775807)]
        [DataRow("1000000000000000000000", FormatType.OCT, (UInt64)9223372036854775808)]
        [DataRow("1777777777777777777777", FormatType.OCT, (UInt64)18446744073709551615)]
        [DataRow("0", FormatType.OCT, (UInt64)0)]
        [DataRow("000", FormatType.OCT, (UInt64)0)]
        [DataRow("00777777777777777777777", FormatType.OCT, (UInt64)9223372036854775807)]
        [DataRow("001777777777777777777777", FormatType.OCT, (UInt64)18446744073709551615)]
        [DataTestMethod]
        public void TestUInt64BackCoverter(string inputValue, string format, UInt64 expectedDisplayValue)
        {
            var varInfo = new UInt64DataValue(new DataValue(new Variant((UInt64)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("-1", FormatType.DEC)]
        [DataRow("18446744073709551616", FormatType.DEC)]
        [DataRow("10000000000000000", FormatType.HEX)]
        [DataRow("2000000000000000000000", FormatType.OCT)]
        [DataTestMethod]
        public void TestUInt64BackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new UInt64DataValue(new DataValue(new Variant((UInt64)0)));
            varInfo.FormatSelectedItem = format;
            Assert.ThrowsException<OverflowException>(() => varInfo.ConvertValueBack(inputValue));
        }

        [DataRow("-3.402823E+38", FormatType.FLOAT, (float)(-3.402823E+38))]
        [DataRow("3.402823E+38", FormatType.FLOAT, (float)(3.402823E+38))]
        [DataRow("0", FormatType.FLOAT, (float)(0))]
        [DataRow("1.234", FormatType.FLOAT, (float)(1.234))]
        [DataRow("-2.345", FormatType.FLOAT, (float)(-2.345))]
        [DataRow("1.401298464324817070923729583289916131280e-45", FormatType.FLOAT, (float)(1.401298464324817070923729583289916131280e-45))]
        [DataRow("-1.401298464324817070923729583289916131280e-45", FormatType.FLOAT, (float)(-1.401298464324817070923729583289916131280e-45))]
        [DataRow("-4.402823E+38", FormatType.FLOAT, (float)(-4.402823E+38))]
        [DataRow("-3.402823E+39", FormatType.FLOAT, (float)(-3.402823E+39))]
        [DataRow("1.401298464324817070923729583289916131280e-46", FormatType.FLOAT, (float)(0))]
        [DataRow("NaN", FormatType.FLOAT, float.NaN)]
        [DataRow("-NaN", FormatType.FLOAT, float.NaN)]
        [DataTestMethod]
        public void TestFloatBackCoverter(string inputValue, string format, float expectedDisplayValue)
        {
            var varInfo = new FloatDataValue(new DataValue(new Variant((float)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [DataRow("Åá", FormatType.FLOAT)]
        [DataRow("-Åá", FormatType.FLOAT)]
        [DataTestMethod]
        public void TestFloatBackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new FloatDataValue(new DataValue(new Variant((float)0)));
            varInfo.FormatSelectedItem = format;
            try
            {
                varInfo.ConvertValueBack(inputValue);
                Assert.Fail("Exception did not occur");
            }
            catch (OverflowException)
            {
                Assert.IsTrue(true);
            }
            catch (FormatException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [DataRow("-1.797693134862315708145274237317043567981e+308", FormatType.FLOAT, (double)(-1.797693134862315708145274237317043567981e+308))]
        [DataRow("1.797693134862315708145274237317043567981e+308", FormatType.FLOAT, (double)(1.797693134862315708145274237317043567981e+308))]
        [DataRow("0", FormatType.FLOAT, (double)(0))]
        [DataRow("1.234", FormatType.FLOAT, (double)(1.234))]
        [DataRow("-2.345", FormatType.FLOAT, (double)(-2.345))]
        [DataRow("4.940656458412465441765687928682213723651e-324", FormatType.FLOAT, (double)(4.940656458412465441765687928682213723651e-324))]
        [DataRow("-4.940656458412465441765687928682213723651e-324", FormatType.FLOAT, (double)(-4.940656458412465441765687928682213723651e-324))]
        [DataRow("4.940656458412465441765687928682213723651e-325", FormatType.FLOAT, (double)(0))]
        [DataRow("-2.797693134862315708145274237317043567981e+308", FormatType.FLOAT, (double)Double.NegativeInfinity)]
        [DataRow("-1.797693134862315708145274237317043567981e+309", FormatType.FLOAT, (double)Double.NegativeInfinity)]
        [DataRow("NaN", FormatType.FLOAT, double.NaN)]
        [DataRow("-NaN", FormatType.FLOAT, double.NaN)]
        [DataTestMethod]
        public void TestDoubleBackCoverter(string inputValue, string format, double expectedDisplayValue)
        {
            var varInfo = new DoubleDataValue(new DataValue(new Variant((double)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(expectedDisplayValue, varInfo.GetRawValue());
        }

        [DataRow("Åá", FormatType.FLOAT)]
        [DataRow("-Åá", FormatType.FLOAT)]
        [DataTestMethod]
        public void TestDoubleBackCoverterForIllegal(string inputValue, string format)
        {
            var varInfo = new DoubleDataValue(new DataValue(new Variant((double)0)));
            varInfo.FormatSelectedItem = format;
            try
            {
                varInfo.ConvertValueBack(inputValue);
                Assert.Fail("Exception did not occur");
            }
            catch (OverflowException)
            {
                Assert.IsTrue(true);
            }
            catch (FormatException)
            {
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [DataRow("", FormatType.STRING, "")]
        [DataRow(null, FormatType.STRING, "")]
        [DataRow("a", FormatType.STRING, "a")]
        [DataRow("Ç†", FormatType.STRING, "Ç†")]
        [DataRow("abcdefg", FormatType.STRING, "abcdefg")]
        [DataTestMethod]
        public void TestStringBackCoverter(string inputValue, string format, string expectedDisplayValue)
        {
            var varInfo = new StringDataValue(new DataValue(new Variant((Int64)0)));
            varInfo.FormatSelectedItem = format;
            varInfo.ConvertValueBack(inputValue);
            Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
        }

        [TestMethod]
        public void TestDateTimeBackCoverter()
        {
            var errMessages = new List<string>();

            var fn = new Action<string, string, DateTime>((inputValue, format, expectedDisplayValue) =>
            {
                var varInfo = new DateTimeDataValue(new DataValue(new Variant((Int64)0)));
                varInfo.FormatSelectedItem = format;
                varInfo.ConvertValueBack(inputValue);
                try
                {
                    Assert.AreEqual(varInfo.GetRawValue(), expectedDisplayValue);
                }
                catch (Exception e)
                {
                    errMessages.Add(e.Message);
                }
            });

            fn("0001-01-01 00:00:00.000", FormatType.DATE_AND_TIME, DateTime.MinValue);
            fn("9999-12-31 23:59:59.999", FormatType.DATE_AND_TIME, new DateTime(9999, 12, 31, 23, 59, 59, 999));

            if (errMessages.Count != 0)
            {
                var messages = "\n";
                foreach (var msg in errMessages)
                {
                    messages += string.Format(" - {0}\n", msg);
                }
                Assert.Fail(messages);
            }
        }
    }
}
