using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;
using System;
public class SpeechRecognitionTest : MonoBehaviour
{
    private Board board;
    private DictationRecognizer dictationRecognizer;
    

    void Start()
    {
        
        // Initialize the board reference
        board = FindObjectOfType<Board>();
        
        //  Piece pieceName = board.getPieceByName("Horse");
        //         if (pieceName != null)
        //         {
        //             Debug.Log("this is running");
        //             Coordinate targetSquare = board.convertToCoordinate("a3");
        //             Debug.Log("came back to speechrecog");
        //             Debug.Log(targetSquare.x+" "+targetSquare.y);
        //             Piece pieceToMove=board.FindPieceToMove("Horse", targetSquare);
        //             pieceToMove.movePiece(board.getSquareFromCoordinate(targetSquare));
        //             board.changeTurn();
        //         }
            
        // Initialize DictationRecognizer
       
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationHypothesis += OnDictationHypothesis;
        dictationRecognizer.DictationResult += OnDictationResult;
        StartListening();
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Final Recognized Text: " + text);
        ProcessCommand(text);
    }

    private void OnDictationHypothesis(string text)
    {
         Debug.Log("hypo");
        Debug.Log("Hypothesis Text: " + text);
        ProcessCommand(text);
    }

    private void ProcessCommand(string text)
    {
        Debug.Log("fwehbfe");
        // Normalize the text by trimming and converting to lowercase
        string normalizedText = text.Trim().ToLower();
        
        if (normalizedText == "hello")
        {
            Debug.Log("Hello command received!");
            return; // Exit if "hello" is detected
        }

        // Process movement commands if the game is ongoing
        if (!board.isCheckMate(board.cur_turn))
        {
            string[] words = normalizedText.Split(' ');
            string pieceName;
            if (words.Length >= 4 && words[0] == "move")
            {
                if(words[1]=="on"||words[1]=="On"){
                    pieceName="Pawn";
                }
                else
                 pieceName =char.ToUpper(words[1][0]) + words[1].Substring(1);
                Debug.Log(pieceName+"piece is recieved");
                string destination = words[3];

                // Attempt to move the piece
                //Piece pieceToMove = board.getPieceByName(pieceName);
                if (pieceName != null)
                {
                    Coordinate targetSquare = board.convertToCoordinate(destination);
                    Piece pieceToMove=board.FindPieceToMove(pieceName, targetSquare);
                    pieceToMove.movePiece(board.getSquareFromCoordinate(targetSquare));
                    
                }
                else
                {
                    Debug.Log("Piece not found: " + pieceName);
                }
            }
            else
            {
                Debug.Log("Command not recognized. Try 'move [piece] to [destination]'");
            }
        }
        else
        {
            StopListening();
            Debug.Log("Game Over!");
        }
    }

    private void StartListening()
    {
        dictationRecognizer.Start();
        Debug.Log("Listening for commands...");
        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
    {
        Debug.Log("DictationRecognizer is running and listening for commands...");
    }
    else
    {
        Debug.Log("Failed to start DictationRecognizer.");
    }
    }

    private void StopListening()
    {
        dictationRecognizer.Stop();
        Debug.Log("Stopped listening.");
    }

    void OnDestroy()
    {
        if (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            dictationRecognizer.DictationResult -= OnDictationResult;
            dictationRecognizer.DictationHypothesis -= OnDictationHypothesis;
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
    }
}
