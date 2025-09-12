using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed;


    private Vector2 lastPosition;
    private float speedX;
    private bool hasReachedFullSpeed = false;

    [SerializeField] private float expectedSpeedX = 6f;
    [SerializeField] private float tolerance = 0.1f;
    [SerializeField] private float activationThreshold = 2.5f;

    public float jumpForce = 5f;          // Die Stärke des Sprungs
    public float fallSpeedMultiplier = 10f; // Wie schnell der Spieler magnetisch zu Boden gezogen wird
    private KeyCode jumpKey = KeyCode.Space; // Taste für den Sprung

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isOnTop = false; // Überprüft, ob der Spieler den Boden berührt hat
    private bool isOnBotton = false;
    public bool isJumping = false;
    private bool isDoingMagneticFall = false;

    public LayerMask groundLayer; // Layer des Bodens, um nur mit dem Boden zu kollidieren
    public float raycastDistance = 0.1f;

    private List<GameObject> ActualFireEffect = new List<GameObject>();
    public GameObject FireBallEffect;
    public GameObject MagneticBallEffect;
    public GameObject BallEffect;
    public GameObject BallEffect2;

    private float previousSpeedX = 4.5f;

    public Color MagneticColor;
    public Color JumpingColor;


    public bool GameIsStarted;

    public int LifePoints;

    public bool IsDead;

    public bool isVelocityPositive = true;
    private bool IsOnPointA = true;

    private bool IsOnWayDown;

    private List<GameObject> passedObjects = new List<GameObject>();

    public Volume motionBlurVolume;

    public float horizontalSpeed = 2f;
    public Camera mainCamera;

    public Collider2D ClickingSphereCollider;

    public Transform arrow;

    public Transform rayOrigin;
    public float rayOffsetY = 0.35f;
    public float rayDistance = 1f;

    private LineRenderer lineRenderer;
    public ParticleSystem particleSystem;
    public ParticleSystem particleSystem2;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FireBallEffect.SetActive(false);
        //MagneticBallEffect.SetActive(true);

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        if (mainCamera == null)
            mainCamera = Camera.main;

        StartMenu.Instance.gameObject.SetActive(true);

        ClickingSphereCollider = FindObjectOfType<ClickingSphere>().GetComponent<Collider2D>();


#if UNITY_ANDROID
        //FindObjectOfType<JumpButton>().GetComponent<Button>().onClick.AddListener(() => JumpButtonClicked());
        //FindObjectOfType<JumpButton>().GetComponent<Button>().onClick.AddListener(() => StartButtonClicked());

        //StartMenu.Instance.text.text = "Press Jump Button to start game.";

        FindObjectOfType<JumpButton>().gameObject.SetActive(true);
        StartMenu.Instance.text.text = "Click on Button to start game.";
#elif UNITY_STANDALONE_WIN
        FindObjectOfType<JumpButton>().gameObject.SetActive(false);
        StartMenu.Instance.text.text = "Press Space Button to start game.";
#elif UNITY_EDITOR
        FindObjectOfType<JumpButton>().gameObject.SetActive(false);
        StartMenu.Instance.text.text = "Press Space Button to start game.";
        //FindObjectOfType<JumpButton>().GetComponent<Button>().onClick.AddListener(() => JumpButtonClicked());
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Instantiate(PrefabManager.Instance.XpText);
        }

        var targets = GameObject.FindObjectsOfType<SawBlade>().ToList().Select(s => s.gameObject);

        foreach (GameObject target in targets)
        {
            // Check if the player has passed the object
            if (!passedObjects.Contains(target) && IsApproximatelyEqual(transform.position.x , target.transform.position.x, 0.5f) && target.GetComponent<SawBlade>().IsMoving)
            {
                passedObjects.Add(target);
                Instantiate(PrefabManager.Instance.XpText);
                StartCoroutine(ResetSawBladeXP(target));
            }
        }

#if UNITY_ANDROID
        if (!GameIsStarted && Input.GetKeyDown(KeyCode.Mouse0) && !TutorialMenu.Instance.gameObject.activeSelf)
        {
            GameIsStarted = true;
            rb.simulated = true;
            StartMenu.Instance.gameObject.SetActive(false);

        }
