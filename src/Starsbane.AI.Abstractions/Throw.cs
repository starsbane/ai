namespace Starsbane.AI
{
    public static class Throw
    {
        public static InvalidOperationException CreateMissingServiceException(Type serviceType, object? serviceKey) =>
            new InvalidOperationException(serviceKey is null ?
                $"No service of type '{serviceType}' is available." :
                $"No service of type '{serviceType}' for the key '{serviceKey}' is available.");

        public static T IfNull<T>(T obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException();
            }

            return obj;
        }

        public static string IfNullOrEmpty(string obj)
        {
            if (string.IsNullOrEmpty(obj))
            {
                throw new ArgumentNullException();
            }

            return obj;
        }

        public static int NotEqualTo(int first, int second)
        {
            if (!first.Equals(second))
            {
                throw new ArgumentOutOfRangeException($"The value must be equal to {second}");
            }

            return first;
        }

        public static int NotInRangeOf(int value, int min, int max)
        {
            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException($"The value must be in between {min} and {max}.");
            }
            return value;
        }

        public static int IfLessThan(int value, int minThreshold)
        {
            if (value < minThreshold)
            {
                throw new ArgumentException($"The value must not less than {minThreshold}", nameof(value));
            }

            return value;
        }

        public static int IfGreaterThan(int value, int maxThreshold)
        {
            if (value > maxThreshold)
            {
                throw new ArgumentException($"The value must not greater than {maxThreshold}", nameof(value));
            }

            return value;
        }

        public static T IfNotInEnumerable<T>(T value, IEnumerable<T> enumerable)
        {
            var values = enumerable as T[] ?? enumerable.ToArray();
            if (!values.Contains(value))
            {
                throw new ArgumentException($"The value must be one of the following values: {string.Join(",", values)}");
            }

            return value;
        }
    }
}
