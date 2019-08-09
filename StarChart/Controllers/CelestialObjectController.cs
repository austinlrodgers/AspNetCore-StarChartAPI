﻿using System;
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

        public CelestialObjectController(ApplicationDbContext dbContext)
        {
            this._context = dbContext;
        }
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = this._context.CelestialObjects.Find(id);

            if (celestialObject == null)
            {
                return NotFound();
            }
            celestialObject.Satellites = this._context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = this._context.CelestialObjects.Where(x => x.Name == name).ToList();

            if (!celestialObjects.Any())
                return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = this._context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = this._context.CelestialObjects.ToList();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = this._context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            this._context.CelestialObjects.Add(celestialObject);

            this._context.SaveChanges();

            return CreatedAtRoute("GetById", new CelestialObject { Id = celestialObject.Id });
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObjectToUpdate)
        {
            var celestialObject = this._context.CelestialObjects.Where(e => e.Id == id).FirstOrDefault();

            if (celestialObject == null)
            {
                NotFound();
            }

            celestialObject.Name = celestialObjectToUpdate.Name;
            celestialObject.OrbitalPeriod = celestialObjectToUpdate.OrbitalPeriod;
            celestialObject.OrbitedObjectId = celestialObjectToUpdate.OrbitedObjectId;

            this._context.CelestialObjects.Update(celestialObject);
            this._context.SaveChanges();

            return NoContent();
        }

        [HttpPost("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = this._context.CelestialObjects.Where(e => e.Id == id).FirstOrDefault();

            if (celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name;

            this._context.CelestialObjects.Update(celestialObject);
            this._context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialBodies = this._context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id).ToList();

            if (!celestialBodies.Any())
            {
                NotFound();
            }

            this._context.CelestialObjects.RemoveRange(celestialBodies);
            this._context.SaveChanges();

            return NoContent();
        }
    }
}
