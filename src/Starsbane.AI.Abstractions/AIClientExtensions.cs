using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;

namespace Starsbane.AI
{
    public static class AIClientExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();
        public static void ApplyDotEnv(string dotEnvFile = ".env")
        {
            if (!File.Exists(dotEnvFile)) return;

            foreach (var line in File.ReadAllLines(dotEnvFile))
            {
                var parts = line.Split(['='], StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2) Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }

        /// <summary>Retrieves the value of an environment variable from the current process if the value is null.</summary>
        /// <param name="vvalue">The variable to be checked.</param>
        /// <param name="variable">The name of the environment variable.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="variable" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission to perform this operation.</exception>
        /// <returns>The value of the environment variable specified by <paramref name="variable" />, or <see langword="null" /> if the environment variable is not found.</returns>
        public static string? GetEnvironmentVariableIfNull(string? value, string variable)
        {
            return value ?? Environment.GetEnvironmentVariable(variable);
        }


        public static void SetPropertiesInObject<T>(ref T? obj) where T: IPropertyEnvVariableMapping
        {
            if (obj == null) obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
            if (obj.PropertyEnvVariableMapping == null) return;

            foreach (var mapping in obj.PropertyEnvVariableMapping)
            {
                if (!_propertyCache.TryGetValue(typeof(T), out var propInfos))
                {
                    propInfos = typeof(T).GetProperties();
                    _propertyCache.TryAdd(typeof(T),propInfos);
                }
                var propInfo = propInfos.FirstOrDefault(a=>a.Name == mapping.Key);
                if (propInfo == null) continue;

                if (propInfo.GetValue(obj) != null) continue;
                foreach (var environmentVariable in mapping.Value)
                {
                    var envValue = Environment.GetEnvironmentVariable(environmentVariable);
                    if (envValue == null) continue;
                    propInfo.SetValue(obj, envValue);
                    break;
                }
            }
        }
    }
}
