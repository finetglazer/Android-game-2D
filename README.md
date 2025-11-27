# ⚔️ Multiplayer 2D Action RPG

A feature-rich 2D action game developed in **Unity** utilizing **C#** for core logic and **Photon (PUN)** for real-time multiplayer networking. This project demonstrates complex game architecture, bridging client-side gameplay with backend services for data persistence and social features.

## 🚀 Key Features

### 🎮 Gameplay Mechanics
* **Dynamic Combat:** Implements attack handlers, projectile systems (Arrows), and health management.
* **Player Movement:** Advanced movement systems including sliding (`PlayerSlide.cs`) and standard platformer controls.
* **Character Classes:** Distinct logic for various entities including Soldier, Thief, Princess, Priest, Peasant, and Merchant.
* **Environmental Hazards:** Interactive elements like Meteors, Fire, and Water zones.
* **Boss System:** Includes boss logic and enemy respawn systems (`BossAndEnemiesRespawner.cs`).

### 🌐 Multiplayer & Networking
* **Real-time Sync:** Powered by **Photon Unity Networking (PUN2)** for seamless player synchronization.
* **In-Game Chat:** Fully functional chat system allowing communication between players in rooms (`PhotonChat`).
* **Lobby System:** Room management and matchmaking capabilities.

### 💾 Backend Integration & Data
* **API Integration:** Connects to an external backend using HTTP requests.
* **Leaderboards:** Fetches and displays global and friend rankings (`LeaderboardManager.cs`).
* **Match History:** Tracks Solo and Team match history (`MatchHistoryManager.cs`).
* **Authentication:** Handles user data, password changes, and profile management.
* **Data Models:** Structured architecture using strictly typed Models, Payloads, and Responses.

## 🛠️ Tech Stack

* **Engine:** Unity 2D (Target Platform: Android/Mobile)
* **Language:** C#
* **Networking:** Photon Unity Networking 2 (PUN2) & Photon Chat
* **Input:** Unity Input System (`InputActions`)
* **Architecture:** Component-based with separation of concerns (Managers, Handlers, Models).
  
## 🔧 Installation & Setup

1. **Clone the repository**
   ```bash
   git clone [https://github.com/yourusername/Android-game-2D.git](https://github.com/yourusername/Android-game-2D.git)
