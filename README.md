# Multiplayer Tic-Tac-Toe with Photon Networking

A Unity-based multiplayer Tic-Tac-Toe game that implements robust reconnection handling and turn-based gameplay using Photon Unity Networking (PUN). This project serves as a multiplayer instance of the classic Tic-Tac-Toe game.

## Features

### Networking & Reconnection
- Automatic room joining and creation
- Robust reconnection system with multiple attempts
- Graceful handling of disconnections
- Room state preservation during reconnection
- Timeout mechanism for failed reconnection attempts
- Parallel synchronization of game state across multiple instances

### Gameplay
- 3x3 grid-based Tic-Tac-Toe board
- Turn-based gameplay system
- Move validation
- Win condition detection
- Real-time turn switching between players
- Synchronized game state across all connected clients

### User Interface
- Clean and intuitive UI design
- Player name input with validation
- Loading states with visual feedback
- Game status display
- Turn indicator
- Win/lose messages

## Technical Implementation

### Core Components

#### ConnectionManager
- Handles all Photon networking operations
- Manages room joining/leaving
- Implements reconnection logic
- Handles player connections/disconnections
- Controls game state based on player count
- Ensures parallel synchronization of game state

#### UIManager
- Manages all UI elements and their interactions
- Handles panel visibility and transitions
- Validates player input
- Provides visual feedback for game states
- Manages the game board UI
- Updates UI elements based on synchronized game state

#### GameStateController
- Controls game logic and state
- Manages turn system
- Validates moves
- Checks win conditions
- Synchronizes game state across network
- Handles parallel state updates

### Key Features Implementation

#### Reconnection System
```csharp
- Automatic detection of disconnections
- Multiple reconnection attempts with configurable timeout
- Room state preservation
- Fallback to main menu after failed attempts
```

#### Turn-Based System
```csharp
- Turn validation
- Move validation
- Turn switching logic
- Win condition checking
```

#### Parallel Synchronization
```csharp
- Real-time state synchronization across all clients
- Automatic room state preservation
- Synchronized turn management
- Parallel move validation
- Instant UI updates across all instances
```

## Setup Instructions

1. Clone the repository
2. Open the project in Unity (2021.3 or later)
3. Import Photon Unity Networking from the Asset Store
4. Configure your Photon App ID in the PhotonServerSettings
5. Build and run the project

## Project Structure

```
Assets/
├── Scripts/
│   ├── ConnectionManager.cs      # Handles networking and reconnection
│   ├── UIManager.cs             # Manages UI elements and interactions
│   ├── GameStateController.cs   # Controls game logic and state

```

## Requirements

- Unity 2022.3.38f1
- Photon Unity Networking (PUN)
- TextMeshPro package
- .NET Framework 4.x

## Dependencies

- Photon Unity Networking (PUN)
- TextMeshPro
- Unity UI system

## Usage

1. Launch the game
2. Enter your player name
3. Click "Play" to join a game
4. Wait for an opponent to join
5. Play Tic-Tac-Toe with turn-based moves
6. Game automatically handles disconnections and reconnections
7. All game instances stay synchronized in real-time

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Photon Unity Networking team for the networking solution
- Unity Technologies for the game engine
