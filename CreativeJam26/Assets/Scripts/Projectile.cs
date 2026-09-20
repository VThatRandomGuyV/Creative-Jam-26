using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int projectileDamage = 1;

    public bool rotates;
    private Transform target;
    private AudioCueEntry[] soundOverrides;
    private bool hasHit;
    private Vector3 lastPosition;


    public void SetTarget(Transform _target)
    {
        target = _target; 
    }

    public void SetSoundOverrides(AudioCueEntry[] overrides)
    {
        soundOverrides = overrides;
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
        
        // set sprite rotation
        if (rotates) {
            transform.Rotate(0, 0, 1000 * Time.deltaTime);
        }
        else {
            Vector3 difference = transform.position - lastPosition;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
            
            lastPosition = transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (hasHit) return;
        hasHit = true;
        AudioManager.Play(AudioCue.ProjectileImpact, soundOverrides);
        Enemy_Controller enemy = other.gameObject.GetComponent<Enemy_Controller>();
        if (enemy != null) enemy.TakeDamage(projectileDamage);
        Destroy(gameObject);
    }
}
