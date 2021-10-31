using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var item = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            item.Satellites = _context.CelestialObjects.Where(i => i.OrbitedObjectId == item.Id).ToList();

            return Ok(item);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var items = _context.CelestialObjects.Where(i => i.Name == name).ToList();

            if (items.Count == 0)
            {
                return NotFound();
            }

            foreach (var item in items)
            {
                item.Satellites = _context.CelestialObjects.Where(i => i.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(items);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _context.CelestialObjects.ToList();

            foreach (var item in items)
            {
                item.Satellites = _context.CelestialObjects.Where(i => i.OrbitedObjectId == item.Id).ToList();
            }

            return Ok(items);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject item)
        {
            _context.CelestialObjects.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", routeValues: new { id = item.Id }, value: item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject item)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = item.Name;
            celestialObject.OrbitalPeriod = item.OrbitalPeriod;
            celestialObject.OrbitedObjectId = item.OrbitedObjectId;

            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(i => i.Id == id);

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name;

            _context.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var items = _context.CelestialObjects.Where(i => i.Id == id || i.OrbitedObjectId == id).ToList();

            if (items.Count == 0)
            {
                return NotFound();
            }

            _context.RemoveRange(items);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
