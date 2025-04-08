using App.Server.CSP;
using App.Server.DTOs;


namespace Tests.CSP.DataProcessorTests
{
    public class GetWorkingRangersTest
    {
        [Fact]
        public void NoDaysTest()
        {

            //act
            var workingRangers = DataProcessor.GetWorkingRangers([], 0, new DateOnly(2025, 1, 1));

            //assert
            Assert.NotNull(workingRangers);
            Assert.Empty(workingRangers);
        }

        [Fact]
        public void EmptyAttendenceTest()
        {

            //act
            var workingRangers = DataProcessor.GetWorkingRangers([], 2, new DateOnly(2025, 1, 1));

            //assert
            Assert.NotNull(workingRangers);
            Assert.Equal(2, workingRangers.Count);
            Assert.Contains(0, workingRangers);
            Assert.Contains(1, workingRangers);
            Assert.Empty(workingRangers[1]);
            Assert.Empty(workingRangers[0]);
        }

        [Fact]
        public void AllNegativeAttendenceTest()
        {
            //arrange
            DateOnly start = new(2025, 1, 1);
            List<RangerDto> rangers =
            [
                new RangerDto
                {
                    Id = 1,
                    FirstName = "Jan",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                },
                new RangerDto
                {
                    Id = 2,
                    FirstName = "Pavel",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                },
                new RangerDto
                {
                    Id = 3,
                    FirstName = "Jakub",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                }
            ];
            List<AttendenceDto> attendences =
            [
                new AttendenceDto
                {
                    Date = start,
                    Ranger = rangers[0],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.D,
                    Working= false
                },
                new AttendenceDto
                {
                    Date = start,
                    Ranger = rangers[1],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.D,
                    Working= false
                },
                new AttendenceDto
                {
                    Date = start,
                    Ranger = rangers[2],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.N,
                    Working= false
                },
                new AttendenceDto
                {
                    Date = start.AddDays(1),
                    Ranger = rangers[0],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.None,
                    Working= false
                },
                new AttendenceDto
                {
                    Date = start.AddDays(1),
                    Ranger = rangers[1],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.D,
                    Working= false
                },
                new AttendenceDto
                {
                    Date = start.AddDays(1),
                    Ranger = rangers[2],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.None,
                    Working= false
                }
            ];


            //act
            var workingRangers = DataProcessor.GetWorkingRangers(attendences, 2, start);

            //assert
            Assert.NotNull(workingRangers);
            Assert.Equal(2, workingRangers.Count);
            Assert.Contains(0, workingRangers);
            Assert.Contains(1, workingRangers);
            Assert.Empty(workingRangers[1]);
            Assert.Empty(workingRangers[0]);
        }

        [Fact]
        public void SimpleAttendenceTest()
        {
            //arrange
            DateOnly start = new(2025, 1, 1);
            List<RangerDto> rangers =
            [
                new RangerDto
                {
                    Id = 1,
                    FirstName = "Jan",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                },
                new RangerDto
                {
                    Id = 2,
                    FirstName = "Pavel",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                },
                new RangerDto
                {
                    Id = 3,
                    FirstName = "Jakub",
                    LastName = "Novak",
                    DistrictId = 1,
                    Email = "abc@gmail"
                }
            ];
            // first day ranger 2,3 working, second day ranger 1 working
            List<AttendenceDto> attendences =
            [
                new AttendenceDto
                {
                    Date = start,
                    Ranger = rangers[0],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.D,
                    Working= false
                },
                new AttendenceDto
                {
                    Date = start,
                    Ranger = rangers[1],
                    Working= true
                },
                new AttendenceDto
                {
                    Date = start,
                    Ranger = rangers[2],
                    Working= true
                },
                new AttendenceDto
                {
                    Date = start.AddDays(1),
                    Ranger = rangers[0],
                    Working= true
                },
                new AttendenceDto
                {
                    Date = start.AddDays(1),
                    Ranger = rangers[1],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.D,
                    Working= false
                },
                new AttendenceDto
                {
                    Date = start.AddDays(1),
                    Ranger = rangers[2],
                    ReasonOfAbsence = App.Server.Models.AppData.ReasonOfAbsence.None,
                    Working= false
                }
            ];


            //act
            var workingRangers = DataProcessor.GetWorkingRangers(attendences, 2, start);

            //assert
            Assert.NotNull(workingRangers);
            Assert.Equal(2, workingRangers.Count);
            Assert.Contains(0, workingRangers);
            Assert.Contains(1, workingRangers);
            List<int> expectedFirst = [2, 3];
            List<int> expectedSecodn = [1];
            Assert.Equal(expectedFirst, workingRangers[0]);
            Assert.Equal(expectedSecodn, workingRangers[1]);
        }
    }
}
