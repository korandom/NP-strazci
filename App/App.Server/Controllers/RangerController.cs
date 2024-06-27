using App.Server.DTOs;
using App.Server.Models;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RangerController : ControllerBase
    {

        private IUnitOfWork _unitOfWork;
        public RangerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("in-district/{DistrictId}")]
        public async Task<ActionResult<IEnumerable<RangerDto>>> GetRangersInDistrict(int DistrictId)
        {
            var rangers = await _unitOfWork.RangerRepository.Get(ranger => ranger.DistrictId == DistrictId);
            if (rangers == null || !rangers.Any())
            {
                return NotFound("No rangers found in district");
            }
            var rangerDtos = rangers.Select(ranger => ranger.ToDto()).ToList();
            return Ok(rangerDtos);
        }

        [HttpPost("create")]
        public async Task<ActionResult<RangerDto>> Create(RangerDto rangerDto)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(rangerDto.DistrictId);
            if (district == null)
            {
                return BadRequest("District id not found");
            }
            Ranger ranger = new()
            {
                FirstName = rangerDto.FirstName,
                LastName = rangerDto.LastName,
                Email = rangerDto.Email,
                DistrictId = rangerDto.DistrictId,
                District = district
            };

            _unitOfWork.RangerRepository.Add(ranger);
            await _unitOfWork.SaveAsync();
            return Ok(ranger.ToDto());
        }

        [HttpDelete("delete")]
        public async  Task<ActionResult> Delete(int RangerId)
        {
            var ranger = await _unitOfWork.RangerRepository.GetById(RangerId);
            if (ranger == null)
            {
                return BadRequest("Ranger id not found");
            }

            _unitOfWork.RangerRepository.Delete(ranger);
            await _unitOfWork.SaveAsync();
            return Ok("Succesfully deleted ranger");
        }

        [HttpPut("update")]
        public async Task<ActionResult<RangerDto>> Update(RangerDto rangerDto)
        {
            var ranger = await _unitOfWork.RangerRepository.GetById(rangerDto.Id);
            if (ranger == null)
            {
                return NotFound("Ranger not found");
            }

            ranger.FirstName = rangerDto.FirstName;
            ranger.LastName = rangerDto.LastName;
            ranger.Email = rangerDto.Email;
            ranger.DistrictId = rangerDto.DistrictId;

            _unitOfWork.RangerRepository.Update(ranger);
            await _unitOfWork.SaveAsync();

            return Ok(ranger.ToDto());  
        }
    }
}
