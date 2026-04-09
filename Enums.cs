namespace DuneModel
{
    /// <summary>
    /// Medio natural al que pertenece una criatura o instalación.
    /// </summary>
    public enum Medio
    {
        Desierto,
        Aereo,
        Subterraneo
    }

    /// <summary>
    /// Patrón de alimentación de una criatura.
    /// </summary>
    public enum Alimentacion
    {
        Recolector,
        Depredador
    }

    /// <summary>
    /// Nivel adquisitivo del público de un enclave de exhibición.
    /// Determina el multiplicador σ en el cálculo de donaciones.
    /// </summary>
    public enum NivelAdquisitivo
    {
        Bajo,   // σ = 1
        Medio,  // σ = 15
        Alto    // σ = 30
    }

    /// <summary>
    /// Tipo de recinto físico de una instalación.
    /// </summary>
    public enum TipoRecinto
    {
        RocaSellada,
        EscudoEstatico,
        CupulaBlindada,
        PozoReforzado
    }

    /// <summary>
    /// Indica si una instalación es de aclimatación o de exhibición.
    /// </summary>
    public enum TipoInstalacion
    {
        Aclimatacion,
        Exhibicion
    }

    /// <summary>
    /// Escenario inicial elegido por el jugador al crear la partida.
    /// </summary>
    public enum Escenario
    {
        ArrakeenDominioDeLaEspecia,
        GiediPrimeGaleriaIndustrial,
        CaladanReservaDucal
    }
}
