using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    protected int pieceType = 0;
    [SerializeField] List<Sprite> sprites;
    [SerializeField] protected ParticleSystem breakParticles;

    ConnectionGroup group;
    bool isConnected = false;
    public bool isMoving = false;

    protected AudioSource sound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update

    public virtual void Initialize()
    {
        pieceType = Random.Range(0, 4);
        SetColor();
    }

    void SetColor()
    {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        switch (pieceType)
        {
            case 0:
                r.color = Color.red;
                r.sprite = sprites[pieceType];
                transform.rotation = Quaternion.Euler(0, 0, 45);
                transform.localScale = new Vector3(.6f, .6f, 1);
                break;
            case 1:
                r.color = Color.blue;
                r.sprite = sprites[pieceType];
                break;
            case 2:
                r.color = Color.yellow;
                r.sprite = sprites[pieceType];
                break;
            case 3:
                r.color = Color.green;
                r.sprite = sprites[pieceType];
                break;
        }
    }

    public int GetPieceType()
    {
        return pieceType;
    }

    public void Connect(ConnectionGroup g)
    {
        group = g;
        isConnected = true;
    }

    public virtual void Break()
    {
        GetComponent<Collider2D>().enabled = false;
        sound.Play();
        StartCoroutine(BreakAnim());
    }

    public bool IsConnected()
    {
        return isConnected;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isMoving)
        {
            if (collision.gameObject.TryGetComponent<Laser>(out Laser l))
            {
                if (isConnected)
                {
                    l.Drain(group.GetGroupSize(), pieceType);
                    group.DestroyGroup();
                    return;
                }
                if (!isConnected)
                {
                    l.Drain(1, pieceType);
                    FindObjectOfType<Board>().DestroyPiece(this);
                    return;
                }
            }
        }
    }

    private IEnumerator BreakAnim()
    {
        var part = Instantiate(breakParticles, transform.position, Quaternion.identity);
        part.GetComponent<ParticleSystemRenderer>().material.color = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitUntil(SoundDonePlaying);
        Destroy(gameObject);
    }

    private bool SoundDonePlaying()
    {
        return !sound.isPlaying;
    }
}
