using UnityEngine;

public class ProjectileAOE : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int projectileDamage = 1;
    
    private AudioCueEntry[] soundOverrides;
    private float timeAlive;
    private Vector3 position1;

    void Start() {
        position1 = transform.position;
    }

    public void SetSoundOverrides(AudioCueEntry[] overrides)
    {
        soundOverrides = overrides;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        transform.position = position1;
        timeAlive += Time.fixedDeltaTime;
        if (timeAlive >= 1.5) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("this does a thing");
        AudioManager.Play(AudioCue.ProjectileImpact, soundOverrides);
        Enemy_Controller enemy = other.gameObject.GetComponent<Enemy_Controller>();
        if (enemy != null) enemy.TakeDamage(projectileDamage);
    }
}
