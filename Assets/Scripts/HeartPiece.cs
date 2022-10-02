using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPiece : Piece
{
    GameState state;

    public override void Initialize()
    {
        pieceType = -1;
        state = FindObjectOfType<GameState>();
        GetComponent<SpriteRenderer>().color = Color.magenta;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Laser>(out Laser l))
        {
            state.DamageHealth(l.GetStrength());
            l.Kill();
            DamageEffects();
        }

    }

    public void DamageEffects()
    {
        sound.Play();
        var part = Instantiate(breakParticles, transform.position, Quaternion.identity);
        part.GetComponent<ParticleSystemRenderer>().material.color = GetComponent<SpriteRenderer>().color;
    }
}
