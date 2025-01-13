using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EventCard : MonoBehaviour
    {
        public TMP_Text EventTitle;
        public TMP_Text EventDescription;
        public TMP_Text EventOptionTitle;
        public RoundList EventOptionsList;
        public Image EventPicture;
        
        public GameEvent Data;
        private Option _selectedOption;

        private void Awake()
        {
            GameManager.EventStorage.Init();
            
            OptionIcon.SelectOption += OnSelectOption;
        }

        private void Start()
        {
            Data = GameManager.EventStorage.GetNext();
            UpdateCard();
            
            GameManager.PlayerStats.UpdateUI();
            TaggedValue.UpdateAll("BuildVersion", GameManager.GetVersion());
        }
        
        public void AcceptOption()
        {
            bool canBeAccepted = true;
            List<Flag> limitations = _selectedOption.Limits;
            if (limitations != null)
            {
                foreach (Flag limitation in limitations)
                {
                    canBeAccepted = canBeAccepted &&
                                    GameManager.PlayerStats.GetFlag(limitation.Type) >= limitation.Value;
                }
            }

            if (!canBeAccepted)
            {
                // TODO: ANIMATE
                return;
            }
            
            List<Modifier> modifiers = _selectedOption.Modifiers;
            if (modifiers != null)
            {
                foreach (Modifier modifier in modifiers)
                {
                    GameManager.PlayerStats.SetStat(modifier.Type,
                        GameManager.PlayerStats.GetStat(modifier.Type) + modifier.Value);
                }
            }
            
            List<Flag> flags = _selectedOption.Flags;
            if (flags != null)
            {
                foreach (Flag flag in flags)
                {
                    GameManager.PlayerStats.SetFlag(flag.Type, flag.Value);
                }
            }
            
            GameManager.PlayerStats.CalculateFormulas();
            
            Data = GameManager.EventStorage.GetNext();
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
            EventPicture.sprite = ResourceLoader.GetResource<Sprite>("Icons/Events/" + Data.Category);

            List<RoundListElement> list = new();
            foreach (Option option in Data.Options)
            {
                OptionIcon prefab = ResourceLoader.GetResource<OptionIcon>("Prefabs/OptionIcon");
                OptionIcon obj = Instantiate(prefab);
                obj.Data = option;
                list.Add(obj);
            }

            EventOptionsList.Elements = list;

            if (Data.Soundtrack != MusicManager.Instance.GetCurrent())
            {
                MusicManager.Instance.PlayAudio(Data.Soundtrack);
            }
        }
    }
}
