using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using RaceDayAPI.Services;
using Xunit;

namespace RaceDayAPI.Tests
{
    public class RaceDayApiTests
    {
        private RaceDayDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new RaceDayDbContext(options);
            context.Database.EnsureCreated();

            // Seed Roles if empty
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleId = 1, RoleName = "Organiser" },
                    new Role { RoleId = 2, RoleName = "Participant" }
                );
                context.SaveChanges();
            }

            return context;
        }

        private IConfiguration GetMockConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"Jwt:Key", "SuperSecretKeyForTestingJwtTokenGeneration12345!"},
                {"Jwt:Issuer", "RaceDayAPI"},
                {"Jwt:Audience", "RaceDayApp"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenValidDtoProvided()
        {
            // Arrange
            var context = GetDatabaseContext();
            var config = GetMockConfiguration();
            var authService = new AuthService(context, config);

            var registerDto = new RegisterDto(
                "Simphiwe",
                "Jijana",
                "simphiwe.test@raceday.io",
                "Password123!",
                2
            );

            // Act
            var result = await authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("simphiwe.test@raceday.io", result.Email);
            Assert.True(context.Users.Any(u => u.Email == "simphiwe.test@raceday.io"));
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenDuplicateEmailUsed()
        {
            // Arrange
            var context = GetDatabaseContext();
            var config = GetMockConfiguration();
            var authService = new AuthService(context, config);

            var registerDto = new RegisterDto(
                "Duplicate",
                "User",
                "duplicate@raceday.io",
                "Password123!",
                2
            );

            // Register first time
            await authService.RegisterAsync(registerDto);

            // Act - Try registering again with the same email
            var result = await authService.RegisterAsync(registerDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddCategory_ShouldSaveCategoryToDatabase()
        {
            // Arrange
            var context = GetDatabaseContext();
            var category = new Category
            {
                CategoryName = "Trail Run",
                Description = "Off-road distance running"
            };

            // Act
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Assert
            var savedCategory = await context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Trail Run");
            Assert.NotNull(savedCategory);
            Assert.Equal("Off-road distance running", savedCategory.Description);
        }

        [Fact]
        public async Task AddEvent_ShouldSaveEventToDatabase()
        {
            // Arrange
            var context = GetDatabaseContext();

            var category = new Category { CategoryName = "Cycling", Description = "Road cycling" };
            context.Categories.Add(category);

            var user = new User
            {
                FirstName = "Organiser",
                LastName = "Admin",
                Email = "organiser@raceday.io",
                PasswordHash = "hashed",
                RoleId = 1
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var raceEvent = new Event
            {
                Title = "Cape Town Cycle Tour",
                Description = "109km coastal cycling race",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Cape Town",
                CategoryId = category.CategoryId,
                OrganiserId = user.UserId,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            context.Events.Add(raceEvent);
            await context.SaveChangesAsync();

            // Assert
            var savedEvent = await context.Events.FirstOrDefaultAsync(e => e.Title == "Cape Town Cycle Tour");
            Assert.NotNull(savedEvent);
            Assert.Equal("Cape Town", savedEvent.Location);
        }

        [Fact]
        public async Task EnrollParticipant_ShouldRecordEnrolmentSuccessfully()
        {
            // Arrange
            var context = GetDatabaseContext();

            var user = new User
            {
                FirstName = "Participant",
                LastName = "Runner",
                Email = "runner@raceday.io",
                PasswordHash = "hashed",
                RoleId = 2
            };
            context.Users.Add(user);

            var raceEvent = new Event
            {
                Title = "Soweto Marathon",
                Description = "42km township marathon",
                EventDate = DateTime.Now.AddDays(60),
                Location = "Soweto",
                CategoryId = 1,
                OrganiserId = 1,
                CreatedAt = DateTime.UtcNow
            };
            context.Events.Add(raceEvent);
            await context.SaveChangesAsync();

            var enrolment = new EventEnrolment
            {
                EventId = raceEvent.EventId,
                ParticipantId = user.UserId
            };

            // Act
            context.EventEnrolments.Add(enrolment);
            await context.SaveChangesAsync();

            // Assert
            var savedEnrolment = await context.EventEnrolments
                .FirstOrDefaultAsync(e => e.EventId == raceEvent.EventId && e.ParticipantId == user.UserId);

            Assert.NotNull(savedEnrolment);
        }
    }
}