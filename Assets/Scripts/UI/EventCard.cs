using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EventCard : MonoBehaviour
    {
        public TMP_Text EventDescription;
        
        public GameEvent Data;

        private int _selectedOptionId;

        private void Start()
        {
            Data = EventStorage.GetNext();
            UpdateCard();
        }

        public void SelectOption(int id)
        {
            _selectedOptionId = id;
        }
        
        public void AcceptOption()
        {
            List<Modifier> modifiers = Data.Options[_selectedOptionId].Modifiers;
            if (modifiers != null)
            {
                foreach (Modifier modifier in modifiers)
                {
                    PlayerRates.UpdateRate(modifier.Type, modifier.Value);
                }
            }
            
            Data = EventStorage.GetNext();
            UpdateCard();
        }

        private void UpdateCard()
        {
            EventDescription.text = Data.Description;
        }
    }
}
