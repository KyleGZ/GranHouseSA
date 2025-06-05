using GranHouseSA.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

//Importaciones para PDF
using IronPdf;

namespace GranHouseSA.Data
{
    public class Email
    {
        private static GranHouseSA.Data.TipoCambioApi _tipoCambio = new GranHouseSA.Data.TipoCambioApi();
        public void Enviar(Compras compra, string correo, string numeroCheque, string bancoCheque, string nombreTarjeta, string numeroTarjeta)
        {
            //Se crea una instancia del objeto email
            MailMessage email = new MailMessage();

            //Asunto
            email.Subject = "Datos de reservación en plataforma Hotel Beach S.A";

            //Destinatarios
            email.To.Add(new MailAddress(correo));

            //Emisor del correo
            email.From = new MailAddress("beachhotelsa@gmail.com", "BeachHotelSA");

            string html = "<br><br><br><br><br><h1 style='text-align:center'>Reservación</h1>";
            html += "<br><br>Bienvenido a Hotel Beach S.A, gracias por formar parte de nuestra plataforma.";

            //Si la transaccion es efectivo creo un bloque html especifico
            if (compra.Transaccion.Equals("Efectivo"))
            {
                //Se construye la vista HTML para el body del email
                html += "<br> A continuación detallamos los datos de tu reservación en nuestra plataforma web:";
                html += "<br><br><b>Paquete reservado: </b>" + compra.nombrePaquete;
                html += "<br><b>Noches reservadas: </b>" + compra.CantidadNoches;
                html += "<br><b>Modalidad de pago: </b>" + compra.Transaccion;
                html += "<br><b>Descuento: $</b>" + compra.Descuento;
                html += "<br><b>Total en dolares: $</b>" + compra.MontoFinal;
                html += "<br><b>Total en colones: ₡</b>" + (compra.MontoFinal * _tipoCambio.extraerTipoCambio().venta).ToString("0.00");
                html += "<br><b>Fecha ingreso: </b>" + compra.FechaIngreso;
                html += "<br><br><br><b>No responda a este correo porque fue generado de forma automática por la plataforma Hotel Beach S.A</b>";
            }

            //Si la transaccion es tarjeta creo un bloque html especifico
            if (compra.Transaccion.Equals("Tarjeta"))
            {
                //Se construye la vista HTML para el body del email
                html += "<br> A continuación detallamos los datos de tu reservación en nuestra plataforma web:";
                html += "<br><br><b>Paquete reservado: </b>" + compra.nombrePaquete;
                html += "<br><b>Noches reservadas: </b>" + compra.CantidadNoches;
                html += "<br><b>Modalidad de pago: </b>" + compra.Transaccion;
                html += "<br><b>Titular de la tarjeta: </b>" + nombreTarjeta;
                html += "<br><b>Numero de tarjeta: </b>" + numeroTarjeta;
                html += "<br><b>Total en dolares: $</b>" + compra.MontoFinal;
                html += "<br><b>Total en colones: ₡</b>" + (compra.MontoFinal * _tipoCambio.extraerTipoCambio().venta).ToString("0.00");
                html += "<br><b>Fecha ingreso: </b>" + compra.FechaIngreso;
                html += "<br><br><br><b>No responda a este correo porque fue generado de forma automática por la plataforma Hotel Beach S.A</b>";
            }

            //Si la transaccion es cheque creo un bloque html especifico
            if (compra.Transaccion.Equals("Cheque"))
            {
                //Se construye la vista HTML para el body del email
                html += "<br> A continuación detallamos los datos de tu reservación en nuestra plataforma web:";
                html += "<br><br><b>Paquete reservado: </b>" + compra.nombrePaquete;
                html += "<br><b>Noches reservadas: </b>" + compra.CantidadNoches;
                html += "<br><b>Modalidad de pago: </b>" + compra.Transaccion;
                html += "<br><b>Numero de cheque: </b>" + numeroCheque;
                html += "<br><b>Banco proveniente del cheque: </b>" + bancoCheque;
                html += "<br><b>Total en dolares: $</b>" + compra.MontoFinal;
                html += "<br><b>Total en colones: ₡</b>" + (compra.MontoFinal * _tipoCambio.extraerTipoCambio().venta).ToString("0.00");
                html += "<br><b>Fecha ingreso: </b>" + compra.FechaIngreso;
                html += "<br><br><br><b>No responda a este correo porque fue generado de forma automática por la plataforma Hotel Beach S.A</b>";
            }

            // Generar el PDF a partir del HTML con IronPDF
            HtmlToPdf Renderer = new HtmlToPdf();
            PdfDocument pdf = Renderer.RenderHtmlAsPdf(html);

            // Guardar el PDF en un archivo temporal
            string tempFilePath = "Reservación_"+ compra.nombreCliente +".pdf";
            pdf.SaveAs(tempFilePath);

            // Agregar el archivo PDF adjunto al correo
            Attachment attachment = new Attachment(tempFilePath, MediaTypeNames.Application.Pdf);
            email.Attachments.Add(attachment);


            //Configuracion del protocolo de comunicacion smtp
            SmtpClient smtp = new SmtpClient();

            //Servidor de correo a implemetar
            smtp.Host = "smtp.gmail.com";

            //Puerto de comunicacion
            smtp.Port = 587;

            //Se indica si el buzon utiliza seguridad tipo SSL
            smtp.EnableSsl = true;

            //Se indica si el buzon utiliza credenciales por default
            smtp.UseDefaultCredentials = false;

            //Se asignan los datos para las credenciales
            smtp.Credentials = new NetworkCredential("beachhotelsa@gmail.com", "livk nfdt fcgy lgic"); 

            //Metodo para enviar el email
            smtp.Send(email);
            email.Dispose();
            smtp.Dispose();

            // Eliminar el archivo temporal del PDF
            File.Delete(tempFilePath);
        }

