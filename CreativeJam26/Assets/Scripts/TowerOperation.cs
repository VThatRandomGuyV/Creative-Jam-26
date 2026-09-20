using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
public class TowerOperation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public LayerMask targetMask;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;

    [Header("Attribute")]
    [SerializeField] private float range = 5f;
    [SerializeField] private float bps = 1f; //bullet per seconds
    [SerializeField] public int quality; //bullet per seconds


    private Transform target;
    private float timeUntilFire;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= 1f / bps)
            {
                timeUntilFire = 0;
                Shoot();
            }
        }
    }
    private void Shoot()
    {
        GameObject projectileObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);        
        Projectile projectileScript = projectileObj.GetComponent<Projectile>();
        projectileScript.SetTarget(target);
    }
    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= range;
    }
    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2) transform.position, 0f, targetMask);
        if (hits.Length >0 )
        {
            target = hits[0].transform;
        }
    }
}
