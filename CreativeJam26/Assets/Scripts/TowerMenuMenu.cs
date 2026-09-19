    using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TowerMenuManager : MonoBehaviour
{
    public static TowerMenuManager Instance { get; private set; }

    [Header("UI Panels & Buttons")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject[] buildButtonList;
   // [SerializeField] private GameObject upgradeButton;

    [Header("Tower Settings")]
    [SerializeField] private GameObject[] towerPrefab; // Drag your tower prefab here in the inspector

    private TowerSlot currentActiveSlot;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        menuPanel.SetActive(false);
    }

    public void OpenMenu(TowerSlot slot, Vector3 worldPosition, bool hasTower)
    {
        currentActiveSlot = slot;

        // Position the UI panel over the slot
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        menuPanel.transform.position = screenPosition;

        // Toggle button visibility based on the slot's state
        if (hasTower)
        {
            for (int i = 0; i < buildButtonList.Length; i++){
                buildButtonList[i].SetActive(false);
            }
    //        upgradeButton.SetActive(true);
        }
        else
        {
        for (int i = 0; i < buildButtonList.Length; i++){
            buildButtonList[i].SetActive(true);
        }
    //        upgradeButton.SetActive(false);
        }

        menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        currentActiveSlot = null;
    }

    // Called by the Build Button OnClick event
    public void OnBuildButtonPressed(int quality)
    {
        if (currentActiveSlot != null && towerPrefab != null)
        {
            currentActiveSlot.BuildTower(towerPrefab[quality]);
            CloseMenu();
        }
    }

    // Called by the Upgrade Button OnClick event
    public void OnUpgradeButtonPressed()
    {
        if (currentActiveSlot != null)
        {
            currentActiveSlot.UpgradeTower();
            CloseMenu();
        }
    }
}
