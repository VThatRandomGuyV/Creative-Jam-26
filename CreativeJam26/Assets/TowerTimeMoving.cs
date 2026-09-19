using UnityEngine;

public class TowerTimeMoving : MonoBehaviour
{
    public GameObject towerPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            Instantiate(towerPrefab, pos, Quaternion.identity);
            GameObject future_tower = Instantiate(towerPrefab, pos + new Vector3(Camera.main.orthographicSize * Camera.main.aspect, 0, 0), Quaternion.identity);

            if(future_tower.transform.position.y < 1)
            {
                future_tower.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }
}
