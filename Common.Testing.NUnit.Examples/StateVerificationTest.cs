namespace Common.Testing.NUnit.Examples
{
    using global::NUnit.Framework;
    using Moq;

    public interface ILoggerDependency
    {
        string GetCurrentDirectory();
        string GetDirectoryByLoggerName(string loggerName);
        string DefaultLogger { get; }
    }
    
    [TestFixture]
    public class StateVerificationTest
    {
        [Test]
        public void Stub_GetCurrentDirectory()
        {
            // Mock.Of возвращает саму зависимость (прокси-объект), а не мок-объект.
            // Следующий код означает, что при вызове GetCurrentDirectory()
            // мы получим "D:\\Temp"
            var loggerDependency = Mock.Of<ILoggerDependency>(d => d.GetCurrentDirectory() == "D:\\Temp");
            var currentDirectory = loggerDependency.GetCurrentDirectory();
            Assert.That(currentDirectory, Is.EqualTo("D:\\Temp"));
        }

        [Test]
        public void Stub_GetDirectoryByLoggerName()
        {
            // Для любого аргумента метода GetDirectoryByLoggerName вернуть "C:\\Foo".
            var loggerDependency = Mock.Of<ILoggerDependency>(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Foo");
            var directory = loggerDependency.GetDirectoryByLoggerName("anything");
            Assert.That(directory, Is.EqualTo("C:\\Foo"));
        }

        [Test]
        public void Stub_GetDirectoryByLoggerName_ReturnArg()
        {
            // Инициализируем заглушку таким образом, чтобы возвращаемое значение
            // метода GetDirrectoryByLoggerName зависело от аргумента метода.
            // Код аналогичен заглушке вида:
            // public string GetDirectoryByLoggername(string s) { return "C:\\" + s; }
            var stub = new Mock<ILoggerDependency>();

            stub.Setup(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()))
                .Returns<string>(name => "C:\\" + name);

            var loggerName = "SomeLogger";
            var logger = stub.Object;
            var directory = logger.GetDirectoryByLoggerName(loggerName);

            Assert.That(directory, Is.EqualTo("C:\\" + loggerName));
        }

        [Test]
        public void Stub_DefaultLogger()
        {
            // Свойство DefaultLogger нашей заглушки будет возвращать указанное значение
            var logger = Mock.Of<ILoggerDependency>(d => d.DefaultLogger == "DefaultLogger");
            var defaultLogger = logger.DefaultLogger;
            Assert.That(defaultLogger, Is.EqualTo("DefaultLogger"));
        }

        [Test]
        public void ManyMock()
        {
            // Объединяем заглушки разных методов с помощью логического «И»
            var logger = Mock.Of<ILoggerDependency>(
                d => d.GetCurrentDirectory() == "D:\\Temp" &&
                     d.DefaultLogger == "DefaultLogger" &&
                     d.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Temp");

            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }

        [Test]
        public void ManySetup()
        {
            var stub = new Mock<ILoggerDependency>();
            stub.Setup(ld => ld.GetCurrentDirectory()).Returns("D:\\Temp");
            stub.Setup(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>())).Returns("C:\\Temp");
            stub.SetupGet(ld => ld.DefaultLogger).Returns("DefaultLogger");

            var logger = stub.Object;

            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }
    }
}
