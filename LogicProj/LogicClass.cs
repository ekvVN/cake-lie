namespace LogicProj
{
    public interface ILogic
    {
        int Sum(int x, int y);
    }

    public class LogicClass : ILogic
    {
        public int Sum(int x, int y)
        {
            return x + y;
        }
    }
}
