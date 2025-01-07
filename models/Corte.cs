class Corte {
    public required string IdCorte { get; set; } 
    public required string FechaCorte { get; set; }
    public required string HoraCorte { get; set; }
    public required string HoraInicio { get; set; }
    public required string HoraFin { get; set; }
    public required List<Producto> Productos { get; set; }
    public required int UnidadesVendidas { get; set; }
    public required double TotalGeneral { get; set; }
    public required string Sucursal { get; set; }

    public Corte(dynamic data)
    {
        IdCorte = data.idCorte;
        FechaCorte = data.fechaCorte;
        HoraCorte = data.horaCorte;
        HoraInicio = data.horaInicio;
        HoraFin = data.horaFin;
        Productos = new List<Producto>();
        UnidadesVendidas = data.unidadesVendidas;
        TotalGeneral = data.totalGeneral;
        Sucursal = data.sucursal;

        foreach (dynamic producto in data.productos)
        {
            Productos.Add(new Producto(producto));
        }
    }
}