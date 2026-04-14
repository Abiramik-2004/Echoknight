using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

/*
==============================
[Board] - Main script, controls the game
==============================
*/
public class Board : MonoBehaviour {
    private List<Square> hovered_squares = new List<Square>(); // List squares to hover
    private Square closest_square; // Current closest square when dragging a piece
    private int cur_theme = 0;
public StockfishAI stockfishAI;
    public int cur_turn = -1; // -1 = whites; 1 = blacks
    public Dictionary<int, Piece> checking_pieces = new Dictionary<int, Piece>(); // Which piece is checking the king (key = team)
    // Whites = -1, Blacks = 1
    // UI variables
    public bool use_hover; // Hover valid moves & closest square
    public bool rotate_camera; // Enable/disable camera rotation

    [SerializeField]
    MainCamera main_camera;

    [SerializeField]
    Material square_hover_mat; // Piece's valid squares material

    [SerializeField]
    Material square_closest_mat; // Piece's closest square material

    [SerializeField]
    GameObject win_msg;

    [SerializeField]
    TextMesh win_txt;

    [SerializeField]
    List<Theme> themes = new List<Theme>();

    [SerializeField]
    List<Renderer> board_sides = new List<Renderer>();

    [SerializeField]
    List<Renderer> board_corners = new List<Renderer>();

    [SerializeField]
    List<Square> squares = new List<Square>(); // List of all game squares (64) - ordered

    [SerializeField]
    List<Piece> pieces = new List<Piece>(); // List of all pieces in the game (32)

    void Start() {
        setBoardTheme();
        addSquareCoordinates();
        
        setStartPiecesCoor(); // Update all piece's coordinate
        
       // LogBoardSetup();
    }
// Call this function after setting up the board
// public void LogBoardSetup() {
//      foreach (var square in squares) {
//         square.DisplayPieceInfo();
//     }
// }
public Piece FindPieceToMove(string pieceName, Coordinate targetSquare)
{
    // Get all pieces from the board
    List<Piece> pieces = GetAllPieces(); // Assuming you have a method that retrieves all pieces

    // Iterate through all pieces
    foreach (Piece piece in pieces)
    {
        // Check if the piece matches the requested piece name
         if (piece.piece_name.ToLower().Equals(pieceName.ToLower()))
         {//Debug.Log(piece.piece_name+"heie"+pieceName);
            // Check if this piece can legally move to the target square
            if (piece.checkValidMove(getSquareFromCoordinate(targetSquare)))
            {
                return piece; // Return the first matching piece that can move
            }
        }
    }
    return null; // Return null if no matching piece can move
}
public List<Piece> GetAllPieces()
{
    // Assuming you have a list of pieces stored in the board
    return pieces; // Return the list of pieces
}

//Get pieces by name
public Piece getPieceByName(string name) {
    Debug.Log("Piece"+name+"is received");
    return pieces.FirstOrDefault(p => p.piece_name == name);
}

public Coordinate convertToCoordinate(string notation) {
    if (notation.Length != 2) {
        Debug.LogError("Invalid notation format.");
        return new Coordinate(-1, -1); // Return an invalid coordinate
    }
    // Example: "e5" -> x = 4, y = 4 (since 'a' = 0 and '1' = 0)
    Debug.Log("Coordinate " + notation + " is received");
    
    int x = notation[0] - 'a';
    Debug.Log("x: " + x);
    if (x < 0 || x > 7) {
        Debug.LogError("Invalid file: " + notation[0]);
        return new Coordinate(-1, -1); // Return an invalid coordinate
    }
    int y = 8 - int.Parse(notation[1].ToString()); // Flip the y-coordinate
     if (y < 0 || y > 7) {
        Debug.LogError("Invalid rank: " + notation[1]);
        return new Coordinate(-1, -1); // Return an invalid coordinate
    }
    Debug.Log("y: " + y);
    
    return new Coordinate(x, y);
}



