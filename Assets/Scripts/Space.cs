using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    float respawnTime = 2f;
    float timer = 2f;
    bool go = false;

    Piece piece;
    [SerializeField] Piece piecePrefab;

    Color color = Color.grey;
    SpriteRenderer sprite;

    Board board;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        board = FindObjectOfType<Board>();
    }
    // Start is called before the first frame update
    void Start()
    { 
    }

    public void FirstSpawn()
    {
        SpawnPiece();
    }

    public void Go()
    {
        go = true;
    }

    void SpawnPiece()
    {
        piece = Instantiate(piecePrefab, transform.position,Quaternion.identity, board.transform);
        piece.Initialize();
        timer = respawnTime;
    }

    public void SetPiece(Piece p)
    {
        piece = p;
    }

    public Piece GetPiece()
    {
        return piece;
    }

    public void RemovePiece()
    {
        piece = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (piece == null && go)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0f)
        {
            SpawnPiece();
            board.CheckForConnection(piece);
        }
    }

    public void SetColor(Color c)
    {
        color = c;
        sprite.color = color;
    }

    public void Highlight()
    {
        sprite.color = Color.white;
    }

    public void UnHighlight()
    {
        sprite.color = color;
    }
}
