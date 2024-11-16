using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public enum ParamType
    {
        NP_Loyalty,
        UP_Loyalty,
        WP_Loyalty,
        NP_Power,
        UP_Power,
        WP_Power,
        NP_Coop,
        UP_Coop,
        WP_Coop,
        NeutralLoyalty,
        Power,
        PowerModifier
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

    public static JsonSerializer Serializer = new();
}
