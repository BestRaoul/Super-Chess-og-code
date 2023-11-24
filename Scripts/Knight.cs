using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    [SerializeField] SpriteRenderer cross;

    private Vector3Int endTurnMovePos;

    public override List<Vector3Int> MovePositions()
    {
        List<Vector3Int> list = new List<Vector3Int>();

        if (InBounds(position() + new Vector3Int(2, 1, 0))) list.Add(position() + new Vector3Int(2, 1, 0));
        if (InBounds(position() + new Vector3Int(-2, 1, 0))) list.Add(position() + new Vector3Int(-2, 1, 0));
        if (InBounds(position() + new Vector3Int(1, 2, 0))) list.Add(position() + new Vector3Int(1, 2, 0));
        if (InBounds(position() + new Vector3Int(-1, 2, 0))) list.Add(position() + new Vector3Int(-1, 2, 0));
        if (InBounds(position() + new Vector3Int(2, -1, 0))) list.Add(position() + new Vector3Int(2, -1, 0));
        if (InBounds(position() + new Vector3Int(-2, -1, 0))) list.Add(position() + new Vector3Int(-2, -1, 0));
        if (InBounds(position() + new Vector3Int(1, -2, 0))) list.Add(position() + new Vector3Int(1, -2, 0));
        if (InBounds(position() + new Vector3Int(-1, -2, 0))) list.Add(position() + new Vector3Int(-1, -2, 0));

        return list;
    }

    public override void Move(Vector3Int pos)
    {
        chessboard.OnEndTurnCall.Add(this);

        chessboard.pieces.Remove(position());
        endTurnMovePos = pos;
        sr.enabled = false;
        cross.enabled = true;
        cross.transform.localPosition = pos - position();
    }

    public override void InstantMove(Vector3Int pos)
    {
        chessboard.OnEndTurnCall.Add(this);

        chessboard.pieces.Remove(position());
        endTurnMovePos = pos;
        sr.enabled = false;
        cross.enabled = true;
        cross.transform.localPosition = pos - position();
    }

    public override void OnEndTurn()
    {
        Piece tryPiece;
        if (chessboard.pieces.TryGetValue(endTurnMovePos, out tryPiece)) { tryPiece.Die(); }

        sr.enabled = true;
        cross.enabled = false;
        gameObject.transform.localPosition = endTurnMovePos;
        audioSource.Play();
        chessboard.pieces.Add(position(), this);

        chessboard.Deselect();
    }
}
