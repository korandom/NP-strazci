﻿using App.Server.DTOs;
using App.Server.Models;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public RouteController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("in-district/{DistrictId}")]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesInDistrict(int DistrictId)
        {
            var routes = await _unitOfWork.RouteRepository.Get(route => route.DistrictId == DistrictId);
            if (routes == null || !routes.Any())
            {
                return NotFound("No routes found in district");
            }
            var routesDtos = routes.Select(route => route.ToDto()).ToList();
            return Ok(routesDtos);
        }

        [HttpPost("create")]
        public async Task<ActionResult<RouteDto>> Create(RouteDto routeDto)
        {
            var sector = await _unitOfWork.DistrictRepository.GetById(routeDto.DistrictId);
            if (sector == null)
            {
                return BadRequest("District id not found");
            }
            Models.Route route = new()
            {
                Id = routeDto.Id,
                Name = routeDto.Name,
                Priority = routeDto.Priority,
                ControlPlace = routeDto.ControlPlace,
                DistrictId = routeDto.DistrictId,
            };

            _unitOfWork.RouteRepository.Add(route);
            await _unitOfWork.SaveAsync();
            return Ok(route.ToDto());
        }

        [HttpDelete("delete")]
        public async Task<ActionResult> Delete(int RouteId)
        {
            var route = await _unitOfWork.RouteRepository.GetById(RouteId);
            if (route == null)
            {
                return BadRequest("Route id not found");
            }

            _unitOfWork.RouteRepository.Delete(route);
            await _unitOfWork.SaveAsync();
            return Ok("Succesfully deleted route");
        }

        [HttpPut("update")]
        public async Task<ActionResult<RouteDto>> Update(RouteDto routeDto)
        {
            var route = await _unitOfWork.RouteRepository.GetById(routeDto.Id);
            if (route == null)
            {
                return NotFound("Ranger not found");
            }

            route.Name = routeDto.Name;
            route.Priority = routeDto.Priority;
            route.ControlPlace = routeDto.ControlPlace;
            route.DistrictId = routeDto.DistrictId;

            _unitOfWork.RouteRepository.Update(route);
            await _unitOfWork.SaveAsync();

            return Ok(route.ToDto());
        }

    }
}
