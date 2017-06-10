namespace Common.Testing.NUnit.Examples
{
    using global::NUnit.Framework;
    using Moq;

    public interface ILogWriter
    {
        string GetLogger();
        void SetLogger(string logger);
        void Write(string message);
    }
    public class Logger
    {
        private readonly ILogWriter _logWriter;

        public Logger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public void WriteLine(string message)
        {
            _logWriter.Write(message);
        }
    }
    
    [TestFixture]
    public class BehaviorVerificationTest
    {
        [Test]
        public void CheckCall_WriteAnyArg()
        {
            var mock = new Mock<ILogWriter>();
            var logger = new Logger(mock.Object);

            logger.WriteLine("Hello, logger!");

            // Проверяем, что вызвался метод Write нашего мока с любым аргументом
            mock.Verify(lw => lw.Write(It.IsAny<string>()));
            
            // Проверка вызова метода ILogWriter.Write с заданным аргументами:
            mock.Verify(lw => lw.Write("Hello, logger!"));
        }

        [Test]
        public void CheckCall_WriteArg()
        {
            var mock = new Mock<ILogWriter>();
            var logger = new Logger(mock.Object);

            logger.WriteLine("Hello, logger!");

            // Проверка вызова метода ILogWriter.Write с заданным аргументами:
            mock.Verify(lw => lw.Write("Hello, logger!"));
        }

        [Test]
        public void CheckCall_WriteOnceCall()
        {
            var mock = new Mock<ILogWriter>();
            var logger = new Logger(mock.Object);

            logger.WriteLine("Hello, logger!");

            // Проверка того, что метод ILogWriter.Write вызвался в точности один раз (ни больше, ни меньше): 
            mock.Verify(lw => lw.Write(It.IsAny<string>()),
                Times.Once());
        }
        
        [Test]
        public void CheckCall_Verify()
        { 
            var mock = new Mock<ILogWriter>();
            mock.Setup(lw => lw.Write(It.IsAny<string>()));

            var logger = new Logger(mock.Object);
            logger.WriteLine("Hello, logger!");

            // Мы не передаем методу Verify никаких дополнительных параметров.
            // Это значит, что будут использоваться ожидания установленные
            // с помощью mock.Setup
            mock.Verify();
        }
        
        [Test]
        public void CheckCall_VerifyManySetup()
        {
            // В некоторых случаях неудобно использовать несколько методов Verify для проверки нескольких вызовов. 
            // Вместо этого можно создать мок-объект и задать ожидаемое поведение с помощью методов Setup и проверять 
            // все эти допущения путем вызова одного метода Verify(). Такая техника может быть удобной для повторного 
            // использования мок-объектов, создаваемых в методе Setup теста.
            var mock = new Mock<ILogWriter>();
            mock.Setup(lw => lw.Write(It.IsAny<string>()));
            mock.Setup(lw => lw.SetLogger(It.IsAny<string>()));

            var logger = new Logger(mock.Object);
            logger.WriteLine("Hello, logger!");

            mock.Verify();
        }

        [Test]
        public void CheckCall_VerifyManySetup_MockStrict()
        {
            var mock = new Mock<ILogWriter>(MockBehavior.Strict);
            // Если закомментировать следующюю строку, то
            // метод mock.Verify() завершится с исключением
            mock.Setup(lw => lw.Write(It.IsAny<string>()));
            mock.Setup(lw => lw.SetLogger(It.IsAny<string>()));

            var logger = new Logger(mock.Object);
            logger.WriteLine("Hello, logger!");

            mock.Verify();
        }
    }
}
