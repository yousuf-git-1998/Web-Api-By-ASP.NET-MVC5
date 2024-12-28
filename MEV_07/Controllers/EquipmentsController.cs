using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using MEV_07.Models;
using MEV_07.ViewModels;

namespace MEV_07.Controllers
{
    public class EquipmentsController : ApiController
    {
        private readonly EquipmentDbContext db = new EquipmentDbContext();

        // GET: api/Equipments
        public IQueryable<Equipment> GetEquipments()
        {
            return db.Equipments;
        }
        // With child
        // /////////////////////////////////////////////////
        [Route("api/Equipments/Maintenance/Include")]
        public IQueryable<Equipment> GetEquipmentWithMaitenance()
        {
            return db.Equipments.Include(x => x.Maintenances);
        }
        // GET: api/Equipments/5
        [ResponseType(typeof(Equipment))]
        public IHttpActionResult GetEquipment(int id)
        {
            Equipment equipment = db.Equipments.Find(id);
            if (equipment == null)
            {
                return NotFound();
            }

            return Ok(equipment);
        }
        // With child
        // /////////////////////////////////////////////////
        [Route("api/Equipments/{id}/Include")]
        public IHttpActionResult GetEquipmentWithMaintainence(int id)
        {
            Equipment equipment = db.Equipments.Include(x => x.Maintenances).FirstOrDefault(x => x.EquipmentId == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return Ok(equipment);
        }
        // With child
        // /////////////////////////////////////////////////
        // PUT: api/Equipments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEquipment(int id, Equipment equipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != equipment.EquipmentId)
            {
                return BadRequest();
            }

            var eq = db.Equipments.FirstOrDefault(x => x.EquipmentId == id);
            if (eq == null)
            {
                return NotFound();
            }
            eq.Model = equipment.Model;
            eq.EquipmentType = equipment.EquipmentType;
            eq.CommissionDate = equipment.CommissionDate;
            eq.CapacityInTon = equipment.CapacityInTon;
            eq.Picture = equipment.Picture;
            eq.IdentificationCode = equipment.IdentificationCode;
            eq.IsActive = equipment.IsActive;
            db.Database.ExecuteSqlCommand($"DELETE FROM Maintenances WHERE EquipmentId={id}");
            foreach (var m in equipment.Maintenances)
            {
                db.Maintenances.Add(new Maintenance { EquipmentId = id, StartDate = m.StartDate, EndDate = m.EndDate, Comment = m.Comment });
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Equipments
        [ResponseType(typeof(Equipment))]
        public IHttpActionResult PostEquipment(Equipment equipment)
        {


            db.Equipments.Add(equipment);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = equipment.EquipmentId }, equipment);
        }

        // DELETE: api/Equipments/5
        [ResponseType(typeof(Equipment))]
        public IHttpActionResult DeleteEquipment(int id)
        {
            Equipment equipment = db.Equipments.Find(id);
            if (equipment == null)
            {
                return NotFound();
            }

            db.Equipments.Remove(equipment);
            db.SaveChanges();

            return Ok(equipment);
        }
        // Picture upload
        // //////////////////////
        [HttpPost, Route("api/Equipments/Image/Upload")]
        public IHttpActionResult Upload()
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                var file = HttpContext.Current.Request.Files[0];
                string ext = Path.GetExtension(file.FileName);
                string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                string savePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Pictures"), f);
                file.SaveAs(savePath);
                return Ok(new FileUploadResult { NewFileName = f });
            }
            return BadRequest();

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EquipmentExists(int id)
        {
            return db.Equipments.Count(e => e.EquipmentId == id) > 0;
        }
    }


}