namespace Common.Testing.NUnit.Examples
{
    using System.Linq;
    using System.Net.Mail;
    using global::NUnit.Framework;
    using Moq;

    public interface ILogMailer
    {
        void Send(MailMessage msg);
    }

    public class SmartLogger
    {
        private readonly ILogWriter _logWriter;
        private readonly ILogMailer _logMailer;

        public SmartLogger(ILogWriter logWriter, ILogMailer logMailer)
        {
            _logWriter = logWriter;
            _logMailer = logMailer;
        }

        public void WriteLine(string str)
        {
            _logWriter.Write(str);
            _logMailer.Send(null);
        }
    }
      
    [TestFixture]
    public class MockRepositoryTest
    {
        [Test]
        public void MockRep_Of()
        {
            var repository = new MockRepository(MockBehavior.Default);
            var logger = repository.Of<ILoggerDependency>()
                .Where(ld => ld.DefaultLogger == "DefaultLogger")
                .Where(ld => ld.GetCurrentDirectory() == "D:\\Temp")
                .First(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()) == "C:\\Temp");

            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("CustomLogger"), Is.EqualTo("C:\\Temp"));
        }
        
        /// <summary>
        /// Не понятно
        /// </summary>
        [Test]
        public void MockRep_TwoDependencies()
        {
            var repo = new MockRepository(MockBehavior.Strict);
            var logWriterMock = repo.Create<ILogWriter>();
            logWriterMock.Setup(lw => lw.Write(It.IsAny<string>()));

            var logMailerMock = repo.Create<ILogMailer>();
            logMailerMock.Setup(lm => lm.Send(It.IsAny<MailMessage>()));

            var smartLogger = new SmartLogger(logWriterMock.Object, logMailerMock.Object);

            smartLogger.WriteLine("Hello, Logger");

            repo.Verify();
        }

        [Test]
        public void MockRep_3()
        {
            var logger = Mock.Of<ILoggerDependency>(
                ld => ld.GetCurrentDirectory() == "D:\\Temp" &&
                      ld.DefaultLogger == "DefaultLogger");

            // Задаем более сложное поведение метода GetDirectoryByLoggerName
            // для возвращения разных результатов, в зависимости от аргумента
            Mock.Get(logger)
                .Setup(ld => ld.GetDirectoryByLoggerName(It.IsAny<string>()))
                .Returns<string>(loggerName => "C:\\" + loggerName);

            Assert.That(logger.GetCurrentDirectory(), Is.EqualTo("D:\\Temp"));
            Assert.That(logger.DefaultLogger, Is.EqualTo("DefaultLogger"));
            Assert.That(logger.GetDirectoryByLoggerName("Foo"), Is.EqualTo("C:\\Foo"));
            Assert.That(logger.GetDirectoryByLoggerName("Boo"), Is.EqualTo("C:\\Boo"));
        }
    }
}
