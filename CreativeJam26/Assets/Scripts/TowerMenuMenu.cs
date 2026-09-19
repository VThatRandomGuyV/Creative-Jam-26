using UnityEngine;
using UnityEngine.EventSystems;

public class TowerMenuManager : MonoBehaviour
{
    public static TowerMenuManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup menuGroup;
    [SerializeField] private GameObject buildButton;
    [SerializeField] private GameObject upgradeButton;

    [Header("Tower Settings")]
    [SerializeField] private GameObject towerPrefab;

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

        buildButton.SetActive(!hasTower);
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

    public void OnBuildButtonPressed()
    {
        if (ClickTooSoon() || currentActiveSlot == null || towerPrefab == null) return;
        currentActiveSlot.BuildTower(towerPrefab);
        CloseMenu();
    }

    public void OnUpgradeButtonPressed()
    {
        if (ClickTooSoon() || currentActiveSlot == null) return;
        currentActiveSlot.UpgradeTower();
        CloseMenu();
    }
}