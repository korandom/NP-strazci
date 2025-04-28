using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace App.Server.Controllers
{

    /// <summary>
    /// API controller that manages locks.
    /// </summary>
    /// <param name="unitOfWork">Injected Unit of Work, for accessing repositories.</param>
    [ApiController]
    [Route("api/[controller]")]
    public class LockController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;


        /// <summary>
        /// Creates a new lock, locking attendence and plans in a certain date and district.
        /// </summary>
        /// <param name="date">Date being locked</param>
        /// <param name="districtId">District being locked</param>
        /// <returns>Status Code 200 Ok, if succesful, else 400 Bad Request</returns>
        [Authorize(Roles = "HeadOfDistrict")]
        [HttpPost("lock/{districtId}/{date}")]
        public async Task<IActionResult> Lock(int districtId, DateOnly date)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(districtId);
            if (district == null)
            {
                return BadRequest("District id not found.");
            }
            Models.AppData.Lock newLock = new()
            {
                Date = date,
                DistrictId = districtId,
                District = district
            };

            _unitOfWork.LockRepository.Add(newLock);
            await _unitOfWork.SaveAsync();
            return Ok($"Succesfully locked {date} against changes.");
        }

        /// <summary>
        /// Deletes a lock, unlocking attendence and plans on certain date in a specific district.
        /// </summary>
        /// <param name="date">Date being unlocked</param>
        /// <param name="districtId">District being unlocked</param>
        /// <returns>Status Code 200 Ok, if succesful, else 400 Bad Request (lock doesnt exist)</returns>
        [Authorize(Roles = "HeadOfDistrict")]
        [HttpDelete("unlock/{districtId}/{date}")]
        public async Task<IActionResult> Unlock(DateOnly date, int districtId)
        {
            var deleteLock = await _unitOfWork.LockRepository.Get(l => l.Date == date && l.DistrictId == districtId);
            if (deleteLock == null || !deleteLock.Any())
            {
                return BadRequest("Lock doesn't exist.");
            }

            _unitOfWork.LockRepository.Delete(deleteLock.First());
            await _unitOfWork.SaveAsync();
            return Ok($"Succesfully unlocked {date} for changes.");
        }

        /// <summary>
        /// Gets all locks in one district.
        /// </summary>
        /// <param name="districtId">District Id of locks</param>
        /// <returns>
        /// LockDto representing locks in district with DistrictId.
        /// Status Code 200 Ok, if succesful, else 400 Bad Request
        /// </returns>
        [Authorize()]
        [HttpGet("locks/{districtId}")]
        public async Task<ActionResult<IEnumerable<LockDto>>> GetLocks(int districtId)
        {
            var locks = await _unitOfWork.LockRepository.Get(l => l.DistrictId == districtId);
            if (locks == null)
            {
                return BadRequest("Locks not found.");
            }
            return Ok(locks.Select(l => new LockDto { Date = l.Date, DistrictId = l.DistrictId }).ToList());
        }
    }
}

