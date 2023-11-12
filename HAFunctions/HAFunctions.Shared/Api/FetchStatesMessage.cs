namespace HAFunctions.Shared;

public class FetchStatesMessage : ApiCommandMessage
{
    public FetchStatesMessage()
    {
        Type = "get_states";
    }
}