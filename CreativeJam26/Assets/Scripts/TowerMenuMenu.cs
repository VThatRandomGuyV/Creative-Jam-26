using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TowerMenuManager : MonoBehaviour
{
    public static TowerMenuManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup menuGroup;
    [SerializeField] private GameObject[] buildButtons;
    [SerializeField] private GameObject upgradeButton;

    [Header("Tower Settings")]
    [SerializeField] private GameObject[] towerPrefabs;

    private TowerSlot currentActiveSlot;
    private Camera cam;
    private int openedFrame = -1;

    public bool IsOpen { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        cam = Camera.main;
        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        IsOpen = visible;
        menuGroup.alpha = visible ? 1f : 0f;
        menuGroup.interactable = visible;
        menuGroup.blocksRaycasts = visible;
    }

    public void OpenMenu(TowerSlot slot, Vector3 worldPosition, bool hasTower)
    {
        currentActiveSlot = slot;
        menuGroup.transform.position = cam.WorldToScreenPoint(worldPosition);
        for (int i = 0; i <  (buildButtons).Length; i++){
        buildButtons[i].SetActive(!hasTower);
        }
        upgradeButton.SetActive(hasTower);

        openedFrame = Time.frameCount;
        SetVisible(true);
    }

    public void CloseMenu()
    {
        SetVisible(false);
        currentActiveSlot = null;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private bool ClickTooSoon() => Time.frameCount <= openedFrame;

    public void OnBuildButtonPressed(int _towerPrefab)
    {
        int quality = _towerPrefab%3;
        if (ClickTooSoon() || currentActiveSlot == null || towerPrefabs[_towerPrefab] == null) return;
        currentActiveSlot.BuildTower(towerPrefabs[_towerPrefab]);
        CloseMenu();
    }

    public void OnUpgradeButtonPressed()
    {
        if (ClickTooSoon() || currentActiveSlot == null) return;
        currentActiveSlot.UpgradeTower();
        CloseMenu();
    }
}