    /*
    ---------------
    Squares related functions+
    ---------------
    */ 
    // Returns closest square to the given position
    public Square getClosestSquare(Vector3 pos) {
        Square square = squares[0];
        float closest = Vector3.Distance(pos, squares[0].coor.pos);

        for (int i = 0; i < squares.Count ; i++) {
            float distance = Vector3.Distance(pos, squares[i].coor.pos);

            if (distance < closest) {
                square = squares[i];
                closest = distance;
            }
        }
        return square;
    }

    // Returns the square that is at the given coordinate (local position in the board)
    //change if error, original.
    // public Square getSquareFromCoordinate(Coordinate coor) {
    //     Square square = squares[0];
    //     for (int i = 0; i < squares.Count ; i++) {
    //         if (squares[i].coor.x == coor.x && squares[i].coor.y == coor.y) {
    //             return squares[i];
    //         }
    //     }
    //     return square;
    // }
    


 public Square getSquareFromCoordinate(Coordinate coor) {
        Square square = squares[0];
        for (int i = 0; i < squares.Count ; i++) {
            if (squares[i].coor.x == coor.x && squares[i].coor.y == coor.y) {
                return squares[i];
            }
        }
        return square;
    }
    // Hover piece's closest square
    public void hoverClosestSquare(Square square) {
        if (closest_square) closest_square.unHoverSquare();
        square.hoverSquare(themes[cur_theme].square_closest);
        closest_square = square;
    }

    // Hover all the piece's allowed moves squares
    public void hoverValidSquares(Piece piece) {
        addPieceBreakPoints(piece);
        for (int i = 0; i < squares.Count ; i++) {
            if (piece.checkValidMove(squares[i])) {
                squares[i].hoverSquare(themes[cur_theme].square_hover);
                hovered_squares.Add(squares[i]);
            }
        }
    }

    // Once the piece is dropped, reset all squares materials to the default
    public void resetHoveredSquares() {
        for (int i = 0; i < hovered_squares.Count ; i++) {
            hovered_squares[i].resetMaterial();
        }
        hovered_squares.Clear();
        closest_square.resetMaterial();
        closest_square = null;
    }

    // If the king is trying to castle with a tower, we'll check if an enemy piece is targeting any square
    // between the king and the castling tower
    public bool checkCastlingSquares(Square square1, Square square2, int castling_team) {
        List<Square> castling_squares = new List<Square>();

        if (square1.coor.x < square2.coor.x) {
            for (int i = square1.coor.x; i < square2.coor.x; i++) {
                Coordinate coor = new Coordinate(i, square1.coor.y);
                castling_squares.Add(getSquareFromCoordinate(coor));
            }
        }
        else {
            for (int i = square1.coor.x; i > square2.coor.x; i--) {
                Coordinate coor = new Coordinate(i, square1.coor.y);
                castling_squares.Add(getSquareFromCoordinate(coor));
            }
        }
        for (int i = 0; i < pieces.Count; i++) {
            if (pieces[i].team != castling_team) {
                addPieceBreakPoints(pieces[i]);
                for (int j = 0; j < castling_squares.Count; j++) {
                    if (pieces[i].checkValidMove(castling_squares[j])) return false;
                }
            }
        }
        
        return true;
    }

    // Set start square's local coordinates & its current position
    private void addSquareCoordinates() {
        int coor_x = 0;
        int coor_y = 0;
        for (int i = 0; i < squares.Count ; i++) {
            squares[i].coor = new Coordinate(coor_x, coor_y);
            squares[i].coor.pos = new Vector3(squares[i].transform.position.x - 0.5f, squares[i].transform.position.y, squares[i].transform.position.z - 0.5f);
            if (squares[i].team == -1) squares[i].GetComponent<Renderer>().material = themes[cur_theme].square_white;
            else if (squares[i].team == 1) squares[i].GetComponent<Renderer>().material = themes[cur_theme].square_black;
            squares[i].start_mat = squares[i].GetComponent<Renderer>().material;

            if (coor_y > 0 && coor_y % 7 == 0) {
                coor_x++;
                coor_y = 0;
            }
            else {
                coor_y++;
            }
        }
    }

