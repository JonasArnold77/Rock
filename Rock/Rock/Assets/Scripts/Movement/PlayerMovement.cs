using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    public float jumpForce = 10f;          // Die Stärke des Sprungs
    public float fallSpeedMultiplier = 10f; // Wie schnell der Spieler magnetisch zu Boden gezogen wird
    private KeyCode jumpKey = KeyCode.Space; // Taste für den Sprung

    private Rigidbody2D rb;
    private bool isGrounded = false;       // Überprüft, ob der Spieler den Boden berührt hat
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

    private bool isVelocityPositive = true;
    private bool IsOnPointA = true;

    private List<GameObject> passedObjects = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FireBallEffect.SetActive(false);
        //MagneticBallEffect.SetActive(true);

        StartMenu.Instance.gameObject.SetActive(true);


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

        MoveToRight();

        Debug.Log("Velocity: " + rb.velocity);

#if UNITY_ANDROID
        if (ChallengeManager.Instance.actualChallengeButton.title == "Normal" || ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode" || ChallengeManager.Instance.actualChallengeButton.title == "HardcoreMode" || ChallengeManager.Instance.actualChallengeButton.title == "Highspeed")
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
        }else if(ChallengeManager.Instance.actualChallengeButton.title == "Gravity")
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
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
            if (Input.GetKeyDown(KeyCode.Mouse0))
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

#elif UNITY_STANDALONE_WIN

        if (ChallengeManager.Instance.actualChallengeButton.title == "Normal" || ChallengeManager.Instance.actualChallengeButton.title == "BouncyMode" || ChallengeManager.Instance.actualChallengeButton.title == "HardcoreMode" || ChallengeManager.Instance.actualChallengeButton.title == "Highspeed" || ChallengeManager.Instance.actualChallengeButton.title == "MoveWithBall" || ChallengeManager.Instance.actualChallengeButton.title == "MoveCamera") 
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
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, 20);
                }

                isVelocityPositive = !isVelocityPositive;

            }
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


        IsGrounded();
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
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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
        }

        isDoingMagneticFall = true;
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
        if (collision.gameObject.CompareTag("Spikes"))
        {
            if (collision.gameObject.GetComponent<SawBlade>() != null && !collision.gameObject.GetComponent<SawBlade>().IsMoving)
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

                IsDead = true;

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
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position - new Vector2(0, 0.35f), Vector2.right, raycastDistance, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, 0.35f), Vector2.right, raycastDistance, groundLayer);


        
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, raycastDistance, groundLayer);


        if ((hit.collider != null || hit2.collider != null) && hit.collider.GetComponent<PushDown>() == null && !hit.collider.gameObject.GetComponent<SawBlade>() && !hit2.collider.gameObject.GetComponent<SawBlade>())
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


                StartCoroutine(WaitForReset());
            }
            else
            {
                LifePoints--;
            }
        }

    }

    private IEnumerator WaitForReset()
    {
        if (InventoryManager.Instance.GodMode)
        {
            yield break;
        }

        SaveManager.Instance.Save();
        DeathMenu.Instance.gameObject.SetActive(true);

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
