using App.Server.Models;
using App.Server.DTOs;
using Microsoft.AspNetCore.Mvc;
using App.Server.Repositories.Interfaces;


namespace App.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public PlanController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<Plan> Create(DateOnly date, int rangerId ) 
        {
            Ranger? ranger =  await _unitOfWork.RangerRepository.GetById(rangerId) 
                ?? throw new InvalidOperationException("Ranger id not found.");

            var plan = new Plan(date, ranger);

            _unitOfWork.PlanRepository.Add(plan);
            await _unitOfWork.SaveAsync();
            return plan;
        }

        [HttpGet("{date}/{rangerId}")]
        public async Task<ActionResult<PlanDto>> GetById(DateOnly date, int rangerId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById (date,rangerId);

            if (plan == null)
            {
                return NotFound();
            }

            return Ok(plan.ToDto());
        }


        // TODO authorization - ranger has to be authorized - same rangerId or head of the district
        [HttpPut("add-route/{date}/{rangerId}")]
        public async Task<IActionResult> AddRoute(DateOnly date, int rangerId, int routeId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            // if plan not yet created, try to create
            if (plan == null)
            {
                try
                {
                    plan = await Create(date, rangerId);
                }
                catch(InvalidOperationException e)
                {
                    return BadRequest(e.Message);
                }
            }

            var route = await _unitOfWork.RouteRepository.GetById(routeId);
            if (route == null)
                return BadRequest("Route id not found.");

            // add route to plan 
            plan.Routes.Add(route);
            route.Plans.Add(plan);

            _unitOfWork.PlanRepository.Update(plan);

            await _unitOfWork.SaveAsync();

            return Ok("Succesfully added route to plan.");
        }

        // TODO: head of rangers for district authorized
        // Add a vehicle to the plan
        [HttpPut("add-vehicle/{date}/{rangerId}")]
        public async Task<IActionResult> AddVehicle(DateOnly date, int rangerId, int vehicleId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            // if plan not yet created, try to create
            if (plan == null)
            {
                try
                {
                    plan = await Create(date, rangerId);
                }
                catch (InvalidOperationException e)
                {
                    return BadRequest(e.Message);
                }
            }

            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                return BadRequest("Vehicle id not found.");

            // add vehicle to plan
            plan.Vehicles.Add(vehicle);
            vehicle.Plans.Add(plan);

            _unitOfWork.PlanRepository.Update(plan);

            await _unitOfWork.SaveAsync();

            return Ok("Successfully added vehicle to plan.");
        }

        // TODO: authorized
        // Remove a vehicle from the plan
        [HttpPut("remove-vehicle/{date}/{rangerId}")]
        public async Task<IActionResult> RemoveVehicle(DateOnly date, int rangerId, int vehicleId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            if (plan == null)
                return NotFound("Plan not found.");

            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                return BadRequest("Vehicle id not found.");

            // remove vehicle from plan
            plan.Vehicles.Remove(vehicle);
            vehicle.Plans.Remove(plan);

            _unitOfWork.PlanRepository.Update(plan);

            // Check if the plan has no routes and no vehicles
            if (plan.Routes.Count == 0 && plan.Vehicles.Count == 0)
            {
                _unitOfWork.PlanRepository.Delete(plan);
            }

            await _unitOfWork.SaveAsync();

            return Ok("Successfully removed vehicle from plan.");
        }

        // TODO:: authorized, lock multiple? range
        // Lock the plan
        [HttpPut("lock/{date}/{rangerId}")]
        public async Task<IActionResult> LockPlan(DateOnly date, int rangerId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            if (plan == null)
                return NotFound("Plan not found.");

            plan.Locked = true;

            _unitOfWork.PlanRepository.Update(plan);
            await _unitOfWork.SaveAsync();

            return Ok("Successfully locked the plan.");
        }

        // TODO authorized, unlock multiple?
        // Unlock the plan
        [HttpPut("unlock/{date}/{rangerId}")]
        public async Task<IActionResult> UnlockPlan(DateOnly date, int rangerId)
        {
            var plan = await _unitOfWork.PlanRepository.GetById(date, rangerId);

            if (plan == null)
                return NotFound("Plan not found.");

            plan.Locked = false;

            _unitOfWork.PlanRepository.Update(plan);
            await _unitOfWork.SaveAsync();

            return Ok("Successfully unlocked the plan.");
        }

        // TODO finish - by district, 
        // Get plans by date range
        [HttpGet("by-dates/{startDate}/{endDate}")]
        public async Task<ActionResult<IEnumerable<PlanDto>>> GetPlansByDateRange(DateOnly startDate, DateOnly endDate)
        {
            var plans = await _unitOfWork.PlanRepository.Get(plan=> plan.Date >= startDate && plan.Date<= endDate, null, "Routes,Vehicles");

            if (plans == null || !plans.Any())
            {
                return NotFound("No plans found for the given date range.");
            }

            var planDtos = plans.Select(plan => plan.ToDto()).ToList();
            return Ok(planDtos);
        }
    }
}