    /*
    ---------------
    Pieces related functions
    ---------------
    */ 
    // Add pieces squares that are breaking the given piece's allowed positions
    public void addPieceBreakPoints(Piece piece) {
        piece.break_points.Clear();
        for (int i = 0; i < squares.Count ; i++) {
            piece.addBreakPoint(squares[i]);
        }
    }

    // Check if the king's given team is in check
    public bool isCheckKing(int team) {
        Piece king = getKingPiece(team);

        for (int i = 0; i < pieces.Count; i++) {
            if (pieces[i].team != king.team) {
                addPieceBreakPoints(pieces[i]);
                if (pieces[i].checkValidMove(king.cur_square)) {
                    checking_pieces[team] = pieces[i];
                    return true;
                } 
            }
        }
        return false;
    }

    // Check if the given team lost
    public bool isCheckMate(int team) {
        if (isCheckKing(team)) {
            int valid_moves = 0;

            for (int i = 0; i < squares.Count ; i++) {
                for (int j = 0; j < pieces.Count; j++) {
                    if (pieces[j].team == team) {
                        if (pieces[j].checkValidMove(squares[i])) {
                            valid_moves++;
                        }
                    }
                }
            }

            if (valid_moves == 0) {
                return true;
            }
        }
        return false;
    }

    // Get king's given team
    public Piece getKingPiece(int team) {
        for (int i = 0; i < pieces.Count; i++) {
            if (pieces[i].team == team && pieces[i].piece_name == "King") {
                return pieces[i];
            }
        }
        return pieces[0];
    }

    // Remove the given piece from the pieces list
    public void destroyPiece(Piece piece) {
        pieces.Remove(piece);
    }

    // Update each piece's coordinates getting the closest square
    private void setStartPiecesCoor() {
        for (int i = 0; i < pieces.Count ; i++) {
            Square closest_square = getClosestSquare(pieces[i].transform.position);
            closest_square.holdPiece(pieces[i]);
            pieces[i].setStartSquare(closest_square);
            pieces[i].board = this;
            if (pieces[i].team == -1) setPieceTheme(pieces[i].transform, themes[cur_theme].piece_white);
            else if (pieces[i].team == 1) setPieceTheme(pieces[i].transform, themes[cur_theme].piece_black);
        }
    }

    private void setPieceTheme(Transform piece_tr, Material mat) {
        for (int i = 0; i < piece_tr.childCount; ++i) {
            Transform child = piece_tr.GetChild(i);
            try {
                child.GetComponent<Renderer>().material = mat;
            }
            catch (Exception e) {
                for (int j = 0; j < child.childCount; ++j) {
                    Transform child2 = child.GetChild(j);
                    child2.GetComponent<Renderer>().material = mat;
                }
            }
        }
    }

    /*
    ---------------
    Game related functions
    ---------------
    */ 
    // Change current turn, we check if a team lost before rotating the camera
   public async Task changeTurn()
{
    cur_turn = (cur_turn == -1) ? 1 : -1;

    if (isCheckMate(cur_turn))
    {
        doCheckMate(cur_turn);
    }
    else if (rotate_camera)
    {
        main_camera.changeTeam(cur_turn);
    }

    if (cur_turn == 1)
    {Debug.Log("AI turn");
        string fen = GetFENFromBoard(); // Convert the board state to FEN format
        
        string aiMove = await stockfishAI.GetBestMove(fen); // Await the result of the async method
        ApplyAIMove(aiMove);
       cur_turn=-1;
       return;
        
    }
}
private string GetFENFromBoard()
{
    string fen = "";
    for (int x = 0; x<8; x++) // Loop through rows (7 to 0 for top-down)
    {
        int emptyCount = 0; // Count consecutive empty squares in a row
        for (int y = 0; y < 8; y++) // Loop through columns (0 to 7 for left-right)
        {
            Square square = squares[y * 8 + x]; // Assuming squares is a list of Square objects
            if (square.holding_piece != null) // If there's a piece on this square
            {
                if (emptyCount > 0)
                {
                    fen += emptyCount; // If there were empty squares, add their count
                    emptyCount = 0;
                }

                string pieceName = square.holding_piece.piece_name;

                // Now using proper piece naming
                if (pieceName.StartsWith("Tower"))
                    pieceName = "R"; // Convert Tower to Rook notation
                else if (pieceName.StartsWith("Pawn"))
                    pieceName = "P"; // Pawn
                else if (pieceName.StartsWith("Horse"))
                    pieceName = "N"; // Knight
                else if (pieceName.StartsWith("Bishop"))
                    pieceName = "B"; // Bishop
                else if (pieceName.StartsWith("Queen"))
                    pieceName = "Q"; // Queen
                else if (pieceName.StartsWith("King"))
                    pieceName = "K"; // King

                // Add the piece character (uppercase for white, lowercase for black)
                if (square.holding_piece.team == -1) // White piece
                {
                    fen += pieceName.ToUpper(); 
                }
                else // Black piece
                {
                    fen += pieceName.ToLower();
                }
            }
            else
            {
                emptyCount++; // If there's no piece, count the empty square
            }
        }

        // If there were empty squares, add the count at the end of the row
        if (emptyCount > 0)
        {
            fen += emptyCount;
        }

        // Add slash between rows
        if (x <7) fen += "/"; 
    }

    // Add additional FEN components like active color, castling rights, en passant, etc.
    fen += " b KQkq - 0 1"; // Example FEN footer (active color, castling rights, en passant, half-move, full-move)

    return fen;
}


