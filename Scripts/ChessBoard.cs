using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChessBoard : NetworkBehaviour
{
    #region Offline

    public Piece selectedPiece;
    public List<SpriteRenderer> selectedCells = new List<SpriteRenderer>();

    public bool debug;
    public bool offline = true;
    public bool myTurn = true;
    public int teamID = 1;

    [HideInInspector] public bool gameEnded = false;

    [Header("Colors")]
    public Color piece_normal;
    public Color piece_selected;

    public Color cell_normal;
    public Color cell_selected;
    public Color cell_opponent;

    public GameObject cellPF;

    public Transform cellParent;
    public Transform pieceParent;
    public EndTurnBoard endTurnBoard;


    [Header("Pieces PFs")]
    public GameObject pawn;
    public GameObject rook;
    public GameObject knight;
    public GameObject bishop;
    public GameObject queen;
    public GameObject king;

    public Dictionary<Vector3Int, Piece> pieces;
    private Dictionary<Vector3Int, SpriteRenderer> cells;

    public List<Piece> OnEndTurnCall;

    //array of moves
    private List<Move> allMoves = new List<Move>();
    private int turn = 0;
    #endregion

    [Header("Networking")]
    public int[] teamIDs;
    private int currentTeamIndex = 0;
    [SyncVar(hook = nameof(StartTurn))] public int currentTeamId = 0;

    public float turnDelay = 1f;

    private void Awake()
    {
        pieces = new Dictionary<Vector3Int, Piece>();
        cells = new Dictionary<Vector3Int, SpriteRenderer>();
        OnEndTurnCall = new List<Piece>();
    }

    void Start()
    {
        SpawnCells(8, 8);
        SpawnPieces();

        //initialize move list

        if (offline) { StartCoroutine(StartGame()); return; }

        if (!isServer)
            teamID = teamIDs[connectionToServer.connectionId + 1];
        else
        {
            teamID = teamIDs[0];
            myTurn = true;
        }
    }

    private void Update()
    {
        //PC
        if (Input.GetMouseButtonDown(0) & myTurn)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3Int clickPos = new Vector3Int((int)mousePos.x / (Screen.width / 8), (int)mousePos.y / (Screen.width / 8), 0);
            if (selectedPiece == null) { SelectPiece(clickPos); }
            else { MovePiece(clickPos); }
        }

        // Mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (Input.touchCount == 1 & myTurn)
            {
                touch = Input.GetTouch(1);

                if (touch.phase == TouchPhase.Began)
                {
                    Vector3 fingerPos = touch.position;
                    Vector3Int clickPos = new Vector3Int((int)fingerPos.x / (Screen.width / 8), (int)fingerPos.y / (Screen.width / 8), 0);
                    if (selectedPiece == null) { SelectPiece(clickPos); }
                    else { MovePiece(clickPos); }
                }
            }
        }
    }

    private King[] SpawnPieces()
    {
        King[] kings = new King[2];

        //whitebois
        SpawnPiece(new Vector3Int(0, 1, 0), pawn, 1);
        SpawnPiece(new Vector3Int(1, 1, 0), pawn, 1);
        SpawnPiece(new Vector3Int(2, 1, 0), pawn, 1);
        SpawnPiece(new Vector3Int(3, 1, 0), pawn, 1);
        SpawnPiece(new Vector3Int(4, 1, 0), pawn, 1);
        SpawnPiece(new Vector3Int(5, 1, 0), pawn, 1);
        SpawnPiece(new Vector3Int(6, 1, 0), pawn, 1);
        SpawnPiece(new Vector3Int(7, 1, 0), pawn, 1);

        SpawnPiece(new Vector3Int(0, 0, 0), rook, 1);
        SpawnPiece(new Vector3Int(1, 0, 0), knight, 1);
        SpawnPiece(new Vector3Int(2, 0, 0), bishop, 1);
        kings[0] = SpawnPiece(new Vector3Int(3, 0, 0), king, 1).gameObject.GetComponent<King>();
        SpawnPiece(new Vector3Int(4, 0, 0), queen, 1);
        SpawnPiece(new Vector3Int(5, 0, 0), bishop, 1);
        SpawnPiece(new Vector3Int(6, 0, 0), knight, 1);
        SpawnPiece(new Vector3Int(7, 0, 0), rook, 1);

        //niggas
        SpawnPiece(new Vector3Int(0, 6, 0), pawn, -1);
        SpawnPiece(new Vector3Int(1, 6, 0), pawn, -1);
        SpawnPiece(new Vector3Int(2, 6, 0), pawn, -1);
        SpawnPiece(new Vector3Int(3, 6, 0), pawn, -1);
        SpawnPiece(new Vector3Int(4, 6, 0), pawn, -1);
        SpawnPiece(new Vector3Int(5, 6, 0), pawn, -1);
        SpawnPiece(new Vector3Int(6, 6, 0), pawn, -1);
        SpawnPiece(new Vector3Int(7, 6, 0), pawn, -1);

        SpawnPiece(new Vector3Int(0, 7, 0), rook, -1);
        SpawnPiece(new Vector3Int(1, 7, 0), knight, -1);
        SpawnPiece(new Vector3Int(2, 7, 0), bishop, -1);
        kings[1] = SpawnPiece(new Vector3Int(3, 7, 0), king, -1).gameObject.GetComponent<King>();
        SpawnPiece(new Vector3Int(4, 7, 0), queen, -1);
        SpawnPiece(new Vector3Int(5, 7, 0), bishop, -1);
        SpawnPiece(new Vector3Int(6, 7, 0), knight, -1);
        SpawnPiece(new Vector3Int(7, 7, 0), rook, -1);

        return kings;
    }
    private void DespawnPieces()
    {
        pieces = new Dictionary<Vector3Int, Piece>();
        Piece[] childPieces = GetComponentsInChildren<Piece>();
        foreach(Piece p in childPieces)
        {
            Destroy(p.gameObject);
        }
    }
    private Piece SpawnPiece(Vector3Int spawnPos, GameObject pf, int teamId)
    {
        Piece newPiece = Instantiate(pf, spawnPos, Quaternion.identity, pieceParent).GetComponent<Piece>();
        newPiece.gameObject.transform.localPosition = spawnPos;
        newPiece.chessboard = this;
        newPiece.sr.color = piece_normal;
        newPiece.team = teamId;
        newPiece.Refresh();
        pieces.Add(spawnPos, newPiece);

        return newPiece;
    }

    private void SpawnCells(int h, int w)
    {
        for (int j = 0; j < h; j++) {
            for (int i = 0; i < w; i++) {
                SpawnCell(new Vector3Int(i, j, 0));
            }
        }
    }
    private void SpawnCell(Vector3Int spawnPos)
    {
        GameObject newCell = Instantiate(cellPF, spawnPos, Quaternion.identity, cellParent);
        newCell.transform.localPosition = spawnPos;
        newCell.GetComponent<SpriteRenderer>().color = cell_normal;
        cells.Add(spawnPos, newCell.GetComponent<SpriteRenderer>()); ;
    }

    public void SelectPiece(Vector3Int selectPos)
    {
        //check if empty tile
        if (!pieces.TryGetValue(selectPos, out selectedPiece)) { Deselect(); return; }
        //check if enemy is on tile
        if (selectedPiece.team != teamID & !debug) { Deselect(); return; }

        selectedPiece.sr.color = piece_selected;
        foreach (Vector3Int cellPos in selectedPiece.MovePositions())
        {
            selectedCells.Add(cells[cellPos]);
            cells[cellPos].color = cell_selected;

            //check if opponenet there
            Piece tryPiece;
            if (pieces.TryGetValue(cellPos, out tryPiece))
            {
                if (tryPiece.team != selectedPiece.team) cells[cellPos].color = cell_opponent;
            }
        }
    }

    public void MovePiece(Vector3Int movePos)
    {
        foreach (Vector3Int cellPos in selectedPiece.MovePositions())
        {
            if (movePos == cellPos)
            {
                if (!offline) { myTurn = false; CmdMove(selectedPiece.position(), movePos); return; } //CALL SEND MOVE MSG TO SERVER

                allMoves.Add(new Move(selectedPiece.position(), movePos));


                selectedPiece.Move(movePos);
                turn += 1;

                EndTurn();

                return;
            }
        }

        Deselect();
        SelectPiece(movePos);
    }

    //TODO: correct move order
    private void EndTurn()
    {
        if (gameEnded) { return; }
        endTurnBoard.PlayEndTurnSFX();

        List<Piece> temporary = new List<Piece>();
        foreach (Piece p in OnEndTurnCall)
        {
            if (p.team != currentTeamId) {
                p.OnEndTurn();
                temporary.Add(p);
            }
        } //Call every OnEndTurn action
        foreach (Piece p in temporary)
        {
            OnEndTurnCall.Remove(p);
        } //remove the called pieces from list

        if (offline)
        {
            teamID = -teamID;
            currentTeamId = teamID;
            StartCoroutine("OfflineDelayedTurn", turnDelay);
        }

        Deselect();
    }

    public void StartTurn(int oldId, int startTeamId)
    {
        if (gameEnded) { return; }

        if (!debug)
            endTurnBoard.PlayAnim(startTeamId);
        if (startTeamId == teamID)
        {
            myTurn = true;
            endTurnBoard.PlayStartTurnSFX();
        }
    }

    public void EndGame(bool victorious)
    {
        Deselect();
        myTurn = false;
        gameEnded = true;
        endTurnBoard.PlayEndGameAnim(victorious);
        StartCoroutine("ReturnToMenu", 20f);
    }

    public Dictionary<Vector3Int, Piece> GetRewindTurn()
    {
        /*
        Dictionary<Vector3Int, Piece> rewindTurn = previousTurns[5];

        previousTurns.RemoveRange(5, 5);

        List<Dictionary<Vector3Int, Piece>> tempo = new List<Dictionary<Vector3Int, Piece>>();
        tempo.Add(previousTurns[0]);
        tempo.Add(previousTurns[0]);
        tempo.Add(previousTurns[0]);
        tempo.Add(previousTurns[0]);
        tempo.Add(previousTurns[0]);

        tempo.AddRange(previousTurns);

        previousTurns = tempo;
        */
        return null;
    }

    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0f);
        endTurnBoard.PlayStartGameSFX();
        if (offline)
        {
            currentTeamId = teamID;
            StartCoroutine("OfflineDelayedTurn", 5f);
        }
    }
    
    IEnumerator ReturnToMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }

    public bool IsEmpty(Vector3Int pos)
    {
        Piece tryPiece;
        return !pieces.TryGetValue(pos, out tryPiece);
    }

    public void Deselect()
    {
        if (selectedPiece != null)
        {
            selectedPiece.sr.color = piece_normal;
            selectedPiece = null;
        }

        foreach (SpriteRenderer spriteRenderer in selectedCells)
        {
            spriteRenderer.color = cell_normal;
        }
        selectedCells = new List<SpriteRenderer>();
    }

    IEnumerator OfflineDelayedTurn(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartTurn(0, teamID);
    }

    public IEnumerator RewindTurn(int killedKingteam)
    {
        yield return new WaitForSeconds(0f);
        //playAnimationHere

        UpdateBoard(turn - 4, killedKingteam);
    }

    void UpdateBoard(int newTurn, int killedKingteam)
    {
        allMoves.RemoveRange(newTurn, turn - newTurn);
        turn = newTurn;

        DespawnPieces();

        
        King[] kings = SpawnPieces();

        if (kings[0].team == killedKingteam) kings[0].shield = false;
        if (kings[1].team == killedKingteam) kings[1].shield = false;

        for (int i = 0; i < turn; i++)
        {
            Piece pieceMoved = pieces[allMoves[i].from];
            pieceMoved.InstantMove(allMoves[i].to);
        }
    }

    /// <summary>
    /// Multiplayer only side
    /// </summary>


    public int GetPieceTeamId(Vector3Int pos)
    {
        Piece tryPiece;
        if (pieces.TryGetValue(pos, out tryPiece)) return tryPiece.team;
        return 0;
    }

    public bool ValidateMove(Vector3Int from, Vector3Int to)
    {
        Piece tryPiece;
        if (!pieces.TryGetValue(from, out tryPiece)) return false;

        foreach (Vector3Int cellPos in tryPiece.MovePositions())
        {
            if (to == cellPos)
            {
                return true;
            }
        }

        return false;
    }

    public void MovePiece(Vector3Int from, Vector3Int to)
    {
        allMoves.Add(new Move(from, to));
        turn += 1;

        Piece pieceMoved = pieces[from];
        pieceMoved.Move(to);


        EndTurn();
    }

    #region TryNetworking HERELUL

    public override void OnStartServer()
    {
        if (NetworkServer.connections.Count == teamIDs.Length) // begin game
        {
            currentTeamId = teamIDs[0];
            print("Game Started!!!");
        }
    }

    [Command(ignoreAuthority = true)]
    void CmdMove(Vector3Int from, Vector3Int to)
    {
        //Validating move

        //check if networkConnection is the correct one
        //if (teamIDs[connectionToClient.connectionId] != currentTeamId) { print("Cheater changed his teamID"); return; }
        // check if piece is team's
        if (currentTeamId != GetPieceTeamId(from)) { print("Invalid Piece."); return; }
        // check if move is valid
        if (!ValidateMove(from, to)) { print("Invalid Move."); return; }

        //Sending Moves to connections
        RpcApplyMove(from, to);

        StartCoroutine(NextTurn());
    }

    [ClientRpc] public void RpcApplyMove(Vector3Int from, Vector3Int to) => MovePiece(from, to);

    private IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(turnDelay);
        nextTeam();
    }

    private int nextTeam()
    {
        currentTeamIndex += 1;
        if (currentTeamIndex == teamIDs.Length)
            currentTeamIndex = 0;

        currentTeamId = teamIDs[currentTeamIndex];

        return currentTeamIndex;
    }
    #endregion
}

public class Move
{
    public Vector3Int from;
    public Vector3Int to;

    public Move(Vector3Int vec1, Vector3Int vec2)
    {
        from = vec1;
        to = vec2;
    }
}

