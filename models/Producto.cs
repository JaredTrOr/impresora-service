class Producto {
    public required string IdProducto { get; set; }
    public required int IdProductoNumerico { get; set; }
    public required string NombreProducto { get; set; }
    public required double Importe { get; set; }
    public required int Cantidad { get; set; }
    public required double Total { get; set; }

    public Producto()
    {

    }

    public Producto(dynamic data)
    {
        IdProducto = data.idProducto;
        IdProductoNumerico = data.idProductoNumerico;
        NombreProducto = data.nombreProducto;
        Importe = data.importe;
        Cantidad = data.cantidad;
        Total = data.total;
    }
}