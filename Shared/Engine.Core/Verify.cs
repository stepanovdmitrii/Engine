using System;

namespace Engine.Core
{
    public static class Verify
    {
        public static void InRangeInclude<T>(T value, T lowerBound, T upperBound, string paramName) 
            where T: IComparable<T>
        {
            if (value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
                throw new ArgumentOutOfRangeException(paramName, $"Value {value} is out of range [{lowerBound}; {upperBound}]");
        }

        public static void InRangeExclude<T>(T value, T lowerBound, T upperBound, string paramName)
            where T : IComparable<T>
        {
            if (value.CompareTo(lowerBound) <= 0 || value.CompareTo(upperBound) >= 0)
                throw new ArgumentOutOfRangeException(paramName, $"Value {value} is out of range ({lowerBound}; {upperBound})");
        }

        public static void IsNotNull<T>(T value, string paramName) where T: class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void IsEqual<T>(T value, T compareTo, string paramName) where T: IComparable<T>
        {
            if (value.CompareTo(compareTo) != 0)
                throw new ArgumentException($"Value {value} is not equal to {compareTo}", paramName);
        }

        public static void Assert(bool b)
        {
            if (!b)
                throw new InvalidOperationException();
        }

        public static void IsNotDisposed(bool disposed, object @this)
        {
            if (disposed)
                throw new ObjectDisposedException(@this.ToString());
        }
    }
}
