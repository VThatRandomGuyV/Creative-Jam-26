using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ChoiceCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button clickButton;

    private string myValue;

    // This event notifies the EventManager when THIS specific card is clicked
    public static event Action<string> OnCardSelected;

    public void SetupCard(CardData data)
    {
        titleText.text = data.cardTitle;
        descriptionText.text = data.cardDescription;
        myValue = data.choiceValue;

        // Clean up old listeners just in case, then add the click action
        clickButton.onClick.RemoveAllListeners();
        clickButton.onClick.AddListener(SelectThisCard);
    }

    private void SelectThisCard()
    {
        // Broadcast the choice value to the EventManager
        OnCardSelected?.Invoke(myValue);
    }
}
