using App.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace App.Server.Services.Hubs
{
    /// <summary>
    /// SignalR hub for real-time notifications of district-related updates.
    /// Authenticated clients can receive updates related to routes, vehicles, rangers, and locks.
    /// </summary>
    [Authorize]
    public class DistrictHub : Hub
    {
        /// <summary>
        /// Sends a notification to other users in the same district group when a route is updated, added, or deleted.
        /// </summary>
        /// <param name="operation">The type of operation - "RouteUpdated", "RouteAdded", "RouteDeleted".</param>
        /// <param name="districtId">The district ID.</param>
        /// <param name="route">The Routedto involved in the operation.</param>
        public async Task SendRouteNotification(string operation, int districtId, RouteDto route)
        {
            await Clients.OthersInGroup(districtId.ToString()).SendAsync(operation, route);
        }

        /// <summary>
        /// Sends a notification to other users in the same district group when a vehicle is updated, added, or deleted.
        /// </summary>
        /// <param name="operation">The type of operation - "VehicleUpdated", "VehicleAdded", "VehicleDeleted".</param>
        /// <param name="districtId">The district ID.</param>
        /// <param name="route">The Vehicledto involved in the operation.</param>
        public async Task SendVehicleNotification(string operation, int districtId, VehicleDto vehicle)
        {
            await Clients.OthersInGroup(districtId.ToString()).SendAsync(operation, vehicle);
        }

        /// <summary>
        /// Sends a notification to other users in the same district group when a ranger is updated, added, or deleted.
        /// </summary>
        /// <param name="operation">The type of operation - "RangerUpdated", "RangerAdded", "RangerDeleted".</param>
        /// <param name="districtId">The district ID.</param>
        /// <param name="route">The Rangerdto involved in the operation.</param>
        public async Task SendRangerNotification(string operation, int districtId, RangerDto ranger)
        {
            await Clients.OthersInGroup(districtId.ToString()).SendAsync(operation, ranger);
        }

        /// <summary>
        /// Sends a notification to other users in the same district group when a lock is added, or deleted.
        /// </summary>
        /// <param name="operation">The type of operation - "LockAdded", "LockDeleted".</param>
        /// <param name="districtId">The district ID.</param>
        /// <param name="route">The Lockdto involved in the operation.</param>
        public async Task SendLockNotification(string operation, int districtId, LockDto l)
        {
            await Clients.OthersInGroup(districtId.ToString()).SendAsync(operation, l);
        }

        /// <summary>
        /// Adds the current connection to the SignalR group of given district.
        /// </summary>
        /// <param name="districtId">The id of district.</param>
        public async Task AddToDistrictGroup(int districtId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, districtId.ToString());
        }

        /// <summary>
        /// Removes the current connection from the SignalR group of given district.
        /// </summary>
        /// <param name="districtId">The if of district group to leave.</param>
        public async Task RemoveFromDistrictGroup(int districtId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, districtId.ToString());
        }
    }
}
