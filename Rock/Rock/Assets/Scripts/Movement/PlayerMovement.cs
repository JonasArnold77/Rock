using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    public float jumpForce = 10f;          // Die Stärke des Sprungs
    public float fallSpeedMultiplier = 10f; // Wie schnell der Spieler magnetisch zu Boden gezogen wird
    public KeyCode jumpKey = KeyCode.Space; // Taste für den Sprung

    private Rigidbody2D rb;
    private bool isGrounded = false;       // Überprüft, ob der Spieler den Boden berührt hat
    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
    }

    void Jump()
    {
        // Setze den Spieler in den Sprungzustand und füge eine Aufwärtskraft hinzu
        isJumping = true;
        isGrounded = false;
        rb.velocity = new Vector2(rb.velocity.x, 0); // Nullt die vertikale Geschwindigkeit
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void MagneticFall()
    {
        // Erhöht die Fallgeschwindigkeit, um den Spieler schnell zu Boden zu ziehen
        rb.velocity = new Vector2(rb.velocity.x, -fallSpeedMultiplier);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Überprüfen, ob der Spieler den Boden berührt hat
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    private void MoveToRight()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
