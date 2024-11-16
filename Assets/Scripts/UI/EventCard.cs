using UnityEngine;

namespace UI
{
    public class EventCard : MonoBehaviour
    {
        public GameEvent Data;

        public void SelectOption()
        {
            Data = EventStorage.GetNext();
        }
    }
}
