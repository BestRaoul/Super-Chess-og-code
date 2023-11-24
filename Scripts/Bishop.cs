using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Vector3Int> MovePositions()
    {
        List<Vector3Int> list = new List<Vector3Int>();

        list.AddRange(GetPositionsInLine(new Vector3Int(1, 1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1, -1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1, 1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(1, -1, 0)));


        return list;
    }

    public override void Move(Vector3Int pos)
    {
        StartCoroutine("LAZER",pos - position());
        base.Move(pos);
    }

    public override void InstantMove(Vector3Int pos)
    {
        StartCoroutine("InstantLAZER", pos - position());
        base.InstantMove(pos);
    }
}