#elif UNITY_STANDALONE_WIN
        if (!GameIsStarted && Input.GetKeyDown(KeyCode.Space) && !TutorialMenu.Instance.gameObject.activeSelf)
        {
            GameIsStarted = true;
            rb.simulated = true;
            StartMenu.Instance.gameObject.SetActive(false);
            
        } 
#elif UNITY_EDITOR
        if (!GameIsStarted && Input.GetKeyDown(KeyCode.Space) && !TutorialMenu.Instance.gameObject.activeSelf)
        {
            GameIsStarted = true;
            rb.simulated = true;
            StartMenu.Instance.gameObject.SetActive(false);
        }
#endif


        if (!GameIsStarted)
        {
            rb.simulated = false;
            return;
        }

        if (IsDead)
        {
            return;
        }

        if (ChallengeManager.Instance.actualChallengeButton.title != "Dash")
        {
            MoveToRight();

            if (rb.velocity.x<1)
            {
                speed = 0;
                FireBallEffect.SetActive(false);
                //MagneticBallEffect.SetActive(false);
                BallEffect.SetActive(false);
                BallEffect2.SetActive(false);

                rb.simulated = true;


                StartCoroutine(WaitForReset());
            }
        }
        

        Debug.Log("Velocity: " + rb.velocity);

