using HAFunctions.Shared;

[System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public abstract class HAFunctionTriggerAttribute : System.Attribute
{
    public HAFunctionTriggerAttribute()
    {
    }

    public abstract bool IsMatch(Event e);
}