// При работе с js  необходимо создать объект с такими же свойствами
public enum JsonResponseState
{
    Ok = 0,
    Error = -1
}

/// <summary>
/// Represents Json response Message/Data attributes
/// </summary>
public class JsonResponse
{
    public JsonResponseState State = JsonResponseState.Ok;
    public string Message = "";
    public object Data = "";

    public JsonResponse(JsonResponseState state, string message)
    {
        State = state;
        Message = message;
    }

    public JsonResponse(JsonResponseState state, object data)
    {
        State = state;
        Data = data;
    }

    public JsonResponse(JsonResponseState state)
    {
        State = state;
    }

    public JsonResponse()
    {
    }
}