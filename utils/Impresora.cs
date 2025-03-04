using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning;
using System.Text;

[SupportedOSPlatform("windows6.1")]
class Impresora
{

    private PrintDocument? printDocument;

    public Dictionary<string, dynamic> HandleCommand(dynamic data) 
    {
        string command = data.command;

        if (command == "GetPrinters")
        {
            return GetInstalledPrinters();
        }

        if (command == "PrintTicket")
        {
            return PrintTicket(data);
        }

        if (command == "PrintTicketCorte")
        {
            return PrintTicketCorte(data);
        }

        if (command == "TestPrint") {
            return PrintTestTicket(data);
        }

        return new Dictionary<string, dynamic>
        {
            { "success", false },
            { "message", "Comando no reconocido" }
        };
    }

    private Dictionary<string, dynamic> GetInstalledPrinters()
    {
        List<string> printers = new List<string>();
        foreach (string printer in PrinterSettings.InstalledPrinters)
        {
            printers.Add(printer);
        }

        return new Dictionary<string, dynamic>
        {
            { "success", true },
            { "printers", printers }
        };
    }

    private Dictionary<string, dynamic> PrintTicket(dynamic data) 
    {   
        string printerName = data.printerName;
        Venta venta = new Venta(data.venta);
        string fuente = data.fuente;
        printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;

        printDocument.PrintPage += (sender, e) => DrawVentaTicket(e, venta, fuente);

        try 
        {
            printDocument.Print();
            return new Dictionary<string, dynamic>
            {
                { "success", true },
                { "message", "Ticket impreso correctamente" }
            };
        }
        catch(Exception e)
        {
            return new Dictionary<string, dynamic>
            {
                { "success", false },
                { "message", e.Message }
            };
        }
    }

    private Dictionary<string, dynamic> PrintTicketCorte(dynamic data) 
    {
        string printerName = data.printerName;
        Corte corte = new Corte(data.corte);
        string fuente = data.fuente;
        printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;

        printDocument.PrintPage += (sender, e) => DrawVentaTicketCorte(e, corte, fuente);

        try 
        {
            printDocument.Print();
            return new Dictionary<string, dynamic>
            {
                { "success", true },
                { "message", "Ticket de corte impreso correctamente" }
            };
        }
        catch(Exception e)
        {
            return new Dictionary<string, dynamic>
            {
                { "success", false },
                { "message", e.Message }
            };
        }
    }

    private Dictionary<string, dynamic> PrintTestTicket(dynamic data)
    {
        printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = data.printerName;
        string fuente = data.fuente;

        printDocument.PrintPage += (sender, e) => DrawTestTicket(e, fuente);

        try
        {
            printDocument.Print();
            return new Dictionary<string, dynamic>
            {
                { "success", true },
                { "message", "Ticket de prueba impreso correctamente" }
            };
        } 
        catch(Exception e) 
        {
            return new Dictionary<string, dynamic>
            {
                { "success", false },
                { "message", e.Message }
            };
            
        }
    }