    // Apply the AI's move to the board
   private void ApplyAIMove(string move)
{
    if (move.Length < 4) return;

    string from = move.Substring(0, 2); // e.g., "e2"
    string to = move.Substring(2, 2);   // e.g., "e4"

    Coordinate fromCoor = convertToCoordinate(from);
    Coordinate toCoor = convertToCoordinate(to);

    Square fromSquare = getSquareFromCoordinate(fromCoor);
    Square toSquare = getSquareFromCoordinate(toCoor);

    if (fromSquare != null && toSquare != null)
    {
        Piece movingPiece = fromSquare.holding_piece;
        if (movingPiece != null)
        {
            // Use the movePiece method from the Piece class
            movingPiece.movePiece(toSquare);

            // Optionally, clear the starting square if necessary
           // fromSquare.clearPiece();

            Debug.Log($"Moved {movingPiece.piece_name} from {from} to {to}");
        }
        else
        {
            Debug.LogError("No piece found at the starting square.");
        }
    }
    else
    {
        Debug.LogError("Invalid from or to square coordinates.");
    }
}


    // Show check mate message
    public void doCheckMate(int loser) {
        string winner = (loser == 1) ? "White" : "Black";

        win_txt.text = winner + win_txt.text;
        int txt_rotation = (cur_turn == -1) ? 0 : 180;

        win_msg.transform.rotation = Quaternion.Euler(0, txt_rotation, 0);
        win_msg.GetComponent<Rigidbody>().useGravity = true;
    }

    /*
    ---------------
    User Interface related functions
    ---------------
    */ 
    public void useHover(bool use) {
        use_hover = use;
    }

    public void rotateCamera(bool rotate) {
        rotate_camera = rotate;
    }

    public void setBoardTheme() {
        for (int i = 0; i < board_sides.Count ; i++) {
            board_sides[i].material = themes[cur_theme].board_side;
            board_corners[i].material = themes[cur_theme].board_corner;
        }
    }

    public void updateGameTheme(int theme) {
        cur_theme = theme;
        setBoardTheme();
        for (int i = 0; i < pieces.Count ; i++) {
            if (pieces[i].team == -1) setPieceTheme(pieces[i].transform, themes[cur_theme].piece_white);
            else if (pieces[i].team == 1) setPieceTheme(pieces[i].transform, themes[cur_theme].piece_black);
        }
        for (int i = 0; i < squares.Count ; i++) {
            if (squares[i].team == -1) squares[i].GetComponent<Renderer>().material = themes[cur_theme].square_white;
            else if (squares[i].team == 1) squares[i].GetComponent<Renderer>().material = themes[cur_theme].square_black;
            squares[i].start_mat = squares[i].GetComponent<Renderer>().material;
        }
    }
    
}



 // Add "local" coordinates to all squares
        //