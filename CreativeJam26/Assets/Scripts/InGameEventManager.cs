using UnityEngine;

public class LimitedTimeEventsManager : MonoBehaviour
{
    [SerializeField] private TwoCardPopupPanel popupPanel;

    public string savedChoiceValue;

    private void OnEnable() => ChoiceCard.OnCardSelected += HandleCardSelection;
    private void OnDisable() => ChoiceCard.OnCardSelected -= HandleCardSelection;

    public void TriggerChoicePopup(CardData optionA, CardData optionB)
    {
        popupPanel.PopulatePopup(optionA, optionB);
    }

    private void HandleCardSelection(string chosenValue)
    {
        savedChoiceValue = chosenValue;
        Debug.Log($"Choice saved in LimitedTimeEventsManager: {savedChoiceValue}");

        // Closes your entire PopupContainer automatically
        popupPanel.gameObject.SetActive(false);
    }
}
