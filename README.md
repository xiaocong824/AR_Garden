# AR Garden

An Augmented Reality garden application built with Unity, supporting Oculus VR/AR devices.

## Overview

AR Garden is an immersive augmented reality experience that allows users to plant, cultivate, and enjoy various plants in a virtual environment. This project leverages Unity's powerful capabilities and the Oculus XR platform to provide an intuitive interactive gardening experience.

## Tech Stack

- **Unity Engine**: Game engine and development environment
- **Oculus XR Plugin**: AR/VR support
- **Meta XR SDK**: Augmented reality functionality
- **C#**: Primary programming language

## Project Structure

```
AR_Garden/
├── Assets/
│   ├── _AR_Garden/          # Main project assets
│   │   ├── ART/            # Art assets
│   │   ├── Scene/          # Scene files
│   │   └── Scripts/        # C# scripts
│   ├── Oculus/             # Oculus configuration
│   ├── Plugins/            # Plugin files
│   ├── Resources/          # Resource files
│   └── XR/                 # XR-related settings
├── ProjectSettings/         # Unity project settings
└── Packages/               # Package manager files
```

## System Requirements

### Minimum Requirements
- Unity 2022.3 LTS or higher
- Oculus Quest 2/3 or compatible AR/VR device
- Windows 10/11 or macOS 10.15+
- At least 4GB available storage

### Recommended
- Unity 2022.3 or higher
- Meta Quest 3
- High-performance computer for development

## Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/haotinggao/AR_Garden.git
   cd AR_Garden
   ```

2. **Open Unity project**
   - Launch Unity Hub
   - Click "Open" and select the project folder
   - Ensure you're using Unity 2022.3 LTS or higher

3. **Configure XR settings**
   - Open Project Settings > XR Plug-in Management
   - Enable Oculus plugin
   - Configure the appropriate XR device

4. **Build and deploy**
   - Select target platform (Android for Quest devices)
   - Configure build settings
   - Connect device and build application

## Features

- 🌱 **Plant Cultivation**: Plant various virtual plants in AR environment
- 🎨 **Interactive Design**: Intuitive gesture controls and interactive interface
- 🌿 **Plant Growth**: Observe plant growth processes and life cycles
- 🎮 **Immersive Experience**: High-quality 3D rendering and AR effects
- 📱 **Cross-platform Support**: Support for multiple AR/VR devices

## Development

### Script Organization
- All custom scripts are located in `Assets/_AR_Garden/Scripts/`
- Use namespaces to organize code
- Follow Unity C# coding conventions

### Scene Management
- Main scene files are located in `Assets/_AR_Garden/Scene/`
- Use scene loading manager to handle scene transitions

### Asset Management
- Art assets are stored in `Assets/_AR_Garden/ART/`
- Use Resources folder for runtime-loaded assets

## Contributing

1. Fork this repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Contact

- Maintainer: HaotingGao
- Email: 821281747@qq.com
- Project Link: [https://github.com/haotinggao/AR_Garden](https://github.com/haotinggao/AR_Garden)

## Acknowledgments

- Unity Technologies for the powerful game engine
- Meta for Oculus XR support and documentation
- Open source community for various tools and resources

---

*Last updated: June 2024* 