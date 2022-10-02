using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    int strength = 5;
    int type = 0;

    public void SetStrength(int i)
    {
        strength = i;
    }

    public int GetStrength()
    {
        return strength;
    }

    public void SetType(int i)
    {
        type = i;

        SpriteRenderer r = GetComponent<SpriteRenderer>();
        switch (type)
        {
            case 0:
                r.color = Color.red;
                break;
            case 1:
                r.color = Color.blue;
                break;
            case 2:
                r.color = Color.yellow;
                break;
            case 3:
                r.color = Color.green;
                break;
        }
    }

    public void Drain(int number, int pieceType)
    {
        int multiplier = 1;
        if (pieceType == type) multiplier = 2;
            var gs = FindObjectOfType<GameState>();
            if (number == 1) gs.AddScore(10);
            if (number != 1) gs.AddScore(100 * number * multiplier);
        strength -= number * multiplier;
        Debug.Log(strength);
        if (strength <= 0) Kill();
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
