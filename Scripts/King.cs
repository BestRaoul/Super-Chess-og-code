using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public bool shield = true;

    public override List<Vector3Int> MovePositions()
    {
        List<Vector3Int> list = new List<Vector3Int>();

        list.AddRange(GetPositionsInLine(new Vector3Int(1, 0, 0), 2));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1, 0, 0), 2));
        list.AddRange(GetPositionsInLine(new Vector3Int(0, 1, 0), 2));
        list.AddRange(GetPositionsInLine(new Vector3Int(0, -1, 0), 2));
        list.AddRange(GetPositionsInLine(new Vector3Int(1, 1, 0), 2));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1, -1, 0), 2));
        list.AddRange(GetPositionsInLine(new Vector3Int(-1, 1, 0), 2));
        list.AddRange(GetPositionsInLine(new Vector3Int(1, -1, 0), 2));

        return list;
    }

    public override void Die()
    {
        if (shield)
        {
            chessboard.StartCoroutine("RewindTurn", team);
            shield = false;
            return; 
        }

        bool victorious = !(team == chessboard.teamID);
        chessboard.EndGame(victorious);
    }

    public override void InstantDeath()
    {
        if (shield)
        {
            chessboard.StartCoroutine("RewindTurn", team);
            shield = false;
            return;
        }

        bool victorious = !(team == chessboard.teamID);
        chessboard.EndGame(victorious);
    }
}
