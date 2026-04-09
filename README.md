Actúa como un arquitecto de software senior especializado en sistemas distribuidos en C# y .NET.

Quiero que me ayudes a desarrollar paso a paso la práctica "Dune: Arrakis Dominion Distributed", que consiste en crear una simulación distribuida con cliente, servicio de simulación, servicio de persistencia y modelo compartido .

Necesito que me guíes como si fuera un estudiante, pero con nivel profesional, cubriendo lo siguiente:

1. **Diseño de arquitectura**

   * Propón una arquitectura distribuida clara (cliente-servidor, microservicios o híbrida).
   * Define los proyectos de la solución (.NET) y sus responsabilidades.
   * Explica cómo se comunican (REST, gRPC, colas, etc.).

2. **Modelo de dominio**

   * Define las clases principales (Partida, Enclave, Criatura, Instalación, etc.).
   * Incluye enums, DTOs y relaciones.
   * Aplica buenas prácticas de POO.

3. **Persistencia**

   * Diseña el sistema de guardado (JSON u otro).
   * Explica serialización/deserialización.
   * Manejo de errores de persistencia.

4. **Simulación**

   * Implementa la lógica de rondas:

     * alimentación de criaturas
     * generación de visitantes
     * cálculo de ingresos
   * Explica paso a paso los algoritmos.

5. **Concurrencia**

   * Usa async/await, Tasks o threading.
   * Evita condiciones de carrera.
   * Propón mecanismos de sincronización.

6. **Comunicación distribuida**

   * Diseña cómo interactúan cliente, simulador y persistencia.
   * Manejo de fallos de red y reintentos.

7. **Gestión de errores**

   * Valida operaciones (saldo, traslados, estados inválidos).
   * Manejo de excepciones robusto.

8. **Centro de mando**

   * Diseña una interfaz (consola o GUI).
   * Mostrar estado agregado del sistema.

9. **Buenas prácticas**

   * Clean Code
   * SOLID
   * separación de capas

10. **Entrega académica**

* Ayúdame a redactar la memoria técnica.
* Incluye justificación de decisiones arquitectónicas.

Quiero que estructures la respuesta en fases (como las entregas del proyecto) y que incluyas ejemplos de código en C# cuando sea necesario.

Explícame cada decisión como si tuviera que defenderla ante un profesor.
