using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSlot : MonoBehaviour
{
    private GameObject currentTower = null;
    private int towerLevel = 1;
    public bool baby;

    private void OnMouseUpAsButton()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        bool hasTower = currentTower != null;
        TowerMenuManager.Instance.OpenMenu(this, transform.position, hasTower);
    }

    public void BuildTower(GameObject towerPrefab)
    {
        if (currentTower == null)
        {
            currentTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);
            towerLevel = 1;
            if (baby)
            {
                currentTower.GetComponent<TowerOperation>().targetMask = 128;
            }
            Debug.Log($"{gameObject.name}: Tower built!");
            this.gameObject.SetActive(false);
        }
    }

    public void UpgradeTower()
    {
        if (currentTower != null)
        {
            towerLevel++;
            Debug.Log($"{gameObject.name}: Tower upgraded to Level {towerLevel}!");
        }
    }
}