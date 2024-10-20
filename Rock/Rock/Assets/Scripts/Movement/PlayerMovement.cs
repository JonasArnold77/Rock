using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    public float jumpForce = 10f;          // Die St�rke des Sprungs
    public float fallSpeedMultiplier = 10f; // Wie schnell der Spieler magnetisch zu Boden gezogen wird
    public KeyCode jumpKey = KeyCode.Space; // Taste f�r den Sprung

    private Rigidbody2D rb;
    private bool isGrounded = false;       // �berpr�ft, ob der Spieler den Boden ber�hrt hat
    private bool isJumping = false;

    public LayerMask groundLayer; // Layer des Bodens, um nur mit dem Boden zu kollidieren
    public float raycastDistance = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
        isJumping = true;
        isGrounded = false;
        rb.velocity = new Vector2(rb.velocity.x, 0); // Nullt die vertikale Geschwindigkeit
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void MagneticFall()
    {
        // Erh�ht die Fallgeschwindigkeit, um den Spieler schnell zu Boden zu ziehen
        rb.velocity = new Vector2(rb.velocity.x, -fallSpeedMultiplier);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // �berpr�fen, ob der Spieler den Boden ber�hrt hat
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        isGrounded = true;
    //        isJumping = false;
    //    }
    //}

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
}
