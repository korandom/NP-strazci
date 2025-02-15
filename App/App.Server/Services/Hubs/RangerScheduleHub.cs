using App.Server.DTOs;
using App.Server.Models.AppData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Numerics;


namespace App.Server.Services.Hubs
{
    [Authorize]
    public class RangerScheduleHub :  Hub
    {

        public async Task UpdatePlan( int districtId, PlanDto plan)
        { 
            await Clients.OthersInGroup($"{districtId}").SendAsync("PlanUpdated", plan);
        }
        public async Task UpdateAttendence (int districtId, AttendenceDto attendence)
        {
            await Clients.OthersInGroup($"{districtId}").SendAsync("AttendenceUpdated", attendence);

        }
        public async Task AddToPlanGroup(int districtId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{districtId}");
        }

        public async Task RemoveFromPlanGroup(int districtId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{districtId}");
        }
    }
}
