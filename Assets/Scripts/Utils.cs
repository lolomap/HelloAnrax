
// ReSharper disable UnusedMember.Global

using SimpleExpressionEvaluator;

public static class Utils
{
    public enum RateType
    {
        NationalistsLoyalty,
        FederationistsLoyalty,
        DemocratsLoyalty,
        NationalistsPower,
        FederationistsPower,
        DemocratsPower,
        NationalistsCoop,
        FederationistsCoop,
        DemocratsCoop,
        NeutralLoyalty,
        NeutralPower,
        Treasury,
        Power
    }

    public enum Category
    {
        Default,
        Logging
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
