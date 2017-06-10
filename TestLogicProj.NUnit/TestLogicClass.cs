namespace Testing.TestLogicProj.NUnit
{
    using global::NUnit.Framework;
    using LogicProj;

    [TestFixture]
    public class TestLogicClass
    {
        [Test]
        public void Sum()
        {
            var logic = new LogicClass();
            Assert.That(logic.Sum(3, 4), Is.EqualTo(7));
        }
    }
}
