
// ReSharper disable UnusedMember.Global

using SimpleExpressionEvaluator;

public static class Utils
{
    public enum RateType
    {
        NPLoyalty,
        UPLoyalty,
        WPLoyalty,
        NPPower,
        UPPower,
        WPPower,
        NPCoop,
        UPCoop,
        WPCoop,
        NeutralLoyalty,
        NeutralPower,
        Treasury,
        Power
    }

    public enum Party
    { 
        Nationalists,
        Unionists,
        Westernists
    }

    public enum GameEventType
    {
        Common,
        PartyTemplate,
        PartySpecial
    }

    public static readonly ExpressionEvaluator Evaluator = new();
}
