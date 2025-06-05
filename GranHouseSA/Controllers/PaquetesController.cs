using Microsoft.AspNetCore.Mvc;
using GranHouseSA.Models;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using GranHouseSA.Data;
using Newtonsoft.Json;

namespace GranHouseSA.Controllers
{
    public class PaquetesController : ControllerBase
    {
        private readonly AppDbContext dbContex;

        public PaquetesController()
        {
            
        }
        //Constructor con parámetros recibe la inyección del objeto context
        public PaquetesController(AppDbContext context)
        {
            //La referencia se almacena en la variable dbContext
            dbContex = context;
        }

        // GET: api/<PaquetesController>
        [HttpGet]
        public IEnumerable<Paquete> Get()
        {
            return dbContex.paquete.ToList();
        }

        public List<Paquete> GetPaquetes()
        {
            return new List<Paquete>();
        }

        // GET api/<PaquetesController>/5
        [HttpGet("{id}")]
        public Paquete GetPaquete(int id)
        {
            try
            {
                var paquete = dbContex.paquete.Find(id);
                return paquete;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        // POST api/<PaquetesController>
        [HttpPost("agregar")]
        public ActionResult <Paquete> CreatePaquete (Paquete paquete)
        {
            dbContex.paquete.Add(paquete);
            dbContex.SaveChanges();

            return CreatedAtAction(nameof(GetPaquete), new { id = paquete.ID }, paquete);
        }

        // PUT api/<PaquetesController>/5
        [HttpPut("{id}")]
        public IActionResult UpdatePaquete(int id, Paquete paquete)
        {
            if (id != paquete.ID)
            {
                return BadRequest();
            }

            dbContex.paquete.Update(paquete);
            dbContex.SaveChanges();


            return NoContent();
        }

        // DELETE api/<PaquetesController>/5
        [HttpDelete("{id}")]
        public IActionResult DeletePaquete(int id)
        {
            var paquete = dbContex.paquete.Find(id);

            if (paquete == null)
            {
                return NotFound();
            }

            dbContex.paquete.Remove(paquete);
            dbContex.SaveChanges();

            return NoContent();
        }
    }
}
