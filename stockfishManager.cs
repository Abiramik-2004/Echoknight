using System.Diagnostics;
using UnityEngine;
using System.IO;

public class StockfishManager : MonoBehaviour {
    public static StockfishManager Instance { get; private set; }
    private Process stockfishProcess;
    private StreamWriter inputWriter;
    private StreamReader outputReader;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the instance between scenes
            InitializeStockfish();
        } else {
            Destroy(gameObject);
        }
    }

    private void InitializeStockfish() {
        string stockfishPath =Path.Combine(Application.dataPath, "Plugins/Stockfish/stockfish-windows-x86-64-sse41-popcnt.exe");
        
        stockfishProcess = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = stockfishPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        stockfishProcess.Start();
        inputWriter = stockfishProcess.StandardInput;
        outputReader = stockfishProcess.StandardOutput;

        SendCommand("uci");
    }

    private void SendCommand(string command) {
        inputWriter.WriteLine(command);
        inputWriter.Flush();
    }

    private string ReadResponse() {
        string output = outputReader.ReadLine();
        return output;
    }

    public void SetPosition(string moves) {
        SendCommand($"position startpos moves {moves}");
    }

    public string GetBestMove() {
        SendCommand("go depth 15");
        string line;
        while ((line = ReadResponse()) != null) {
            if (line.StartsWith("bestmove")) {
                return line.Split(' ')[1];
            }
        }
        return null;
    }

    private void OnApplicationQuit() {
        if (stockfishProcess != null && !stockfishProcess.HasExited) {
            stockfishProcess.Kill();
        }
    }
}
