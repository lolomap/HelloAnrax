using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EventCard : MonoBehaviour
    {
        public GameObject RestartPanel;
        
        public TMP_Text EventTitle;
        public TMP_Text EventDescriptionBoxDummy;
        public TMP_Text EventDescription;
        public ScrollRect EventDescriptionScroll;
        public TMP_Text EventOptionTitle;
        public RoundList EventOptionsList;
        public Image EventPicture;
        public AudioSource EventSound;

        public GameObject ErrorIcon;
        
        public GameEvent Data;
        private Option _selectedOption;
        private int _tldrPosition;
        private GameEvent _tldrData;
        
        private void Awake()
        {
            GameManager.EventStorage.Init();
            GameManager.Instance.EventUI = this;
            
            OptionIcon.SelectOption += OnSelectOption;
        }

        private void Start()
        {
            Data = GameManager.EventStorage.CurrentEvent ?? GameManager.EventStorage.GetNext();
            UpdateCard();
            
            GameManager.PlayerStats.UpdateUI();
            GameManager.PlayerStats.Updated += MusicManager.Instance.TriggerAudio;
            TaggedValue.UpdateAll("BuildVersion", GameManager.GetVersion());
        }

        /// <summary>
        /// Try to process modifiers and flags of option if it is available
        /// </summary>
        /// <returns>true - if option is available</returns>
        private bool ProcessOption()
        {
            if (!_selectedOption.IsAvailable(out List<Flag> blockedFlags))
            {
                ((OptionIcon) EventOptionsList.GetSelected()).PlayAnimation();

                foreach (Flag flag in blockedFlags)
                {
                    TaggedValue.AnimateAll(flag.Type, UIGenericAnimation.Animation.ButtonShake);
                }
                
                return false;
            }
            
            List<Modifier> modifiers = _selectedOption.Modifiers;
            if (modifiers != null)
            {
                foreach (Modifier modifier in modifiers)
                {
                    if (string.IsNullOrEmpty(modifier.Limit?.Type) || GameManager.PlayerStats.HasFlag(modifier.Limit))
                    {
                        GameManager.PlayerStats.SetStat(modifier.Type,
                            GameManager.PlayerStats.GetStat(modifier.Type) + modifier.Value, false);
                    }
                }
            }
            
            List<Flag> flags = _selectedOption.Flags;
            if (flags != null)
            {
                foreach (Flag flag in flags)
                {
                    GameManager.PlayerStats.SetFlag(flag.Type, flag.Value, false);
                }
            }
            
            if (GameManager.PlayerStats.HasFlag("RESTART_GAME"))
            {
                AcceptRestart();
                return false;
            }

            return true;
        }

        public void ShowRestart() => RestartPanel.SetActive(!RestartPanel.activeInHierarchy);
        
        public void AcceptRestart()
        {
            RestartPanel.SetActive(false);
            GameManager.Restart();
            GameManager.PlayerStats.Updated += MusicManager.Instance.TriggerAudio;
            GameManager.EventStorage.Init();
            Data = GameManager.EventStorage.GetNext();
            UpdateCard();
            GameManager.PlayerStats.UpdateUI();
        }
        
        public void AcceptOption()
        {
            // Process long events
            if (_tldrData != null || Data.TLDR is {Count: > 0})
            {
                _tldrData ??= Data;

                if (_tldrPosition < _tldrData.TLDR.Count)
                {
                    if (!ProcessOption()) return;
                    
                    Data = _tldrData.TLDR[_tldrPosition];
                    if (Data.Title == "")
                        Data.Title = _tldrData.Title;
                    _selectedOption = null;
                    UpdateCard();
                    
                    _tldrPosition++;
                    return;
                }
            }
            _tldrData = null;
            _tldrPosition = 0;
            
            if (!ProcessOption()) return;
            
            if (!Data.SkipTurn)
                GameManager.PlayerStats.CalculateFormulas(false);
            
            GameManager.PlayerStats.OnUpdated();

            Data = (
                GameManager.PlayerStats.HasFlag("FAIL")
                    ? GameManager.EventStorage.GetFail()
                    : GameManager.PlayerStats.HasFlag("WIN_ENDGAME")
                        ? null
                        : GameManager.EventStorage.GetNext()
            ) ?? GameManager.EventStorage.GetWin();

            _selectedOption = null;
            
            UpdateCard();
        }
        
        private void OnSelectOption(Option option)
        {
            if (option == null) return;

            TaggedValue.ClearPreviewAll();
            
            _selectedOption = option;
            EventOptionTitle.text = option.Title;

            // Preview all modifiers of current option
            if (option.Modifiers != null)
            {
                foreach (Modifier modifier in option.Modifiers)
                {
                    TaggedValue.PreviewAll(modifier.Type,
                        GameManager.PlayerStats.GetStat(modifier.Type) + modifier.Value);
                }
            }
        }

        public void UpdateCard()
        {
            EventDescription.text = Data.Description;
            EventDescriptionBoxDummy.text = Data.Description;

            EventDescriptionScroll.DOVerticalNormalizedPos(1f, 0.75f);
            EventTitle.text = Data.Title;
            EventPicture.sprite = ResourceLoader.GetResource<Sprite>("Icons/Events/" + Data.Category);
            if (EventPicture.sprite == null)
                EventPicture.sprite = ResourceLoader.GetResource<Sprite>("Icons/Events/Default");

            List<RoundListElement> list = new();

            Data.Options ??= new() {new() {Title = "OK"}};
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

            AudioClip categorySound = ResourceLoader.GetResource<AudioClip>("Audio/Events/" + Data.Category);
            if (categorySound != null)
                EventSound.PlayOneShot(categorySound);
        }
    }
}
