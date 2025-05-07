using App.Server.DTOs;
using App.Server.Models.AppData;
using App.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace App.Server.Controllers
{
    /// <summary>
    /// Api controller for Creating, Updating, Deleting and getting routes in certain district.
    /// </summary>
    /// <param name="unitOfWork">Injected Unit Of Work, for accessing repositories</param>
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController(IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        /// <summary>
        /// Gets all routes in certain district with Id DistrictId.
        /// </summary>
        /// <param name="districtId">Id of district of the routes</param>
        /// <returns>Status code 200 ok and a IEnumerable of all the routes, or 404 Not found, if getting routes fails.</returns>
        [Authorize]
        [HttpGet("in-district/{districtId}")]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesInDistrict(int districtId)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(districtId);
            if (district == null)
            {
                return NotFound("District not found.");
            }
            var routes = await _unitOfWork.RouteRepository.Get(route => route.DistrictId == districtId);
            if (routes == null)
            {
                return NotFound("Failed to fetch routes.");
            }
            var routesDtos = routes.Select(route => route.ToDto()).ToList();
            return Ok(routesDtos);
        }


        /// <summary>
        /// Creates new route, only HeadofDistrict and Admin are authorized.
        /// </summary>
        /// <param name="routeDto">New Route.</param>
        /// <returns>Status code 200 OK and the new Route, or 400 BadRequest if the district of the new route could not be found.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPost("create")]
        public async Task<ActionResult<RouteDto>> Create(RouteDto routeDto)
        {
            var district = await _unitOfWork.DistrictRepository.GetById(routeDto.DistrictId);
            if (district == null)
            {
                return BadRequest("District id not found.");
            }
            Models.AppData.Route route = new()
            {
                Name = routeDto.Name,
                Priority = routeDto.Priority,
                ControlPlace = routeDto.ControlPlace,
                DistrictId = routeDto.DistrictId,
                District = district
            };

            _unitOfWork.RouteRepository.Add(route);
            await _unitOfWork.SaveAsync();
            return Ok(route.ToDto());
        }

        /// <summary>
        /// Delete a route by its Id.
        /// </summary>
        /// <param name="routeId">Id of the route being deleted.</param>
        /// <returns>Status Code 200 oK, or 400 BadRequest if no route with RouteId was found.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpDelete("delete/{routeId}")]
        public async Task<ActionResult> Delete(int routeId)
        {
            var route = await _unitOfWork.RouteRepository.GetById(routeId);
            if (route == null)
            {
                return BadRequest("Route id not found.");
            }

            _unitOfWork.RouteRepository.Delete(route);
            await _unitOfWork.SaveAsync();
            return Ok("Succesfully deleted route");
        }

        /// <summary>
        /// Update route, route must already exist.
        /// </summary>
        /// <param name="routeDto">New updated route.</param>
        /// <returns>Status code 200 Ok, or 404 not found if route was not found by the id.</returns>
        [Authorize(Roles = "Admin,HeadOfDistrict")]
        [HttpPut("update")]
        public async Task<ActionResult<RouteDto>> Update(RouteDto routeDto)
        {
            var route = await _unitOfWork.RouteRepository.GetById(routeDto.Id);
            if (route == null)
            {
                return NotFound("Route not found.");
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
