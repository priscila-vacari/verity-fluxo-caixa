using FluxoCaixa.Domain.Entities;
using FluxoCaixa.Infra.Context;
using FluxoCaixa.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FluxoCaixa.Tests.Infra
{
    public class RepositoryTests
    {
        private readonly Mock<ILogger<Repository<Launch>>> _loggerMock;

        public RepositoryTests()
        {
            _loggerMock = new Mock<ILogger<Repository<Launch>>>();
        }

        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "DatabaseTest")
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllItems()
        {
            var context = GetInMemoryDbContext();
            var repository = new Repository<Launch>(context, _loggerMock.Object);

            var date = new DateTime(2025, 03, 01, 0, 0, 0);
            context.Launches.AddRange(new List<Launch>
            {
                new() { Date = date, Type = Domain.Enum.LaunchType.Credit, Amount = 10.0m },
                new() { Date = date, Type = Domain.Enum.LaunchType.Debit, Amount = 5.0m },
            });
            await context.SaveChangesAsync();

            var launches = await repository.GetAllAsync();

            Assert.Contains(launches, l => l.Date == date && l.Type == Domain.Enum.LaunchType.Credit);
            Assert.Contains(launches, l => l.Date == date && l.Type == Domain.Enum.LaunchType.Debit);
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewItem()
        {
            var context = GetInMemoryDbContext();
            var repository = new Repository<Launch>(context, _loggerMock.Object);

            var date = new DateTime(2025, 03, 02, 0, 0, 0);
            Launch newLaunch = new() { Date = date, Type = Domain.Enum.LaunchType.Credit, Amount = 20.0m };

            await repository.AddAsync(newLaunch);
            var launches = await repository.GetAllAsync();

            Assert.Contains(launches, l => l.Date == date && l.Type == Domain.Enum.LaunchType.Credit);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectItem()
        {
            var context = GetInMemoryDbContext();
            var repository = new Repository<Launch>(context, _loggerMock.Object);

            var date = new DateTime(2025, 03, 03, 0, 0, 0);
            context.Launches.Add(new Launch { Date = date, Type = Domain.Enum.LaunchType.Credit, Amount = 20.0m });
            await context.SaveChangesAsync();

            var launch = await repository.GetByKeysAsync(date, Domain.Enum.LaunchType.Credit);

            Assert.NotNull(launch);
            Assert.Equal(20.0m, launch.Amount);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveItem()
        {
            var context = GetInMemoryDbContext();
            var repository = new Repository<Launch>(context, _loggerMock.Object);

            var date = new DateTime(2025, 03, 04, 0, 0, 0);
            context.Launches.Add(new Launch { Date = date, Type = Domain.Enum.LaunchType.Credit, Amount = 20.0m });
            await context.SaveChangesAsync();

            await repository.DeleteAsync(date, Domain.Enum.LaunchType.Credit);
            var launches = await repository.GetAllAsync();

            Assert.DoesNotContain(launches, l => l.Date == date && l.Type == Domain.Enum.LaunchType.Credit);
        }
    }
}
