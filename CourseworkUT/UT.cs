using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary;

namespace CourseworkUT
{
    [TestClass]
    public class UT
    {
        [TestMethod]
        public void CalculateEnergy_1()
        {
            int[,] energyMatrix = { { 0, 6, 4, 8, 7, 14 },
                                    { 6, 0, 7, 11, 7, 10 },
                                    { 4, 7, 0, 4, 3, 10 },
                                    { 8, 11, 4, 0, 5, 11 },
                                    { 7, 7, 3, 5, 0, 7 },
                                    { 14, 10, 10, 11, 7, 0} };
            int expectedResult = 36;
            int actualResult = SimulatedAnnealingClass.CalculateEnergy(new int[] { 1, 2, 6, 5, 4, 3 }, energyMatrix);

            Assert.AreEqual(expectedResult, actualResult, "Посчитанная энергия состояния (" + actualResult + ") не совпадает с ожидаемой (" + expectedResult + ").");
        }

        [TestMethod]
        public void CalculateEnergy_2()
        {
            int[,] energyMatrix = { { 0, 4, 6, 2, 9 },
                                    { 4, 0, 3, 2, 9 },
                                    { 6, 3, 0, 5, 9 },
                                    { 2, 2, 5, 0, 8 },
                                    { 9, 9, 9, 8, 0 },
                                                    };
            int expectedResult = 25;
            int actualResult = SimulatedAnnealingClass.CalculateEnergy(new int[] { 1, 4, 2, 3, 5 }, energyMatrix);

            Assert.AreEqual(expectedResult, actualResult, "Посчитанная энергия состояния (" + actualResult + ") не совпадает с ожидаемой (" + expectedResult + ").");
        }

        [TestMethod]
        public void CalculateEnergy_3()
        {
            int[,] energyMatrix = { { 0, 1, 2, 7, 5 },
                                    { 1, 0, 4, 4, 3 },
                                    { 2, 4, 0, 1, 2 },
                                    { 7, 4, 1, 0, 3 },
                                    { 5, 3, 2, 3, 0 }
                                                    };
            int expectedResult = 13;
            int actualResult = SimulatedAnnealingClass.CalculateEnergy(new int[] { 1, 5, 3, 4, 2 }, energyMatrix);

            Assert.AreEqual(expectedResult, actualResult, "Посчитанная энергия состояния (" + actualResult + ") не совпадает с ожидаемой (" + expectedResult + ").");
        }
    }
}
