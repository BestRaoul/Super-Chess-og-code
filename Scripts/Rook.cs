using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public override List<Vector3Int> MovePositions()
    {
        List<Vector3Int> list = new List<Vector3Int>();

        list.AddRange(GetPositionsInLine(new Vector3Int(1, 0, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1, 0, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(0, 1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(0, -1, 0)));

        return list;
    }
}
