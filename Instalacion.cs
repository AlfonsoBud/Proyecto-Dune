namespace DuneModel
{
    /// <summary>
    /// Representa una instalación dentro de un enclave.
    /// Puede ser de aclimatación (cría) o de exhibición (monetización).
    /// </summary>
    public class Instalacion
    {
        // ── Identidad y tipo ─────────────────────────────────────────────────
        public Guid Id { get; private set; }
        public string Codigo { get; private set; }
        public TipoInstalacion Tipo { get; private set; }
        public TipoRecinto TipoRecinto { get; private set; }

        // ── Atributos físicos (fijos al construir) ────────────────────────────
        public Medio Medio { get; private set; }
        public Alimentacion Alimentacion { get; private set; }
        public int CosteConstruction { get; private set; }   // en solaris
        public int Capacidad { get; private set; }           // nº máximo de criaturas
        public int Hectareas { get; private set; }

        // ── Estado dinámico ──────────────────────────────────────────────────
        public double Suministros { get; private set; }
        public List<Criatura> Criaturas { get; private set; }

        // Máximo de suministros que puede almacenar (= coste de construcción)
        public double MaxSuministros => CosteConstruction;

        public bool EstaLlena => Criaturas.Count >= Capacidad;
        public int PlazasLibres => Capacidad - Criaturas.Count;

        // ── Constructor ──────────────────────────────────────────────────────
        public Instalacion(string codigo, TipoInstalacion tipo, Medio medio,
                           Alimentacion alimentacion, int coste, int capacidad,
                           int hectareas, TipoRecinto tipoRecinto,
                           double suministrosIniciales = 0)
        {
            Id = Guid.NewGuid();
            Codigo = codigo;
            Tipo = tipo;
            Medio = medio;
            Alimentacion = alimentacion;
            CosteConstruction = coste;
            Capacidad = capacidad;
            Hectareas = hectareas;
            TipoRecinto = tipoRecinto;
            Suministros = Math.Min(suministrosIniciales, MaxSuministros);
            Criaturas = new List<Criatura>();
        }

        // ── Gestión de criaturas ─────────────────────────────────────────────

        /// <summary>Añade una criatura si hay plaza.</summary>
        public bool AnadirCriatura(Criatura criatura)
        {
            if (EstaLlena) return false;
            Criaturas.Add(criatura);
            return true;
        }

        /// <summary>Elimina una criatura por id.</summary>
        public bool QuitarCriatura(Guid id)
        {
            var c = Criaturas.FirstOrDefault(x => x.Id == id);
            if (c == null) return false;
            Criaturas.Remove(c);
            return true;
        }

        // ── Gestión de suministros ────────────────────────────────────────────

        /// <summary>
        /// Añade suministros respetando el límite máximo.
        /// Retorna la cantidad realmente añadida.
        /// </summary>
        public double AnadirSuministros(double cantidad)
        {
            double espacio = MaxSuministros - Suministros;
            double real = Math.Min(cantidad, espacio);
            Suministros += real;
            return real;
        }

        /// <summary>
        /// Consume suministros. No baja de 0. Retorna lo consumido.
        /// </summary>
        public double ConsumirSuministros(double cantidad)
        {
            double real = Math.Min(cantidad, Suministros);
            Suministros -= real;
            return real;
        }

        // ── Lógica de ronda ──────────────────────────────────────────────────

        /// <summary>
        /// Ejecuta la fase de alimentación de todas las criaturas de esta instalación.
        /// Retorna un resumen de lo ocurrido.
        /// </summary>
        public ResumenAlimentacion EjecutarAlimentacion()
        {
            bool enAclimatacion = Tipo == TipoInstalacion.Aclimatacion;
            var resumen = new ResumenAlimentacion(Codigo);

            foreach (var criatura in Criaturas.ToList())   // ToList para iterar de forma segura
            {
                double necesita = criatura.CalcularIngesta(enAclimatacion);
                double consumido = criatura.Alimentar(Suministros, enAclimatacion);
                Suministros = Math.Max(0, Suministros - consumido);

                resumen.AnadirEvento(criatura, necesita, consumido);

                if (criatura.EnLetargo)
                    resumen.CriaturasEnLetargo.Add(criatura.Nombre);
            }

            return resumen;
        }

        /// <summary>
        /// Avanza la edad de todas las criaturas activas un mes.
        /// </summary>
        public void EnvejecerCriaturas()
        {
            foreach (var c in Criaturas.Where(c => !c.EnLetargo))
                c.EnvejeceUnMes();
        }

        /// <summary>
        /// Intenta generar una nueva criatura por reproducción (solo aclimatación).
        /// Probabilidad de éxito: 20% por intento mensual.
        /// Retorna la criatura creada, o null si no hay éxito o no hay espacio.
        /// </summary>
        public Criatura? IntentarReproduccion(Random rng)
        {
            if (Tipo != TipoInstalacion.Aclimatacion) return null;
            if (EstaLlena) return null;
            if (rng.NextDouble() > 0.20) return null;   // 20% de probabilidad

            string? especie = CatalogoEspecies.EspecieAleatoria(Medio, Alimentacion, rng);
            if (especie == null) return null;

            var nueva = CatalogoEspecies.Crear(especie);
            AnadirCriatura(nueva);
            return nueva;
        }

        /// <summary>
        /// Salud media de las criaturas activas. 100 si no hay ninguna.
        /// </summary>
        public double SaludMedia()
        {
            var activas = Criaturas.Where(c => !c.EnLetargo).ToList();
            if (activas.Count == 0) return 100;
            return activas.Average(c => c.Salud);
        }

        public override string ToString() =>
            $"[{Codigo}] {Tipo} | {Medio}/{Alimentacion} | " +
            $"{Criaturas.Count}/{Capacidad} criaturas | Suministros: {Suministros:F0}/{MaxSuministros}";
    }

    /// <summary>Datos de lo ocurrido en una ronda de alimentación.</summary>
    public class ResumenAlimentacion
    {
        public string CodigoInstalacion { get; }
        public List<string> CriaturasEnLetargo { get; } = new();
        public List<string> Eventos { get; } = new();

        public ResumenAlimentacion(string codigo) => CodigoInstalacion = codigo;

        public void AnadirEvento(Criatura c, double necesitaba, double consumio)
        {
            double pct = necesitaba > 0 ? consumio / necesitaba * 100 : 100;
            string estado = c.EnLetargo ? "LETARGO" : $"salud {c.Salud:F0}%";
            Eventos.Add($"{c.Nombre}: necesitaba {necesitaba:F1}, comió {consumio:F1} ({pct:F0}%) → {estado}");
        }
    }
}
