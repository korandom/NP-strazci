using App.Server.DTOs;
using App.Server.Models.AppData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace App.Server.Services.Hubs
{
    [Authorize]
    public class PlanHub :  Hub
    {

        public async Task UpdatePlan(string month, int districtId, PlanDto plan)
        { 
            await Clients.OthersInGroup($"{districtId}-{month}").SendAsync("PlanUpdated", plan);
        }

        public async Task AddToPlanGroup(string month, int districtId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{districtId}-{month}");
        }

        public async Task RemoveFromPlanGroup(string month, int districtId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{districtId}-{month}");
        }
    }
}
