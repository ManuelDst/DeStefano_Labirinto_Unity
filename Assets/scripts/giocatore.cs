using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class giocatore : MonoBehaviour
{
    float velocitaMovimento = 20f;
    Rigidbody2D rb;
    Vector2 movimento;
    generatoreLab generatoreLabirinto;
    bool puoMuoversi = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        generatoreLabirinto = FindObjectOfType<generatoreLab>();
    }

    private void Update()
    {
        if (!puoMuoversi)
        {
            return;
        }


        movimento.x = Input.GetAxisRaw("Horizontal");
        movimento.y = Input.GetAxisRaw("Vertical");

        // serve per non avere velocità doppia quando mi muovo in diagonale
        if (movimento.magnitude > 1)
        {
            movimento.Normalize();
        }


    }
    private void FixedUpdate()
        {
            if (!puoMuoversi) return;

            // Muove il personaggio
            rb.MovePosition(rb.position + movimento * velocitaMovimento * Time.fixedDeltaTime);
            // fixeddeltatime serve per non far andare troppo veloce il personaggio
        }

    private void OnTriggerEnter2D(Collider2D t) // viene chiamato ogni volta che si va a contatto con un trigger
        {

            if (t.CompareTag("Exit")) // se ha tag exit si passa al livello successivo
            {
                puoMuoversi = false;

            CaricaProssimoLivello();
            }
        }

        private void CaricaProssimoLivello()
        {
            if (generatoreLabirinto != null)
            {
                generatoreLabirinto.ProssimoLivello();
            }
            puoMuoversi = true;
        }
    
}