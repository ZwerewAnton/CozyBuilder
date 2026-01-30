# Cozy Builder

[![Tests](https://img.shields.io/github/actions/workflow/status/ZwerewAnton/CozyBuilder/tests.yml?branch=master&label=Tests)](
https://github.com/ZwerewAnton/CozyBuilder/actions/workflows/tests.yml
) ![Unity](https://img.shields.io/badge/Unity-2022.3_LTS-black?logo=unity)

A cozy building game developed in Unity, focused on relaxed gameplay, simple progression, and stylized visuals.

![Gameplay Preview](https://raw.githubusercontent.com/ZwerewAnton/Media/refs/heads/master/CozyBuilder/preview.gif)

## About the Project

Cozy Builder is a personal game project created for learning, experimentation, and portfolio purposes.
The game is fully playable and currently published on Google Play.

📱 Google Play: https://play.google.com/store/apps/details?id=com.StudioSaturdayAfternoon.CozyBuilder

## Features

- Cozy building gameplay
- Touch-friendly controls
- Stylized visuals
- Mobile-oriented performance considerations
- Custom editor tooling

## Getting Started

1. Clone the repository
2. Open the project via Unity Hub
3. Use Unity version 2022.x LTS
4. Open `Assets/Scenes/MainMenu.unity`

> Note: The project is configured for Android and uses URP. Core services are initialized in the **Boot** scene, which is loaded automatically on startup.

## Tech Stack

- **Unity:** 2022.x LTS
- **Render Pipeline:** Universal Render Pipeline (URP)
- **Input:** New Unity Input System

## Architecture & Libraries

- **Extenject** — dependency injection
- **UniTask** — async/await support optimized for Unity
- **PrimeTween** — animations and tweening

## Testing

The project includes automated tests covering both logic and runtime behavior.

- **Edit Mode tests**
- **Play Mode tests**

### Testing Tools
- Unity Test Framework
- FluentAssertions
- NSubstitute

## Shaders & Visuals

- Custom shaders created using **Shader Graph**
- Stylized materials designed specifically for URP
