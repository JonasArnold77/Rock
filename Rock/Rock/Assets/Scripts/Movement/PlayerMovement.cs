using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    public float jumpForce = 10f;          // Die St�rke des Sprungs
    public float fallSpeedMultiplier = 10f; // Wie schnell der Spieler magnetisch zu Boden gezogen wird
    private KeyCode jumpKey = KeyCode.Mouse0; // Taste f�r den Sprung

    private Rigidbody2D rb;
    private bool isGrounded = false;       // �berpr�ft, ob der Spieler den Boden ber�hrt hat
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


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FireBallEffect.SetActive(false);
        MagneticBallEffect.SetActive(false);
    }


    void Update()
    {
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

        // �berpr�fen, ob der Spieler auf dem Boden steht und die Sprungtaste dr�ckt
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            Jump();
        }
        // �berpr�fen, ob der Spieler im Sprung ist und die Sprungtaste erneut dr�ckt
        else if (isJumping && Input.GetKeyDown(jumpKey))
        {
            MagneticFall();
        }
        IsGrounded();
    }

    void Jump()
    {
        // Setze den Spieler in den Sprungzustand und f�ge eine Aufw�rtskraft hinzu
        //isJumping = true;
        //isGrounded = false;
        //ActualFireEffect.Add(Instantiate(PrefabManager.Instance.JumpDashEffect, position: transform.position - new Vector3(0, 5.19f, 0), new Quaternion(0.497374982f, 0.502611339f, -0.502611339f, -0.497374982f)));
        FireBallEffect.SetActive(true);

        rb.velocity = new Vector2(rb.velocity.x, 0); // Nullt die vertikale Geschwindigkeit
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void MagneticFall()
    {
        // Erh�ht die Fallgeschwindigkeit, um den Spieler schnell zu Boden zu ziehen
        //Destroy(ActualFireEffect);

        GetComponent<PlayerSound>().PlayWhoosh();

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

        // F�hre den Raycast durch und �berpr�fe, ob er auf den Boden trifft
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
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        return;
        // �berpr�fen, ob der Spieler den Boden ber�hrt hat
        if (collision.gameObject.CompareTag("Spikes"))
        {
            if (collision.gameObject.GetComponent<SawBlade>() != null && !collision.gameObject.GetComponent<SawBlade>().IsMoving)
            {
                return;
            }

            Instantiate(PrefabManager.Instance.DieEffect, position: transform.position, new Quaternion(0f, 0.707106769f, -0.707106769f, 0));
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        return;
        if (collision.gameObject.CompareTag("Spikes"))
        {
            Instantiate(PrefabManager.Instance.DieEffect, position: transform.position, new Quaternion(0f, 0.707106769f, -0.707106769f, 0));
            speed = 0;
            FireBallEffect.SetActive(false);
            MagneticBallEffect.SetActive(false);
            BallEffect.SetActive(false);
            BallEffect2.SetActive(false);

            rb.simulated = true;
            GetComponent<Collider2D>().enabled = false;

            StartCoroutine(WaitForReset());
        }

    }

    private IEnumerator WaitForReset()
    {
        yield return new WaitForSeconds(1f);

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
