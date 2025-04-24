using App.Server.DTOs;

namespace App.Server.CSP
{
    public interface IRoutePlanGenerator
    {
        GenerateResultDto Generate(
            List<PlanDto> previousPlans,
            List<PlanDto> preexistingPlans,
            List<AttendenceDto> attendences,
            List<RouteDto> routes,
            List<RangerDto> rangers,
            DateOnly startDate);
    }
}
