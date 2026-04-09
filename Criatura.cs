namespace DuneModel
{
    /// <summary>
    /// Representa una criatura dentro del sistema.
    /// Contiene toda la lógica de alimentación, salud y ciclo de vida.
    /// </summary>
    public class Criatura
    {
        // ── Identidad ────────────────────────────────────────────────────────
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }

        // ── Atributos de especie (fijos) ─────────────────────────────────────
        public Medio Medio { get; private set; }
        public Alimentacion Alimentacion { get; private set; }
        public int EdadAdulta { get; private set; }   // en meses
        public int ApetitoBase { get; private set; }

        // ── Estado dinámico ──────────────────────────────────────────────────
        public int EdadActual { get; private set; }   // en meses
        public double Salud { get; private set; }     // 0 – 100
        public int VecesElegidaFavorita { get; private set; }
        public bool EnLetargo { get; private set; }   // salud = 0 → letargo irreversible

        // ── Constructor ──────────────────────────────────────────────────────
        public Criatura(string nombre, Medio medio, Alimentacion alimentacion,
                        int edadAdulta, int apetitoBase)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Medio = medio;
            Alimentacion = alimentacion;
            EdadAdulta = edadAdulta;
            ApetitoBase = apetitoBase;

            EdadActual = 0;
            Salud = 100;
            EnLetargo = false;
            VecesElegidaFavorita = 0;
        }

        // Constructor de deserialización (para cargar partidas guardadas)
        public Criatura(Guid id, string nombre, Medio medio, Alimentacion alimentacion,
                        int edadAdulta, int apetitoBase, int edadActual,
                        double salud, int vecesElegida, bool enLetargo)
        {
            Id = id;
            Nombre = nombre;
            Medio = medio;
            Alimentacion = alimentacion;
            EdadAdulta = edadAdulta;
            ApetitoBase = apetitoBase;
            EdadActual = edadActual;
            Salud = salud;
            VecesElegidaFavorita = vecesElegida;
            EnLetargo = enLetargo;
        }

        // ── Propiedades calculadas ────────────────────────────────────────────
        public bool EsAdulta => EdadActual >= EdadAdulta;

        /// <summary>
        /// Calcula los suministros que la criatura necesita este mes.
        /// α = 1 en exhibición, α = 15 en aclimatación (según enunciado).
        /// </summary>
        public double CalcularIngesta(bool enAclimatacion)
        {
            double alpha = enAclimatacion ? 15.0 : 1.0;

            if (!EsAdulta)
                return ApetitoBase * EdadActual;

            return ApetitoBase * Math.Pow(2, EdadActual - EdadAdulta) * alpha;
        }

        // ── Métodos de ciclo de vida ──────────────────────────────────────────

        /// <summary>
        /// Alimenta a la criatura con la cantidad de suministros disponible.
        /// Aplica las reglas de pérdida/ganancia de salud del enunciado.
        /// Retorna los suministros realmente consumidos.
        /// </summary>
        public double Alimentar(double suministrosDisponibles, bool enAclimatacion)
        {
            if (EnLetargo) return 0;

            double ingestaNecesaria = CalcularIngesta(enAclimatacion);
            double consumido = Math.Min(suministrosDisponibles, ingestaNecesaria);
            double porcentaje = ingestaNecesaria > 0 ? consumido / ingestaNecesaria : 1.0;

            if (porcentaje < 0.25)
                Salud = Math.Max(0, Salud - 30);
            else if (porcentaje < 0.75)
                Salud = Math.Max(0, Salud - 20);
            else if (porcentaje < 1.0)
                Salud = Math.Max(0, Salud - 10);
            else if (Salud < 100)
                Salud = Math.Min(100, Salud + 5);  // recuperación si bien alimentada

            if (Salud <= 0)
            {
                Salud = 0;
                EnLetargo = true;
            }

            return consumido;
        }

        /// <summary>
        /// Avanza la edad de la criatura un mes.
        /// </summary>
        public void EnvejeceUnMes()
        {
            if (!EnLetargo)
                EdadActual++;
        }

        /// <summary>
        /// Registra que un visitante ha elegido esta criatura como favorita.
        /// </summary>
        public void MarcarComoFavorita()
        {
            VecesElegidaFavorita++;
        }

        /// <summary>
        /// Calcula la donación que genera esta criatura cuando es elegida favorita.
        /// donacion = 10 × (salud/100) × (edad/edadAdulta) × σ
        /// </summary>
        public double CalcularDonacion(NivelAdquisitivo nivel)
        {
            if (EnLetargo || Salud <= 0) return 0;

            double sigma = nivel switch
            {
                NivelAdquisitivo.Bajo  => 1,
                NivelAdquisitivo.Medio => 15,
                NivelAdquisitivo.Alto  => 30,
                _ => 1
            };

            double factorEdad = EdadAdulta > 0 ? (double)EdadActual / EdadAdulta : 1.0;
            return 10.0 * (Salud / 100.0) * factorEdad * sigma;
        }

        public override string ToString() =>
            $"{Nombre} | Edad: {EdadActual}/{EdadAdulta} | Salud: {Salud:F0}% " +
            $"| {(EnLetargo ? "LETARGO" : "Activa")}";
    }
}
