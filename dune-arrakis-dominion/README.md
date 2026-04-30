# Dune: Arrakis Dominion Distributed

## Overview
"Dune: Arrakis Dominion Distributed" is a modular distributed system built in C# (.NET) that simulates the management of a House on Arrakis. The project encompasses various aspects of gameplay, including creature management, enclave management, resource allocation, and event logging.

## Project Structure
The solution is organized into multiple projects, each with distinct responsibilities:

- **Domain**: Contains the core entities and enums that define the game's structure.
- **Application**: Implements the business logic and services that manage game operations.
- **Infrastructure**: Handles data persistence using JSON for saving and loading game states.
- **SimulationService**: A standalone service that executes game rounds and updates game states asynchronously.
- **PersistenceService**: A service dedicated to saving and loading game data.
- **AdminClient**: A console application that provides an interface for users to interact with the game.
- **Shared**: Contains Data Transfer Objects (DTOs) and shared contracts used across services.

## Key Features
- **Game Simulation**: Monthly simulation of creature feeding, health calculation, growth, reproduction, visitor generation, and donation calculations.
- **Event Logging**: Tracks significant events such as creature creation, deaths, feeding failures, and financial gains.
- **Concurrency Management**: Utilizes async/await for asynchronous operations and ensures thread safety where necessary.
- **Error Handling**: Implements robust validation and global exception handling to maintain system integrity.

## Getting Started
To get started with the project, clone the repository and open the solution file `DuneArrakisDominion.sln` in your preferred .NET development environment. 

### Prerequisites
- .NET SDK (version 6.0 or higher)
- A code editor such as Visual Studio or Visual Studio Code

### Running the Application
1. Navigate to the `AdminClient` project.
2. Build the solution.
3. Run the `AdminClient` to interact with the game.

## Future Work
- Integration with external APIs for enhanced gameplay features.
- Expansion of game mechanics and additional entities.
- Implementation of a RESTful API for remote interactions.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.