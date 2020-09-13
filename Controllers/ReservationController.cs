using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserWebApi.Models;

namespace UserWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationContext _context;
        private readonly ILogger _logger;

        public ReservationController(ReservationContext context, ILogger<ReservationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Reservation/GetReservations
        [HttpGet]
        [Route("GetReservations")]
        public IEnumerable<Reservation> GetReservations()
        {
            _logger.LogInformation("Getting all reservations");
            return _context.Reservations;
        }

        // GET: api/Reservation/GetReservationById/5
        [HttpGet]
        [Route("GetReservationById/{id}")]
        public async Task<IActionResult> GetReservation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Model state is not valid: {ModelState}");
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Getting reservation with id = {id}");
            var reservation = await _context.Reservations.FindAsync(id);
            
            // Check if there is a reservation with given id
            if (reservation == null)
                {
                    _logger.LogWarning($"Reservation with id = {id} NOT FOUND!");
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
                _logger.LogWarning($"Model state is not valid: {ModelState}");
                return BadRequest(ModelState);
            }

            Reservation objReservation = new Reservation();
            objReservation = await _context.Reservations.FindAsync(reservation.Id);
            
            // Get the type of the room, date of arrival and departure date of the reservation that is about to be updated
            var getRoomType = objReservation.RoomType;
            var getDateFrom = objReservation.DateFrom;
            var getDateTo = objReservation.DateTo;

            // Check if a room is already booked in a specified period
            var RoomBooked = _context.Reservations
                .Where(r => r.RoomType == reservation.RoomType && reservation.DateFrom < r.DateTo && reservation.DateTo > r.DateFrom)
                .Any();

            // Format the dates
            var DateFromTransformed = reservation.DateFrom.ToString().Substring(0, 11);
            var DateToTransformed = reservation.DateTo.ToString().Substring(0, 11);

            // Display message to the user if certain room is already booked for given period
            foreach (var r in _context.Reservations)
            {
                if (reservation.RoomType != getRoomType && reservation.RoomType == r.RoomType && RoomBooked || reservation.RoomType == getRoomType && (reservation.DateFrom != getDateFrom || reservation.DateTo != getDateTo || (reservation.DateFrom != getDateFrom && reservation.DateTo != getDateTo)) && RoomBooked)
                {
                    _logger.LogInformation($"{reservation.RoomType} is not available {(DateToTransformed == DateFromTransformed ? $"on the date {DateFromTransformed}" : $"on all or some dates in the period from {DateFromTransformed} to {DateToTransformed}")}");
                    return UnprocessableEntity(new { message = $"{reservation.RoomType} is not available {(DateToTransformed == DateFromTransformed ? $"on the date {DateFromTransformed}" : $"on all or some dates in the period from {DateFromTransformed} to {DateToTransformed}")}" });
                }
            }

            // Check if reservation to be updated exists
            if (objReservation == null)
            {
                _logger.LogWarning($"Reservation with id = {reservation.Id} NOT FOUND!");
                return NotFound();
            }

            if (objReservation != null)
            {
                objReservation.FirstName = reservation.FirstName;
                objReservation.LastName = reservation.LastName;
                objReservation.DateFrom = reservation.DateFrom;
                objReservation.DateTo = reservation.DateTo;
                objReservation.NumOfPeople = reservation.NumOfPeople;
                objReservation.RoomType = reservation.RoomType;
            }

            // Log info about reservation to be edited
            _logger.LogInformation($"Reservation with id = {reservation.Id} is about to be edited!");
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }

        // POST: api/Reservation/AddNewReservation
        [HttpPost]
        [Route("AddNewReservation")]
        public async Task<IActionResult> PostReservation([FromBody] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Model state is not valid: {ModelState}");
                return BadRequest(ModelState);
            }

            // Check if a room is already booked in a specified period
            var RoomBooked = _context.Reservations
                .Where(r => r.RoomType == reservation.RoomType && reservation.DateFrom < r.DateTo && reservation.DateTo > r.DateFrom)
                .Any();

            // Format the dates
            var DateFromTransformed = reservation.DateFrom.ToString().Substring(0, 11);
            var DateToTransformed = reservation.DateTo.ToString().Substring(0, 11);

            // Display message to the user if certain room is already booked for given period
            foreach (var r in _context.Reservations)
            {
                if (reservation.RoomType == r.RoomType && RoomBooked)
                {
                    _logger.LogInformation($"{reservation.RoomType} is not available {(DateToTransformed == DateFromTransformed ? $"on the date {DateFromTransformed}" : $"on all or some dates in the period from {DateFromTransformed} to {DateToTransformed}")}");
                    return UnprocessableEntity(new { message = $"{reservation.RoomType} is not available {(DateToTransformed == DateFromTransformed ? $"on the date {DateFromTransformed}" : $"on all or some dates in the period from {DateFromTransformed} to {DateToTransformed}")}" });
                }
            }       

            _context.Reservations.Add(reservation);

            // Log info about reservation to be added
            _logger.LogInformation($"New reservation for {reservation.FirstName} {reservation.LastName} is about to be added!");
            await _context.SaveChangesAsync();

            // Log new reservation's id
            _logger.LogInformation($"New reservation's id = {reservation.Id}");
            return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
        }

        // DELETE: api/Reservation/DeleteReservation/5
        [HttpDelete]
        [Route("DeleteReservation/{id}")]
        public async Task<IActionResult> DeleteReservation([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Model state is not valid: {ModelState}");
                return BadRequest(ModelState);
            }

            var reservation = await _context.Reservations.FindAsync(id);

            // Check if there is reservation with given id
            if (reservation == null)
            {
                _logger.LogWarning($"Reservation with id = {reservation.Id} NOT FOUND!");
                return NotFound();
            }

            _context.Reservations.Remove(reservation);

            // Log info about reservation to be deleted
            _logger.LogInformation($"Reservation with id = {reservation.Id} is about to be deleted!");
            await _context.SaveChangesAsync();
            return Ok(reservation);
        }
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}