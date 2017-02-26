namespace Server.Lib.Infrastructure
{
    public struct Optional<T>
    {
        public Optional(T value)
        {
            this.Value = value;
            this.HasValue = true;
        }

        public T Value { get; }
        public bool HasValue { get; }
    }
}