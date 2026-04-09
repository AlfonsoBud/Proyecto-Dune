using DuneModel;

/// <summary>
/// Tests manuales del modelo de dominio.
/// Ejecuta cada caso y muestra el resultado en consola.
/// En la entrega final podéis migrar esto a xUnit o NUnit.
/// </summary>
class PruebasModelo
{
    static int _ok = 0, _fail = 0;

    static void Main()
    {
        Console.WriteLine("=== PRUEBAS DEL MODELO DE DOMINIO — DUNE ===\n");

        PruebaAlimentacionCriatura();
        PruebaLetargo();
        PruebaDonacion();
        PruebaInstalacionCapacidad();
        PruebaPartidaInicializacion();
        PruebaTraslado();
        PruebaRonda();
        PruebaCatalogoEspeciesCompatibles();

        Console.WriteLine($"\n=== RESULTADO: {_ok} OK  |  {_fail} FAIL ===");
    }

    // ── Alimentación ──────────────────────────────────────────────────────────

    static void PruebaAlimentacionCriatura()
    {
        Console.WriteLine("--- Alimentación y salud ---");

        var c = CatalogoEspecies.Crear(CatalogoEspecies.MuadDib); // apetito=2, edadAdulta=12

        // Edad 0 → ingesta = 2 × 0 = 0 → no necesita nada → salud sube (ya es 100, se queda)
        double consumido = c.Alimentar(1000, enAclimatacion: false);
        Assert("Ingesta edad 0 = 0 → consumido=0", consumido == 0);
        Assert("Salud sigue en 100", c.Salud == 100);

        // Avanzamos a edad 6 (no adulta)
        for (int i = 0; i < 6; i++) c.EnvejeceUnMes();
        double ingesta6 = c.CalcularIngesta(false);  // 2 × 6 = 12
        Assert("Ingesta a edad 6 = 12", ingesta6 == 12);

        // Alimentar con 0 → menos del 25% → pierde 30 salud
        c.Alimentar(0, false);
        Assert("Salud baja 30 al no comer", c.Salud == 70);

        Console.WriteLine();
    }

    static void PruebaLetargo()
    {
        Console.WriteLine("--- Letargo irreversible ---");

        var c = CatalogoEspecies.Crear(CatalogoEspecies.TigreLaza);
        for (int i = 0; i < 3; i++) c.Alimentar(0, false); // 3 × −30 = salud 10
        Assert("Salud tras 3 meses sin comer = 10", c.Salud == 10);
        c.Alimentar(0, false); // salud → 0
        Assert("Entra en letargo", c.EnLetargo);
        Assert("Salud = 0 en letargo", c.Salud == 0);

        // No consume nada en letargo
        double cons = c.Alimentar(9999, false);
        Assert("No consume en letargo", cons == 0);

        Console.WriteLine();
    }

    static void PruebaDonacion()
    {
        Console.WriteLine("--- Donaciones por nivel adquisitivo ---");

        var c = CatalogoEspecies.Crear(CatalogoEspecies.HalconDelDesierto); // edadAdulta=16
        for (int i = 0; i < 16; i++) c.EnvejeceUnMes(); // adulta, edad=16
        c.Alimentar(9999, false); // bien alimentada

        double donBajo  = c.CalcularDonacion(NivelAdquisitivo.Bajo);   // 10×1×1×1  = 10
        double donMedio = c.CalcularDonacion(NivelAdquisitivo.Medio);  // 10×1×1×15 = 150
        double donAlto  = c.CalcularDonacion(NivelAdquisitivo.Alto);   // 10×1×1×30 = 300

        Assert("Donación BAJO = 10",  Math.Abs(donBajo  - 10)  < 0.01);
        Assert("Donación MEDIO = 150", Math.Abs(donMedio - 150) < 0.01);
        Assert("Donación ALTO = 300",  Math.Abs(donAlto  - 300) < 0.01);

        Console.WriteLine();
    }

    // ── Instalación ───────────────────────────────────────────────────────────