    private void DrawVentaTicket(PrintPageEventArgs e, Venta venta, string fuente)
    {
        // Load the image
        string imagePath = "./assets/logo.png"; // Specify the image path
        Image logo = Image.FromFile(imagePath);

        float printWidth = e.Graphics!.VisibleClipBounds.Width;

        // Draw the logo at the very top
        float logoWidth = 300; // Adjust width as necessary
        float logoHeight = 100; // Adjust height as necessary
        float logoX = (printWidth - logoWidth) / 2; // Center the image horizontally
        float logoY = 5; // Reduced space at the top
        e.Graphics.DrawImage(logo, new RectangleF(logoX, logoY, logoWidth, logoHeight));
        
        string header = "\nSAN JUAN DEL RIO, QRO.\nTEL: 4276903455";
        string ticketHeader = $"{venta.IdVenta}\n\nSucursal: {venta.Sucursal}\nFecha: {venta.Fecha}\nHora: {venta.Hora}\n";
        string divider = "--------------------------------------------";
        string footer = "\nGracias por su compra";

        int headerFontSize = 14;
        int bodyFontSize = 8;
        int footerFontSize = 14;

        Font headerFont = new Font(fuente, headerFontSize, FontStyle.Regular);
        Font bodyFont = new Font(fuente, bodyFontSize, FontStyle.Regular);
        Font footerFont = new Font(fuente, footerFontSize, FontStyle.Italic);

        // Adjust header position to account for reduced space
        float headerY = logoY + logoHeight + 5; // Reduced gap below the logo
        SizeF headerSize = e.Graphics.MeasureString(header, headerFont);
        float headerX = (printWidth - headerSize.Width) / 2;
        e.Graphics.DrawString(header, headerFont, Brushes.Black, new PointF(headerX, headerY));

        StringBuilder ticketContentBuilder = new StringBuilder();
        ticketContentBuilder.AppendLine(ticketHeader);
        ticketContentBuilder.AppendLine(divider);
        ticketContentBuilder.AppendLine("Producto       Cant    Precio       Total");

        foreach (var producto in venta.Productos)
        {
            // Truncate product name to 12 characters
            string truncatedName = producto.NombreProducto.Length > 12 
                ? producto.NombreProducto.Substring(0, 12) 
                : producto.NombreProducto;

            ticketContentBuilder.AppendLine(
                $"{truncatedName,-13} {producto.Cantidad,3} {producto.Importe,12:C2} {producto.Total,10:C2}");
        }

        ticketContentBuilder.AppendLine(divider);
        ticketContentBuilder.AppendLine($"Total: {venta.TotalGeneral:C2}");

        // Draw Content
        string ticketContent = ticketContentBuilder.ToString();
        SizeF ticketContentSize = e.Graphics.MeasureString(ticketContent, bodyFont);
        float ticketContentY = headerY + headerSize.Height + 10; // Reduced space below the header
        e.Graphics.DrawString(ticketContent, bodyFont, Brushes.Black, new PointF(10, ticketContentY));

        // Draw Footer
        SizeF footerSize = e.Graphics.MeasureString(footer, footerFont);
        float footerX = (printWidth - footerSize.Width) / 2;
        float footerY = ticketContentY + ticketContentSize.Height + 10; // Reduced space above the footer
        e.Graphics.DrawString(footer, footerFont, Brushes.Black, new PointF(footerX, footerY));

        // Draw Footer Lines
        float lineSpacing = bodyFont.GetHeight();
        for (int i = 0; i < 3; i++) 
        {
            e.Graphics.DrawLine(Pens.Black, 10, footerY + footerSize.Height + (i * lineSpacing), printWidth - 10, footerY + footerSize.Height + (i * lineSpacing));
        }
    }

