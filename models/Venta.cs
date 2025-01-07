class Venta
{
    public required string IdVenta { get; set; }
    public required string Sucursal { get; set; }
    public required string Fecha { get; set; }
    public required string Hora { get; set; }
    public required DateTime TimeStamp { get; set; }

    public required List<Producto> Productos { get; set; }
    public required int CantidadGeneral { get; set; }
    public required double TotalGeneral { get; set; }

    public Venta()
    {

    }

    public Venta(dynamic data)
    {
        List<Producto> productos = new List<Producto>();
        foreach (var producto in data.productos)
        {
            productos.Add(new Producto(producto));
        }

        IdVenta = data.idVenta;
        Sucursal = data.sucursal;
        Fecha = data.fecha;
        Hora = data.hora;
        TimeStamp = data.timestamp;
        Productos = productos;
        CantidadGeneral = data.cantidadGeneral;
        TotalGeneral = data.totalGeneral;
    }
}