#if UNITY_ANDROID

        if (ChallengeManager.Instance.actualChallengeButton.title == "Normal" || ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode" || ChallengeManager.Instance.actualChallengeButton.title == "HardcoreMode" || ChallengeManager.Instance.actualChallengeButton.title == "Highspeed" || ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall" || ChallengeManager.Instance.actualChallengeButton.title == "MoveCamera" || ChallengeManager.Instance.actualChallengeButton.title == "UpsideDown" || ChallengeManager.Instance.actualChallengeButton.title == "RotateCamera")
        {
            // Überprüfen, ob der Spieler auf dem Boden steht und die Sprungtaste drückt
            if (isGrounded && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Jump();
                //StartCoroutine(ElectricSoundCoroutine());
            }
            // Überprüfen, ob der Spieler im Sprung ist und die Sprungtaste erneut drückt
            else if (isJumping && Input.GetKeyDown(KeyCode.Mouse0))
            {
                MagneticFall();
            }
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Gravity")
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //rb.simulated = true;
                // Wenn velocity.y positiv ist, setze sie auf -velocityValue, sonst auf +velocityValue

                if (isVelocityPositive)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -7);
                    IsOnWayDown = true;
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 7);
                    IsOnWayDown = false;
                }

                isVelocityPositive = !isVelocityPositive;
            }

            Vector2 directionUp = Vector2.up;

            Vector2 position2 = new Vector2(transform.position.x - 0.5f, transform.position.y);
            RaycastHit2D hit2 = Physics2D.Raycast(position2, directionUp, raycastDistance, groundLayer);

            if (isOnTop && !IsOnWayDown && hit2.collider == null)
            {
                rb.velocity = new Vector2(speed, 20);
            }

            Vector2 directionDown = Vector2.down;

            Vector2 position3 = new Vector2(transform.position.x - 0.5f, transform.position.y);
            RaycastHit2D hit3 = Physics2D.Raycast(position3, directionDown, raycastDistance, groundLayer);

            if (isOnBotton && IsOnWayDown && hit3.collider == null)
            {
                //rb.velocity = new Vector2(speed, -20);
            }
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "StrongGravity")
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //rb.simulated = true;
                // Wenn velocity.y positiv ist, setze sie auf -velocityValue, sonst auf +velocityValue



                if (isVelocityPositive)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -17);
                    IsOnWayDown = true;
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 17);
                    IsOnWayDown = false;
                }

                isVelocityPositive = !isVelocityPositive;

            }

            Vector2 direction = Vector2.up;

            Vector2 position2 = new Vector2(transform.position.x - 0.5f, transform.position.y);
            RaycastHit2D hit2 = Physics2D.Raycast(position2, direction, raycastDistance, groundLayer);
            RaycastHit2D hit3 = Physics2D.Raycast(position2, -direction, raycastDistance, groundLayer);

            if (isOnBotton && IsOnWayDown)
            {
                rb.velocity = new Vector2(speed, 0);
            }
            else if(isOnTop && !IsOnWayDown)
            {
                rb.velocity = new Vector2(speed, 0);
            }

            if (isOnTop && !IsOnWayDown && hit2.collider == null)
            {
                rb.velocity = new Vector2(speed, 20);
            }

            if (isOnBotton && IsOnWayDown && hit3.collider == null)
            {
                rb.velocity = new Vector2(speed, -20);
            }

        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Flappy")
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                rb.velocity = new Vector2(rb.velocity.x,0);
                rb.AddForce(Vector2.up * 4.5f, ForceMode2D.Impulse);
            }
            if (isGrounded)
            {
                if (LifePoints == 0)
                {
                    if (InventoryManager.Instance.GodMode)
                    {
                        return;
                    }

                    Instantiate(PrefabManager.Instance.DieEffect, position: transform.position, new Quaternion(0f, 0.707106769f, -0.707106769f, 0));

                    speed = 0;
                    FireBallEffect.SetActive(false);
                    //MagneticBallEffect.SetActive(false);
                    BallEffect.SetActive(false);
                    BallEffect2.SetActive(false);

                    rb.simulated = true;


                    StartCoroutine(WaitForReset());
                }
            }
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Clicking")
        {
            if (isGrounded && Input.GetKeyDown(KeyCode.Mouse0))
            {

                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (ClickingSphereCollider.OverlapPoint(mousePos))
                {
                    Debug.Log("Circle clicked!");

                    Jump();
                    // Hier kannst du weitere Aktionen ausführen
                }

               
                //StartCoroutine(ElectricSoundCoroutine());
            }
            // Überprüfen, ob der Spieler im Sprung ist und die Sprungtaste erneut drückt
            else if (isJumping && Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (ClickingSphereCollider.OverlapPoint(mousePos))
                {
                    Debug.Log("Circle clicked!");

                    MagneticFall();
                    // Hier kannst du weitere Aktionen ausführen
                }
            }
        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Dash")
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                arrow.gameObject.SetActive(true);
            }
            else
            {
                arrow.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Time.timeScale = 0.5f;
            }

                // Mausposition in Weltkoordinaten
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            // Richtung von Zentrum zur Maus
            Vector3 direction = (mouseWorldPos - transform.position).normalized;

            // Neue Position des Pfeils (auf dem Kreis)
            arrow.transform.position = transform.position + direction * 2;

            // Rotation des Pfeils zur Maus
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0, 0, angle);

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Time.timeScale = 1f;
                rb.velocity = Vector3.zero;
                rb.AddForce(direction.normalized*280f,ForceMode2D.Force);
            }
        }

