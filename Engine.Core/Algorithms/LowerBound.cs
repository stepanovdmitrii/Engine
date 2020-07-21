using System;

namespace Engine.Core.Algorithms
{
    public sealed class LowerBound<T> where T: IComparable<T>
    {
        private readonly T[] _source;

        public LowerBound(T[] items)
        {
            Verify.IsNotNull(items, nameof(items));
            _source = items;
        }

        public bool Find(T target, out int index)
        {
            Verify.IsNotNull(target, nameof(target));
            int left = 0;
            int right = _source.Length - 1;
            while (left <= right)
            {
                int mid = (right - left) / 2 + left;
                if (_source[mid].CompareTo(target) < 0)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            index = left < _source.Length ? left : -1;
            return left < _source.Length;
        }
    }
}
