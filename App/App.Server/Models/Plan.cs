﻿using App.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Server.Models
{
    [PrimaryKey(nameof(Date), nameof(RangerId))]
    public class Plan
    {
        public DateOnly Date { get; set; }
        public int RangerId { get; set; }
        public Ranger Ranger { get; set; }

        public ICollection<Route> Routes { get; } = new List<Route>();
        public ICollection<Vehicle> Vehicles { get; } = new List<Vehicle>();
        public bool Locked { get; set; }

        public Plan() { }
        public Plan(DateOnly date, Ranger ranger)
        {
            Date = date;
            RangerId = ranger.Id;
            Ranger = ranger;
            Locked = false;
        }
    }

    public static class PlanExtensions
    {
        public static PlanDto ToDto(this Plan plan)
        {
            return new PlanDto
            {
                Date = plan.Date,
                Ranger = plan.Ranger.ToDto(),
                Locked = plan.Locked,
                Routes = plan.Routes.Select(r => r.ToDto()).ToList(),
                Vehicles = plan.Vehicles.Select(v => v.ToDto()).ToList()
            };
        }
    }
}
