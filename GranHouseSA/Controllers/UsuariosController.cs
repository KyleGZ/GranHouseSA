using GranHouseSA.Data;
using GranHouseSA.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GranHouseSA.Controllers
{
    public class UsuariosController : Controller
    {
        //Se instancia el dbcontext
        private static AppDbContext _context;

        //Variable para los mensajes del temp
        private static string mensaje = "";

        public UsuariosController(AppDbContext context)
        {
            //Se inicializa la clase
            _context = context;
        }

        public IActionResult Index()
        {
            //Se verifica si está autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Verifico el rol para acceder
                if (User.IsInRole("A"))
                {
                    //Retorno la vista con la lista
                    return View(_context.usuarios.ToList());
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([Bind()] Usuarios usuario)
        { 
            //Variable que valida el usuario
            var temp = ValidarUsuario(usuario);

            //Verifica si existe
            if (temp != null)
            {

                //Se crea la instancia para entidad del usuario y el tipo de autenticación
                var userClaims = new List<Claim>() { new Claim(ClaimTypes.Name, temp.NickName), new Claim(ClaimTypes.Role, temp.Rol.ToString()),
                    new Claim(ClaimTypes.Email, temp.Email) };
                var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });

                //Se realiza la autenticación dentro del contexto http
                HttpContext.SignInAsync(userPrincipal);

                //Se ubica al usuario en la pagina de inicio
                return RedirectToAction("Index", "Home");
            }
            TempData["Mensaje"] = mensaje;
            return View(usuario);
        }

        public async Task<IActionResult> Logout()
        {
            //Salir de la sesion
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult CrearCuenta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrearCuenta([Bind()] Usuarios usuario)
        {
            //Verifica los campos
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            else
            {
                //Verifica si el usuario viene lleno
                if (usuario != null)
                {
                    //Verifico si existe un usuario con el email registrado
                    var temp = _context.usuarios.FirstOrDefault(x => x.Email.Equals(usuario.Email));

                    //Verifico si existe o no
                    if (temp == null)
                    {
                        //Agrego el rol de cliente automaticamente
                        usuario.Rol = 'C';

                        //Agrego el usuario
                        _context.usuarios.Add(usuario);
                        _context.SaveChanges();
                        TempData["MensajeCreado"] = "Usuario creado correctamente.";
                        return RedirectToAction("Login", "Usuarios");
                    }
                    else
                    {
                        TempData["MensajeError"] = "Ya hay una cuenta con este email.";
                        return View();
                    }
                }
                else
                {
                    TempData["MensajeError"] = "No se logró crear la cuenta..";
                    return View();
                }
            }
        }

        private Usuarios ValidarUsuario(Usuarios temp)
        {
            Usuarios autorizado = null;

            //Se busca el usuario en la base de datos con el email autenticado
            var user = _context.usuarios.FirstOrDefault(u => u.Email == temp.Email);

            //Se pregunta si existen datos del usuario autenticado
            if (user != null)
            {
                //Se verifica su password
                if (user.Password.Equals(temp.Password))
                {
                    autorizado = user;
                }
                else
                {
                    mensaje = "Contraseña incorrecta";
                }
            }
            else
            {
                mensaje = "No pudimos encontrar tu cuenta";
            }
            return autorizado;
        }

        [HttpGet]
        public IActionResult Create()
        {
            //Se verifica si está autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Verifica el rol para el acceso
                if (User.IsInRole("A"))
                {
                    //Retorna la vista
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
        public IActionResult Create([Bind()] Usuarios usuario)
        {
            //Verifica los campos
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            else
            {
                //Se verifica si el objeto viene vacio
                if (usuario != null)
                {
                    //Verifico si existe un usuario con el email registrado
                    var temp = _context.usuarios.FirstOrDefault(u => u.Email == usuario.Email);

                    //Verifico si existe o no
                    if (temp == null)
                    {
                        //Se agrega el usuario
                        _context.usuarios.Add(usuario);
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["UsuarioError"] = "Ya existe una cuenta asociada con ese correo. ¡Prueba otro!";
                        return View(usuario);
                    }
                }
                else
                {
                    TempData["UsuarioError"] = "No se logró editar el usuario...";
                    return View(usuario);
                }
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            //Se verifica si está autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Verifica el rol para el acceso
                if (User.IsInRole("A"))
                {
                    //Retorna la vista
                    return View(_context.usuarios.Find(id));
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
        public IActionResult Edit([Bind()] Usuarios usuario)
        {
            //Verifica los campos
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            else
            {
                //Verifica que el usuario no llegue vacio
                if (usuario != null)
                {
                    //Verifico que no exista un usuario con el email registrado
                    var temp = _context.usuarios.FirstOrDefault(u => u.Email == usuario.Email);

                    //Se verifica si existe o no
                    if (temp == null)
                    {
                        //Se elimina lo que esté guardado temporalmente en el dbcontext
                        _context.ChangeTracker.Clear();

                        //Se actualiza el usuario
                        _context.usuarios.Update(usuario);
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //Si el correo ya existe pero es de este usuario, igual se deja modificar
                        if (temp.ID == usuario.ID)
                        {
                            //Se elimina lo que esté guardado temporalmente en el dbcontext
                            _context.ChangeTracker.Clear();

                            //Se actualiza el usuario
                            _context.usuarios.Update(usuario);
                            _context.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        TempData["EditError"] = "Ya existe una cuenta asociada con ese correo. ¡Prueba otro!";
                        return View(usuario);
                    }
                }
                else
                {
                    TempData["EditError"] = "No se logró editar el usuario...";
                    return View(usuario);
                }
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            //Se verifica si está autenticado
            if (User.Identity.IsAuthenticated)
            {
                //Se verifica el rol para el acceso
                if (User.IsInRole("A"))
                {
                    //Se busca el usuario con el id
                    var temp = _context.usuarios.Find(id);

                    //Se verifica si existe o no
                    if (temp != null)
                    {
                        //Se retorna la vista
                        return View(_context.usuarios.Find(id));
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
            //Se busca el usuario con el id
            var temp = _context.usuarios.Find(id);

            //Verifica si existe o no
            if (temp != null)
            {
                //Se elimina el usuario
                _context.usuarios.Remove(temp);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
    }
}
