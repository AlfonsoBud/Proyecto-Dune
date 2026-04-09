namespace DuneModel
{
    /// <summary>
    /// Catálogo de instalaciones disponibles exactamente según el enunciado.
    /// Usa el patrón Factory: cada método crea una instancia nueva lista para usar.
    /// </summary>
    public static class CatalogoInstalaciones
    {
        // ── Instalaciones de ACLIMATACIÓN ─────────────────────────────────────

        /// <summary>ADR05 – Desierto / Recolector</summary>
        public static Instalacion ADR05() => new(
            codigo: "ADR05",
            tipo: TipoInstalacion.Aclimatacion,
            medio: Medio.Desierto,
            alimentacion: Alimentacion.Recolector,
            coste: 1000,
            capacidad: 5,
            hectareas: 10,
            tipoRecinto: TipoRecinto.RocaSellada,
            suministrosIniciales: 200);

        /// <summary>ADP03 – Desierto / Depredador</summary>
        public static Instalacion ADP03() => new(
            codigo: "ADP03",
            tipo: TipoInstalacion.Aclimatacion,
            medio: Medio.Desierto,
            alimentacion: Alimentacion.Depredador,
            coste: 2500,
            capacidad: 3,
            hectareas: 50,
            tipoRecinto: TipoRecinto.EscudoEstatico,
            suministrosIniciales: 300);

        /// <summary>AAV02 – Aéreo / Depredador</summary>
        public static Instalacion AAV02() => new(
            codigo: "AAV02",
            tipo: TipoInstalacion.Aclimatacion,
            medio: Medio.Aereo,
            alimentacion: Alimentacion.Depredador,
            coste: 5000,
            capacidad: 2,
            hectareas: 100,
            tipoRecinto: TipoRecinto.CupulaBlindada,
            suministrosIniciales: 500);

        /// <summary>ASU04 – Subterráneo / Depredador</summary>
        public static Instalacion ASU04() => new(
            codigo: "ASU04",
            tipo: TipoInstalacion.Aclimatacion,
            medio: Medio.Subterraneo,
            alimentacion: Alimentacion.Depredador,
            coste: 3500,
            capacidad: 4,
            hectareas: 25,
            tipoRecinto: TipoRecinto.PozoReforzado,
            suministrosIniciales: 100);

        // ── Instalaciones de EXHIBICIÓN ────────────────────────────────────────

        /// <summary>EDR02 – Desierto / Recolector</summary>
        public static Instalacion EDR02() => new(
            codigo: "EDR02",
            tipo: TipoInstalacion.Exhibicion,
            medio: Medio.Desierto,
            alimentacion: Alimentacion.Recolector,
            coste: 21000,
            capacidad: 2,
            hectareas: 200,
            tipoRecinto: TipoRecinto.RocaSellada,
            suministrosIniciales: 0);

        /// <summary>EDP03 – Desierto / Depredador</summary>
        public static Instalacion EDP03() => new(
            codigo: "EDP03",
            tipo: TipoInstalacion.Exhibicion,
            medio: Medio.Desierto,
            alimentacion: Alimentacion.Depredador,
            coste: 12500,
            capacidad: 3,
            hectareas: 300,
            tipoRecinto: TipoRecinto.EscudoEstatico,
            suministrosIniciales: 0);

        /// <summary>EAV02 – Aéreo / Depredador</summary>
        public static Instalacion EAV02() => new(
            codigo: "EAV02",
            tipo: TipoInstalacion.Exhibicion,
            medio: Medio.Aereo,
            alimentacion: Alimentacion.Depredador,
            coste: 15000,
            capacidad: 2,
            hectareas: 200,
            tipoRecinto: TipoRecinto.CupulaBlindada,
            suministrosIniciales: 0);

        /// <summary>ESU03 – Subterráneo / Depredador</summary>
        public static Instalacion ESU03() => new(
            codigo: "ESU03",
            tipo: TipoInstalacion.Exhibicion,
            medio: Medio.Subterraneo,
            alimentacion: Alimentacion.Depredador,
            coste: 25000,
            capacidad: 3,
            hectareas: 400,
            tipoRecinto: TipoRecinto.PozoReforzado,
            suministrosIniciales: 0);

        // ── Utilidades ────────────────────────────────────────────────────────

        /// <summary>
        /// Devuelve una instalación de aclimatación por código.
        /// </summary>
        public static Instalacion CrearPorCodigo(string codigo) => codigo.ToUpper() switch
        {
            "ADR05" => ADR05(),
            "ADP03" => ADP03(),
            "AAV02" => AAV02(),
            "ASU04" => ASU04(),
            "EDR02" => EDR02(),
            "EDP03" => EDP03(),
            "EAV02" => EAV02(),
            "ESU03" => ESU03(),
            _ => throw new ArgumentException($"Código de instalación desconocido: {codigo}")
        };

        /// <summary>Códigos de todas las instalaciones de aclimatación disponibles.</summary>
        public static string[] CodigosAclimatacion => new[] { "ADR05", "ADP03", "AAV02", "ASU04" };

        /// <summary>Códigos de todas las instalaciones de exhibición disponibles.</summary>
        public static string[] CodigosExhibicion => new[] { "EDR02", "EDP03", "EAV02", "ESU03" };
    }
}
