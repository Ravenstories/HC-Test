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
            var testData = "149,126,250,18-04-2024 08:00:05\n150,127,251,19-04-2024 08:00:06";
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(testData));
            var streamReader = new StreamReader(memoryStream);
            var streamWriter = new StreamWriter(memoryStream);

            var dal = new TextBasedDAL(streamReader, streamWriter);
            memoryStream.Position = 0; // Reset the stream position for reading

            var lines = dal.ReadAllLines();
            Assert.AreEqual(2, lines.Count, "Expected to read two lines.");
        }

        [TestMethod]
        public void LoadRecordings_LoadsCorrectNumberOfRecordings()
        {
            var testData = "10,20,30,2023-04-01 14:20:00\n11,21,31,2023-04-02 15:25:00";
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            streamWriter.AutoFlush = true;

            // Write test data to the memory stream
            streamWriter.WriteLine(testData);
            streamWriter.Flush();
            memoryStream.Position = 0;  // Reset the stream position for reading

            // Inject StreamReader into DAL for testing
            var dal = new TextBasedDAL(streamReader, streamWriter);
            var recordings = dal.LoadRecordings();
            int expectedNumber = 2; // This should match the number of lines written to memoryStream
            Assert.AreEqual(expectedNumber, recordings.Count, "Loaded recordings count does not match expected.");
        }


        [TestMethod]
        public void ConvertDataToHeliostat_ConvertsCorrectly()
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var streamReader = new StreamReader(memoryStream))
            {
                var dal = new TextBasedDAL(streamReader, streamWriter);
                var input = "10,20,30,2023-04-01 14:20:00";
                var result = dal.ConvertDataToHeliostat(input);
                Assert.AreEqual(10, result.HorizontalDegrees);
                Assert.AreEqual(20, result.VerticalDegrees);
                Assert.AreEqual(30, result.LightLevel);
                Assert.AreEqual(new DateTime(2023, 4, 1, 14, 20, 0), result.DateTimeStamp);
            }
        }

        [TestMethod]
        public void SaveRecording_SavesDataCorrectly()
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            streamWriter.AutoFlush = true;  // Important to flush content into the stream

            // Inject the DAL with the properly initialized StreamWriter
            var dal = new TextBasedDAL(streamReader, streamWriter);

            var hrs = new List<HeliostatRecording>
            {
                new HeliostatRecording(149, 126, 250, DateTime.Parse("18-04-2024 08:00:05"), true)
            };

            // Act: save recordings
            dal.SaveRecording(hrs);
            streamWriter.Flush();
            memoryStream.Position = 0;  // Reset the position to read from the beginning
            var reader = new StreamReader(memoryStream);
            var writtenContent = reader.ReadToEnd();
            Console.WriteLine($"Written Content: \"{writtenContent}\"");  // Output the actual content for debugging
            Assert.IsTrue(writtenContent.Contains("149,126,250,18-04-2024 08:00:05"), "Content does not match expected value.");
        }

    }
}