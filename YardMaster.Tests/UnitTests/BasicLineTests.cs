using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YardMaster.Tests.UnitTests
{
    [TestClass]
    public class BasicLineTests
    {
        [TestMethod]
        public void TestCTOR_Default()
        {
            var basicLine = new BasicLine();
            Assert.AreEqual(String.Empty, basicLine.Cars);
            Assert.AreEqual(-1, basicLine.Index);
            Assert.AreEqual("the train line", basicLine.Name);
        }

        [TestMethod]
        public void TestCTOR_ParametersGood()
        {
            var basicLine = new BasicLine("0012", 2);
            Assert.AreEqual("12", basicLine.Cars);
            Assert.AreEqual(2, basicLine.Index);
            Assert.AreEqual("line 3", basicLine.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
    "the line '0123456789A' has more than 10 characters")]
        public void TestCTOR_ParametersWrong()
        {
            var basicLine = new BasicLine("0123456789A");
        }

        [TestMethod]
        public void TestCTOR_MoveGood()
        {
            var basicLine = new BasicLine("0012", 2);
            var basicLine2 = new BasicLine("00222", 3);
            basicLine.Move(basicLine2, 2);
            Assert.AreEqual("12222", basicLine2.Cars);
            Assert.AreEqual("", basicLine.Cars);
        }

        [TestMethod]
        public void TestCTOR_MoveTwoPartsGood()
        {
            var basicLine = new BasicLine("0012233", 2);
            var basicLine2 = new BasicLine("00222", 3);
            basicLine.Move(basicLine2, 5);
            Assert.AreEqual("33122222", basicLine2.Cars);
            Assert.AreEqual("", basicLine.Cars);
        }

    }
}
