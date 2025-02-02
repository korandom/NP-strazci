using App.Server.DTOs;
using App.Server.Migrations;
using Microsoft.EntityFrameworkCore;

namespace App.Server.Models.AppData
{

    public enum ReasonOfAbsence
    {
        None,
        NV,
        D,
        S,
        ST,
        N,
        PV,
        P,
        X
    }

    [PrimaryKey(nameof(Date), nameof(RangerId))]
    public class Attendence
    {
        public DateOnly Date { get; set; }
        public int RangerId { get; set; }
        public Ranger Ranger { get; set; }

        public bool Working { get; set; } = false;
        public TimeOnly? From { get; set; } = null;

        public string ReasonOfAbsence { get; set; } = "None";

        public ReasonOfAbsence ReasonOfAbsenceEnum
        {
            get => Enum.Parse<ReasonOfAbsence>(ReasonOfAbsence ?? "None");
            set => ReasonOfAbsence = value.ToString();
        }
        public Attendence() { }
        public Attendence(DateOnly date, Ranger ranger)
        {
            Date = date;
            RangerId = ranger.Id;
            Ranger = ranger;
        }
    }

    public static class AttendenceExtensions
    {
        public static AttendenceDto ToDto(this Attendence attend)
        {
            return new AttendenceDto
            {
                Date = attend.Date,
                Ranger = attend.Ranger.ToDto(),
                Working = attend.Working,
                From = attend.From,
                ReasonOfAbsence = attend.ReasonOfAbsenceEnum
            };
        }
    }
}
