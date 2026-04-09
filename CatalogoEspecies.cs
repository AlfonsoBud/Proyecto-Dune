namespace DuneModel
{
    /// <summary>
    /// Catálogo de todas las especies disponibles en el juego.
    /// Usa el patrón Factory para crear instancias con los atributos correctos.
    /// </summary>
    public static class CatalogoEspecies
    {
        // Nombres de especie como constantes para evitar typos
        public const string GusanoDeArenaJuvenil = "Gusano de arena juvenil";
        public const string TigreLaza            = "Tigre laza";
        public const string MuadDib              = "Muad'Dib";
        public const string HalconDelDesierto    = "Halcón del desierto";
        public const string TruchaDeArena        = "Trucha de arena";

        // Datos de cada especie según el enunciado
        private static readonly Dictionary<string, (Medio medio, Alimentacion alim, int edadAdulta, int apetito)>
            _especies = new()
            {
                [GusanoDeArenaJuvenil] = (Medio.Subterraneo, Alimentacion.Depredador, 24, 5),
                [TigreLaza]            = (Medio.Desierto,    Alimentacion.Depredador, 38, 8),
                [MuadDib]              = (Medio.Desierto,    Alimentacion.Recolector, 12, 2),
                [HalconDelDesierto]    = (Medio.Aereo,       Alimentacion.Depredador, 16, 2),
                [TruchaDeArena]        = (Medio.Subterraneo, Alimentacion.Recolector, 42, 10),
            };

        /// <summary>
        /// Devuelve los nombres de todas las especies.
        /// </summary>
        public static IEnumerable<string> Nombres => _especies.Keys;

        /// <summary>
        /// Crea una criatura nueva de la especie indicada con salud 100 y edad 0.
        /// </summary>
        public static Criatura Crear(string nombreEspecie)
        {
            if (!_especies.TryGetValue(nombreEspecie, out var datos))
                throw new ArgumentException($"Especie desconocida: {nombreEspecie}");

            return new Criatura(nombreEspecie, datos.medio, datos.alim,
                                datos.edadAdulta, datos.apetito);
        }

        /// <summary>
        /// Devuelve las especies compatibles con el medio y alimentación de una instalación.
        /// </summary>
        public static IEnumerable<string> EspeciesCompatibles(Medio medio, Alimentacion alimentacion)
        {
            return _especies
                .Where(kv => kv.Value.medio == medio && kv.Value.alim == alimentacion)
                .Select(kv => kv.Key);
        }

        /// <summary>
        /// Elige aleatoriamente una especie compatible con los parámetros dados.
        /// Devuelve null si no hay ninguna compatible.
        /// </summary>
        public static string? EspecieAleatoria(Medio medio, Alimentacion alimentacion, Random rng)
        {
            var compatibles = EspeciesCompatibles(medio, alimentacion).ToList();
            if (compatibles.Count == 0) return null;
            return compatibles[rng.Next(compatibles.Count)];
        }
    }
}
