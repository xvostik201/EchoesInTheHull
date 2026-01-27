# Project: Echoes of the Void

A technical demo showcasing interaction systems and level architecture. The project focuses on creating an atmospheric environment with deep integration of station life-support systems.

---

## 🛠 Technical Systems

### 1. Power Management System
A centralized control system for power grids, allowing management of the state of all active objects within the level.
* **Script:** `PowerManager.cs`
* **Functionality:** System reboots, forced blast door overrides, and global `_isPowerWorking` state monitoring.

### 2. Interactive Door System
Automatic sliding doors with local coordinate support for modular deployment.
* **Features:** Smooth animations powered by `DOTween`, optimization via `TryGetComponent`, and trigger spam protection using `DOKill`.
* **Logic:** Power-grid dependency. Doors automatically lock in a secure (closed) state during power outages.

---

## 🎒 Equipment

### Player Gear
* **Flashlight:** A dynamic light source with high-fidelity shadows, designed to build tension and atmosphere.
* **Tablet:** The player's mobile terminal. In the current iteration, it serves as a base for system logs and map data.

![Flashlight](Assets/Documentation/Screenshots/Equipment/Flashlight.png)
![Tablet](Assets/Documentation/Screenshots/Equipment/Tablet0.png)
*Demonstration of lighting effects and the in-hand tablet model.*

---

## 🗺 Map Layout

The level is designed using a modular principle, allowing for flexible player logistics and precise power-zone management.

### Global Schematic (Top-Down View)
![Map Layout Top](Assets/Documentation/Screenshots/Map/Screenshot_2.png)
*Station architecture: showing main nodes, transitions, and door placement zones.*

### Spatial Perspective
![Map Perspective](Assets/Documentation/Screenshots/Map/Screenshot_3.png)
*Scale testing and visual volume calibration of the station interior.*

---

## 📸 Development Screenshots

### Interactions and Triggers
![Interactions](Assets/Documentation/Screenshots/Equipment/Door.png)
*Visualization of trigger zones for automated systems.*

### Development Environment
![Development environment](Assets/Documentation/Screenshots/Equipment/TabletAndFlashlight.png)
*Testing interaction systems within the Unity Editor.*

---

## 🚀 Tech Stack
* **Engine:** Unity 2022.3 LTS
* **Animation:** DOTween (Digital Ruby)
* **Architecture:** Namespace-based, Event-driven (C#)
