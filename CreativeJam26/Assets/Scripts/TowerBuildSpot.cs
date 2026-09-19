using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// ONE-FILE Kingdom Rush style build spot.
///
/// SETUP:
///  1. Put this script on any object that should be a tower slot (a cube, sprite, empty w/ collider...).
///     If it has no collider, a BoxCollider is added for you.
///  2. Make sure your camera is tagged "MainCamera".
///  3. Press Play and click the object.
///
/// Everything else (EventSystem, raycaster, Canvas, radial menu, gold counter,
/// placeholder towers if you leave prefabs empty) is created automatically.
/// Assign real prefabs in the Options list in the Inspector when you have them.
/// </summary>
[Serializable]
public class TowerOption
{
    public string name = "Archer";
    public GameObject prefab;          // leave empty to spawn a coloured placeholder
    public int cost = 70;
    public Color color = new Color(0.2f, 0.7f, 0.3f);
}

public class TowerBuildSpot : MonoBehaviour, IPointerClickHandler
{
    // ---------- Inspector ----------
    [Header("Tower choices shown in the menu")]
    public TowerOption[] options =
    {
        new TowerOption { name = "Archer", cost = 70,  color = new Color(0.25f, 0.70f, 0.30f) },
        new TowerOption { name = "Mage",   cost = 100, color = new Color(0.45f, 0.35f, 0.85f) },
        new TowerOption { name = "Barracks", cost = 80, color = new Color(0.85f, 0.55f, 0.20f) },
    };

    [Header("Menu look")]
    public float menuRadius = 95f;     // pixels from the spot centre
    public float buttonSize = 76f;
    [Range(0f, 1f)] public float sellRefund = 0.6f;

    [Header("Placement")]
    public Vector3 towerOffset = Vector3.zero;
    public bool hideSpotWhenBuilt = true;

    [Header("Economy (shared by all spots)")]
    public int startingGold = 300;
    static bool goldInitialised;

    // ---------- State ----------
    public static int Gold { get; private set; }
    GameObject builtTower;
    TowerOption builtOption;
    Renderer[] spotRenderers;

    // ---------- Shared UI (one menu for the whole scene) ----------
    static Canvas canvas;
    static RectTransform menuRoot;
    static GameObject backdrop;
    static Text goldText;
    static TowerBuildSpot openSpot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        canvas = null; menuRoot = null; backdrop = null; goldText = null;
        openSpot = null; goldInitialised = false; Gold = 0;
    }

    // =====================================================================
    void Awake()
    {
        if (!goldInitialised) { Gold = startingGold; goldInitialised = true; }

        if (GetComponentInChildren<Collider>() == null && GetComponentInChildren<Collider2D>() == null)
            gameObject.AddComponent<BoxCollider>();

        spotRenderers = GetComponentsInChildren<Renderer>();

        EnsureEventSystem();
        EnsureCameraRaycasters();
        EnsureUI();
    }

    // Fires for clicks on this object OR any child collider (e.g. the built tower).
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        OpenMenu();
    }

    // =====================================================================
    //  MENU
    // =====================================================================
    void OpenMenu()
    {
        ClearMenu();
        openSpot = this;
        backdrop.SetActive(true);

        Camera cam = Camera.main;
        Vector2 centre = cam.WorldToScreenPoint(transform.position);

        if (builtTower == null)
        {
            int n = options.Length;
            for (int i = 0; i < n; i++)
            {
                TowerOption opt = options[i];
                float angle = (90f - i * (360f / n)) * Mathf.Deg2Rad;      // first option on top, clockwise
                Vector2 pos = centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * menuRadius;
                bool affordable = Gold >= opt.cost;
                MakeButton(opt.name + "\n" + opt.cost + "g", pos, opt.color, affordable, () => Build(opt));
            }
        }
        else
        {
            int refund = Mathf.RoundToInt(builtOption.cost * sellRefund);
            Vector2 pos = centre + Vector2.up * menuRadius;
            MakeButton("Sell\n+" + refund + "g", pos, new Color(0.8f, 0.25f, 0.25f), true, Sell);
        }
    }

    static void CloseMenu()
    {
        ClearMenu();
        if (backdrop != null) backdrop.SetActive(false);
        openSpot = null;
    }

    static void ClearMenu()
    {
        if (menuRoot == null) return;
        for (int i = menuRoot.childCount - 1; i >= 0; i--)
        {
            Transform c = menuRoot.GetChild(i);
            if (c.gameObject != backdrop) Destroy(c.gameObject);
        }
    }

    // =====================================================================
    //  BUILD / SELL
    // =====================================================================
    void Build(TowerOption opt)
    {
        if (builtTower != null || Gold < opt.cost) { CloseMenu(); return; }

        Gold -= opt.cost;
        builtOption = opt;
        builtTower = opt.prefab != null ? Instantiate(opt.prefab) : CreatePlaceholder(opt);
        builtTower.transform.SetParent(transform, true);
        builtTower.transform.position = transform.position + towerOffset +
            (opt.prefab == null ? PlaceholderLift() : Vector3.zero);
        builtTower.name = opt.name + " Tower";

        SetSpotVisible(false);
        RefreshGold();
        CloseMenu();
    }

    void Sell()
    {
        if (builtTower == null) { CloseMenu(); return; }
        Gold += Mathf.RoundToInt(builtOption.cost * sellRefund);
        Destroy(builtTower);
        builtTower = null;
        builtOption = null;
        SetSpotVisible(true);
        RefreshGold();
        CloseMenu();
    }

    void SetSpotVisible(bool visible)
    {
        if (!hideSpotWhenBuilt) return;
        foreach (Renderer r in spotRenderers)
            if (r != null && (builtTower == null || !r.transform.IsChildOf(builtTower.transform)))
                r.enabled = visible;
    }

    // Placeholder tower so the script works before you have any prefabs.
    GameObject CreatePlaceholder(TowerOption opt)
    {
        bool ortho = Camera.main != null && Camera.main.orthographic;
        GameObject go = GameObject.CreatePrimitive(ortho ? PrimitiveType.Cube : PrimitiveType.Cylinder);
        go.transform.localScale = ortho ? new Vector3(0.7f, 0.9f, 0.1f) : new Vector3(0.7f, 0.8f, 0.7f);
        Destroy(go.GetComponent<Collider>());   // spot's own collider keeps handling clicks

        Renderer r = go.GetComponent<Renderer>();
        r.material = new Material(r.sharedMaterial) { color = opt.color };
        return go;
    }

    Vector3 PlaceholderLift()
    {
        bool ortho = Camera.main != null && Camera.main.orthographic;
        return ortho ? new Vector3(0, 0, -0.1f) : new Vector3(0, 0.8f, 0);
    }

    // =====================================================================
    //  RUNTIME UI CONSTRUCTION
    // =====================================================================
    static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
        go.AddComponent<InputSystemUIInputModule>();
