namespace DuneModel
{
    /// <summary>
    /// Entidad raíz del juego. Agrupa el estado completo de una partida:
    /// jugador, enclaves, fondos, ronda actual y registro de eventos.
    /// </summary>
    public class Partida
    {
        // ── Identidad ────────────────────────────────────────────────────────
        public Guid Id { get; private set; }
        public string AliasJugador { get; private set; }
        public Escenario Escenario { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateTime UltimoGuardado { get; private set; }

        // ── Estado económico ──────────────────────────────────────────────────
        public double Fondos { get; private set; }

        // ── Progreso ─────────────────────────────────────────────────────────
        public int RondaActual { get; private set; }   // empieza en 1

        // ── Enclaves ──────────────────────────────────────────────────────────
        /// <summary>Enclave de aclimatación (común a todas las partidas).</summary>
        public Enclave EnclaveAclimatacion { get; private set; }

        /// <summary>Enclave de exhibición (determinado por el escenario).</summary>
        public Enclave EnclaveExhibicion { get; private set; }

        // ── Registro de eventos ──────────────────────────────────────────────
        public List<EventoPartida> Eventos { get; private set; }

        // ── Constructor (nueva partida) ───────────────────────────────────────
        public Partida(string aliasJugador, Escenario escenario)
        {
            if (string.IsNullOrWhiteSpace(aliasJugador))
                throw new ArgumentException("El alias del jugador no puede estar vacío.");

            Id = Guid.NewGuid();
            AliasJugador = aliasJugador.Trim();
            Escenario = escenario;
            FechaCreacion = DateTime.UtcNow;
            UltimoGuardado = DateTime.UtcNow;
            RondaActual = 1;
            Eventos = new List<EventoPartida>();

            // Inicializar enclaves y fondos según el escenario
            (Fondos, EnclaveAclimatacion, EnclaveExhibicion) = InicializarEscenario(escenario);

            RegistrarEvento($"Partida creada. Escenario: {escenario}. Fondos iniciales: {Fondos} solaris.");
        }

        // Constructor de carga (deserialización)
        public Partida(Guid id, string aliasJugador, Escenario escenario,
                       DateTime fechaCreacion, int rondaActual, double fondos,
                       Enclave enclaveAclimatacion, Enclave enclaveExhibicion,
                       List<EventoPartida> eventos)
        {
            Id = id;
            AliasJugador = aliasJugador;
            Escenario = escenario;
            FechaCreacion = fechaCreacion;
            UltimoGuardado = DateTime.UtcNow;
            RondaActual = rondaActual;
            Fondos = fondos;
            EnclaveAclimatacion = enclaveAclimatacion;
            EnclaveExhibicion = enclaveExhibicion;
            Eventos = eventos;
        }

        // ── Economía ──────────────────────────────────────────────────────────

        /// <summary>Añade fondos (p.ej. por donaciones).</summary>
        public void AnadirFondos(double cantidad, string motivo)
        {
            if (cantidad < 0) throw new ArgumentException("La cantidad debe ser positiva.");
            Fondos += cantidad;
            RegistrarEvento($"+{cantidad:F0} solaris por {motivo}. Total: {Fondos:F0}");
        }

        /// <summary>
        /// Gasta fondos. Lanza excepción si no hay suficientes.
        /// </summary>
        public void GastarFondos(double cantidad, string motivo)
        {
            if (cantidad < 0) throw new ArgumentException("La cantidad debe ser positiva.");
            if (Fondos < cantidad)
                throw new InvalidOperationException(
                    $"Fondos insuficientes para {motivo}. Necesitas {cantidad:F0}, tienes {Fondos:F0}.");
            Fondos -= cantidad;
            RegistrarEvento($"-{cantidad:F0} solaris por {motivo}. Total: {Fondos:F0}");
        }

        // ── Traslado de criaturas ─────────────────────────────────────────────

        /// <summary>
        /// Traslada una criatura adulta y sana de aclimatación a exhibición.
        /// Valida: adulta, salud ≥ 75, plaza libre, fondos suficientes.
        /// costeTraslado = 100 × 3^(edad − edadAdulta) × σ
        ///   σ = 5 (Desierto), 15 (Aéreo), 25 (Subterráneo)
        /// </summary>
        public void TrasladarCriatura(Guid idCriatura, Guid idInstalacionOrigen,
                                      Guid idInstalacionDestino)
        {
            var origen = EnclaveAclimatacion.Instalaciones
                .FirstOrDefault(i => i.Id == idInstalacionOrigen)
                ?? throw new ArgumentException("Instalación de origen no encontrada.");

            var criatura = origen.Criaturas.FirstOrDefault(c => c.Id == idCriatura)
                ?? throw new ArgumentException("Criatura no encontrada en la instalación de origen.");

            var destino = EnclaveExhibicion.Instalaciones
                .FirstOrDefault(i => i.Id == idInstalacionDestino)
                ?? throw new ArgumentException("Instalación de destino no encontrada.");

            // Validaciones
            if (!criatura.EsAdulta)
                throw new InvalidOperationException($"{criatura.Nombre} no es adulta aún.");
            if (criatura.Salud < 75)
                throw new InvalidOperationException(
                    $"{criatura.Nombre} tiene salud {criatura.Salud:F0}% (mínimo 75%).");
            if (destino.EstaLlena)
                throw new InvalidOperationException("La instalación de destino está llena.");
            if (destino.Medio != criatura.Medio)
                throw new InvalidOperationException(
                    $"Medio incompatible: criatura es {criatura.Medio}, destino es {destino.Medio}.");

            double sigma = criatura.Medio switch
            {
                Medio.Desierto    => 5,
                Medio.Aereo       => 15,
                Medio.Subterraneo => 25,
                _ => 5
            };
            double coste = 100.0 * Math.Pow(3, criatura.EdadActual - criatura.EdadAdulta) * sigma;

            GastarFondos(coste, $"traslado de {criatura.Nombre}");
            origen.QuitarCriatura(criatura.Id);
            destino.AnadirCriatura(criatura);

            RegistrarEvento($"{criatura.Nombre} trasladada a {destino.Codigo} (coste: {coste:F0} solaris).");
        }

        /// <summary>
        /// Descarta una criatura (transferencia a Bene Tleilax). Coste fijo: 20000 solaris.
        /// </summary>
        public void DescartarCriatura(Guid idCriatura)
        {
            const double CosteDescarte = 20000;
            Instalacion? instalacion = null;
            Criatura? criatura = null;

            foreach (var enc in new[] { EnclaveAclimatacion, EnclaveExhibicion })
            {
                foreach (var inst in enc.Instalaciones)
                {
                    criatura = inst.Criaturas.FirstOrDefault(c => c.Id == idCriatura);
                    if (criatura != null) { instalacion = inst; break; }
                }
                if (instalacion != null) break;
            }

            if (instalacion == null || criatura == null)
                throw new ArgumentException("Criatura no encontrada en ninguna instalación.");

            GastarFondos(CosteDescarte, $"descarte de {criatura.Nombre} (Bene Tleilax)");
            instalacion.QuitarCriatura(criatura.Id);
        }

        // ── Construcción de instalaciones ─────────────────────────────────────

        /// <summary>
        /// Construye una nueva instalación en el enclave indicado con cargo a los fondos.
        /// </summary>
        public void ConstruirInstalacion(Instalacion instalacion, bool enExhibicion)
        {
            var enclave = enExhibicion ? EnclaveExhibicion : EnclaveAclimatacion;

            if (enclave.HectareasLibres < instalacion.Hectareas)
                throw new InvalidOperationException(
                    $"No hay suficientes hectáreas libres en {enclave.Nombre}.");

            GastarFondos(instalacion.CosteConstruction,
                         $"construcción de {instalacion.Codigo} en {enclave.Nombre}");
            enclave.AnadirInstalacion(instalacion);
            RegistrarEvento($"Instalación {instalacion.Codigo} construida en {enclave.Nombre}.");
        }

        // ── Avance de ronda ────────────────────────────────────────────────────

        /// <summary>
        /// Avanza la partida un mes: alimentación, envejecimiento,
        /// visitantes, donaciones y reproducción.
        /// </summary>
        public ResumenRonda AvanzarRonda(Random rng)
        {
            var resumen = new ResumenRonda(RondaActual);

            // 1. Alimentación en todos los enclaves
            foreach (var inst in EnclaveAclimatacion.Instalaciones)
                resumen.ResumenesAlimentacion.Add(inst.EjecutarAlimentacion());
            foreach (var inst in EnclaveExhibicion.Instalaciones)
                resumen.ResumenesAlimentacion.Add(inst.EjecutarAlimentacion());

            // 2. Envejecimiento
            foreach (var inst in EnclaveAclimatacion.Instalaciones) inst.EnvejecerCriaturas();
            foreach (var inst in EnclaveExhibicion.Instalaciones)    inst.EnvejecerCriaturas();

            // 3. Visitantes y donaciones (exhibición)
            var (llegaron, salieron) = EnclaveExhibicion.ActualizarVisitantes();
            resumen.VisitantesLlegaron = llegaron;
            resumen.VisitantesSalieron = salieron;

            double donaciones = EnclaveExhibicion.GenerarDonaciones(rng);
            resumen.Donaciones = donaciones;
            if (donaciones > 0) AnadirFondos(donaciones, "donaciones de visitantes");

            // 4. Reproducción en aclimatación
            foreach (var inst in EnclaveAclimatacion.Instalaciones)
            {
                var nueva = inst.IntentarReproduccion(rng);
                if (nueva != null)
                {
                    resumen.CriaturasNacidas.Add(nueva.Nombre);
                    RegistrarEvento($"Nueva criatura: {nueva.Nombre} en {inst.Codigo}.");
                }
            }

            RondaActual++;
            RegistrarEvento($"Ronda {RondaActual - 1} completada. Donaciones: {donaciones:F0} solaris.");
            return resumen;
        }

        // ── Registro de eventos ───────────────────────────────────────────────

        public void RegistrarEvento(string descripcion)
        {
            Eventos.Add(new EventoPartida(descripcion));
        }

        public void MarcarGuardado()
        {
            UltimoGuardado = DateTime.UtcNow;
            RegistrarEvento("Partida guardada.");
        }

        // ── Vista del centro de mando ─────────────────────────────────────────

        /// <summary>
        /// Devuelve todas las criaturas ordenadas de mayor a menor salud.
        /// </summary>
        public IEnumerable<(Criatura criatura, string ubicacion)> CriaturasOrdenadaPorSalud()
        {
            var todas = new List<(Criatura, string)>();

            foreach (var inst in EnclaveAclimatacion.Instalaciones)
                foreach (var c in inst.Criaturas)
                    todas.Add((c, $"{EnclaveAclimatacion.Nombre} / {inst.Codigo}"));

            foreach (var inst in EnclaveExhibicion.Instalaciones)
                foreach (var c in inst.Criaturas)
                    todas.Add((c, $"{EnclaveExhibicion.Nombre} / {inst.Codigo}"));

            return todas.OrderByDescending(t => t.Item1.Salud);
        }

        public override string ToString() =>
            $"Partida {Id} | {AliasJugador} | Escenario: {Escenario} " +
            $"| Ronda: {RondaActual} | Fondos: {Fondos:F0} solaris";

        // ── Inicialización de escenarios ──────────────────────────────────────

        private static (double fondos, Enclave aclimatacion, Enclave exhibicion)
            InicializarEscenario(Escenario escenario)
        {
            // Enclave de aclimatación común a todos los escenarios
            var aclimatacion = new Enclave(
                nombre: "Cuenca Experimental de Arrakis",
                tipo: TipoInstalacion.Aclimatacion,
                hectareas: 5000,
                suministrosIniciales: 20000);

            Enclave exhibicion = escenario switch
            {
                Escenario.ArrakeenDominioDeLaEspecia => new Enclave(
                    "Arrakeen", TipoInstalacion.Exhibicion,
                    hectareas: 7700, suministrosIniciales: 10000,
                    visitantesMensualesBase: 1000, nivelAdquisitivo: NivelAdquisitivo.Alto),

                Escenario.GiediPrimeGaleriaIndustrial => new Enclave(
                    "Giedi Prime", TipoInstalacion.Exhibicion,
                    hectareas: 100, suministrosIniciales: 5000,
                    visitantesMensualesBase: 2000, nivelAdquisitivo: NivelAdquisitivo.Bajo),

                Escenario.CaladanReservaDucal => new Enclave(
                    "Caladan", TipoInstalacion.Exhibicion,
                    hectareas: 10000, suministrosIniciales: 25000,
                    visitantesMensualesBase: 3000, nivelAdquisitivo: NivelAdquisitivo.Medio),

                _ => throw new ArgumentException("Escenario desconocido.")
            };

            double fondos = escenario switch
            {
                Escenario.ArrakeenDominioDeLaEspecia  => 100000,
                Escenario.GiediPrimeGaleriaIndustrial => 50000,
                Escenario.CaladanReservaDucal          => 150000,
                _ => 0
            };

            return (fondos, aclimatacion, exhibicion);
        }
    }

    // ── Clases auxiliares ─────────────────────────────────────────────────────

    /// <summary>Evento registrado en el historial de la partida.</summary>
    public class EventoPartida
    {
        public DateTime Timestamp { get; }
        public string Descripcion { get; }

        public EventoPartida(string descripcion)
        {
            Timestamp = DateTime.UtcNow;
            Descripcion = descripcion;
        }
    }

    /// <summary>Resumen de lo ocurrido en una ronda mensual.</summary>
    public class ResumenRonda
    {
        public int NumeroRonda { get; }
        public List<ResumenAlimentacion> ResumenesAlimentacion { get; } = new();
        public List<string> CriaturasNacidas { get; } = new();
        public int VisitantesLlegaron { get; set; }
        public int VisitantesSalieron { get; set; }
        public double Donaciones { get; set; }

        public ResumenRonda(int numero) => NumeroRonda = numero;
    }
}
