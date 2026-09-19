using UnityEngine;

public class TwoCardPopupPanel : MonoBehaviour
{
    // Drag your existing hierarchy objects into these slots in the inspector
    [SerializeField] private ChoiceCard leftCardScript;  // CardPopup
    [SerializeField] private ChoiceCard rightCardScript; // CardPopup2

    public void PopulatePopup(CardData leftData, CardData rightData)
    {
        // Assign the data straight to your existing scene cards
        if (leftCardScript != null) leftCardScript.SetupCard(leftData);
        if (rightCardScript != null) rightCardScript.SetupCard(rightData);

        // Turn on the entire window panel
        gameObject.SetActive(true);
    }
}
