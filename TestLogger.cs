namespace TestProject
{
    public class TestLogger
    {
        private Task1.Logger logger;

        private string excelFile = "log.xlsx";

        private string wordFile = "log.docx";

        [SetUp]

        public void Setup()

        {

            logger = new Task1.Logger();

            if (File.Exists(excelFile)) File.Delete(excelFile);

            if (File.Exists(wordFile)) File.Delete(wordFile);

        }

        [Test]

        public void Log_ShouldCreateExcelFile()

        {

            logger.Log("Test message");

            Assert.IsTrue(File.Exists(excelFile));

        }

        [Test]

        public void Log_ShouldCreateWordFile()

        {

            logger.Log("Test message");

            Assert.IsTrue(File.Exists(wordFile));

        }

    }
}