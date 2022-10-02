using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] SpriteRenderer connector;
    [SerializeField] Space tileSprite;
    [SerializeField] HeartPiece HeartPiecePrefab;
    [SerializeField] Vector2 boardSize = new Vector2(11, 11);

    List<List<Space>> boardSpaces;

    Space selectedSpace;

    bool clicklock = false;
    GameState gs;

    AudioSource sound;
    [SerializeField] AudioClip connectclip;

    private void Awake()
    {
        gs = FindObjectOfType<GameState>();
        sound = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Construct();
    }

    public void Construct()
    {
        boardSpaces = new List<List<Space>>();

        for (int i = 0; i < boardSize.x; i++)
        {
            boardSpaces.Add(new List<Space>());
            for (int j = 0; j < boardSize.y; j++)
            {
                Space space = Instantiate(tileSprite, new Vector2(i, j), Quaternion.identity, transform);
                boardSpaces[i].Add(space);
                if ((i + j) % 2 == 1) space.SetColor(new Color(0.7f, 0.7f, 0.7f,1f));
                if ((i + j) % 2 == 0) space.SetColor(new Color(0.5f, 0.5f, 0.5f, 1f));
            }
        }
        bool good = false;
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                good = false;
                do
                {
                    boardSpaces[i][j].FirstSpawn();
                    if (i > 1)
                    {
                        if ( boardSpaces[i - 1][j].GetPiece().GetPieceType() == boardSpaces[i][j].GetPiece().GetPieceType()
                            && boardSpaces[i - 2][j].GetPiece().GetPieceType() == boardSpaces[i][j].GetPiece().GetPieceType())
                        {
                            var piece = boardSpaces[i][j].GetPiece();
                            boardSpaces[i][j].RemovePiece();
                            Destroy(piece.gameObject);
                            continue;
                        }
                    }
                    if (j > 1)
                    {
                        if (boardSpaces[i][j - 1].GetPiece().GetPieceType() == boardSpaces[i][j].GetPiece().GetPieceType()
                            && boardSpaces[i][j - 2].GetPiece().GetPieceType() == boardSpaces[i][j].GetPiece().GetPieceType())
                        {
                            var piece = boardSpaces[i][j].GetPiece();
                            boardSpaces[i][j].RemovePiece();
                            Destroy(piece.gameObject);
                            continue;
                        }
                    }
                    
                    good = true;
                } while (!good);
            }
        }

        var centerPiece = boardSpaces[Mathf.FloorToInt(boardSize.x / 2)][Mathf.FloorToInt(boardSize.y / 2)].GetPiece();
        boardSpaces[Mathf.FloorToInt(boardSize.x / 2)][Mathf.FloorToInt(boardSize.y / 2)].SetPiece(Instantiate(HeartPiecePrefab,
            boardSpaces[Mathf.FloorToInt(boardSize.x / 2)][Mathf.FloorToInt(boardSize.y / 2)].transform.position ,Quaternion.identity));
        boardSpaces[Mathf.FloorToInt(boardSize.x / 2)][Mathf.FloorToInt(boardSize.y / 2)].GetPiece().Initialize();
        Destroy(centerPiece.gameObject);

        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                boardSpaces[i][j].Go();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !clicklock)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject g = hit.transform.gameObject;
                Space space;
                if (g.TryGetComponent<Space>(out space))
                {
                    if (space.GetPiece() != null && !space.GetPiece().IsConnected())
                    {
                        if (selectedSpace is null)
                        {
                            selectedSpace = space;
                            space.Highlight();
                            return;
                        }
                        if (selectedSpace == space)
                        {
                            space.UnHighlight();
                            selectedSpace = null;
                            return;
                        }
                        if (selectedSpace != space)
                        {
                            if (SpacesAreAdjacent(selectedSpace, space))
                            {
                                selectedSpace.UnHighlight();
                                StartCoroutine(AttemptSwitch(selectedSpace, space));
                                selectedSpace = null;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator AttemptSwitch(Space space1, Space space2)
    {
        sound.Play();
        clicklock = true;
        Piece piece1 = space1.GetPiece();
        Piece piece2 = space2.GetPiece();
        piece1.isMoving = true;
        piece2.isMoving = true;
        float delta = 0;

        do
        {
            delta += Time.fixedDeltaTime * 2f; if (delta > 1f) delta = 1f; 
            piece1.transform.position = Vector2.Lerp(space1.transform.position, space2.transform.position, delta);
            piece2.transform.position = Vector2.Lerp(space2.transform.position, space1.transform.position, delta);
            yield return new WaitForFixedUpdate();
        } while (piece1.transform.position != space2.transform.position);
        
        space1.SetPiece(piece2);
        space2.SetPiece(piece1);

        bool check = (CheckForConnection(space1)|CheckForConnection(space2));
        if (!check && (piece1.GetPieceType() == -1 || piece2.GetPieceType() == -1))
        {
            check = true;
            gs.DamageHealth(1);
            if (piece1 is HeartPiece)
            {
                HeartPiece h = (HeartPiece)piece1;
                h.DamageEffects();
            }
            if (piece2 is HeartPiece)
            {
                HeartPiece h = (HeartPiece)piece2;
                h.DamageEffects();
            }
        }

        if (!check)
        {
            delta = 0;
            sound.Play();
            do
            {
                delta += Time.fixedDeltaTime * 2f; if (delta > 1f) delta = 1f;
                piece1.transform.position = Vector2.Lerp(space2.transform.position, space1.transform.position, delta);
                piece2.transform.position = Vector2.Lerp(space1.transform.position, space2.transform.position, delta);
                yield return new WaitForFixedUpdate();
            } while (piece1.transform.position != space1.transform.position);

            space1.SetPiece(piece1);
            space2.SetPiece(piece2);
        }
        piece1.isMoving = false;
        piece2.isMoving = false;
        clicklock = false;
    }

    public bool CheckForConnection(Piece p)
    {
        Space s = FindSpaceByPiece(p);
        return CheckForConnection(s);
    }

    public bool CheckForConnection(Space s)
    {
        Color c = s.GetPiece().GetComponent<SpriteRenderer>().color;
        Vector2 v2 = FindSpaceCoordinates(s);
        List<Piece> connection = new List<Piece>();
        List<SpriteRenderer> indicators = new List<SpriteRenderer>();
        List<SpriteRenderer> passindicators = new List<SpriteRenderer>();
        connection.Add(s.GetPiece());
        //Horizontal
        List<Piece> horizontal = new List<Piece>();
        if (v2.x > 0)
        {
            if (boardSpaces[(int)v2.x - 1][(int)v2.y].GetPiece() != null &&
                s.GetPiece().GetPieceType() == boardSpaces[(int)v2.x - 1][(int)v2.y].GetPiece().GetPieceType()
                && !boardSpaces[(int)v2.x - 1][(int)v2.y].GetPiece().IsConnected())
            {
                horizontal.Add(boardSpaces[(int)v2.x - 1][(int)v2.y].GetPiece());
                indicators.Add(Instantiate(connector, new Vector3(v2.x - .5f, v2.y, 0), Quaternion.Euler(0,0,90)));

                if (v2.x > 1)
                {
                    if (boardSpaces[(int)v2.x - 2][(int)v2.y].GetPiece() != null &&
                        s.GetPiece().GetPieceType() == boardSpaces[(int)v2.x - 2][(int)v2.y].GetPiece().GetPieceType()
                        && !boardSpaces[(int)v2.x - 2][(int)v2.y].GetPiece().IsConnected())
                    {
                        horizontal.Add(boardSpaces[(int)v2.x - 2][(int)v2.y].GetPiece());
                        indicators.Add(Instantiate(connector, new Vector3(v2.x - 1.5f, v2.y, 0), Quaternion.Euler(0, 0, 90)));
                    }
                }
            }
        }
        if (v2.x < boardSize.x - 1)
        {
            if (boardSpaces[(int)v2.x + 1][(int)v2.y].GetPiece() != null &&
                s.GetPiece().GetPieceType() == boardSpaces[(int)v2.x + 1][(int)v2.y].GetPiece().GetPieceType()
                && !boardSpaces[(int)v2.x + 1][(int)v2.y].GetPiece().IsConnected())
            {
                horizontal.Add(boardSpaces[(int)v2.x + 1][(int)v2.y].GetPiece());
                indicators.Add(Instantiate(connector, new Vector3(v2.x + .5f, v2.y, 0), Quaternion.Euler(0, 0, 90)));
                if (v2.x < boardSize.x - 2)
                {
                    if (boardSpaces[(int)v2.x + 2][(int)v2.y].GetPiece() != null &&
                        s.GetPiece().GetPieceType() == boardSpaces[(int)v2.x + 2][(int)v2.y].GetPiece().GetPieceType()
                    && !boardSpaces[(int)v2.x + 2][(int)v2.y].GetPiece().IsConnected())
                    {
                        horizontal.Add(boardSpaces[(int)v2.x + 2][(int)v2.y].GetPiece());
                        indicators.Add(Instantiate(connector, new Vector3(v2.x + 1.5f, v2.y, 0), Quaternion.Euler(0, 0, 90)));
                    }
                }
            }
        }
        if (horizontal.Count >= 2)
        {
            connection.AddRange(horizontal);
            passindicators.AddRange(indicators);

            foreach (SpriteRenderer r in indicators)
            {
                r.color = c;
            }

            indicators.Clear();
        }
        if (horizontal.Count < 2)
        {
            foreach (SpriteRenderer r in indicators)
            {
                Destroy(r.gameObject);
            }
            indicators.Clear();
        }
        //Vertical
        List<Piece> vertical = new List<Piece>();
        if (v2.y > 0)
        {
            if (boardSpaces[(int)v2.x][(int)v2.y - 1].GetPiece() != null 
                && s.GetPiece().GetPieceType() == boardSpaces[(int)v2.x][(int)v2.y - 1].GetPiece().GetPieceType()
                && !boardSpaces[(int)v2.x][(int)v2.y - 1].GetPiece().IsConnected())
            {
                vertical.Add(boardSpaces[(int)v2.x][(int)v2.y - 1].GetPiece());
                indicators.Add(Instantiate(connector, new Vector3(v2.x, v2.y - .5f, 0), Quaternion.identity));
                if (v2.y > 1)
                {
                    if (boardSpaces[(int)v2.x][(int)v2.y - 2].GetPiece()
                        && s.GetPiece().GetPieceType() == boardSpaces[(int)v2.x][(int)v2.y - 2].GetPiece().GetPieceType() 
                        && !boardSpaces[(int)v2.x][(int)v2.y - 2].GetPiece().IsConnected())
                    {
                        vertical.Add(boardSpaces[(int)v2.x][(int)v2.y - 2].GetPiece());
                        indicators.Add(Instantiate(connector, new Vector3(v2.x, v2.y - 1.5f, 0), Quaternion.identity));
                    }
                }
            }
        }
        if (v2.y < boardSize.y - 1)
        {
            if (boardSpaces[(int)v2.x][(int)v2.y + 1].GetPiece() != null 
                && s.GetPiece().GetPieceType() == boardSpaces[(int)v2.x][(int)v2.y+1].GetPiece().GetPieceType()
                && !boardSpaces[(int)v2.x][(int)v2.y + 1].GetPiece().IsConnected())
            {
                vertical.Add(boardSpaces[(int)v2.x][(int)v2.y+1].GetPiece());
                indicators.Add(Instantiate(connector, new Vector3(v2.x, v2.y + .5f, 0), Quaternion.identity));
                if (v2.y < boardSize.y - 2)
                {
                    if (boardSpaces[(int)v2.x][(int)v2.y + 2].GetPiece() != null 
                        && s.GetPiece().GetPieceType() == boardSpaces[(int)v2.x][(int)v2.y+2].GetPiece().GetPieceType() 
                        && !boardSpaces[(int)v2.x][(int)v2.y + 2].GetPiece().IsConnected())
                    {
                       vertical.Add(boardSpaces[(int)v2.x][(int)v2.y+2].GetPiece());
                        indicators.Add(Instantiate(connector, new Vector3(v2.x, v2.y + 1.5f, 0), Quaternion.identity));
                    }
                }
            }
        }
        if (vertical.Count >= 2)
        {
            connection.AddRange(vertical);

            passindicators.AddRange(indicators);
            foreach (SpriteRenderer r in indicators)
            {
                r.color = c;
            }

            indicators.Clear();
        }
        if (vertical.Count < 2)
        {
            foreach (SpriteRenderer r in indicators)
            {
                Destroy(r.gameObject);
            }
            indicators.Clear();
        }


        if (connection.Count > 1)
        {
            sound.PlayOneShot(connectclip);
            ConnectionGroup newGroup = new GameObject().AddComponent<ConnectionGroup>();

            newGroup.Create(connection, passindicators);

            return true;
        }

        return false;
    }

    Vector2 FindSpaceCoordinates(Space s)
    {
        Vector2 v2 = Vector2.zero;
        foreach (List<Space> list in boardSpaces)
        {
            if (list.Contains(s))
            {
                v2.x = boardSpaces.IndexOf(list);
                v2.y = list.IndexOf(s);
            }
        }

        return v2;
    }

    Space FindSpaceByPiece(Piece p)
    {
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                Space s = boardSpaces[i][j];
                if (s.GetPiece() == p) return s;
            }
        }

        return null;
    }

    bool SpacesAreAdjacent(Space space1,Space space2)
    {
        Vector2 v1 = FindSpaceCoordinates(space1);
        Vector2 v2 = FindSpaceCoordinates(space2);

        if ((v2.x == v1.x + 1 || v2.x == v1.x - 1) && v1.y == v2.y)
        {
            return true;
        }
        if ((v2.y == v1.y + 1 || v2.y == v1.y - 1) && v1.x == v2.x)
        {
            return true;
        }
        return false;
    }

    public void DestroyPiece(Piece piece)
    {
        Vector2 v2 = FindSpaceCoordinates(FindSpaceByPiece(piece));
        boardSpaces[(int)v2.x][(int)v2.y].RemovePiece();
        piece.Break();
    }
}
