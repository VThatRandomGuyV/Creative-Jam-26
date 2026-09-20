using System;
using UnityEngine;

[Serializable]
public class TowerMenuConfig
{
    public const int TypeCount = 6;
    public const int QualityCount = 3; // 0 = Low, 1 = Medium, 2 = High

    [Tooltip("The full 6x3 grid mockup image shown behind the invisible click buttons.")]
    public Sprite gridBackground;

    [Tooltip("Must have exactly 6 entries, one per column in the grid image.")]
    public TowerTypeEntry[] towerTypes = new TowerTypeEntry[TypeCount];

    [Serializable]
    public class TowerTypeEntry
    {
        public string displayName;

        [Tooltip("Must have exactly 3 entries: Low, Medium, High.")]
        public GameObject[] prefabsByQuality = new GameObject[QualityCount];
    }

    public GameObject GetPrefab(int typeIndex, int qualityIndex)
    {
        if (towerTypes == null || typeIndex < 0 || typeIndex >= towerTypes.Length)
            return null;

        TowerTypeEntry entry = towerTypes[typeIndex];
        if (entry?.prefabsByQuality == null || qualityIndex < 0 || qualityIndex >= entry.prefabsByQuality.Length)
            return null;

        return entry.prefabsByQuality[qualityIndex];
    }

    public bool IsValid()
    {
        return towerTypes != null && towerTypes.Length == TypeCount;
    }
}
