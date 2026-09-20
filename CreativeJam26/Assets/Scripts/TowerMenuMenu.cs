using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TowerMenuManager : MonoBehaviour
{
    public static TowerMenuManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup menuGroupPast;

    [SerializeField] private GameObject[] buildButtonsPast;

    [SerializeField] private CanvasGroup menuGroupFuture;
    [SerializeField] private GameObject[] buildButtonsFuture;


    //[SerializeField] private GameObject upgradeButton;

    [Header("Tower Settings")]
    [SerializeField] public GameObject[] towerPrefabs;
    [SerializeField] private int[] towerCosts; // same order/length as towerPrefabs

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
        menuGroupPast.alpha = visible ? 1f : 0f;
        menuGroupPast.interactable = visible;
        menuGroupPast.blocksRaycasts = visible;
        
        menuGroupFuture.alpha = visible ? 1f : 0f;
        menuGroupFuture.interactable = visible;
        menuGroupFuture.blocksRaycasts = visible;
    }

    public void OpenMenu(TowerSlot slot, Vector3 worldPosition, bool hasTower, bool isBaby)
    {

        currentActiveSlot = slot;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);
        float sx = Screen.width / 1920f;
        float sy = Screen.height / 1080f;
        screenPos.x = Mathf.Clamp(screenPos.x, MinX * sx, MaxX * sx);
        screenPos.y = Mathf.Clamp(screenPos.y, MinY * sy, MaxY * sy);
        if (isBaby)
        {
            menuGroupFuture.transform.position = screenPos;
 
        }
        else {
            menuGroupPast.transform.position = screenPos;
        }
        if (isBaby){
            for (int i = 0; i <  (buildButtonsFuture).Length; i++){
            buildButtonsFuture[i].SetActive(!hasTower);
            }
        }
        if (!isBaby){
            for (int i = 0; i <  (buildButtonsPast).Length; i++){
            buildButtonsPast[i].SetActive(!hasTower);
            }
        }
        //upgradeButton.SetActive(hasTower);

        openedFrame = Time.frameCount;
        this.SetVisible(true);
        if (!isBaby)
        {
            transform.Find("GameObjectPast").gameObject.SetActive(true);

            transform.Find("GameObjectFuture").gameObject.SetActive(false);


        }
        if (isBaby)
        {
            transform.Find("GameObjectPast").gameObject.SetActive(false);
            transform.Find("GameObjectFuture").gameObject.SetActive(true);



        }
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
        if (ClickTooSoon() || currentActiveSlot == null || towerPrefabs[_towerPrefab] == null) return;

        int cost = towerCosts[_towerPrefab];
        var stats = Game_Stats.Instance;

        if (currentActiveSlot.baby)
        {
            if (stats.Gold2 < cost) return;   // future = Gold2
            stats.Gold2 -= cost;
        }
        else
        {
            if (stats.Gold1 < cost) return;   // past = Gold1
            stats.Gold1 -= cost;
        }

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