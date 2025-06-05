using GranHouseSA.Data;
using GranHouseSA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GranHouseSA.Controllers
{
    public class ComprasController : Controller
    {
        private static Compras _compras;

        //Variable para manejar los datos del JSON
        private HttpApi _api;

        private static AppDbContext _context;

        //Se instancia la clase que contienen los metodos de paquete
        PaquetesController _paquetes;

        public ComprasController(AppDbContext context)
        {
            //Se inicializan las clases
            _context = context;
            _paquetes = new PaquetesController();
        }

        public IActionResult Index()
        {
            //Se verifica que esté autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Se verifica el rol para el acceso
                if (User.IsInRole("A") || User.IsInRole("M"))
                {
                    return View(_context.compras.ToList());
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuarios");
            }
        }

        [HttpGet]
        public IActionResult Reservar()
        {
            //Se verifica que esté autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Se verifica el rol para el acceso
                if (User.IsInRole("A") || User.IsInRole("C") || User.IsInRole("M"))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuarios");
            }

        }

        [HttpPost]
        public IActionResult Reservar(bool Efectivo, bool Tarjeta, bool Cheque, string numeroCheque, string bancoCheque, string nombreTarjeta, string numeroTarjeta, Compras compra)
        {
            //Se verifica que el objeto no llegue vacio
            if (compra != null)
            {
                //Se controla si no hay paquetes disponibles para no dejar reservar
                if (compra.IDPaquete == 00)
                {
                    TempData["MensajeError"] = "No hay paquetes disponibles para reservar";
                    return View(compra);
                }
                else
                {
                    //Buscamos al cliente con la cedula
                    var temp = _context.clientes.FirstOrDefault(x => x.Cedula.Equals(compra.Cedula));

                    //Verificamos si existe
                    if (temp != null)
                    {
                        //Buscamos en las reservaciones si existe un cliente con esa cedula
                        var existeReservacion = _context.reservaciones.FirstOrDefault(x => x.Cedula.Equals(compra.Cedula));

                        //Verificamos si existe
                        if (existeReservacion == null)
                        {
                            //Si la casilla checkbox llega en true, se guarda transaccion como efectivo
                            if (Efectivo == true)
                            {
                                compra.Transaccion = "Efectivo";
                            }

                            //Si la casilla checkbox llega en true, se guarda transaccion como tarjeta
                            if (Tarjeta == true)
                            {
                                compra.Transaccion = "Tarjeta";
                                //Se hace una cadena de caracteres con asteriscos del tamaño del numero de tarjeta menos los 4 ultimos
                                string asteriscos = new string('*', numeroTarjeta.Length - 4);

                                //Se sacan los ultimos 4 caracteres del numero de tarjeta
                                string ultimosDigitos = numeroTarjeta.Substring(numeroTarjeta.Length - 4);

                                //Se unen los asteriscos con los ultimos numeros de la tarjeta
                                numeroTarjeta = asteriscos + ultimosDigitos;

                            }

                            //Si la casilla checkbox llega en true, se guarda transaccion como cheque
                            if (Cheque == true)
                            {
                                compra.Transaccion = "Cheque";
                            }

                            //Se guarda el nombre del paquete dependiendo del IdPaquete
                            compra.nombrePaquete = _paquetes.GetPaquete(compra.IDPaquete).NombrePaquete;

                            //Se guarda el nombre del cliente
                            compra.nombreCliente = temp.Nombre;

                            //ME HACE LA RESERVACION
                            Reservacion reservacion = new Reservacion();
                            reservacion.Cedula = compra.Cedula;
                            reservacion.NombreCliente = compra.nombreCliente;
                            reservacion.IDPaquete = compra.IDPaquete;

                            _context.reservaciones.Add(reservacion);
                            _context.SaveChanges();

                            //ME GENERA LA FACTURA DE LA COMPRA DE LA RESERVACION
                            compra.TotalPagar = _paquetes.GetPaquete(compra.IDPaquete).Costo * compra.CantidadNoches;
                            compra.Prima = compra.MontoFinal - (compra.MontoFinal * _paquetes.GetPaquete(compra.IDPaquete).Prima);
                            compra.Mensualidad = (compra.MontoFinal - compra.Prima) / _paquetes.GetPaquete(compra.IDPaquete).Mensualidad;
                            _context.compras.Add(compra);

                            try
                            {
                                _context.SaveChanges();
                                
                                //Se envia el email de la reservación
                                if (this.EnviarEmail(compra, temp.Email, numeroCheque, bancoCheque, nombreTarjeta, numeroTarjeta))
                                {
                                    TempData["ClienteRegistrado"] = "Reservación exitosa";
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    TempData["MensajeError"] = "Reservación creada pero no se envió el email. Comuniquese con el administrador";
                                }
                            }
                            catch (Exception ex)
                            {

                                TempData["MensajeError"] = "No se logró hacer la reservación.." + ex.Message;
                            }
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["MensajeError"] = "Ya hay una reservación asociada a esta cédula";
                            return View(compra);
                        }
                    }
                    else
                    {
                        TempData["MensajeError"] = "Debes registrarte primero como cliente usando esta cédula";
                        return View(compra);
                    }
                }
            }
            else
            {
                return View(compra);
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            //Verifica si está autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Verifica el rol para el acceso
                if (User.IsInRole("A") || User.IsInRole("M"))
                {
                    //Se busca la compra con su id
                    var temp = _context.compras.Find(id);

                    //Verifica si existe
                    if (temp != null)
                    {
                        //Se muestra
                        return View(temp);
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuarios");
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            //Verifica que está autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Verifica el rol para el acceso
                if (User.IsInRole("A") || User.IsInRole("M"))
                {
                    //Busca la compra con el id
                    var temp = _context.compras.Find(id);

                    //Verifica si existe
                    if (temp != null)
                    {
                        //Elimina la compra
                        _context.compras.Remove(temp);
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Usuarios");
            }
        }

        private bool EnviarEmail(Compras compra, string correo, string numeroCheque, string bancoCheque, string nombreTarjeta, string numeroTarjeta)
        {
            try
            {
                //Variable de control
                bool enviado = false;

                //Se instancia el objeto
                Email email = new Email();

                //Se utiliza el metodo para enviar el correo
                email.Enviar(compra, correo, numeroCheque, bancoCheque, nombreTarjeta, numeroTarjeta);

                //Se indica que se envio
                enviado = true;

                //Enviamos el valor
                return enviado;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
