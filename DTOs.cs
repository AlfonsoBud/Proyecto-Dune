namespace DuneModel.DTOs
{
    /// <summary>
    /// DTO con el estado resumido de una partida para mostrar en el centro de mando.
    /// Solo datos, sin lógica. Se serializa/deserializa libremente entre servicios.
    /// </summary>
    public record PartidaResumenDto(
        Guid Id,
        string AliasJugador,
        string Escenario,
        int RondaActual,
        double Fondos,
        DateTime UltimoGuardado);

    /// <summary>DTO con el estado detallado de un enclave.</summary>
    public record EnclaveDto(
        Guid Id,
        string Nombre,
        string Tipo,
        int Hectareas,
        int HectareasOcupadas,
        double Suministros,
        double MaxSuministros,
        int VisitantesActuales,
        string? NivelAdquisitivo,
        List<InstalacionDto> Instalaciones);

    /// <summary>DTO con el estado de una instalación.</summary>
    public record InstalacionDto(
        Guid Id,
        string Codigo,
        string Tipo,
        string Medio,
        string Alimentacion,
        int Capacidad,
        int CriaturasActuales,
        double Suministros,
        double MaxSuministros,
        List<CriaturaDto> Criaturas);

    /// <summary>DTO con el estado de una criatura.</summary>
    public record CriaturaDto(
        Guid Id,
        string Nombre,
        string Medio,
        string Alimentacion,
        int EdadActual,
        int EdadAdulta,
        double Salud,
        bool EsAdulta,
        bool EnLetargo,
        int VecesElegidaFavorita);

    /// <summary>DTO para solicitar el traslado de una criatura.</summary>
    public record SolicitudTrasladoDto(
        Guid IdPartida,
        Guid IdCriatura,
        Guid IdInstalacionOrigen,
        Guid IdInstalacionDestino);

    /// <summary>DTO para solicitar la construcción de una instalación.</summary>
    public record SolicitudConstruccionDto(
        Guid IdPartida,
        string CodigoInstalacion,
        bool EnExhibicion);

    /// <summary>DTO de resultado de una operación (éxito o error).</summary>
    public record ResultadoOperacionDto(
        bool Exito,
        string Mensaje,
        string? DetalleError = null);

    /// <summary>DTO con el resumen de una ronda ejecutada.</summary>
    public record ResumenRondaDto(
        int NumeroRonda,
        double Donaciones,
        int VisitantesLlegaron,
        int VisitantesSalieron,
        List<string> CriaturasNacidas,
        List<string> EventosDestacados);

    // ── Extensiones de conversión (Modelo → DTO) ──────────────────────────────

    public static class DtoExtensions
    {
        public static PartidaResumenDto ToResumenDto(this Partida p) => new(
            p.Id, p.AliasJugador, p.Escenario.ToString(),
            p.RondaActual, p.Fondos, p.UltimoGuardado);

        public static CriaturaDto ToDto(this Criatura c) => new(
            c.Id, c.Nombre, c.Medio.ToString(), c.Alimentacion.ToString(),
            c.EdadActual, c.EdadAdulta, c.Salud,
            c.EsAdulta, c.EnLetargo, c.VecesElegidaFavorita);

        public static InstalacionDto ToDto(this Instalacion i) => new(
            i.Id, i.Codigo, i.Tipo.ToString(), i.Medio.ToString(),
            i.Alimentacion.ToString(), i.Capacidad, i.Criaturas.Count,
            i.Suministros, i.MaxSuministros,
            i.Criaturas.Select(c => c.ToDto()).ToList());

        public static EnclaveDto ToDto(this Enclave e) => new(
            e.Id, e.Nombre, e.TipoEnclave.ToString(),
            e.Hectareas, e.HectareasOcupadas,
            e.Suministros, e.MaxSuministros,
            e.VisitantesActuales,
            e.NivelAdquisitivo?.ToString(),
            e.Instalaciones.Select(i => i.ToDto()).ToList());
    }
}