    private void DrawVentaTicketCorte(PrintPageEventArgs e, Corte corte, string fuente)
{
    float printWidth = e.Graphics!.VisibleClipBounds.Width;

    string header = "CORTE DE CAJA";
    string ticketHeader = $"{corte.IdCorte}\n\nSucursal: {corte.Sucursal}\nFecha: {corte.FechaCorte}\nHora de corte: {corte.HoraCorte}\nHora inicio de corte: {corte.HoraInicio}\nHora fin de corte: {corte.HoraFin}\n";
    string divider = "--------------------------------------------";
    string footer = "";

    int headerFontSize = 15;
    int bodyFontSize = 8;
    int footerFontSize = 14;

    Font headerFont = new Font(fuente, headerFontSize, FontStyle.Regular);
    Font bodyFont = new Font(fuente, bodyFontSize, FontStyle.Regular);
    Font footerFont = new Font(fuente, footerFontSize, FontStyle.Italic);

    // Adjust header position without logo
    float headerY = 10; // Starting position for header
    SizeF headerSize = e.Graphics.MeasureString(header, headerFont);
    float headerX = (printWidth - headerSize.Width) / 2;
    e.Graphics.DrawString(header, headerFont, Brushes.Black, new PointF(headerX, headerY));

    StringBuilder ticketContentBuilder = new StringBuilder();
    ticketContentBuilder.AppendLine(ticketHeader);
    ticketContentBuilder.AppendLine(divider);
    ticketContentBuilder.AppendLine("Producto       Cant    Precio       Total");

    foreach (var producto in corte.Productos)
    {
        // Truncate product name to 12 characters
        string truncatedName = producto.NombreProducto.Length > 12 
            ? producto.NombreProducto.Substring(0, 12) 
            : producto.NombreProducto;

        ticketContentBuilder.AppendLine(
            $"{truncatedName,-13} {producto.Cantidad,3} {producto.Importe,12:C2} {producto.Total,10:C2}");
    }

    ticketContentBuilder.AppendLine(divider);
    ticketContentBuilder.AppendLine($"Unidades vendidas: {corte.UnidadesVendidas}");
    ticketContentBuilder.AppendLine($"Total: {corte.TotalGeneral:C2}");

    // Draw Content
    string ticketContent = ticketContentBuilder.ToString();
    SizeF ticketContentSize = e.Graphics.MeasureString(ticketContent, bodyFont);
    float ticketContentY = headerY + headerSize.Height + 10; // Reduced space below the header
    e.Graphics.DrawString(ticketContent, bodyFont, Brushes.Black, new PointF(10, ticketContentY));

    // Draw Footer
    SizeF footerSize = e.Graphics.MeasureString(footer, footerFont);
    float footerX = (printWidth - footerSize.Width) / 2;
    float footerY = ticketContentY + ticketContentSize.Height + 10; // Reduced space above the footer
    e.Graphics.DrawString(footer, footerFont, Brushes.Black, new PointF(footerX, footerY));

    // Draw Footer Lines
    float lineSpacing = bodyFont.GetHeight();
    for (int i = 0; i < 3; i++) 
    {
        e.Graphics.DrawLine(Pens.Black, 10, footerY + footerSize.Height + (i * lineSpacing), printWidth - 10, footerY + footerSize.Height + (i * lineSpacing));
    }
}

    static void DrawTestTicket(PrintPageEventArgs e, string fuente)
    {
        Graphics g = e.Graphics!;
        Font font = new Font(fuente, 10, FontStyle.Bold);
        float yPos = 10;
        int leftMargin = 10;

        // Cargar la imagen (Asegúrate de que la ruta es correcta)
        string imagePath = "./assets/logo.png";  // Cambia esta ruta por la de tu imagen
        try
        {
            Image logo = Image.FromFile(imagePath);
            int imageWidth = 300; // Ajusta según el ancho del ticket
            int imageHeight = 100; // Ajusta según la proporción de la imagen

            // Dibujar la imagen en el ticket
            g.DrawImage(logo, new Rectangle(leftMargin, (int)yPos, imageWidth, imageHeight));
            yPos += imageHeight + 10; // Ajustar la posición después de la imagen
        }
        catch (Exception ex)
        {
            g.DrawString("Error cargando imagen", font, Brushes.Black, leftMargin, yPos);
            yPos += 20;
            Console.WriteLine("No se pudo cargar la imagen: " + ex.Message);
        }

        // Encabezado
        g.DrawString("==== PRUEBA DE TICKET ====", font, Brushes.Black, leftMargin, yPos);
        yPos += 20;

        // Fecha y hora
        g.DrawString("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), font, Brushes.Black, leftMargin, yPos);
        yPos += 20;

        // Contenido de prueba
        g.DrawString("Este es un ticket de prueba.", font, Brushes.Black, leftMargin, yPos);
        yPos += 20;

        g.DrawString("Impresora: " + e.PageSettings.PrinterSettings.PrinterName, font, Brushes.Black, leftMargin, yPos);
        yPos += 20;

        // Pie de página
        g.DrawString("======================", font, Brushes.Black, leftMargin, yPos);
    }
}
