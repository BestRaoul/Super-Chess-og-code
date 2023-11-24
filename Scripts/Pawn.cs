using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public bool firstMove = true;

    public AudioSource wilhelmScream;

    public override List<Vector3Int> MovePositions()
    {
        List<Vector3Int> list = new List<Vector3Int>();
        list.Add(new Vector3Int(0, 1 * team, 0) + position());

        //first move only
        if (firstMove)
        {
            list.Add(new Vector3Int(0, 2 * team, 0) + position());
        }

        //check for enemy on corners
        Piece tryPiece;
        if (chessboard.pieces.TryGetValue(position() + new Vector3Int(1, 1 * team, 0), out tryPiece))
            if(tryPiece.team != team)
                list.Add(position() + new Vector3Int(1, 1 * team, 0));
        if (chessboard.pieces.TryGetValue(position() + new Vector3Int(-1, 1 * team, 0), out tryPiece))
            if (tryPiece.team != team)
                list.Add(position() + new Vector3Int(-1, 1 * team, 0));

        return list;
    }

    public override void Move(Vector3Int pos)
    {
        firstMove = false;

        Piece tryPiece;

        Push(position(), pos-position(), this);

        audioSource.Play();
    }

    private void Push(Vector3Int from, Vector3Int push, Piece p)
    {
        Piece tryPiece;
        if (chessboard.pieces.TryGetValue(from + push, out tryPiece)) { Push(tryPiece.position(), push, tryPiece); }

        chessboard.pieces.Remove(p.position());
        p.gameObject.transform.localPosition += push;

        if (p.position().x >= 8 || p.position().x < 0 || p.position().y >= 8 || p.position().y < 0) {
            wilhelmScream.Play();
            Destroy(p.gameObject);
            return;
        }

        chessboard.pieces.Add(p.position(), p);
    }

    public override void InstantMove(Vector3Int pos)
    {
        firstMove = false;

        Piece tryPiece;

        InstantPush(position(), pos - position(), this);

    }
    private void InstantPush(Vector3Int from, Vector3Int push, Piece p)
    {
        Piece tryPiece;
        if (chessboard.pieces.TryGetValue(from + push, out tryPiece)) { Push(tryPiece.position(), push, tryPiece); }

        chessboard.pieces.Remove(p.position());
        p.gameObject.transform.localPosition += push;

        if (p.position().x >= 8 || p.position().x < 0 || p.position().y >= 8 || p.position().y < 0)
        {
            Destroy(p.gameObject);
            return;
        }

        chessboard.pieces.Add(p.position(), p);
    }

    public override void Refresh()
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
}
