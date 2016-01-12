using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YardMaster.Tests.UnitTests
{
    [TestClass]
    public class YardLineTests
    {
        [TestMethod]
        public void TestCTOR_Easy()
        {
            var yardLine = new YardLine('A', "00ABCDEF", 0);
            Assert.AreEqual(1, yardLine.CarsCount());
            Assert.AreEqual(false, yardLine.IsTrash());
            Assert.AreEqual(1, yardLine.MovementNeeded());
            Assert.AreEqual(4, yardLine.SpaceAvailable());
            Assert.AreEqual(0, yardLine.SpaceNeededForNext());
            Assert.AreEqual(5, yardLine.TrashCapacity());
        }

        [TestMethod]
        public void TestCTOR_Hard()
        {
            var yardLine = new YardLine('A', "00ABCDAF", 0);
            Assert.AreEqual(2, yardLine.CarsCount());
            Assert.AreEqual(false, yardLine.IsTrash());
            Assert.AreEqual(3, yardLine.MovementNeeded());
            Assert.AreEqual(4, yardLine.SpaceAvailable());
            Assert.AreEqual(0, yardLine.SpaceNeededForNext());
            Assert.AreEqual(9, yardLine.TrashCapacity());
        }

        [TestMethod]
        public void TestCTOR_NoCar()
        {
            var yardLine = new YardLine('X', "00ABCDAF", 0);
            Assert.AreEqual(0, yardLine.CarsCount());
            Assert.AreEqual(true, yardLine.IsTrash());
            Assert.AreEqual(0, yardLine.MovementNeeded());
            Assert.AreEqual(4, yardLine.SpaceAvailable());
            Assert.AreEqual(-1, yardLine.SpaceNeededForNext());
            Assert.AreEqual(4, yardLine.TrashCapacity());
        }
    }
}
