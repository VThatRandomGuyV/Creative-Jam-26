using System.Collections;
using UnityEngine;

public class LimitedTimeEventsManager : MonoBehaviour
{
    [System.Serializable]
    public class EventPair
    {
        public CardData optionA;
        public CardData optionB;
    }

    [Header("UI Reference")]
    [SerializeField] private TwoCardPopupPanel popupPanel;

    [Header("Event Timing Settings")]
    [SerializeField] private float timeBeforeFirstPopup = 5f;
    // 120 seconds = 2 minutes
    [SerializeField] private float timeBetweenPopups = 120f;

    [Header("Hardcoded Events Pool")]
    // This allows exactly 3 distinct events to be configured in the Inspector
    [SerializeField] private EventPair[] hardcodedEvents = new EventPair[3];

    private int currentEventIndex = 0;

    private void OnEnable() => ChoiceCard.OnCardSelected += HandleCardSelection;
    private void OnDisable() => ChoiceCard.OnCardSelected -= HandleCardSelection;

    private void Start()
    {
        StartCoroutine(PeriodicPopupRoutine());
    }

    private IEnumerator PeriodicPopupRoutine()
    {
        yield return new WaitForSecondsRealtime(timeBeforeFirstPopup);

        while (true)
        {
            // Ensure we have events configured to prevent crashes
            if (hardcodedEvents != null && hardcodedEvents.Length > 0)
            {
                Time.timeScale = 0f; // 1. Pause game time

                // Fetch the current event from our 3 choices
                EventPair activeEvent = hardcodedEvents[currentEventIndex];

                popupPanel.gameObject.SetActive(true); // 2. Show UI
                popupPanel.PopulatePopup(activeEvent.optionA, activeEvent.optionB);

                // Advance to the next event, wrapping back to 0 if it exceeds the list size
                currentEventIndex = (currentEventIndex + 1) % hardcodedEvents.Length;
            }
            else
            {
                Debug.LogWarning("No events configured in the LimitedTimeEventsManager array!");
            }

            // 3. Keep waiting while the popup is visible
            while (popupPanel.gameObject.activeSelf)
            {
                yield return null;
            }

            Time.timeScale = 1f; // 5. Resume game time once UI is closed

            yield return new WaitForSecondsRealtime(timeBetweenPopups);
        }
    }

    private void HandleCardSelection(string chosenValue)
    {
        // Pass the result directly to Game_Stats to handle the data modifications
        Game_Stats.Instance.ProcessChoiceResult(chosenValue);

        // 4. Close the container (which breaks the inner while-loop above)
        popupPanel.gameObject.SetActive(false);
    }
}
