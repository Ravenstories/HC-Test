using HeliostatCentral.DAL;
using HeliostatCentral.Models;
using Moq;
using System.IO;
using System.Text;


namespace HC_TestEnvironment
{
    /// <summary>
    /// Test Environment for DAL Unit Tests
    /// </summary>
    [TestClass]
    public class DAL_UnitTest
    {
        // Recordings example: 149,126,250,18-04-2024 08:00:05

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
            int expectedNumber = 5; // Change this to the expected number of recordings in the file
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
        public void SaveRecording_SavesDataCorrectly()
        {
            // Setup a memory stream to capture StreamWriter output without file I/O
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.AutoFlush = true;  // Important to flush content into the stream

            // Create a mock StreamWriter using the actual stream writer
            var mockStreamWriter = new Mock<StreamWriter>(memoryStream, Encoding.UTF8, 1024, true) { CallBase = true };
            mockStreamWriter.Setup(x => x.WriteLine(It.IsAny<string>()));

            // Inject mock StreamWriter into DAL
            var dal = new TextBasedDAL(mockStreamWriter.Object);

            // Create sample data
            var hrs = new List<HeliostatRecording>
            {
                new HeliostatRecording(149, 126, 250, DateTime.Parse("18-04-2024 08:00:05"), true)
            };

            // Act: save recordings
            dal.SaveRecording(hrs);

            // Verify that WriteLine was called at least once
            mockStreamWriter.Verify(x => x.WriteLine(It.IsAny<string>()), Times.AtLeastOnce());

            // Optionally, check the content written to MemoryStream
            memoryStream.Position = 0; // Reset the position to read from the beginning
            var reader = new StreamReader(memoryStream);
            var writtenContent = reader.ReadToEnd();
            Assert.IsTrue(writtenContent.Contains("149,126,250,18-04-2024 08:00:05"), "Content does not match expected value.");
        }

    }
}