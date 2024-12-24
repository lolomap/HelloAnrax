
// ReSharper disable UnusedMember.Global

using SimpleExpressionEvaluator;

public static class Utils
{
    public enum GameEventType
    {
        Common,
        PartyTemplate,
        PartySpecial
    }

    public static readonly ExpressionEvaluator Evaluator = new();
}
