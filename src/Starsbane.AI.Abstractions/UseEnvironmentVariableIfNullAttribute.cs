#if FALSE
namespace Starsbane.AI
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class UseEnvironmentVariableIfNullAttribute : Attribute
    {
        public string[] VariableNames { get; }

        /// <summary>
        /// Specifies that the value of environment variable is taken if the value of the property is null.
        /// </summary>
        /// <param name="variableName">The name of the environment variable.</param>
        public UseEnvironmentVariableIfNullAttribute(string variableName)
        {
            VariableNames = [Throw.IfNullOrEmpty(variableName)];
        }

        /// <summary>
        /// Specifies that the value of environment variables following the passed order is taken if the value of the property is null.
        /// </summary>
        /// <param name="variableNames">The name of the environment variable.</param>
        public UseEnvironmentVariableIfNullAttribute(params string[] variableNames)
        {
            _ = Throw.IfNull(variableNames);
            _ = Throw.IfLessThan(variableNames.Count(a => !string.IsNullOrEmpty(a)), 1);

            VariableNames = variableNames;
        }
    }
}
#endif