#elif UNITY_STANDALONE_WIN

        if (ChallengeManager.Instance.actualChallengeButton.title == "Normal" || ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode" || ChallengeManager.Instance.actualChallengeButton.title == "HardcoreMode" || ChallengeManager.Instance.actualChallengeButton.title == "Highspeed" || ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall" || ChallengeManager.Instance.actualChallengeButton.title == "MoveCamera") 
        {
            //Debug.Log("VelocityY: " + rb.velocity.y);
            //ActivateMotionBlur(5f,0f,1f,20);
            // Überprüfen, ob der Spieler auf dem Boden steht und die Sprungtaste drückt
            if (isGrounded && Input.GetKeyDown(jumpKey))
            {
                Jump();
                //StartCoroutine(ElectricSoundCoroutine());
            }
            // Überprüfen, ob der Spieler im Sprung ist und die Sprungtaste erneut drückt
            else if (isJumping && Input.GetKeyDown(jumpKey))
            {
                MagneticFall();
            }
        }else if(ChallengeManager.Instance.actualChallengeButton.title == "Gravity")
        {
            if (Input.GetKeyDown(jumpKey))
            {
                //rb.simulated = true;
                // Wenn velocity.y positiv ist, setze sie auf -velocityValue, sonst auf +velocityValue

                if (isVelocityPositive)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -7);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 7);
                }

                isVelocityPositive = !isVelocityPositive;

            }
        }else if(ChallengeManager.Instance.actualChallengeButton.title == "StrongGravity")
        {
            if (Input.GetKeyDown(jumpKey))
            {
                //rb.simulated = true;
                // Wenn velocity.y positiv ist, setze sie auf -velocityValue, sonst auf +velocityValue

                

                if (isVelocityPositive)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -20);
                    IsOnWayDown = true;
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 20);
                    IsOnWayDown = false;
                }

                isVelocityPositive = !isVelocityPositive;

            }

            Vector2 direction = Vector2.up;

            Vector2 position2 = new Vector2(transform.position.x - 0.5f, transform.position.y);
            RaycastHit2D hit2 = Physics2D.Raycast(position2, direction, raycastDistance, groundLayer);

            if (isOnTop && !IsOnWayDown && hit2.collider == null)
            {
                rb.velocity = new Vector2(speed, 20);
            }

        }
        else if (ChallengeManager.Instance.actualChallengeButton.title == "Flappy")
        {
            if (Input.GetKeyDown(jumpKey))
            {
                rb.AddForce(Vector2.up * 4, ForceMode2D.Impulse);
            }
            if (isGrounded)
            {
                if (LifePoints == 0)
                {
                    if (InventoryManager.Instance.GodMode)
                    {
                        return;
                    }

                    speed = 0;
                    FireBallEffect.SetActive(false);
                    //MagneticBallEffect.SetActive(false);
                    BallEffect.SetActive(false);
                    BallEffect2.SetActive(false);

                    rb.simulated = true;


                    StartCoroutine(WaitForReset());
                }
            }
        }else if (ChallengeManager.Instance.actualChallengeButton.title == "Follow")
        {
            //HandleInput();
        }


#elif UNITY_EDITOR
        if (ChallengeManager.Instance.actualChallengeButton.title == "Normal"|| ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode" || ChallengeManager.Instance.actualChallengeButton.title == "HardcoreMode" || ChallengeManager.Instance.actualChallengeButton.title == "Highspeed")
        {
            // Überprüfen, ob der Spieler auf dem Boden steht und die Sprungtaste drückt
            if (isGrounded && Input.GetKeyDown(jumpKey))
            {
                Jump();
                //StartCoroutine(ElectricSoundCoroutine());
            }
            // Überprüfen, ob der Spieler im Sprung ist und die Sprungtaste erneut drückt
            else if (isJumping && Input.GetKeyDown(jumpKey))
            {
                MagneticFall();
            }
        }else if(ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode")
        {
            if (Input.GetKeyDown(jumpKey))
            {
                if (isVelocityPositive)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -7);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 7);
                }

                isVelocityPositive = !isVelocityPositive;
            }
        }else if(ChallengeManager.Instance.actualChallengeButton.title == "StrongGravity")
        {
            if (Input.GetKeyDown(jumpKey))
            {
                //rb.simulated = true;
                // Wenn velocity.y positiv ist, setze sie auf -velocityValue, sonst auf +velocityValue

                if (isVelocityPositive)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -20);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 20);
                }

                isVelocityPositive = !isVelocityPositive;

            }
        }
