using App.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace App.Server.Services.Hubs
{
    [Authorize]
    public class DistrictHub : Hub
    {
        // operation is RouteUpdated, RouteAdded, RouteDeleted
        public async Task SendRouteNotification(string operation, int districtId, RouteDto route)
        {
            await Clients.OthersInGroup(districtId.ToString()).SendAsync(operation, route);
        }

        // operation is VehicleUpdated, VehicleAdded, VehicleDeleted
        public async Task SendVehicleNotification(string operation, int districtId, VehicleDto vehicle)
        {
            await Clients.OthersInGroup(districtId.ToString()).SendAsync(operation, vehicle);
        }

        // operation is RangerUpdated, RangerAdded, RangerDeleted
        public async Task SendRangerNotification(string operation, int districtId, RangerDto ranger)
        {
            await Clients.OthersInGroup(districtId.ToString()).SendAsync(operation, ranger);
        }

        // operation is LockAdded, LockDeleted
        public async Task SendLockNotification(string operation, int districtId, LockDto l)
        {
            await Clients.OthersInGroup(districtId.ToString()).SendAsync(operation, l);
        }

        public async Task AddToDistrictGroup(int districtId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, districtId.ToString());
        }

        public async Task RemoveFromDistrictGroup(int districtId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, districtId.ToString());
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("CONNECTED TO SIGNAL.");
            await base.OnConnectedAsync();
        }
    }
}
