using GranHouseSA.Data;
using GranHouseSA.Models;
using Microsoft.AspNetCore.Mvc;

namespace GranHouseSA.Controllers
{
    public class ReservacionController : Controller
    {
        //Se instancia el dbcontext
        private static AppDbContext _context;

        //Se instancia la clase de ClientesAPI donde están todos los metodos.
        ClientesController _clientes;

        public ReservacionController(AppDbContext context)
        {
            //Se inicializan las clases
            _context = context;
            _clientes = new ClientesController();
        }

        public IActionResult Index()
        {
            //Se verifica si está autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Se verifica el rol para acceder
                if (User.IsInRole("A") || User.IsInRole("M"))
                {
                    //Se retorna la vista
                    return View(_context.reservaciones.ToList());
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
        public IActionResult Delete(int? id)
        {
            //Se verifica si está autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Se verifica el rol para acceder
                if (User.IsInRole("A") || User.IsInRole("M"))
                {
                    //Se busca la reservacion por id
                    var temp = _context.reservaciones.Find(id);

                    //Verifica si existe o no
                    if (temp != null)
                    {
                        //Se retorna la vista con el objeto
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
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            //Se busca la reservacion con el id
            var temp = _context.reservaciones.Find(id);

            //Verifica si exite o no
            if (temp != null)
            {  
                //Me traigo todos los clientes
                var clientes = _clientes.Get();

                //Recorro los clientes
                foreach (var item in clientes)
                {
                    //Verifico la cedula que está en la reservacion con la cedula de los clientes
                    if (item.Cedula.Equals(temp.Cedula)) 
                    {
                        //Elimino la reservacion
                        _context.reservaciones.Remove(temp);
                        try
                        {
                            _context.SaveChanges();

                            //Se envia el email
                            if (this.EnviarEmail(temp, item.Email))
                            {
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["MensajeError"] = "Reservación cancelada pero no se envió el email. Comuniquese con el administrador";
                            }
                        }
                        catch (Exception ex)
                        {

                            TempData["MensajeError"] = "No se logró cancelar la reservación.." + ex.Message;
                        }
                        return RedirectToAction("Index");
                    }
                }
                return View(temp);
            }
            else
            {
                return View();
            }
        }

        private bool EnviarEmail(Reservacion reservacion, string correo)
        {
            try
            {
                //Variable de control
                bool enviado = false;

                //Se instancia el objeto
                Email email = new Email();

                //Se utiliza el metodo para enviar el correo
                email.EnviarCancelacion(reservacion, correo);

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
