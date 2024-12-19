using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    public float jumpForce = 10f;          // Die Stärke des Sprungs
    public float fallSpeedMultiplier = 10f; // Wie schnell der Spieler magnetisch zu Boden gezogen wird
    private KeyCode jumpKey = KeyCode.Mouse0; // Taste für den Sprung

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

    public bool GameIsStarted;

    public int LifePoints;

    private List<GameObject> passedObjects = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FireBallEffect.SetActive(false);
        MagneticBallEffect.SetActive(false);
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

        if (!GameIsStarted && Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameIsStarted = true;
            rb.simulated = true;
        }
        if (!GameIsStarted)
        {
            rb.simulated = false;
            return;
        }

        MoveToRight();

        Debug.Log("Velocity: " + rb.velocity);

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
        IsGrounded();
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
        FireBallEffect.SetActive(true);

        GetComponent<PlayerSound>().PlayElectricalSound();

        rb.velocity = new Vector2(rb.velocity.x, 0); // Nullt die vertikale Geschwindigkeit
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void MagneticFall()
    {
        // Erhöht die Fallgeschwindigkeit, um den Spieler schnell zu Boden zu ziehen
        //Destroy(ActualFireEffect);

        GetComponent<PlayerSound>().PlayWhoosh();

        GetComponent<PlayerSound>()._audioSource2.Stop();

        ActualFireEffect.ForEach(a => Destroy(a));
        ActualFireEffect.Clear();

        FireBallEffect.SetActive(false);
        MagneticBallEffect.SetActive(true);

        rb.velocity = new Vector2(rb.velocity.x, -fallSpeedMultiplier);
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
                GetComponent<PlayerSound>().PlayMetal();
            }

            //if(ActualFireEffect != null)
            //{
            //    Destroy(ActualFireEffect);
            //    FireBallEffect.SetActive(false);
            //}

            MagneticBallEffect.SetActive(false);

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
        rb.velocity = new Vector2(4.5f, rb.velocity.y);
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
                speed = 0;
                FireBallEffect.SetActive(false);
                MagneticBallEffect.SetActive(false);
                BallEffect.SetActive(false);
                BallEffect2.SetActive(false);

                rb.simulated = true;
                GetComponent<Collider2D>().enabled = false;

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
        //return;

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position - new Vector2(0,0.5f), Vector2.right, raycastDistance*4, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, 0.5f), Vector2.right, raycastDistance*4, groundLayer);

        if (hit.collider != null && hit2.collider != null)
        {
            Instantiate(PrefabManager.Instance.DieEffect, position: transform.position, new Quaternion(0f, 0.707106769f, -0.707106769f, 0));

            if (LifePoints == 0)
            {
                speed = 0;
                FireBallEffect.SetActive(false);
                MagneticBallEffect.SetActive(false);
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
                speed = 0;
                FireBallEffect.SetActive(false);
                MagneticBallEffect.SetActive(false);
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

    }

    private IEnumerator WaitForReset()
    {
        SaveManager.Instance.Save();
        DeathMenu.Instance.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        //string currentSceneName = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(currentSceneName);
    }
}
