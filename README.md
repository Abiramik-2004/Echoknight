# ECHOKNIGHT

ECHOKNIGHT is an advanced 3D chess application that reimagines the traditional game by combining voice recognition, artificial intelligence, and immersive visual design. The primary goal of this project is to make chess more accessible and inclusive for a wide range of users, including visually impaired individuals, children, and those with physical disabilities.

Unlike conventional chess applications that rely solely on manual interaction, ECHOKNIGHT introduces voice-controlled gameplay, allowing users to perform moves through simple spoken commands. This not only improves accessibility but also enhances the overall user experience by making gameplay more interactive and engaging.



## Project Overview

The project focuses on creating a user-friendly and accessible chess platform that integrates multiple modern technologies. It combines a powerful chess engine for intelligent gameplay, a voice recognition system for hands-free control, and a visually rich 3D environment for an immersive experience.

ECHOKNIGHT is designed not just as a game, but as an assistive tool that promotes cognitive development, independent interaction, and equal access to digital entertainment.



## Key Features

### Voice-Controlled Gameplay
The application allows users to control the game using voice commands. Players can make moves by speaking instructions such as “Move pawn to e5.” The system processes the input, identifies the piece and destination, and executes the move accordingly.

### AI-Based Opponent
ECHOKNIGHT integrates the Stockfish chess engine, which is known for its strong gameplay and accuracy. The AI analyzes the board state and generates optimal moves, providing a challenging experience for users at different skill levels.

### Multiple Difficulty Levels
The application supports different difficulty settings, allowing beginners to learn the game gradually while also offering advanced challenges for experienced players.

### 3D Visual Environment
The chessboard and pieces are designed in a 3D environment using Unity and Blender. This creates a realistic and visually appealing interface that enhances user engagement.

### Accessibility-Focused Design
A key aspect of the project is inclusivity. The system is designed to support users with visual or physical limitations by minimizing the need for manual interaction and enabling voice-based control.



## Technical Stack

- Unity (C#) for application development and game logic  
- Blender for designing and modeling 3D chess pieces and assets  
- Stockfish engine for AI-based move calculation  
- Windows DictationRecognizer for speech-to-text processing  



## System Workflow

1. The user launches the application and starts a new game.  
2. The user can choose between voice input or manual controls.  
3. When a voice command is given, the system captures the audio input.  
4. The speech is converted into text using the voice recognition module.  
5. The system extracts key details such as the piece and destination.  
6. The move is validated based on chess rules and executed on the board.  
7. The current board state is converted into FEN notation.  
8. The FEN string is passed to the Stockfish engine.  
9. The AI processes the position and returns the best possible move.  
10. The system updates the board with the AI’s move and continues the game.



## Screenshots

<p align="center">
  <img src="https://github.com/user-attachments/assets/46ba17b7-4d6c-4b1a-a614-f18a53afc89a" width="600"/>
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/a4b541d7-04db-4353-a919-40840ac6d9b6" width="600"/>
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/451c7855-20bb-4fa5-bd33-8e4df1244513" width="600"/>
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/5962b7da-ffc6-4855-819b-51b14747b1f8" width="600"/>
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/7cf96345-a413-4e63-9649-4e0692499ab7" width="600"/>
</p>



## Advantages

- Provides hands-free interaction using voice commands  
- Improves accessibility for visually impaired and physically challenged users  
- Offers intelligent gameplay using a powerful AI engine  
- Enhances user experience with 3D visualization  
- Encourages learning and skill development in chess  



## Future Enhancements

- Implementation of save and resume functionality using a database  
- Addition of audio feedback for moves and game events  
- Development of multiplayer mode for online gameplay  
- Integration of player performance tracking and analytics  
- Adaptive difficulty system that adjusts based on user skill level  


## Conclusion

ECHOKNIGHT is a step toward making digital gaming more inclusive and accessible. By combining voice recognition, artificial intelligence, and immersive design, the application provides a unique and meaningful chess experience. It enables users to interact with the game in a natural way, promotes independence, and ensures that everyone can enjoy the strategic depth of chess regardless of physical limitations.
