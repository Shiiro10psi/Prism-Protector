using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionGroup : MonoBehaviour
{
    List<Piece> pieces;
    List<SpriteRenderer> connects;

    public void Create(List<Piece> p, List<SpriteRenderer> c)
    {
        pieces = p;
        connects = c;

        foreach(Piece pi in pieces)
        {
            pi.Connect(this);
        }
        if (pieces.Count >= 5)
        {
            FindObjectOfType<GameState>().DamageHealth(4 - pieces.Count);
        }
    }

    public int GetGroupSize() { return pieces.Count; }

    public void DestroyGroup() 
    {
        Board b = FindObjectOfType<Board>();
        
        foreach (SpriteRenderer c in connects)
        {
            Destroy(c.gameObject);
        }
        foreach (Piece p in pieces)
        {
            b.DestroyPiece(p);
        }

        Destroy(gameObject);
    }
}
