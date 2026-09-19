using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int projectileDamage = 1;

    private Transform target;

    public void SetTarget(Transform _target)
    {
        target = _target; 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!target) return;
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.gameObject.GetComponent<Enemy_Controller>().TakeDamage(projectileDamage);
        Destroy(gameObject);
    }
}
