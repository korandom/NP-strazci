﻿using App.Server.DTOs;

namespace App.Server.Models.AppData
{
    public class District
    {
        public int Id { get; set; }

        public ICollection<Ranger> Rangers { get; } = new List<Ranger>();
        public ICollection<Vehicle> Vehicles { get; } = new List<Vehicle>();
        public required string Name { get; set; }
    }

    public static class DistrictExtensions
    {
        public static DistrictDto ToDto(this District district)
        {
            return new DistrictDto
            {
                Id = district.Id,
                Name = district.Name
            };
        }
    }
}
