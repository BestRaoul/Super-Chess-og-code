using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public string name;
    public Sprite icon8x8_white;
    public Sprite icon8x8_black;

    public int team;
    public GameObject LazerPF;

    [HideInInspector] public ChessBoard chessboard;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
    }

    public virtual List<Vector3Int> MovePositions()
    {
        List<Vector3Int> outList = new List<Vector3Int>();
        outList.Add(new Vector3Int(1, 0, 0));

        return outList;
    }

    public virtual void Move(Vector3Int pos)
    {
        chessboard.pieces.Remove(position());
        Piece tryPiece;
        if(chessboard.pieces.TryGetValue(pos, out tryPiece)) { tryPiece.Die(); }

        gameObject.transform.localPosition = pos;
        audioSource.Play();
        if (chessboard.gameEnded) { return; }
        chessboard.pieces.Add(position(), this);
    }

    #region Instant void for rewind
    public virtual void InstantMove(Vector3Int pos)
    {
        chessboard.pieces.Remove(position());
        Piece tryPiece;
        if (chessboard.pieces.TryGetValue(pos, out tryPiece)) { tryPiece.InstantDeath(); }

        gameObject.transform.localPosition = pos;
        chessboard.pieces.Add(position(), this);
    }
    public virtual void InstantDeath()
    {
        chessboard.pieces.Remove(position());
        sr.enabled = false;
        Destroy(gameObject);
    }
    public void InstantLAZER(Vector3Int vec)
    {
        Vector3Int direction = new Vector3Int(-(int)Mathf.Sign(vec.x), -(int)Mathf.Sign(vec.y), 0);
        if (vec.x == 0) direction.x = 0;
        if (vec.y == 0) direction.y = 0;

        Piece tryPiece = null;
        Vector3Int add = position();
        for (int i = 1; i < 8; i++)
        {
            add += direction;

            if (add.x >= 8 || add.x < 0  //outofboundscheck
                || add.y >= 8 || add.y < 0)  //outofboundscheck
            {
                add += direction;
                break;
            }
            if (chessboard.pieces.TryGetValue(add, out tryPiece))
            {
                tryPiece.InstantDeath();
                break;
            }
        }
    }
    #endregion

    public Vector3Int position()
    {
        return new Vector3Int((int)gameObject.transform.localPosition.x, (int)gameObject.transform.localPosition.y, (int)gameObject.transform.localPosition.z);
    }

    public virtual void Refresh()
    {
        switch (team)
        {
            case 1:
                sr.sprite = icon8x8_white;
                break;

            case -1:
                sr.sprite = icon8x8_black;
                break;
        }
    }

    public List<Vector3Int> GetPositionsInLine(Vector3Int vec, int distance = 8)
    {
        List<Vector3Int> list = new List<Vector3Int>();

        Vector3Int add = Vector3Int.zero;

        for (int i = 1; i < distance; i++)
        {
            add += vec;
            if (position().x + add.x >= 8 || position().x + add.x < 0 || position().y + add.y >= 8 || position().y + add.y < 0) { return list; } //outofboundscheck

            Piece tryPiece;
            if(!chessboard.pieces.TryGetValue(position() + add, out tryPiece))
            {
                list.Add(position() + add);
            } else if (tryPiece.team != team) { list.Add(position() + add); return list; }
            else if (tryPiece.team == team) { return list; }
        }

        return list;
    }

    public bool InBounds(Vector3Int vec)
    {
        if (vec.x >= 8 || vec.x < 0 || vec.y >= 8 || vec.y < 0)
            return false;
        else
            return true;
    }

    public virtual void OnEndTurn()
    {
        print("onendturn " + name);
    }

    public IEnumerator LAZER(Vector3Int vec)
    {
        Vector3Int direction = new Vector3Int(-(int)Mathf.Sign(vec.x), -(int)Mathf.Sign(vec.y), 0);
        if (vec.x == 0) direction.x = 0;
        if (vec.y == 0) direction.y = 0;

        GameObject lazer = Instantiate(LazerPF, Vector3Int.zero, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), transform);
        lazer.transform.localPosition = new Vector3(0.5f * direction.x, 0.5f * direction.y, 0f);
        lazer.GetComponent<Animator>().SetTrigger("Shoot");

        Destroy(lazer, 3f);

        yield return new WaitForSeconds(.3f);

        Piece tryPiece = null;
        Vector3Int add = position();
        for (int i = 1; i < 8; i++)
        {
            add += direction;

            if (add.x >= 8 || add.x < 0  //outofboundscheck
                || add.y >= 8 || add.y < 0)  //outofboundscheck
            {
                add += direction;
                break;
            }
            if (chessboard.pieces.TryGetValue(add, out tryPiece))
            {
                break;
            }
        }

        Transform trail = lazer.transform.Find("trail");
        Transform explosion = lazer.transform.Find("explosion");

        if (tryPiece != null)
        {
            trail.localPosition = new Vector3((position() - tryPiece.position()).magnitude/2, 0, 0);
            trail.localScale = new Vector3((position() - tryPiece.position()).magnitude/2, 1, 1);
            explosion.localPosition = new Vector3((position() - tryPiece.position()).magnitude + 0.3f, 0, 0); ;

            yield return new WaitForSeconds(.4f);
            tryPiece.Die();
        }
        else
        {
            trail.localPosition = new Vector3(8, 0, 0);
            trail.localScale = new Vector3(8, 1, 1);
            explosion.localPosition = new Vector3(10,0,0);
        }

    }
    
    public virtual void Die()
    {
        chessboard.pieces.Remove(position());
        sr.enabled = false; 
        Destroy(gameObject, 1f);
    }
}
