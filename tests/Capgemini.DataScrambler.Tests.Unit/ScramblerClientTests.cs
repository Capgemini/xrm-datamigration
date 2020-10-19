using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Capgemini.DataScrambler.Factories;
using Capgemini.DataScrambler.Scramblers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Capgemini.DataScrambler.Tests.Unit
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ScramblerClientTests
    {
        [TestMethod]
        public void ExecuteStringScramblerTest()
        {
            IScrambler<string> scrambler = new StringScrambler();
            ScramblerClient<string> client = new ScramblerClient<string>(scrambler);
            string input = "Hello World";
            string output = client.ExecuteScramble(input);

            Assert.AreNotEqual(input, output);
            Assert.AreEqual(input.Length, output.Length);
        }

        [TestMethod]
        public void ExecuteIntegerScramblerTest()
        {
            IScrambler<int> scrambler = new IntegerScrambler();
            ScramblerClient<int> client = new ScramblerClient<int>(scrambler);

            int input = 5;
            int output = client.ExecuteScramble(input, 0, 5);

            Assert.AreNotEqual(input, output);
        }

        [TestMethod]
        public void ExecuteEmailScramblerTest()
        {
            IScrambler<string> scrambler = new EmailScambler();
            ScramblerClient<string> client = new ScramblerClient<string>(scrambler);

            string input = "bob@example.com";
            string output = client.ExecuteScramble(input);

            Assert.AreNotEqual(input, output);
            Assert.IsTrue(output.Contains("@"));
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void ExecuteEmailScramblerExceptionTest()
        {
            IScrambler<string> scrambler = new EmailScambler();
            ScramblerClient<string> client = new ScramblerClient<string>(scrambler);

            client.ExecuteScramble("this is not an email");
        }

        [TestMethod()]
        public void ExecuteGuidScramblerTest()
        {
            IScrambler<Guid> scrambler = new GuidScrambler();
            ScramblerClient<Guid> client = new ScramblerClient<Guid>(scrambler);

            Guid input = Guid.NewGuid();
            Guid output = client.ExecuteScramble(input);

            Assert.AreNotEqual(input, output);
        }

        [TestMethod()]
        public void ExecuteDoubleScramblerTest()
        {
            IScrambler<double> scrambler = new DoubleScrambler();
            ScramblerClient<double> client = new ScramblerClient<double>(scrambler);

            double input = 10.5493;
            double output = client.ExecuteScramble(input);

            Assert.AreNotEqual(input, output);
        }

        [TestMethod()]
        public void ExecuteDecimalScramblerTest()
        {
            IScrambler<decimal> scrambler = new DecimalScrambler();
            ScramblerClient<decimal> client = new ScramblerClient<decimal>(scrambler);

            decimal input = 5.432M;
            decimal output = client.ExecuteScramble(input);

            Assert.AreNotEqual(input, output);
        }

        [TestMethod()]
        public void ExecuteMetricsStringScramblerTest()
        {
            IScrambler<string> scrambler = new StringScrambler();
            ScramblerClient<string> client = new ScramblerClient<string>(scrambler);

            List<string> inputStrings = new List<string>();
            for (var i = 0; i <= 100000; i++)
            {
                inputStrings.Add(String.Format("Hello World" + i));
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (string str in inputStrings)
            {
                client.ExecuteScramble(str);
            }
            watch.Stop();
            var elapsedSeconds = watch.ElapsedMilliseconds / 1000;
            Console.WriteLine(String.Format("Time taken: ", elapsedSeconds));

        }

        [TestMethod()]
        public void ExecuteScramblerFactoryTest()
        {
            ScramblerClientFactory factory = new ScramblerClientFactory();

            ScramblerClient<String> strClient = factory.GetScrambler<string>();
            string inputStr = "Hello world";
            string outputStr = strClient.ExecuteScramble(inputStr);
            Assert.AreNotEqual(inputStr, outputStr);

            ScramblerClient<int> intClient = factory.GetScrambler<int>();
            int inputInt = 1;
            int outputInt = intClient.ExecuteScramble(inputInt, 2, 10);
            Assert.AreNotEqual(inputInt, outputInt);

            ScramblerClient<Guid> guidClient = factory.GetScrambler<Guid>();
            Guid inputGuid = Guid.NewGuid();
            Guid outputGuid = guidClient.ExecuteScramble(inputGuid);
            Assert.AreNotEqual(inputGuid, outputGuid);

            ScramblerClient<double> doubleClient = factory.GetScrambler<double>();
            double inputDouble = 1;
            double outputDouble = doubleClient.ExecuteScramble(inputDouble, 2, 10);
            Assert.AreNotEqual(inputDouble, outputDouble);

            ScramblerClient<decimal> decimalClient = factory.GetScrambler<decimal>();
            decimal inputDecimal = 1;
            decimal outputDecimal = decimalClient.ExecuteScramble(inputDecimal, 2, 10);
            Assert.AreNotEqual(inputDecimal, outputDecimal);
        }
    }
}
