using App.Server.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using App.Server;
using System.Net;
using App.Server.Models.AppData;

namespace Tests.Integration
{
    public class Requests : IClassFixture<WebAppFactoryPlanner<Program>>
    {
        private readonly WebAppFactoryPlanner<Program> _factory;
        private readonly HttpClient _client;

        public Requests(WebAppFactoryPlanner<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("Ranger", 1, "1", "Name", "Doe")]  // NOT head of district
        [InlineData("HeadOfDistrict", 999, "new@gmail.com", "Name", "LAst")] // Invalid district id
        [InlineData("HeadOfDistrict", 1, "invalid-email", "Name", "Last")]  // Invalid email
        [InlineData("HeadOfDistrict", 1, "new@gmail.com", "", "Last")]  // Missing first name
        [InlineData("HeadOfDistrict", 1, "new@gmail.com", "Name", "")]  // Missing last name
        [InlineData("HeadOfDistrict", 1, "", "Name", "Doe")]  // Missing email
        public async Task CreateRanger_BadRequest(string role, int districtId, string email, string firstName, string lastName)
        {
            // arrange - sign in as Ranger
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // Create Ranger DTO with non-existent district ID
            var rangerDto = new RangerDto
            {
                Id = 0,
                DistrictId = districtId,  // Non-existent district
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            // act - create the ranger
            var createResponse = await _client.PostAsJsonAsync("/api/ranger/create", rangerDto);

            // assert - expect BadRequest
            Assert.False(createResponse.StatusCode == HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("Ranger", 1, HttpStatusCode.Found)] // not head of district
        [InlineData("HeadOfDistrict", 999, HttpStatusCode.BadRequest)]
        [InlineData("HeadOfDistrict", 0, HttpStatusCode.BadRequest)]
        public async Task DeleteRanger_BadRequest(string role, object rangerId, HttpStatusCode code)
        {
            // sign in
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // act
            var deleteResponse = await _client.DeleteAsync($"/api/ranger/delete/{rangerId}");

            // assert
            Assert.Equal(deleteResponse.StatusCode, code);
        }


        [Theory]
        [InlineData("Ranger", 1, "1", "Name", "Doe")]  // NOT head of district
        [InlineData("HeadOfDistrict", 999, "new@gmail.com", "Name", "LAst")] // Invalid district id
        [InlineData("HeadOfDistrict", 1, "invalid-email", "Name", "Last")]  // Invalid email
        [InlineData("HeadOfDistrict", 1, "new@gmail.com", "", "Last")]  // Missing first name
        [InlineData("HeadOfDistrict", 1, "new@gmail.com", "Name", "")]  // Missing last name
        [InlineData("HeadOfDistrict", 1, "", "Name", "Doe")]  // Missing email
        public async Task UpdateRanger_BadRequest(string role, int districtId, string email, string firstName, string lastName)
        {
            // arrange - sign in as Ranger
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // update Ranger DTO 
            var rangerDto = new RangerDto
            {
                Id = 1,
                DistrictId = districtId,  
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            // act - create the ranger
            var createResponse = await _client.PostAsJsonAsync("/api/ranger/update", rangerDto);

            // assert - expect BadRequest
            Assert.False(createResponse.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task SuccesfulCallUpdateAttendence()
        {
            // arrange - sign in first
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData["Ranger"].Email,
                Password = _factory.UserData["Ranger"].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK


            var attendenceDto = new AttendenceDto
            {
                Date = _factory.Date,
                Ranger = _factory.SeededData.Rangers[1].ToDto(),
                Working = true,
                ReasonOfAbsence = ReasonOfAbsence.None,
                From = null
            };

            // act
            var updateResponse = await _client.PutAsJsonAsync("/api/attendence/update", attendenceDto);

            // assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updatedAttendence = await updateResponse.Content.ReadFromJsonAsync<AttendenceDto>();
            Assert.NotNull(updatedAttendence);
            Assert.True(updatedAttendence.Working);
        }

        [Fact]
        public async Task IncompleteCallUpdateAttendence()
        {
            // arrange - sign in as Ranger
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData["Ranger"].Email,
                Password = _factory.UserData["Ranger"].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // Incomplete attendance data (missing Ranger info)
            var dto = new AttendenceDto
            {
                Date = _factory.Date,
                Ranger = null,  // Missing ranger info
                Working = true,
                ReasonOfAbsence = ReasonOfAbsence.None,
                From = null
            };

            // act - attempt to update attendance with incomplete data
            var updateResponse = await _client.PutAsJsonAsync("/api/attendence/update", dto);

            // assert - check for BadRequest response
            Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
        }

        [Theory]
        [InlineData("Ranger", 1, 1, "Name")]  // NOT head of district
        [InlineData("HeadOfDistrict", 999, 1, "Name")] // Invalid district id
        [InlineData("HeadOfDistrict", 1, 1, "")]  // Invalid name
        [InlineData("HeadOfDistrict", 1, -15, "")]  // Invalid priority
        [InlineData("HeadOfDistrict", 1, 10, "")]  // Invalid priority
        public async Task CreateRoute_BadRequest(string role, int districtId, int priority, string name)
        {
            // arrange - sign in as Ranger
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // Create route
            var routeDto = new RouteDto(0, name, priority, null, districtId);

            // act - create the ranger
            var createResponse = await _client.PostAsJsonAsync("/api/route/create", routeDto);

            // assert - expect BadRequest
            Assert.False(createResponse.StatusCode == HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("Ranger", 1, HttpStatusCode.Found)] // not head of district
        [InlineData("HeadOfDistrict", 999, HttpStatusCode.BadRequest)]
        [InlineData("HeadOfDistrict", 0, HttpStatusCode.BadRequest)]
        public async Task DeleteRoute_BadRequest(string role, object routeId, HttpStatusCode code)
        {
            // sign in
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // act
            var deleteResponse = await _client.DeleteAsync($"/api/route/delete/{routeId}");

            // assert
            Assert.Equal(deleteResponse.StatusCode, code);
        }


        [Theory]
        [InlineData("Ranger", 1, 1, "Name")]  // NOT head of district
        [InlineData("HeadOfDistrict", 999, 1, "Name")] // Invalid district id
        [InlineData("HeadOfDistrict", 1, 1, "")]  // Invalid name
        [InlineData("HeadOfDistrict", 1, -15, "")]  // Invalid priority
        [InlineData("HeadOfDistrict", 1, 10, "")]  // Invalid priority
        public async Task UpdateRoute_BadRequest(string role, int districtId, int priority, string name)
        {
            // arrange - sign in as Ranger
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // Create route
            var routeDto = new RouteDto(0, name, priority, null, districtId);

            // act - create the ranger
            var createResponse = await _client.PostAsJsonAsync("/api/route/update", routeDto);


            // assert - expect BadRequest
            Assert.False(createResponse.StatusCode == HttpStatusCode.OK);
        }
        [Theory]
        [InlineData("HeadOfDistrict", 1, HttpStatusCode.OK)]
        [InlineData("Ranger", 1, HttpStatusCode.OK)]
        [InlineData("HeadOfDistrict", 999, HttpStatusCode.NotFound)]
        public async Task GetAllRoutes(string role, int districtId, HttpStatusCode code)
        {
            // arrange - sign in as Ranger
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK


            // act - create the ranger
            var createResponse = await _client.GetAsync($"/api/route/in-district/{districtId}");

            // assert - expect BadRequest
            Assert.Equal(createResponse.StatusCode, code);
        }

        [Theory]
        [InlineData("HeadOfDistrict", HttpStatusCode.OK)]
        [InlineData("Ranger", HttpStatusCode.OK)]
        public async Task GetAllDistricts(string role, HttpStatusCode expectedStatusCode)
        {
            // arrange - sign in as user
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // act - call get-all districts
            var getResponse = await _client.GetAsync("/api/district/get-all");

            // assert
            Assert.Equal(expectedStatusCode, getResponse.StatusCode);

            if (expectedStatusCode == HttpStatusCode.OK)
            {
                var districts = await getResponse.Content.ReadFromJsonAsync<IEnumerable<DistrictDto>>();
                Assert.NotNull(districts);
                Assert.True(districts.Any()); // expecting at least one district if 200 OK
            }
        }

        [Theory]
        //[InlineData("Ranger", 1, HttpStatusCode.Found)]
        [InlineData("HeadOfDistrict", 1, HttpStatusCode.OK)]
        [InlineData("HeadOfDistrict", 999, HttpStatusCode.BadRequest)]
        public async Task LockDistrict(string role, int districtId, HttpStatusCode expectedStatusCode)
        {
            // arrange - sign in
            var signInRequest = new SignInRequest
            {
                Email = _factory.UserData[role].Email,
                Password = _factory.UserData[role].Password
            };

            var signInResponse = await _client.PostAsJsonAsync("/api/user/signin", signInRequest);
            signInResponse.EnsureSuccessStatusCode(); // Should be 200 OK

            // act - send lock request
            var response = await _client.PostAsync($"/api/Lock/lock/{districtId}/2025-07-01", null);

            // assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}
