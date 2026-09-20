using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TowerMenuManager : MonoBehaviour
{
    public static TowerMenuManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup menuGroup;
    [SerializeField] private GameObject[] buildButtons;
    //[SerializeField] private GameObject upgradeButton;

    [Header("Tower Settings")]
    [SerializeField] private GameObject[] towerPrefabs;

    // Screen-space limits (pixels) for the menu's pivot point. Tweak these.
    private const float MinX = 600f;
    private const float MaxX = 1320f;
    private const float MinY = 400f;
    private const float MaxY = 680f;

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
        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);
        float sx = Screen.width / 1920f;
        float sy = Screen.height / 1080f;
        screenPos.x = Mathf.Clamp(screenPos.x, MinX * sx, MaxX * sx);
        screenPos.y = Mathf.Clamp(screenPos.y, MinY * sy, MaxY * sy);
        menuGroup.transform.position = screenPos;

        for (int i = 0; i <  (buildButtons).Length; i++){
        buildButtons[i].SetActive(!hasTower);
        }
        //upgradeButton.SetActive(hasTower);

        openedFrame = Time.frameCount;
        SetVisible(true);
        Debug.Log($"Screen {Screen.width}x{Screen.height}, pos {screenPos}");
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