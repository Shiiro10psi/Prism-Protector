using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCatcher : MonoBehaviour
{

    GameState gs;
    private void Awake()
    {

        gs = FindObjectOfType<GameState>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Laser>(out Laser l))
        {
            gs.DamageHealth(-l.GetStrength());
            Destroy(l.gameObject);
        }
    }
}