#endif

        


        //if (rb.velocity.x == speed && ChallengeManager.Instance.actualChallengeButton.title != "Dash" && ChallengeManager.Instance.actualChallengeButton.title != "Flappy")
        //{
        //    CheckForStucking();
        //}
    }

    private void FixedUpdate()
    {
        IsGrounded();
        IsOnBottom();
        IsOnTop();

        if (ChallengeManager.Instance.actualChallengeButton.title != "Follow")
        {
            return;
        }

        float targetY = Mathf.Lerp(rb.position.y, GetWorldInputY(),0.5f);

        // Neue Position berechnen
        Vector2 targetPos = new Vector2(rb.position.x + horizontalSpeed * Time.fixedDeltaTime, targetY);
        rb.MovePosition(targetPos);


    }

    void LateUpdate()
    {
        //if (mainCamera != null)
        //{
        //    Vector3 camPos = mainCamera.transform.position;
        //    camPos.x = transform.position.x;
        //    camPos.y = transform.position.y;
        //    mainCamera.transform.position = camPos;
        //}
    }

    float GetWorldInputY()
    {
        Vector3 screenPos = new Vector3();
#if UNITY_EDITOR || UNITY_STANDALONE
         screenPos = Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            screenPos = Input.GetTouch(0).position;
        else
            return transform.position.y;
#else
        return transform.position.y;
#endif

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        return worldPos.y;
    }

    public void CheckForStucking()
    {
        Vector2 currentPosition = transform.position;
        float deltaX = currentPosition.x - lastPosition.x;
        float deltaTime = Time.deltaTime;
        speedX = deltaX / deltaTime;

        // Sobald wir einmal nahe an der vollen Geschwindigkeit waren, aktivieren wir die Überwachung
        if (!hasReachedFullSpeed && speedX >= activationThreshold)
        {
            hasReachedFullSpeed = true;
            Debug.Log("Charakter hat volle Geschwindigkeit erreicht. Überwachung aktiviert.");
        }

        if (hasReachedFullSpeed)
        {
            if (Mathf.Abs(speedX - expectedSpeedX) > tolerance)
            {
                if (InventoryManager.Instance.GodMode)
                {
                    return;
                }

                speed = 0;
                FireBallEffect.SetActive(false);
                //MagneticBallEffect.SetActive(false);
                BallEffect.SetActive(false);
                BallEffect2.SetActive(false);

                rb.simulated = true;
                GetComponent<Collider2D>().enabled = false;

                FindObjectOfType<FollowPlayer>().enabled = false;

                StartCoroutine(WaitForReset());
            }
        }

        lastPosition = currentPosition;
    }

    //    void HandleInput()
    //    {
    //#if UNITY_EDITOR || UNITY_STANDALONE
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            isDragging = true;
    //            lastInputPos = Input.mousePosition;
    //        }
    //        else if (Input.GetMouseButtonUp(0))
    //        {
    //            isDragging = false;
    //        }
    //#elif UNITY_ANDROID || UNITY_IOS
    //        if (Input.touchCount > 0)
    //        {
    //            Touch touch = Input.GetTouch(0);
    //            if (touch.phase == TouchPhase.Began)
    //            {
    //                isDragging = true;
    //                lastInputPos = touch.position;
    //            }
    //            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
    //            {
    //                isDragging = false;
    //            }
    //        }
    //#endif
    //    }

    Vector2 GetInputPosition()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.mousePosition;
#else
        return Input.GetTouch(0).position;
