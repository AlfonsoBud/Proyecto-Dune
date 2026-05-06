# Api (Dune — Arrakis Dominion)

Proyecto API minimal para exponer la lógica de juego y un Hub SignalR para clientes (ej. Unity).

Endpoints incluidos (ejemplo)
- POST `/api/games/broadcast` — broadcast JSON a todos los clientes SignalR.

Ejecución local
1. Restaurar paquetes:
````````
dotnet add src/Api package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.0
dotnet add src/Api package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add src/Api package Microsoft.EntityFrameworkCore.Tools --version 8.0.0