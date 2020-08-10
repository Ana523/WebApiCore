using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserWebApi.Models;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationContext _context;

        public ReservationController(ReservationContext context)
        {
            _context = context;
        }

        // GET: api/Reservation/GetReservations
        [HttpGet]
        [Route("GetReservations")]
        public IEnumerable<Reservation> GetReservations()
        {
            return _context.Reservations;
        }

        // GET: api/Reservation/GetReservationById/5
        [HttpGet]
        [Route("GetReservationById/{id}")]
        public async Task<IActionResult> GetReservation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        // PUT: api/Reservation/EditReservation
        [HttpPut]
        [Route("EditReservation")]
        public async Task<IActionResult> PutReservation(Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Reservation objReservation = new Reservation();
                objReservation = await _context.Reservations.FindAsync(reservation.Id);
                
                if (objReservation != null)
                {
                    objReservation.FirstName = reservation.FirstName;
                    objReservation.LastName = reservation.LastName;
                    objReservation.DateFrom = reservation.DateFrom;
                    objReservation.DateTo = reservation.DateTo;
                    objReservation.NumOfPeople = reservation.NumOfPeople;
                    objReservation.NumOfRooms = reservation.NumOfRooms;
                }
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
            return Ok(reservation);
        }

        // POST: api/Reservation/AddNewReservation
        [HttpPost]
        [Route("AddNewReservation")]
        public async Task<IActionResult> PostReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
        }

        // DELETE: api/Reservation/DeleteReservation/5
        [HttpDelete]
        [Route("DeleteReservation/{id}")]
        public async Task<IActionResult> DeleteReservation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}