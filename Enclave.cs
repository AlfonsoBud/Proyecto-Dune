namespace DuneModel
{
    /// <summary>
    /// Representa un enclave (zona geográfica) que agrupa instalaciones.
    /// Puede ser de aclimatación o de exhibición.
    /// </summary>
    public class Enclave
    {
        // ── Identidad ────────────────────────────────────────────────────────
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public TipoInstalacion TipoEnclave { get; private set; }

        // ── Atributos físicos ─────────────────────────────────────────────────
        public int Hectareas { get; private set; }
        public NivelAdquisitivo? NivelAdquisitivo { get; private set; }  // solo exhibición

        // ── Estado dinámico ──────────────────────────────────────────────────
        public double Suministros { get; private set; }           // almacén general
        public double MaxSuministros => Hectareas * 3.0;          // triple de hectáreas
        public int VisitantesActuales { get; private set; }       // solo exhibición
        public int VisitantesMensualesBase { get; private set; }  // visitantes potenciales/mes

        public List<Instalacion> Instalaciones { get; private set; }

        // ── Constructor ──────────────────────────────────────────────────────
        public Enclave(string nombre, TipoInstalacion tipo, int hectareas,
                       double suministrosIniciales, int visitantesMensualesBase = 0,
                       NivelAdquisitivo? nivelAdquisitivo = null)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            TipoEnclave = tipo;
            Hectareas = hectareas;
            Suministros = Math.Min(suministrosIniciales, MaxSuministros);
            VisitantesMensualesBase = visitantesMensualesBase;
            NivelAdquisitivo = nivelAdquisitivo;
            VisitantesActuales = 0;
            Instalaciones = new List<Instalacion>();
        }

        // ── Instalaciones ─────────────────────────────────────────────────────

        public void AnadirInstalacion(Instalacion inst) => Instalaciones.Add(inst);

        public int HectareasOcupadas => Instalaciones.Sum(i => i.Hectareas);
        public int HectareasLibres => Hectareas - HectareasOcupadas;

        // ── Suministros ───────────────────────────────────────────────────────

        /// <summary>
        /// Compra suministros con cargo a los fondos del jugador.
        /// Coste fijo: 5 solaris por unidad.
        /// Retorna el coste total o -1 si no caben.
        /// </summary>
        public double ComprarSuministros(double unidades, ref double fondosJugador)
        {
            double espacio = MaxSuministros - Suministros;
            if (espacio <= 0) return 0;

            double aComprar = Math.Min(unidades, espacio);
            double coste = aComprar * 5.0;

            if (fondosJugador < coste)
                throw new InvalidOperationException(
                    $"Fondos insuficientes: necesitas {coste} solaris, tienes {fondosJugador:F0}.");

            fondosJugador -= coste;
            Suministros += aComprar;
            return coste;
        }

        /// <summary>
        /// Mueve suministros del almacén general a una instalación concreta.
        /// Sin coste adicional, pero respetando el límite de la instalación.
        /// </summary>
        public double MoverSuministrosAInstalacion(Guid idInstalacion, double cantidad)
        {
            var inst = Instalaciones.FirstOrDefault(i => i.Id == idInstalacion)
                       ?? throw new ArgumentException("Instalación no encontrada.");

            double disponible = Math.Min(cantidad, Suministros);
            double movido = inst.AnadirSuministros(disponible);
            Suministros -= movido;
            return movido;
        }

        // ── Visitantes (solo exhibición) ──────────────────────────────────────

        /// <summary>
        /// Calcula y actualiza los visitantes al inicio de cada mes.
        /// Fórmula del enunciado:
        ///   llegan = (visitantesMes × hectareasInstalaciones / hectareasEnclave) × (saludMedia/100)
        ///   salen  = visitantesActuales − llegan
        /// </summary>
        public (int llegaron, int salieron) ActualizarVisitantes()
        {
            if (TipoEnclave != TipoInstalacion.Exhibicion) return (0, 0);

            int hectareasInst = HectareasOcupadas;
            double saludMedia = SaludMediaGlobal();

            int llegaron = (int)Math.Round(
                (double)VisitantesMensualesBase * hectareasInst / Hectareas * (saludMedia / 100.0));

            int salen = (int)Math.Round(
                VisitantesActuales
                - (double)VisitantesActuales * hectareasInst / Hectareas * (saludMedia / 100.0));

            VisitantesActuales = Math.Max(0, VisitantesActuales - salen + llegaron);
            return (llegaron, salen);
        }

        /// <summary>
        /// Distribuye visitantes entre las instalaciones y calcula donaciones.
        /// Cada instalación recibe visitantes proporcionales a sus hectáreas.
        /// </summary>
        public double GenerarDonaciones(Random rng)
        {
            if (TipoEnclave != TipoInstalacion.Exhibicion) return 0;
            if (NivelAdquisitivo == null) return 0;

            double totalDonaciones = 0;
            int totalHectareas = HectareasOcupadas;
            if (totalHectareas == 0) return 0;

            foreach (var inst in Instalaciones)
            {
                int visitantesInst = (int)Math.Round(
                    (double)VisitantesActuales * inst.Hectareas / totalHectareas);

                var criaturasActivas = inst.Criaturas.Where(c => !c.EnLetargo).ToList();
                if (criaturasActivas.Count == 0) continue;

                // Cada visitante elige una criatura aleatoriamente como favorita
                for (int v = 0; v < visitantesInst; v++)
                {
                    var elegida = criaturasActivas[rng.Next(criaturasActivas.Count)];
                    elegida.MarcarComoFavorita();
                    totalDonaciones += elegida.CalcularDonacion(NivelAdquisitivo.Value);
                }
            }

            return totalDonaciones;
        }

        /// <summary>Salud media de todas las criaturas activas del enclave.</summary>
        public double SaludMediaGlobal()
        {
            var activas = Instalaciones
                .SelectMany(i => i.Criaturas)
                .Where(c => !c.EnLetargo)
                .ToList();

            return activas.Count > 0 ? activas.Average(c => c.Salud) : 100;
        }

        public override string ToString() =>
            $"{Nombre} ({TipoEnclave}) | {HectareasOcupadas}/{Hectareas} ha " +
            $"| Suministros: {Suministros:F0}/{MaxSuministros:F0} " +
            $"| Instalaciones: {Instalaciones.Count}";
    }
}
