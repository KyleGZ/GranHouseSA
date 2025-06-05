using GranHouseSA.Data;
using GranHouseSA.Models;
using Microsoft.AspNetCore.Mvc;

namespace GranHouseSA.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDbContext dbContex;


        //Constructor con parámetros recibe la inyección del objeto context
        public ClientesController(AppDbContext context)
        {
            //La referencia se almacena en la variable dbContext
            dbContex = context;
        }
        public ClientesController()
        {
            
        }
        public IEnumerable<Clientes> Get()
        {
            return dbContex.clientes.ToList();
        }
        public ActionResult<Clientes> GetCliente(int id)
        {
            var cliente = dbContex.clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        public ActionResult<Clientes> CreateCliente(Clientes cliente)
        {
            dbContex.clientes.Add(cliente);
            dbContex.SaveChanges();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.ID }, cliente);
        }

        public IActionResult UpdateCliente(int id, Clientes cliente)
        {
            if (id != cliente.ID)
            {
                return BadRequest();
            }

            dbContex.clientes.Update(cliente);
            dbContex.SaveChanges();


            return NoContent();
        }
        public IActionResult DeleteCliente(int id)
        {
            var clientes = dbContex.clientes.Find(id);

            if (clientes == null)
            {
                return NotFound();
            }

            dbContex.clientes.Remove(clientes);
            dbContex.SaveChanges();

            return NoContent();
        }
    }
}
