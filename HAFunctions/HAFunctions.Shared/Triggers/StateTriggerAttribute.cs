namespace HAFunctions.Shared;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class StateTriggerAttribute : HAFunctionTriggerAttribute
{
    private readonly string[] entityIds;
    private readonly string from;
    private readonly string notFrom;
    private readonly string to;
    private readonly string notTo;

    public string[] EntityIds => entityIds;
    public string From => from;
    public string NotFrom => notFrom;
    public string To => to;
    public string NotTo => notTo;

    public StateTriggerAttribute(string[] entityIds, string from = null, string notFrom = null, string to = null, string notTo = null)
    {
        this.entityIds = entityIds;
        this.from = from;
        this.notFrom = notFrom;
        this.to = to;
        this.notTo = notTo;
    }
    public StateTriggerAttribute(string entityId, string from = null, string notFrom = null, string to = null, string notTo = null)
    {
        this.entityIds = new string[] { entityId };
        this.from = from;
        this.notFrom = notFrom;
        this.to = to;
        this.notTo = notTo;
    }

    public override bool IsMatch(Event e)
    {
        if (!EntityIds.Any(id => id == e.Data.EntityId))
            return false;
        if (From != null && e.Data.OldState.StateValue != from)
            return false;
        if (NotFrom != null && e.Data.OldState.StateValue == notFrom)
            return false;
        if (To != null && e.Data.NewState.StateValue != to)
            return false;
        if (NotTo != null && e.Data.NewState.StateValue == notTo)
            return false;

        return true;
    }
}