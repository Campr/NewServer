using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Lib.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> enumerable, Func<T, object> distinctSelector)
        {
            return enumerable.Distinct(new PropertySelectorEqualityComparer<T>(distinctSelector));
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldBatchElements(enumerator, batchSize - 1);
        }

        public static IDisposable ToDisposable<T>(this IEnumerable<T> source) where T : IDisposable
        {
            return new DisposableWrapper<T>(source.ToList());
        }

        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (var i = 0; i < batchSize && source.MoveNext(); i++)
                yield return source.Current;
        }
    }

    class PropertySelectorEqualityComparer<T> : IEqualityComparer<T>
    {
        public PropertySelectorEqualityComparer(Func<T, object> propertySelector)
        {
            this.propertySelector = propertySelector;
        }

        private readonly Func<T, object> propertySelector;

        public bool Equals(T x, T y)
        {
            return this.propertySelector(x).Equals(this.propertySelector(y));
        }

        public int GetHashCode(T obj)
        {
            return this.propertySelector(obj).GetHashCode();
        }
    }

    class DisposableWrapper<T> : IDisposable where T : IDisposable
    {
        public DisposableWrapper(IList<T> disposableObjects)
        {
            this.disposableObjects = disposableObjects;
        }

        private readonly IList<T> disposableObjects;

        public void Dispose()
        {
            foreach (var disposableObject in this.disposableObjects)
            {
                disposableObject.Dispose();
            }
        }
    }
}