using System.Linq;

public static class TypeUtils
{
    public static T CreateInstance<T>(params object[] parameters)
    {
        var ctors = typeof(T).GetConstructors().OrderBy(x => x.GetParameters().Length);

        if (parameters.Length == 0)
            return ActivatorUtilities.GetInstanceCreator<T>(ctors.First()).Invoke(parameters);

        var fitting = ctors.FirstOrDefault(x => x.GetParameters().Length == parameters.Length 
            && x.GetParameters().Select(p => p.GetType()).Except(parameters.Select(p => p.GetType())).Any());

        if (fitting == null) return default;

        return ActivatorUtilities.GetInstanceCreator<T>(fitting).Invoke(parameters);
    } 
}