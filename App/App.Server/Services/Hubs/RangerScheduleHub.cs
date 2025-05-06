using App.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace App.Server.Services.Hubs
{
    /// <summary>
    /// SignalR hub for real-time notifications for changes of plans, and attendance.
    /// Requires authorization to connect.
    /// </summary>
    [Authorize]
    public class RangerScheduleHub : Hub
    {
        /// <summary>
        /// Sends a "Reload" signal to all other clients in the specified district group.
        /// Used to notify clients to reload all ranger schedules.
        /// </summary>
        /// <param name="districtId">The district group to notify.</param>
        public async Task TriggerReload(int districtId)
        {
            await Clients.OthersInGroup($"{districtId}").SendAsync("Reload");
        }

        /// <summary>
        /// Sends a plan update to all other clients in the specified district group.
        /// </summary>
        /// <param name="districtId">The district group to notify.</param>
        /// <param name="plan">The updated plan.</param>
        public async Task UpdatePlan(int districtId, PlanDto plan)
        {
            await Clients.OthersInGroup($"{districtId}").SendAsync("PlanUpdated", plan);
        }

        /// <summary>
        /// Sends an attendance update to all other clients in the specified district group.
        /// </summary>
        /// <param name="districtId">The district group to notify.</param>
        /// <param name="attendence">The updated attendance.</param>
        public async Task UpdateAttendence(int districtId, AttendenceDto attendence)
        {
            await Clients.OthersInGroup($"{districtId}").SendAsync("AttendenceUpdated", attendence);

        }

        /// <summary>
        /// Adds the current connection to the SignalR group of given district.
        /// </summary>
        /// <param name="districtId">The id of district.</param>
        public async Task AddToPlanGroup(int districtId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{districtId}");
        }

        /// <summary>
        /// Removes the current connection from the SignalR group of given district.
        /// </summary>
        /// <param name="districtId">The id of district group to leave.</param>
        public async Task RemoveFromPlanGroup(int districtId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{districtId}");
        }
    }
}
