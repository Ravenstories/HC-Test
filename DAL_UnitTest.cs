using HeliostatCentral.DAL;
using HeliostatCentral.Models;
using Moq;


namespace HC_TestEnvironment
{
    /// <summary>
    /// Test Environment for DAL Unit Tests
    /// </summary>
    [TestClass]
    public class DAL_UnitTest
    {
        [TestMethod]
        public void ReadAllLines_ReadsDataCorrectly()
        {
            var dal = new TextBasedDAL();
            var lines = dal.ReadAllLines(); // This method will need to be public for testing, change it back to private after testing.
            Assert.IsTrue(lines.Count > 0, "No lines read from file.");
        }

        [TestMethod]
        public void LoadRecordings_LoadsCorrectNumberOfRecordings()
        {
            var dal = new TextBasedDAL();
            var recordings = dal.LoadRecordings();
            int expectedNumber = 10; // Change this to the expected number of recordings in the file
            Assert.AreEqual(expectedNumber, recordings.Count, "Loaded recordings count does not match expected.");
        }

        [TestMethod]
        public void ConvertDataToHeliostat_ConvertsCorrectly()
        {
            var dal = new TextBasedDAL();
            var input = "10,20,30,2023-04-01 14:20:00";
            var result = dal.ConvertDataToHeliostat(input);
            Assert.AreEqual(10, result.HorizontalDegrees);
            Assert.AreEqual(20, result.VerticalDegrees);
            Assert.AreEqual(30, result.LightLevel);
            Assert.AreEqual(new DateTime(2023, 4, 1, 14, 20, 0), result.DateTimeStamp);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertDataToHeliostat_HandlesFormatException()
        {
            var dal = new TextBasedDAL();
            var input = "invalid,data,here";
            var result = dal.ConvertDataToHeliostat(input); // This test expects an exception to be thrown
        }

        [TestMethod]
        public void SaveRecording_SavesDataCorrectly()
        {
            // Use mocking framework to mock the StreamWriter
            var mockStreamWriter = new Mock<StreamWriter>(/* Constructor arguments for StreamWriter */);
            mockStreamWriter.Setup(x => x.WriteLine(It.IsAny<string>()));

            var dal = new TextBasedDAL();
            // Inject mock StreamWriter here or refactor DAL to allow setting StreamWriter

            var hrs = new List<HeliostatRecording> { /* Populate with sample data */ };
            dal.SaveRecording(hrs);

            mockStreamWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce());
        }
    }
}