using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public bool GameIsStarted;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FireBallEffect.SetActive(false);
        MagneticBallEffect.SetActive(false);
    }


    void Update()
    {
        if (!GameIsStarted && Input.GetKeyDown(KeyCode.F))
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

        // Überprüfen, ob der Spieler auf dem Boden steht und die Sprungtaste drückt
        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            Jump();
        }
        // Überprüfen, ob der Spieler im Sprung ist und die Sprungtaste erneut drückt
        else if (isJumping && Input.GetKeyDown(jumpKey))
        {
            MagneticFall();
        }
        IsGrounded();
    }

    void Jump()
    {
        // Setze den Spieler in den Sprungzustand und füge eine Aufwärtskraft hinzu
        //isJumping = true;
        //isGrounded = false;
        //ActualFireEffect.Add(Instantiate(PrefabManager.Instance.JumpDashEffect, position: transform.position - new Vector3(0, 5.19f, 0), new Quaternion(0.497374982f, 0.502611339f, -0.502611339f, -0.497374982f)));
        FireBallEffect.SetActive(true);

        rb.velocity = new Vector2(rb.velocity.x, 0); // Nullt die vertikale Geschwindigkeit
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void MagneticFall()
    {
        // Erhöht die Fallgeschwindigkeit, um den Spieler schnell zu Boden zu ziehen
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

        // Führe den Raycast durch und überprüfe, ob er auf den Boden trifft
        RaycastHit2D hit = Physics2D.Raycast(position, direction, raycastDistance, groundLayer);

        // Wenn der Raycast ein Objekt trifft, das auf dem Ground-Layer liegt, ist der Charakter grounded
        if(hit.collider != null)
        {
            if (isJumping && isDoingMagneticFall)
            {
                Instantiate(PrefabManager.Instance.GroundDashEffect, position: transform.position, new Quaternion(0, 0.707106829f, -0.707106829f, 0));
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
        // Überprüfen, ob der Spieler den Boden berührt hat
        if (collision.gameObject.CompareTag("Spikes"))
        {
            if (collision.gameObject.GetComponent<SawBlade>() != null &&!collision.gameObject.GetComponent<SawBlade>().IsMoving)
            {
                return;
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}
