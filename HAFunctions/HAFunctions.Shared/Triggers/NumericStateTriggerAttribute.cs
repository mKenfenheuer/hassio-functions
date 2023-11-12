using System.Text.RegularExpressions;

namespace HAFunctions.Shared;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class NumericStateTriggerAttribute : HAFunctionTriggerAttribute
{
    private readonly string[] entityIds;
    private readonly double? below;
    private readonly double? above;
    private readonly double? belowOrEqual;
    private readonly double? aboveOrEqual;

    private double? TryParseString(string str)
    {
        double value = 0;
        if(double.TryParse(str, out value))
        {
            return value;
        }

        return null;
    }

    public NumericStateTriggerAttribute(string[] entityId, string below = null, string above = null, string belowOrEqual = null, string aboveOrEqual = null)
    {
        this.entityIds = entityId;
        this.below = TryParseString(below);
        this.above = TryParseString(above);
        this.belowOrEqual = TryParseString(belowOrEqual);
        this.aboveOrEqual = TryParseString(aboveOrEqual);
    }

    public NumericStateTriggerAttribute(string entityId, string below = null, string above = null, string belowOrEqual = null, string aboveOrEqual = null)
    {
        this.entityIds = new string[] { entityId };
        this.below = TryParseString(below);
        this.above = TryParseString(above);
        this.belowOrEqual = TryParseString(belowOrEqual);
        this.aboveOrEqual = TryParseString(aboveOrEqual);
    }

    public string[] EntityIds => entityIds;
    public double? Above => above;
    public double? Below => below;
    public double? AboveOrEqual => aboveOrEqual;
    public double? BelowOrEqual => belowOrEqual;

    

    public override bool IsMatch(Event e)
    {
        double value = 0;

        if(!double.TryParse(e.Data.NewState.StateValue, out value))
            return false;

        if (!EntityIds.Any(id => id == e.Data.EntityId || Regex.IsMatch(e?.Data?.EntityId ?? "", id)))
            return false;

        if (Above.HasValue && value <= Above)
            return false;

        if (Below.HasValue && value >= Below)
            return false;

        if (AboveOrEqual.HasValue && value < AboveOrEqual)
            return false;

        if (BelowOrEqual.HasValue && value > BelowOrEqual)
            return false;

        return true;
    }
}