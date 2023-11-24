using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    public override List<Vector3Int> MovePositions()
    {
        List<Vector3Int> list = new List<Vector3Int>();

        list.AddRange(GetPositionsInLine(new Vector3Int( 1,  0, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1,  0, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int( 0,  1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int( 0, -1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int( 1,  1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1, -1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1,  1, 0)));
        list.AddRange(GetPositionsInLine(new Vector3Int( 1, -1, 0)));


        return list;
    }

    public override void Die()
    {
        chessboard.pieces.Remove(position());
        sr.enabled = false;

        StartCoroutine("LAZER", new Vector3Int( 1,  0, 0));
        StartCoroutine("LAZER", new Vector3Int( 1,  1, 0));
        StartCoroutine("LAZER", new Vector3Int( 0,  1, 0));
        StartCoroutine("LAZER", new Vector3Int(-1,  1, 0));
        StartCoroutine("LAZER", new Vector3Int(-1,  0, 0));
        StartCoroutine("LAZER", new Vector3Int(-1, -1, 0));
        StartCoroutine("LAZER", new Vector3Int( 0, -1, 0));
        StartCoroutine("LAZER", new Vector3Int( 1, -1, 0));

        Destroy(gameObject, 3f);
    }

    public override void InstantDeath()
    {
        chessboard.pieces.Remove(position());
        sr.enabled = false;

        StartCoroutine("InstantLAZER", new Vector3Int(1, 0, 0));
        StartCoroutine("InstantLAZER", new Vector3Int(1, 1, 0));
        StartCoroutine("InstantLAZER", new Vector3Int(0, 1, 0));
        StartCoroutine("InstantLAZER", new Vector3Int(-1, 1, 0));
        StartCoroutine("InstantLAZER", new Vector3Int(-1, 0, 0));
        StartCoroutine("InstantLAZER", new Vector3Int(-1, -1, 0));
        StartCoroutine("InstantLAZER", new Vector3Int(0, -1, 0));
        StartCoroutine("InstantLAZER", new Vector3Int(1, -1, 0));

        Destroy(gameObject);
    }
}