#endif
    }

    void TryMoveVertically(float deltaY)
    {
        Vector2 newPos = rb.position + new Vector2(0, deltaY);
        rb.MovePosition(newPos);
    }

    public void ActivateMotionBlur(float smoothSpeed, float baseWeight, float maxWeight, float maxAbsVelocityY)
    {
        float absYVelocity = Mathf.Abs(rb.velocity.y); // Geschwindigkeit unabhängig vom Vorzeichen
        float t = Mathf.Clamp01(absYVelocity / maxAbsVelocityY); // Normalisiere auf 0–1
        float targetWeight = Mathf.Lerp(baseWeight, maxWeight, t);

        // Sanftes Interpolieren zum Zielwert
        motionBlurVolume.weight = Mathf.Lerp(motionBlurVolume.weight, targetWeight, Time.deltaTime * smoothSpeed);
    }

    public void JumpButtonClicked()
    {
        if (isGrounded)
        {
            Jump();
        }
        else if (isJumping)
        {
            MagneticFall();
        }
    }

    public void StartButtonClicked()
    {
        if (!GameIsStarted && !TutorialMenu.Instance.gameObject.activeSelf)
        {
            GameIsStarted = true;
            rb.simulated = true;
            StartMenu.Instance.gameObject.SetActive(false);
        }
    }

    public IEnumerator ElectricSoundCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(2,5));

        if (isJumping)
        {
            GetComponent<PlayerSound>().PlayElectricalSound();
            StartCoroutine(ElectricSoundCoroutine());
        }
        else
        {
            yield break;
        }
    }

    public IEnumerator ResetSawBladeXP(GameObject actualObject)
    {
        yield return new WaitForSeconds(2f);
        passedObjects.Remove(actualObject);
    }

    public bool IsApproximatelyEqual(float value1, float value2, float tolerance)
    {
        return Mathf.Abs(value1 - value2) < tolerance;
    }

    void Jump()
    {
        // Setze den Spieler in den Sprungzustand und füge eine Aufwärtskraft hinzu
        //isJumping = true;
        //isGrounded = false;
        //ActualFireEffect.Add(Instantiate(PrefabManager.Instance.JumpDashEffect, position: transform.position - new Vector3(0, 5.19f, 0), new Quaternion(0.497374982f, 0.502611339f, -0.502611339f, -0.497374982f)));
        //FireBallEffect.SetActive(true);

        //FindObjectsOfType<LightMovement>().ToList().ForEach(l => l.SetColor(JumpingColor));

        GetComponent<PlayerSound>().PlaySnare();

        rb.velocity = new Vector2(rb.velocity.x, 0); // Nullt die vertikale Geschwindigkeit
        if (ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode")
        {
            rb.AddForce(Vector2.up * 9, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            particleSystem.Play();
        }
        
    }

    private void DetectSpeedChange()
    {
        float currentSpeedX = rb.velocity.x;

        // Überprüfen, ob sich die Geschwindigkeit verändert hat
        if (currentSpeedX != previousSpeedX)
        {
            Debug.Log("Die Geschwindigkeit auf der X-Achse hat sich verändert!");

            // Überprüfen, ob die Geschwindigkeit verringert wurde
            if (currentSpeedX < previousSpeedX)
            {
                Debug.Log("Die Geschwindigkeit auf der X-Achse hat sich verringert!");
            }
        }

        // Aktuelle Geschwindigkeit speichern
        previousSpeedX = currentSpeedX;
    }

    void MagneticFall()
    {
        // Erhöht die Fallgeschwindigkeit, um den Spieler schnell zu Boden zu ziehen
        //Destroy(ActualFireEffect);

        

        GetComponent<PlayerSound>()._audioSource2.Stop();

        ActualFireEffect.ForEach(a => Destroy(a));
        ActualFireEffect.Clear();

        //FireBallEffect.SetActive(false);
        //MagneticBallEffect.SetActive(true);

        GetComponent<PlayerSound>().PlayMetal();

        //FindObjectsOfType<LightMovement>().ToList().ForEach(l => l.SetColor(MagneticColor));

        if (IsDead)
        {
            rb.velocity = new Vector2(0,0);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeedMultiplier);
            particleSystem2.Play();
        }

        isDoingMagneticFall = true;
    }
    private void IsOnBottom()
    {
        Vector2 position = new Vector2(transform.position.x - 0.7f, transform.position.y);
        // Der Raycast geht nach unten, also ist die Richtung Vector2.down
        Vector2 direction = Vector2.down;

        Vector2 position2 = new Vector2(transform.position.x + 0.7f, transform.position.y);
        RaycastHit2D hit2 = Physics2D.Raycast(position2, direction, raycastDistance, groundLayer);

        // Führe den Raycast durch und überprüfe, ob er auf den Boden trifft
        RaycastHit2D hit = Physics2D.Raycast(position, direction, raycastDistance, groundLayer);


        // Wenn der Raycast ein Objekt trifft, das auf dem Ground-Layer liegt, ist der Charakter grounded
        if (hit.collider != null && !hit.collider.GetComponent<SawBlade>() /*&& hit2.collider == null*/)
        {
            isOnBotton = true;
        }
        else
        {
            isOnBotton = false;
        }
    }
    private void IsOnTop()
    {
        Vector2 position = new Vector2(transform.position.x-0.7f, transform.position.y);
        // Der Raycast geht nach unten, also ist die Richtung Vector2.down
        Vector2 direction = Vector2.up;

        Vector2 position2 = new Vector2(transform.position.x + 0.7f, transform.position.y);
        RaycastHit2D hit2 = Physics2D.Raycast(position2, direction, raycastDistance, groundLayer);

        // Führe den Raycast durch und überprüfe, ob er auf den Boden trifft
        RaycastHit2D hit = Physics2D.Raycast(position, direction, raycastDistance, groundLayer);
        

        // Wenn der Raycast ein Objekt trifft, das auf dem Ground-Layer liegt, ist der Charakter grounded
        if (hit.collider != null && !hit.collider.GetComponent<SawBlade>()/*&& hit2.collider == null*/)
        {
            isOnTop = true;
        }
        else 
        {
            isOnTop = false;
        }
    }
    public void IsGrounded()
    {
        // Startpunkt des Raycasts ist die Position des Charakters
        Vector2 position = transform.position;
        // Der Raycast geht nach unten, also ist die Richtung Vector2.down
        Vector2 direction = Vector2.down;

        // Führe den Raycast durch und überprüfe, ob er auf den Boden trifft
        RaycastHit2D hit = Physics2D.Raycast(position, direction, raycastDistance, groundLayer);

        // Wenn der Raycast ein Objekt trifft, das auf dem Ground-Layer liegt, ist der Charakter grounded
        if(hit.collider != null)
        {
            if (isJumping && isDoingMagneticFall)
            {
                //Instantiate(PrefabManager.Instance.GroundDashEffect, position: transform.position, new Quaternion(0, 0.707106829f, -0.707106829f, 0));
                isDoingMagneticFall = false;
                
            }

            if (isJumping)
            {
                GetComponent<PlayerSound>().PlaySnare();
            }

            

            //if(ActualFireEffect != null)
            //{
            //    Destroy(ActualFireEffect);
            //    FireBallEffect.SetActive(false);
            //}

            //MagneticBallEffect.SetActive(false);

            isGrounded = true;
            isJumping = false;
            
        }
        else
        {
            isGrounded = false;
            isJumping = true;
        }
    }

    private void MoveToRight()
    {
        //transform.Translate(Vector2.right * speed * Time.deltaTime);
        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("XP"))
        {
            Instantiate(PrefabManager.Instance.XpText);
        }

            //return;
            // Überprüfen, ob der Spieler den Boden berührt hat
        if (collision.gameObject.CompareTag("Spikes") || collision.gameObject.CompareTag("DeathWall"))
        {
            if (collision.gameObject.GetComponent<SawBlade>() != null && !collision.gameObject.GetComponent<SawBlade>().IsMoving)
            {
                return;
            }

            Vector2 position = new Vector2(transform.position.x, transform.position.y + 0.25f);
            Vector2 direction = Vector2.down;

            RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, raycastDistance, groundLayer);

            // Liste für die Treffer vor den Spikes
            List<RaycastHit2D> hitsBeforeSpikes = new List<RaycastHit2D>();

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null) continue;

                // Falls "Spikes" getroffen wurde: Schleife abbrechen
                if (hit.collider.CompareTag("Spikes"))
                {
                    break;
                }

                // Ansonsten zur Liste hinzufügen
                hitsBeforeSpikes.Add(hit);
            }

            if (hitsBeforeSpikes.Count > 0)
            {
                return;
            }

            Instantiate(PrefabManager.Instance.DieEffect, position: transform.position, new Quaternion(0f, 0.707106769f, -0.707106769f, 0));
            
            if(LifePoints == 0)
            {
                if (InventoryManager.Instance.GodMode)
                {
                    return;
                }

                speed = 0;
                FireBallEffect.SetActive(false);
                //MagneticBallEffect.SetActive(false);
                BallEffect.SetActive(false);
                BallEffect2.SetActive(false);

                rb.simulated = true;
                GetComponent<Collider2D>().enabled = false;

                FindObjectOfType<FollowPlayer>().enabled = false;

                StartCoroutine(WaitForReset()); 
                //string currentSceneName = SceneManager.GetActiveScene().name;
                //SceneManager.LoadScene(currentSceneName);
            }
            else
            {
                LifePoints--;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D hitAll = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, 0), Vector2.right, 1, groundLayer);

        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, raycastDistance, groundLayer);

        if ((hitAll.collider != null && !hitAll.collider.GetComponent<SawBlade>()) || (!isOnTop && !isOnBotton && hitAll.collider))
        {
            Instantiate(PrefabManager.Instance.DieEffect, position: transform.position, new Quaternion(0f, 0.707106769f, -0.707106769f, 0));
            if (LifePoints == 0)
            {
                if (InventoryManager.Instance.GodMode)
                {
                    return;
                }

                speed = 0;
                FireBallEffect.SetActive(false);
                //MagneticBallEffect.SetActive(false);
                BallEffect.SetActive(false);
                BallEffect2.SetActive(false);

                rb.simulated = true;
                GetComponent<Collider2D>().enabled = false;

                StartCoroutine(WaitForReset());
            }
            else
            {
                LifePoints--;
            }
        }
  
        if (collision.gameObject.CompareTag("Spikes"))
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y + 0.25f);
            Vector2 direction = Vector2.down;

            RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, raycastDistance, groundLayer);

            // Liste für die Treffer vor den Spikes
            List<RaycastHit2D> hitsBeforeSpikes = new List<RaycastHit2D>();

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null) continue;

                // Falls "Spikes" getroffen wurde: Schleife abbrechen
                if (hit.collider.CompareTag("Spikes"))
                {
                    break;
                }

                // Ansonsten zur Liste hinzufügen
                hitsBeforeSpikes.Add(hit);
            }

            if(hitsBeforeSpikes.Count > 0)
            {
                return;
            }

            Instantiate(PrefabManager.Instance.DieEffect, position: transform.position, new Quaternion(0f, 0.707106769f, -0.707106769f, 0));

            if (LifePoints == 0)
            {
                if (InventoryManager.Instance.GodMode)
                {
                    return;
                }

                speed = 0;
                FireBallEffect.SetActive(false);
                //MagneticBallEffect.SetActive(false);
                BallEffect.SetActive(false);
                BallEffect2.SetActive(false);

                rb.simulated = true;


                StartCoroutine(WaitForReset());
            }
            else
            {
                LifePoints--;
            }
            
        }

    }

    public void Die()
    {
        if (InventoryManager.Instance.GodMode)
        {
            return;
        }

        speed = 0;
        FireBallEffect.SetActive(false);
        //MagneticBallEffect.SetActive(false);
        BallEffect.SetActive(false);
        BallEffect2.SetActive(false);

        rb.simulated = true;


        StartCoroutine(WaitForReset());
    }

    private IEnumerator WaitForReset()
    {
        if (InventoryManager.Instance.GodMode || IsDead)
        {
            yield break;
        }
        GetComponentsInChildren<Renderer>().ToList().ForEach(r => r.enabled = false);

        if (FindObjectOfType<ClickingSphere>())
        {
            FindObjectOfType<ClickingSphere>().gameObject.SetActive(false);
        }
        

        Time.timeScale = 1f;

        yield return new WaitForSeconds(0.5f);

        IsDead = true;

        SaveManager.Instance.Save();
        DeathMenu.Instance.gameObject.SetActive(true);
        Sidebar.Instance.gameObject.SetActive(true);
        

        FindObjectOfType<FollowPlayer>().enabled = false;

        IsDead = true;

        DeathMenu.Instance.ScoreText.text = InventoryManager.Instance.Score.ToString();
        DeathMenu.Instance.HighscoreText.text = SaveManager.Instance.Highscore.ToString();

        InventoryManager.Instance.HighscoreTicker.enabled = false;

        yield return new WaitForSeconds(1f);

        //string currentSceneName = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(currentSceneName);
    }
}
