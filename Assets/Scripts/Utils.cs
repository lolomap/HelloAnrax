
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;

public static class Utils
{
    public enum Comparasion
    {
        Gt,
        Lt,
        GtE,
        LtE,
        Exists
    }
    
    public static readonly ExpressionEvaluator Evaluator = new();
    public static readonly Random Random = new();

    public static bool Compare<T>(T a, T b, Comparasion oper) where T : IComparable
    {
        return oper switch
        {
            Comparasion.Gt => a.CompareTo(b) > 0,
            Comparasion.Lt => a.CompareTo(b) < 0,
            Comparasion.GtE => a.CompareTo(b) >= 0,
            Comparasion.LtE => a.CompareTo(b) <= 0,
            Comparasion.Exists => a.CompareTo(0) > 0,
            _ => false
        };
    }
    
    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = Random.Next(n + 1);  
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }

}