    static void PruebaInstalacionCapacidad()
    {
        Console.WriteLine("--- Capacidad de instalación ---");

        var inst = CatalogoInstalaciones.ADR05(); // capacidad=5
        var c1 = CatalogoEspecies.Crear(CatalogoEspecies.MuadDib);
        var c2 = CatalogoEspecies.Crear(CatalogoEspecies.MuadDib);

        inst.AnadirCriatura(c1);
        inst.AnadirCriatura(c2);
        Assert("2 criaturas añadidas", inst.Criaturas.Count == 2);

        // Llenar hasta el límite
        for (int i = 0; i < 3; i++)
            inst.AnadirCriatura(CatalogoEspecies.Crear(CatalogoEspecies.MuadDib));
        Assert("Instalación llena al llegar a capacidad", inst.EstaLlena);

        bool resultado = inst.AnadirCriatura(CatalogoEspecies.Crear(CatalogoEspecies.MuadDib));
        Assert("No se puede añadir más allá de la capacidad", !resultado);

        Console.WriteLine();
    }

    // ── Partida ───────────────────────────────────────────────────────────────

    static void PruebaPartidaInicializacion()
    {
        Console.WriteLine("--- Inicialización de partida ---");

        var p = new Partida("Leto", Escenario.ArrakeenDominioDeLaEspecia);
        Assert("Fondos Arrakeen = 100000",       p.Fondos == 100000);
        Assert("Enclave exhibición = Arrakeen",  p.EnclaveExhibicion.Nombre == "Arrakeen");
        Assert("Ronda inicial = 1",              p.RondaActual == 1);
        Assert("Registro tiene al menos 1 evento", p.Eventos.Count >= 1);

        var p2 = new Partida("Paul", Escenario.CaladanReservaDucal);
        Assert("Fondos Caladan = 150000", p2.Fondos == 150000);

        Console.WriteLine();
    }

    static void PruebaTraslado()
    {
        Console.WriteLine("--- Traslado de criatura ---");

        var p = new Partida("Jessica", Escenario.ArrakeenDominioDeLaEspecia);
        var instAclim = CatalogoInstalaciones.ADR05();
        var instExhib = CatalogoInstalaciones.EDR02();

        p.ConstruirInstalacion(instAclim, enExhibicion: false);
        p.ConstruirInstalacion(instExhib, enExhibicion: true);

        // Crear una criatura adulta y sana en aclimatación
        var muad = CatalogoEspecies.Crear(CatalogoEspecies.MuadDib); // edadAdulta=12
        for (int i = 0; i < 12; i++) muad.EnvejeceUnMes();
        instAclim.AnadirCriatura(muad);

        int fondosAntes = (int)p.Fondos;
        p.TrasladarCriatura(muad.Id, instAclim.Id, instExhib.Id);

        Assert("Criatura ya no está en aclimatación", instAclim.Criaturas.Count == 0);
        Assert("Criatura llegó a exhibición",         instExhib.Criaturas.Count == 1);
        Assert("Fondos disminuyeron",                 p.Fondos < fondosAntes);

        Console.WriteLine();
    }

    static void PruebaRonda()
    {
        Console.WriteLine("--- Avance de ronda ---");

        var p = new Partida("Chani", Escenario.CaladanReservaDucal);
        var rng = new Random(42);
        int rondaInicial = p.RondaActual;

        var resumen = p.AvanzarRonda(rng);
        Assert("Ronda avanzó de 1 a 2", p.RondaActual == rondaInicial + 1);
        Assert("Resumen de ronda no es null", resumen != null);

        Console.WriteLine();
    }

    static void PruebaCatalogoEspeciesCompatibles()
    {
        Console.WriteLine("--- Catálogo de especies compatibles ---");

        var compatibles = CatalogoEspecies.EspeciesCompatibles(
            Medio.Desierto, Alimentacion.Recolector).ToList();

        Assert("MuadDib es compatible con Desierto/Recolector",
               compatibles.Contains(CatalogoEspecies.MuadDib));

        var ninguno = CatalogoEspecies.EspeciesCompatibles(
            Medio.Aereo, Alimentacion.Recolector).ToList();
        Assert("No hay especie Aérea/Recolectora en el catálogo base",
               ninguno.Count == 0);

        Console.WriteLine();
    }

    // ── Utilidad ──────────────────────────────────────────────────────────────

    static void Assert(string descripcion, bool condicion)
    {
        if (condicion)
        {
            Console.WriteLine($"  [OK]   {descripcion}");
            _ok++;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [FAIL] {descripcion}");
            Console.ResetColor();
            _fail++;
        }
    }
}
