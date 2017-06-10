namespace Common.Testing.NUnit.Examples
{
    using System;
    using System.Text.RegularExpressions;
    using global::NUnit.Framework;
    using Moq;
    using Moq.Protected;

    public interface IBar
    {
        bool Submit();
    }

    // Assumptions:
    public class Bar : IBar
    {
        // Bar implementation
        public bool Submit()
        {
            return false;
        }
    }

    public class FooEventArgs : EventArgs
    {
        public int Value { get; set; }
    }

    public delegate void MyEventHandler(int i, bool b);

    public interface IFoo
    {
        event EventHandler<FooEventArgs> FooEvent;
        event EventHandler Added;
        event MyEventHandler MyEvent;

        string Name { get; set; }
        int Value { get; set; }
        IBar Bar { get; set; }

        bool DoSomething(string s);
        string DoSomethingStringy(string s);
        bool TryParse(string s1, out string s2);
        bool Submit(ref Bar b);
        bool Add(int a);
        int GetCount();
        int GetCountThing();
        bool Execute(string str);
        bool Execute(int i, string str);

        string ToString();
    }

    [TestFixture]
    public class QuickStartTestMethods
    {
        [Test]
        public void TestMethods_Example01()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(foo => foo.DoSomething("ping")).Returns(true);

            Assert.IsFalse(mock.Object.DoSomething(""));
            Assert.IsTrue(mock.Object.DoSomething("ping"));
        }

        [Test]
        public void TestMethods_Example02()
        {
            var mock = new Mock<IFoo>();
            // out arguments
            var outString = "ack";
            // TryParse will return true, and the out argument will return "ack", lazy evaluated
            mock.Setup(foo => foo.TryParse("ping", out outString)).Returns(true);

            string s2 = "empty";
            mock.Object.TryParse("", out s2);
            Assert.That(s2, Is.EqualTo("empty"));

            mock.Object.TryParse("ping", out s2);
            Assert.That(s2, Is.EqualTo("ack"));
        }

        [Test]
        public void TestMethods_Example03()
        {
            var mock = new Mock<IFoo>();

            // ref arguments
            var instance = new Bar();
            // Only matches if the ref argument to the invocation is the same instance
            mock.Setup(foo => foo.Submit(ref instance)).Returns(true);

            var newBar = new Bar();
            Assert.IsFalse(mock.Object.Submit(ref newBar));

            var bar = instance;
            Assert.IsTrue(mock.Object.Submit(ref bar));
        }

        [Test]
        public void TestMethods_Example04()
        {
            var mock = new Mock<IFoo>();

            // access invocation arguments when returning a value
            mock.Setup(x => x.DoSomethingStringy(It.IsAny<string>()))
                .Returns((string s) => s.ToLower());
            // Multiple parameters overloads available

            Assert.That(mock.Object.DoSomethingStringy("hello"), Is.EqualTo("hello"));
            Assert.That(mock.Object.DoSomethingStringy("HeLLo"), Is.EqualTo("hello"));
            Assert.That(mock.Object.DoSomethingStringy("WORLD"), Is.EqualTo("world"));
        }

        [Test]
        public void TestMethods_Example05()
        {
            var mock = new Mock<IFoo>();

            // throwing when invoked with specific parameters
            mock.Setup(foo => foo.DoSomething("reset")).Throws<InvalidOperationException>();
            mock.Setup(foo => foo.DoSomething("")).Throws(new ArgumentException("command"));

            Assert.DoesNotThrow(() => mock.Object.DoSomething("nothing"));
            Assert.Throws<InvalidOperationException>(() => mock.Object.DoSomething("reset"));
            Assert.Throws<ArgumentException>(() => mock.Object.DoSomething(""));
        }

        [Test]
        public void TestMethods_Example06()
        {
            var mock = new Mock<IFoo>();

            // lazy evaluating return value
            const int count = 2;
            mock.Setup(foo => foo.GetCount()).Returns(() => count);

            Assert.That(mock.Object.GetCount(), Is.EqualTo(2));
        }

        [Test]
        public void TestMethods_Example07()
        {
            var mock = new Mock<IFoo>();


            // returning different values on each invocation
            var calls = 0;
            mock.Setup(foo => foo.GetCountThing())
                .Returns(() => calls)
                .Callback(() => calls++);

            // returns 0 on first invocation, 1 on the next, and so on
            Assert.That(mock.Object.GetCountThing(), Is.EqualTo(0));
            Assert.That(mock.Object.GetCountThing(), Is.EqualTo(1));
            Assert.That(mock.Object.GetCountThing(), Is.EqualTo(2));
        }
    }

    [TestFixture]
    public class QuickStartTestMatchingArguments
    {
        [Test]
        public void TestMatchingArguments_Example01()
        {
            var mock = new Mock<IFoo>();

            // any value
            mock.Setup(foo => foo.DoSomething(It.IsAny<string>())).Returns(true);

            Assert.IsTrue(mock.Object.DoSomething("abc"));
        }

        [Test]
        public void TestMatchingArguments_Example02()
        {
            var mock = new Mock<IFoo>();

            // matching Func<int>, lazy evaluated
            mock.Setup(foo => foo.Add(It.Is<int>(i => i%2 == 0))).Returns(true);

            Assert.IsTrue(mock.Object.Add(2));
            Assert.IsFalse(mock.Object.Add(1));
        }

        [Test]
        public void TestMatchingArguments_Example03()
        {
            var mock = new Mock<IFoo>();

            // matching ranges
            mock.Setup(foo => foo.Add(It.IsInRange<int>(0, 10, Range.Inclusive))).Returns(true);

            Assert.IsFalse(mock.Object.Add(-1));
            Assert.IsTrue(mock.Object.Add(0));
            Assert.IsTrue(mock.Object.Add(10));
            Assert.IsFalse(mock.Object.Add(11));
        }

        [Test]
        public void TestMatchingArguments_Example04()
        {
            var mock = new Mock<IFoo>();

            // matching regex
            mock.Setup(x => x.DoSomethingStringy(It.IsRegex("[a-d]+", RegexOptions.IgnoreCase))).Returns("foo");

            Assert.That(mock.Object.DoSomethingStringy(""), Is.Null);
            Assert.That(mock.Object.DoSomethingStringy("123"), Is.Null);
            Assert.That(mock.Object.DoSomethingStringy("a"), Is.EqualTo("foo"));
            Assert.That(mock.Object.DoSomethingStringy("abcd"), Is.EqualTo("foo"));
        }
    }

    [TestFixture]
    public class QuickStartTestProperties
    {
        [Test]
        public void TestProperties_Example01()
        {
            var mock = new Mock<IFoo>();

            mock.Setup(foo => foo.Name).Returns("bar");

            Assert.That(mock.Object.Name, Is.EqualTo("bar"));
        }

        [Test]
        public void TestProperties_Example02()
        {
            var mock = new Mock<IFoo>();

            // auto-mocking hierarchies (a.k.a. recursive mocks)
            // ToDo mock.Setup(foo => foo.Bar.Baz.Name).Returns("baz");
        }

        [Test]
        public void TestProperties_Example03()
        {
            var mock = new Mock<IFoo>();

            // expects an invocation to set the value to "foo"
            mock.SetupSet(foo => foo.Name = "foo");
        }

        [Test]
        public void TestProperties_Example04()
        {
            var mock = new Mock<IFoo>();

            mock.Object.Name = "foo";

            // or verify the setter directly
            mock.VerifySet(foo => foo.Name = "foo");
        }

        [Test]
        public void TestProperties_Example05()
        {
            var mock = new Mock<IFoo>();

            // Setup a property so that it will automatically start tracking its value (also known as Stub):
            // start "tracking" sets/gets to this property
            mock.SetupProperty(f => f.Name);
        }

        [Test]
        public void TestProperties_Example06()
        {
            var mock = new Mock<IFoo>();

            // alternatively, provide a default value for the stubbed property
            mock.SetupProperty(f => f.Name, "foo");

            Assert.That(mock.Object.Name, Is.EqualTo("foo"));
        }
    }

    [TestFixture]
    public class QuickStartTestEvents
    {
        [Test]
        public void TestEvents_Example01()
        {
            var mock = new Mock<IFoo>();
            var raisedVal = 0;
            mock.Object.FooEvent += (s, e) => raisedVal = e.Value;

            // Raising an event on the mock
            mock.Raise(m => m.FooEvent += null, new FooEventArgs {Value = 25});

            Assert.AreEqual(25, raisedVal);
        }

        [Test]
        public void TestEvents_Example02()
        {
            var mock = new Mock<IFoo>();

            // Raising an event on a descendant down the hierarchy
            // ToDo mock.Raise(m => m.Child.First.FooEvent += null, new FooEventArgs(fooValue));
        }

        [Test]
        public void TestEvents_Example03()
        {
            var mock = new Mock<IFoo>();
            var bar = new Bar();
            // Causing an event to raise automatically when Submit is invoked
            mock.Setup(x => x.Submit(ref bar)).Returns(true)
                .Raises(x => x.FooEvent += null, new FooEventArgs());

            // The raised event would trigger behavior on the object under test, which 
            // you would make assertions about later (how its state changed as a consequence, typically)

            mock.Setup(x => x.Add(It.IsAny<int>())).Returns(true)
                .Raises(x => x.Added += null, EventArgs.Empty);

            var foo = mock.Object;
            foo.FooEvent += (s, e) => Console.WriteLine("FooEvent raised");
            foo.Added += (s, e) => Console.WriteLine("Added raised");

            foo.Submit(ref bar);
            foo.Add(1);

            mock.VerifyAll();
        }

        [Test]
        public void TestEvents_Example04()
        {
            var mock = new Mock<IFoo>();
            var raisedVal = 0;
            mock.Object.MyEvent += (val, e) => raisedVal = val;

            // Raising a custom event which does not adhere to the EventHandler pattern
            // Raise passing the custom arguments expected by the event delegate
            mock.Raise(foo => foo.MyEvent += null, 25, true);

            Assert.AreEqual(25, raisedVal);
        }
    }

    [TestFixture]
    public class QuickStartTestCallbacks
    {
        [Test]
        public void TestCallbacks_Example01()
        {
            var mock = new Mock<IFoo>();
            var calls = 0;

            mock.Setup(foo => foo.Execute("ping")).Returns(true)
                .Callback(() => calls++);

            mock.Object.Execute("ping");
            Assert.AreEqual(1, calls);

            mock.Object.Execute("ping");
            Assert.AreEqual(2, calls);

            mock.Object.Execute("p");
            Assert.AreEqual(2, calls);

            mock.Object.Execute("ping");
            Assert.AreEqual(3, calls);
        }

        [Test]
        public void TestCallbacks_Example02()
        {
            var mock = new Mock<IFoo>();
            var callStr = string.Empty;

            // access invocation arguments
            mock.Setup(foo => foo.Execute(It.IsAny<string>()))
                .Returns(true)
                .Callback((string s) => callStr = s);

            Assert.That(callStr, Is.EqualTo(string.Empty));
            mock.Object.Execute("Callback");
            Assert.That(callStr, Is.EqualTo("Callback"));
        }

        [Test]
        public void TestCallbacks_Example03()
        {
            var mock = new Mock<IFoo>();
            var callStr = string.Empty;

            // alternate equivalent generic method syntax
            mock.Setup(foo => foo.Execute(It.IsAny<string>()))
                .Returns(true)
                .Callback<string>(s => callStr = s);

            Assert.That(callStr, Is.EqualTo(string.Empty));
            mock.Object.Execute("Callback");
            Assert.That(callStr, Is.EqualTo("Callback"));
        }

        [Test]
        public void TestCallbacks_Example04()
        {
            var mock = new Mock<IFoo>();
            var callStr = string.Empty;

            // access arguments for methods with multiple parameters
            mock.Setup(foo => foo.Execute(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<int, string>((i, s) => callStr = s + i);

            Assert.That(callStr, Is.EqualTo(string.Empty));
            mock.Object.Execute(1, "Callback");
            Assert.That(callStr, Is.EqualTo("Callback1"));
        }

        [Test]
        public void TestCallbacks_Example05()
        {
            var mock = new Mock<IFoo>();

            // callbacks can be specified before and after invocation
            mock.Setup(foo => foo.Execute("ping"))
                .Callback(() => Console.WriteLine("Before returns"))
                .Returns(true)
                .Callback(() => Console.WriteLine("After returns"));

            mock.Object.Execute("ping");
        }
    }
    
    [TestFixture]
    public class QuickStartTestVerification
    {
        [Test]
        public void TestVerification_Example01()
        {
            var mock = new Mock<IFoo>();

            // Тест упадет, если закоментировать следующую строку
            mock.Object.Execute("ping");

            mock.Verify(foo => foo.Execute("ping"));
        }

        [Test]
        public void TestVerification_Example02()
        {
            var mock = new Mock<IFoo>();

            // Тест упадет, если закоментировать следующую строку
            // И тогда можно увидеть текст ошибки
            mock.Object.Execute("ping");

            // Verify with custom error message for failure
            mock.Verify(foo => foo.Execute("ping"), "When doing operation X, the service should be pinged always");
        }

        [Test]
        public void TestVerification_Example03()
        {
            var mock = new Mock<IFoo>();
            
            // Method should never be called
            mock.Verify(foo => foo.Execute("ping"), Times.Never());
        }

        [Test]
        public void TestVerification_Example04()
        {
            var mock = new Mock<IFoo>();

            mock.Object.Execute("ping");
            mock.Object.Execute("ping");

            // Called at least once
            mock.Verify(foo => foo.Execute("ping"), Times.AtLeastOnce());
        }

        [Test]
        public void TestVerification_Example05()
        {
            var mock = new Mock<IFoo>();

            Console.WriteLine(mock.Object.Name);

            // Verify getter invocation, regardless of value.
            mock.VerifyGet(foo => foo.Name);
        }

        [Test]
        public void TestVerification_Example06()
        {
            var mock = new Mock<IFoo>();

            mock.Object.Name = "name";

            // Verify setter invocation, regardless of value.
            mock.VerifySet(foo => foo.Name);
        }

        [Test]
        public void TestVerification_Example07()
        {
            var mock = new Mock<IFoo>();

            mock.Object.Name = "foo";

            // Verify setter called with specific value
            mock.VerifySet(foo => foo.Name = "foo");
        }

        [Test]
        public void TestVerification_Example08()
        {
            var mock = new Mock<IFoo>();

            mock.Object.Value = 5;

            // Verify setter with an argument matcher
            mock.VerifySet(foo => foo.Value = It.IsInRange(1, 5, Range.Inclusive));
        }
    }

    [TestFixture]
    public class QuickStartTestCustomizingMockBehavior
    {
        [Test]
        public void TestCustomizingMockBehavior_Example01()
        {
            // Make mock behave like a "true Mock", raising exceptions for anything that doesn't have a corresponding expectation: 
            // in Moq slang a "Strict" mock; default behavior is "Loose" mock, which never throws and returns default values or 
            // empty arrays, enumerables, etc. if no expectation is set for a member
            var mock = new Mock<IFoo>(MockBehavior.Strict);
        }

        [Test]
        public void TestCustomizingMockBehavior_Example02()
        {
            // Invoke base class implementation if no expectation overrides the member (a.k.a. "Partial Mocks" in Rhino Mocks): 
            // default is false. (this is required if you are mocking Web/Html controls in System.Web!)
            var mock = new Mock<IFoo> { CallBase = true };
        }

        [Test]
        public void TestCustomizingMockBehavior_Example03()
        {
            // Make an automatic recursive mock: a mock that will return a new mock for every member that doesn't have an expectation 
            // and whose return value can be mocked (i.e. it is not a value type)
            var mock = new Mock<IFoo> { DefaultValue = DefaultValue.Mock };
            // default is DefaultValue.Empty

            // this property access would return a new mock of IBar as it's "mock-able"
            IBar value = mock.Object.Bar;

            // the returned mock is reused, so further accesses to the property return 
            // the same mock instance. this allows us to also use this instance to 
            // set further expectations on it if we want
            var barMock = Mock.Get(value);
            barMock.Setup(b => b.Submit()).Returns(true);

            Assert.That(value.Submit(), Is.True);
        }

        [Test]
        public void TestCustomizingMockBehavior_Example04()
        {
            // Centralizing mock instance creation and management: you can create and verify all mocks in a single place 
            // by using a MockFactory, which allows setting the MockBehavior, its CallBase and DefaultValue consistently
            var factory = new MockFactory(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

            // Create a mock using the factory settings
            var fooMock = factory.Create<IFoo>();

            // Create a mock overriding the factory settings
            var barMock = factory.Create<IBar>(MockBehavior.Loose);

            // Verify all verifiable expectations on all mocks created through the factory
            factory.Verify();
        }

        [Test]
        public void TestCustomizingMockBehavior_Example05()
        {
            // MockFactory объявлен как Obsolete и заменен на MockRepository
            // Тоже самое через MockRepository
            var repo = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

            // Create a mock using the factory settings
            var fooMock = repo.Create<IFoo>();

            // Create a mock overriding the factory settings
            var barMock = repo.Create<IBar>(MockBehavior.Loose);

            // Verify all verifiable expectations on all mocks created through the factory
            repo.Verify();
        }
    }

    public class CommandBase
    {
        protected virtual int Execute()
        {
            return 0;
        }

        public int PublicExecute()
        {
            return Execute();
        }


        protected virtual bool Execute2(string str)
        {
            return false;
        }

        public bool PublicExecute2(string str)
        {
            return Execute2(str);
        }
    }

    [TestFixture]
    public class QuickStartTestMiscellaneous
    {
        [Test]
        public void TestMiscellaneous_Example01()
        {
            var mock = new Mock<CommandBase>();

            // Setting expectations for protected members (you can't get intellisense for these, 
            // so you access them using the member name as a string):
            // Метод Execute объявлен как protected virtual
            mock.Protected()
                .Setup<int>("Execute")
                .Returns(5);

            Assert.That(mock.Object.PublicExecute(), Is.EqualTo(5));
        }

        [Test]
        public void TestMiscellaneous_Example02()
        {
            var mock = new Mock<CommandBase>();

            // if you need argument matching, you MUST use ItExpr rather than It
            // planning on improving this for vNext
            mock.Protected()
                .Setup<bool>("Execute2", ItExpr.IsAny<string>())
                .Returns(true);

            Assert.IsTrue(mock.Object.PublicExecute2("any string"));
        }
    }

    [TestFixture]
    public class QuickStartTestAdvancedFeatures
    {
        [Test]
        public void TestAdvancedFeatures_Example01()
        {
            var mock = new Mock<IFoo>();

            // get mock from a mocked instance
            IFoo foo = mock.Object; // get mock instance somehow
            var fooMock = Mock.Get(foo);
            fooMock.Setup(f => f.GetCount()).Returns(1);

            Assert.AreEqual(1, mock.Object.GetCount());
        }

        [Test]
        public void TestAdvancedFeatures_Example02()
        {
            // implementing multiple interfaces in mock
            var mock = new Mock<IFoo>();
            var disposableMock = mock.As<IDisposable>();
            // now the IFoo mock also implements IDisposable :)
            disposableMock.Setup(df => df.Dispose());

            disposableMock.Object.Dispose();

            mock.VerifyAll();
        }

        [Test]
        public void TestAdvancedFeatures_Example03()
        {
            //implementing multiple interfaces in single mock
            var mock = new Mock<IFoo>();
            mock.Setup(f => f.ToString()).Returns("Hello World");
            mock.As<IDisposable>().Setup(df => df.Dispose());

            mock.Object.ToString();
            mock.As<IDisposable>().Object.Dispose();
            
            mock.VerifyAll();
        }
    }

    public interface IRepository
    {
        bool IsAuthenticated { get; set; }
    }

    public interface IAuthentication
    {
        string AuthenticationType { get; set; }
    }

    public interface IControllerContext
    {
        bool Property1 { get; set; }
        int Property2 { get; set; }
        double Property3 { get; set; }
    }

    [TestFixture]
    public class QuickStartTestLinqToMocks
    {
        [Test]
        public void TestLinqToMocks_Example01()
        {
            var services = Mock.Of<IServiceProvider>(sp =>
                sp.GetService(typeof(IRepository)) == Mock.Of<IRepository>(r => r.IsAuthenticated == true) &&
                sp.GetService(typeof(IAuthentication)) == Mock.Of<IAuthentication>(a => a.AuthenticationType == "OAuth"));

            var mock = Mock.Get(services);

            var repService = services.GetService(typeof(IRepository)) as IRepository;
            Console.WriteLine(repService.IsAuthenticated);

            var authService = services.GetService(typeof(IAuthentication)) as IAuthentication;
            Console.WriteLine(authService.AuthenticationType);

            mock.VerifyAll();
        }
        
        [Test]
        public void TestLinqToMocks_Example02()
        {
            // Multiple setups on a single mock and its recursive mocks
            var context = Mock.Of<IControllerContext>(ctx =>
                 ctx.Property1 == true &&
                 ctx.Property2 == 22 &&
                 ctx.Property3 == 33.1);
            
            var mock = Mock.Get(context);

            Console.WriteLine(context.Property1);
            Console.WriteLine(context.Property2);
            Console.WriteLine(context.Property3);

            mock.VerifyAll();
        }
    }

    [TestFixture]
    public class TestSetupTearDown
    {
        #region TestFixture SetUp/TearDown

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            Console.WriteLine("TestFixtureSetup");
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            Console.WriteLine("TestFixtureTearDown");
        }

        #endregion TestFixture SetUp/TearDown

        #region Test SetUp/TearDown

        [SetUp]
        public void TestSetup()
        {
            Console.WriteLine("TestSetup");
        }

        [TearDown]
        public void TestTearDown()
        {
            Console.WriteLine("TestTearDown");
        }

        #endregion Test SetUp/TearDown

        #region Tests

        [Test]
        public void Test_01()
        {
            Console.WriteLine("Test_01");
        }

        [Test]
        public void Test_02()
        {
            Console.WriteLine("Test_02");
        }

        [Test]
        public void Test_03()
        {
            Console.WriteLine("Test_03");
        }

        #endregion Tests
    }
}
