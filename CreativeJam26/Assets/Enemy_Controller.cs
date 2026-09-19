using UnityEngine;

// SCOPE: controls enemy movement, reducing health at end, and any special behavior
public class Enemy_Controller : MonoBehaviour {
    
    public int health;
    public int speed;
    public float progress;
    
    [Header("Wobble Sprite")]
    public float wobbleAmount;
    public float wobbleSpeed;
    public AnimationCurve wobbleLerp;
    
    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite sideSprite;
    
    private bool wobbleDirection;
    private float angleState;
    private Spline spline;
    private Vector3 lastPosition;

    void Awake() {
        spline = GameObject.Find("Spline").GetComponent<Spline>();
    }
    
    void Start() {
        transform.position = spline.GetPositionOnSpline(0);
    }
    
    void Update() {
        progress += Time.deltaTime * (speed / 100f);
        transform.position = spline.GetPositionOnSpline(progress);
        
        // if at the end of path deplete health and disappear
        if (progress >= 1f) {
            int x = 100;// REPLACE
            x -= health;
            Destroy(gameObject, .1f);
        }
        
        // control wobble
        // move wobbleState between 0 and 1 (left and right)
        if (wobbleDirection) angleState += Time.deltaTime * wobbleSpeed; 
        else angleState -= Time.deltaTime * wobbleSpeed;
        // if reach either end of the lerp, clamp and go other direction
        if (angleState > 1f) {
            angleState = 1f; 
            wobbleDirection = false;
        }
        if (angleState < 0f) {
            angleState = 0f; 
            wobbleDirection = true;
        }
        // evaluate curve based on state and apply rotation
        var angle = (wobbleLerp.Evaluate(angleState) -.5f) * wobbleAmount *2;
        transform.eulerAngles = new Vector3(0, 0, angle);
        
        // control sprite by direction
        Vector3 difference = transform.position - lastPosition;
        // moving side to side
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y)){
            GetComponent<SpriteRenderer>().sprite = sideSprite;
            // moving right
            if (difference.x >= 0) {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            // moving left
            else {
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        // moving front to backW
        else {
            // moving front
            if (difference.y <= 0) {
                GetComponent<SpriteRenderer>().flipX = false;
                GetComponent<SpriteRenderer>().sprite = frontSprite;
            }
            // moving back
            else {
                GetComponent<SpriteRenderer>().flipX = false;
                GetComponent<SpriteRenderer>().sprite = backSprite;
            }
        }
        
        lastPosition = transform.position;
    }
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}