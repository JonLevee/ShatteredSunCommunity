namespace ShatteredSunCommunity.MiscClasses
{
    public class Disposable : IDisposable
    {
        private readonly Action action;

        public Disposable(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}
