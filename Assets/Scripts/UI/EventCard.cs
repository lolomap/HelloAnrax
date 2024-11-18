using System;
using UnityEngine;

namespace UI
{
    public class EventCard : MonoBehaviour
    {
        public GameEvent Data;

        private void Awake()
        {
            Data = EventStorage.GetNext();
        }

        public void SelectOption(int id)
        {
            foreach (Modifier modifier in Data.Options[id].Modifiers)
            {
                PlayerRates.UpdateRate(modifier.Type, modifier.Value);
            }
            
            Data = EventStorage.GetNext();
        }
    }
}
