using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EventCard : MonoBehaviour
    {
        public TMP_Text EventTitle;
        public TMP_Text EventDescription;
        public TMP_Text EventOptionTitle;
        public RoundList EventOptionsList;
        
        public GameEvent Data;
        private Option _selectedOption;

        private void Awake()
        {
            OptionIcon.SelectOption += OnSelectOption;
        }

        private void Start()
        {
            Data = EventStorage.GetNext();
            UpdateCard();
        }
        
        public void AcceptOption()
        {
            List<Modifier> modifiers = _selectedOption.Modifiers;
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
        
        private void OnSelectOption(Option option)
        {
            if (option == null) return;
            
            _selectedOption = option;
            EventOptionTitle.text = option.Title;
        }

        private void UpdateCard()
        {
            EventDescription.text = Data.Description;
            EventTitle.text = Data.Title;

            List<RoundListElement> list = new();
            foreach (Option option in Data.Options)
            {
                OptionIcon prefab = Resources.Load<OptionIcon>("Prefabs/OptionIcon");
                OptionIcon obj = Instantiate(prefab);
                obj.Data = option;
                list.Add(obj);
            }

            EventOptionsList.Elements = list;
        }
    }
}
