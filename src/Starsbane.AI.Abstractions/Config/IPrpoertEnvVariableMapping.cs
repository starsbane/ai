using System.Collections.ObjectModel;

namespace Starsbane.AI
{
    public interface IPropertyEnvVariableMapping
    {
        ReadOnlyDictionary<string, string[]>? PropertyEnvVariableMapping { get; }
    }
}
