using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class StockfishAI : MonoBehaviour
{
    private Process stockfishProcess;
    private StreamWriter stockfishInput;
    private StreamReader stockfishOutput;
 
    void Start()
    {
        StartStockfish();
    }

    void OnApplicationQuit()
    {
        StopStockfish();
    }

    private void StartStockfish()
    {
        // Set up the process to start Stockfish
        UnityEngine.Debug.Log("stockfish is running");
        stockfishProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                // Path to Stockfish executable
                FileName = Path.Combine(Application.dataPath, "Plugins/Stockfish/stockfish-windows-x86-64-sse41-popcnt.exe"),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        // Start the process
        stockfishProcess.Start();
        stockfishInput = stockfishProcess.StandardInput;
        stockfishOutput = stockfishProcess.StandardOutput;

        // Initialize Stockfish with UCI
        stockfishInput.WriteLine("uci");
    }

    private void StopStockfish()
    {
        if (stockfishProcess != null && !stockfishProcess.HasExited)
        {
            stockfishInput.WriteLine("quit"); // Close Stockfish gracefully
            stockfishProcess.Kill();
        }
    }

    public async Task<string> GetBestMove(string fen)
    {
        UnityEngine.Debug.Log("In stockfishAI");
        // Send the position in FEN format
        stockfishInput.WriteLine("position fen " + fen);
        
        // Ask Stockfish to calculate the best move (adjust time if needed)
        stockfishInput.WriteLine("go movetime 1000");

        // Wait for Stockfish to respond with the best move
        string output;
        do
        {
            output = await stockfishOutput.ReadLineAsync();
        } while (!output.StartsWith("bestmove"));

        // Parse the best move
         UnityEngine.Debug.Log(output);
        return output.Split(' ')[1];
       
    }
}