        public void EnviarCancelacion(Reservacion reserva, string correo)
        {
            //Se crea una instancia del objeto email
            MailMessage email = new MailMessage();

            //Asunto
            email.Subject = "Cancelación de su reserva en plataforma Hotel Beach S.A";

            //Destinatarios
            email.To.Add(new MailAddress(correo));

            //Emisor del correo
            email.From = new MailAddress("beachhotelsa@gmail.com", "BeachHotelSA");

            string html = "Hotel Beach S.A le da un cordial saludo para comunicarle sobre la cancelación de su reservación.";

            //Se construye la vista HTML para el body del email
            html += "<br> Su reservación a sido cancelada con éxito.";
            html += "<br><b>Nos estaremos comunicando con usted para asuntos de reembolso.</b>";
            html += "<br><b>No responda a este correo porque fue generado de forma automática por la plataforma Hotel Beach S.A</b>";


            //Se indica el contenido en html
            email.IsBodyHtml = true;

            //Se indica la prioridad, que debe ser prioridad normal
            email.Priority = MailPriority.Normal;

            //Se instancia la vista del html para el cuerpo del body del email
            AlternateView view = AlternateView.CreateAlternateViewFromString(html,
                Encoding.UTF8, MediaTypeNames.Text.Html);

            //Se agrega la vista html al cuerpo del email
            email.AlternateViews.Add(view);

            //Configuracion del protocolo de comunicacion smtp
            SmtpClient smtp = new SmtpClient();

            //Servidor de correo a implemetar
            smtp.Host = "smtp.gmail.com";

            //Puerto de comunicacion
            smtp.Port = 587;

            //Se indica si el buzon utiliza seguridad tipo SSL
            smtp.EnableSsl = true;

            //Se indica si el buzon utiliza credenciales por default
            smtp.UseDefaultCredentials = false;

            //Se asignan los datos para las credenciales
            smtp.Credentials = new NetworkCredential("beachhotelsa@gmail.com", "livk nfdt fcgy lgic");

            //Metodo para enviar el email
            smtp.Send(email);
            email.Dispose();
            smtp.Dispose();
        }

        public void EnviarCancelacionPaquete(Reservacion reserva, string correo)
        {
            //Se crea una instancia del objeto email
            MailMessage email = new MailMessage();

            //Asunto
            email.Subject = "Cancelación de su reserva en plataforma Hotel Beach S.A";

            //Destinatarios
            email.To.Add(new MailAddress(correo));

            //Emisor del correo
            email.From = new MailAddress("beachhotelsa@gmail.com", "BeachHotelSA");

            string html = "Hotel Beach S.A le da un cordial saludo para comunicarle sobre la cancelación de su reservación.";

            //Se construye la vista HTML para el body del email
            html += "<br> Lamentamos informarle que su paquete ha caducado, su reservación en nuestras instalaciones ha terminado.";
            html += "<br><b>Muchas gracias por formar parte de nuestra comunidad.</b>";
            html += "<br><b>No responda a este correo porque fue generado de forma automática por la plataforma Hotel Beach S.A</b>";


            //Se indica el contenido en html
            email.IsBodyHtml = true;

            //Se indica la prioridad, que debe ser prioridad normal
            email.Priority = MailPriority.Normal;

            //Se instancia la vista del html para el cuerpo del body del email
            AlternateView view = AlternateView.CreateAlternateViewFromString(html,
                Encoding.UTF8, MediaTypeNames.Text.Html);

            //Se agrega la vista html al cuerpo del email
            email.AlternateViews.Add(view);

            //Configuracion del protocolo de comunicacion smtp
            SmtpClient smtp = new SmtpClient();

            //Servidor de correo a implemetar
            smtp.Host = "smtp.gmail.com";

            //Puerto de comunicacion
            smtp.Port = 587;

            //Se indica si el buzon utiliza seguridad tipo SSL
            smtp.EnableSsl = true;

            //Se indica si el buzon utiliza credenciales por default
            smtp.UseDefaultCredentials = false;

            //Se asignan los datos para las credenciales
            smtp.Credentials = new NetworkCredential("beachhotelsa@gmail.com", "livk nfdt fcgy lgic");

            //Metodo para enviar el email
            smtp.Send(email);
            email.Dispose();
            smtp.Dispose();
        }
    }
}
