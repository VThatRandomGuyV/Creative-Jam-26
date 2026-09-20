using UnityEngine;
using System.Collections;

// SCOPE: controls enemy movement, reducing health at end, and any special behavior
public class Enemy_Controller : MonoBehaviour {
    
    public int health;
    public int speed;
    public int gold;
    public float progress;
    public float spawnFrequency;
    public bool isSpawner;
    public bool isTransport;
    
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
    public Spline spline;
    private Vector3 lastPosition;
    private Vector3 offset;

    public GameObject minion;
    private Active_Enemy_List_Manager AELM;
    private float timeSinceSpawn;
    public Sprite bagSprite;
    private float holdSprite;
    private float dying;
    private bool wobbleOff;
    
    void Start() {
        transform.position = spline.GetPositionOnSpline(0);
        
        // create an offset from the spline so that enemies arent on top of each other
        float x = Random.Range(-.2f, .2f);
        float y = Random.Range(-.2f, .2f);
        offset = new Vector3(x, y);
        
        // create minion objects when transport objects die or summoned from summoner
        if (spline == GameObject.Find("SplinePast").GetComponent<Spline>()) {
            minion.GetComponent<Enemy_Controller>().spline = GameObject.Find("SplinePast").GetComponent<Spline>();
            minion.layer = 6;
            AELM = GameObject.Find("EnemySpawner").GetComponent<Active_Enemy_List_Manager>();
        }
        else {
            minion.GetComponent<Enemy_Controller>().spline = GameObject.Find("SplieFuture").GetComponent<Spline>();
            minion.layer = 7;
            AELM = GameObject.Find("Enemy_Spawner_Future").GetComponent<Active_Enemy_List_Manager>();
        }
    }
    
    void Update() {
        progress += Time.deltaTime * (speed / 100f);
        transform.position = spline.GetPositionOnSpline(progress) + offset;
        
        // spawn things if its a spawner
        if (isSpawner) {
            timeSinceSpawn += Time.deltaTime;
            if (timeSinceSpawn > spawnFrequency) {
                timeSinceSpawn = 0;
                GameObject newObject = Instantiate(minion);
                AELM.AddEnemyToList(newObject);
                newObject.GetComponent<Enemy_Controller>().progress = progress;
            }
        }
        
        // if at the end of path deplete health and disappear
        if (progress >= 1f) {
            Game_Stats.Instance.Health -= health;
            Destroy(gameObject, .1f);
        }
        
        // control wobble
        if (!wobbleOff) {
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
        }
        
        lastPosition = transform.position;
        
        if (dying > 0) {
            dying +=  Time.deltaTime;
            transform.position = spline.GetPositionOnSpline(holdSprite);
            if (dying >= 2) {
                for (int i = 0; i < 5; i++) {
                    GameObject newObject = Instantiate(minion);
                    AELM.AddEnemyToList(newObject);
                    newObject.GetComponent<Enemy_Controller>().progress = holdSprite;
                }
                DestroyImmediate(gameObject);
            }
        }
    }
    public void TakeDamage(int dmg) {
        health -= dmg;
        if (health <= 0) {
            if (isTransport) {
                holdSprite = progress;
                GetComponent<SpriteRenderer>().sprite = bagSprite;
                transform.localScale = new Vector3(0.06f, .06f, .06f);
                wobbleOff = true;
                dying = 1;
            }

            if (spline == GameObject.Find("SplieFuture").GetComponent<Spline>()) {
                Game_Stats.Instance.Gold2 += gold;
            }
            else {
                Game_Stats.Instance.Gold1 += gold;
            }
            Destroy(gameObject, .1f);
        }
    }
}