#else
        go.AddComponent<StandaloneInputModule>();
#endif
    }

    static void EnsureCameraRaycasters()
    {
        Camera cam = Camera.main;
        if (cam == null) { Debug.LogError("TowerBuildSpot: no camera tagged MainCamera."); return; }
        if (cam.GetComponent<PhysicsRaycaster>() == null) cam.gameObject.AddComponent<PhysicsRaycaster>();
        if (cam.GetComponent<Physics2DRaycaster>() == null) cam.gameObject.AddComponent<Physics2DRaycaster>();
    }

    static void EnsureUI()
    {
        if (canvas != null) return;

        var cgo = new GameObject("TowerMenuCanvas", typeof(Canvas), typeof(GraphicRaycaster));
        canvas = cgo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        // Gold label (top-left)
        var gGo = new GameObject("Gold", typeof(RectTransform), typeof(Text));
        gGo.transform.SetParent(cgo.transform, false);
        goldText = gGo.GetComponent<Text>();
        goldText.font = GetFont();
        goldText.fontSize = 28;
        goldText.fontStyle = FontStyle.Bold;
        goldText.color = new Color(1f, 0.85f, 0.2f);
        goldText.raycastTarget = false;
        var gRt = goldText.rectTransform;
        gRt.anchorMin = gRt.anchorMax = gRt.pivot = new Vector2(0, 1);
        gRt.anchoredPosition = new Vector2(20, -20);
        gRt.sizeDelta = new Vector2(300, 40);

        // Menu root
        var mGo = new GameObject("MenuRoot", typeof(RectTransform));
        mGo.transform.SetParent(cgo.transform, false);
        menuRoot = mGo.GetComponent<RectTransform>();
        menuRoot.anchorMin = Vector2.zero;
        menuRoot.anchorMax = Vector2.one;
        menuRoot.offsetMin = menuRoot.offsetMax = Vector2.zero;

        // Invisible full-screen backdrop: click anywhere else to close
        backdrop = new GameObject("Backdrop", typeof(RectTransform), typeof(Image), typeof(Button));
        backdrop.transform.SetParent(menuRoot, false);
        var bRt = (RectTransform)backdrop.transform;
        bRt.anchorMin = Vector2.zero; bRt.anchorMax = Vector2.one;
        bRt.offsetMin = bRt.offsetMax = Vector2.zero;
        backdrop.GetComponent<Image>().color = new Color(0, 0, 0, 0.25f);
        backdrop.GetComponent<Button>().onClick.AddListener(CloseMenu);
        backdrop.SetActive(false);

        RefreshGold();
    }

    static void RefreshGold()
    {
        if (goldText != null) goldText.text = "Gold: " + Gold;
    }

    static void MakeButton(string label, Vector2 screenPos, Color color, bool interactable, Action onClick)
    {
        // Keep the button fully on-screen
        float half = 45f;
        screenPos.x = Mathf.Clamp(screenPos.x, half, Screen.width - half);
        screenPos.y = Mathf.Clamp(screenPos.y, half, Screen.height - half);

        var go = new GameObject("Btn_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(menuRoot, false);

        var rt = (RectTransform)go.transform;
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = screenPos;
        rt.sizeDelta = Vector2.one * 76f;

        var img = go.GetComponent<Image>();
        img.color = interactable ? color : new Color(0.35f, 0.35f, 0.35f, 0.9f);

        var btn = go.GetComponent<Button>();
        btn.interactable = interactable;
        btn.onClick.AddListener(() => onClick());

        var tGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
        tGo.transform.SetParent(go.transform, false);
        var t = tGo.GetComponent<Text>();
        t.font = GetFont();
        t.text = label;
        t.alignment = TextAnchor.MiddleCenter;
        t.fontSize = 16;
        t.fontStyle = FontStyle.Bold;
        t.color = interactable ? Color.white : new Color(1f, 0.6f, 0.6f);
        t.raycastTarget = false;
        var tRt = t.rectTransform;
        tRt.anchorMin = Vector2.zero; tRt.anchorMax = Vector2.one;
        tRt.offsetMin = tRt.offsetMax = Vector2.zero;
    }

    static Font GetFont()
    {
        Font f = null;
        try { f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { }
        if (f == null) { try { f = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { } }
        return f;
    }
}