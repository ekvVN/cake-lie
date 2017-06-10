namespace Testing.TestLogicProjNUnit
{
    using LogicProj;
    using NUnit.Framework;

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
