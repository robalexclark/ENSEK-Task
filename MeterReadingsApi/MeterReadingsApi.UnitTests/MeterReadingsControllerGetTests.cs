using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeterReadingsApi.Controllers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class MeterReadingsControllerGetTests
{
    private class DummyService : IMeterReadingUploadService
    {
        public Task<MeterReadingsApi.Models.MeterReadingUploadResult> UploadAsync(IFormFile file) => throw new NotImplementedException();
    }

    private class Repo : IMeterReadingsRepository
    {
        public bool AccountExistsReturn { get; set; }
        public IEnumerable<MeterReading> Readings { get; set; } = new List<MeterReading>();
        public IEnumerable<Account> GetAccounts() => Array.Empty<Account>();
        public Task AddMeterReadingsAsync(IEnumerable<MeterReading> readings) => Task.CompletedTask;
        public bool AccountExists(int accountId) => AccountExistsReturn;
        public bool ReadingExists(int accountId, DateTime dateTime) => false;
        public bool HasNewerReading(int accountId, DateTime dateTime) => false;
        public void EnsureSeedData() { }
        public Task<IEnumerable<MeterReading>> GetReadingsByAccountAsync(int accountId) => Task.FromResult(Readings);
    }

    [Fact]
    public async Task GetByAccountId_ReturnsNotFound_ForMissingAccount()
    {
        var repo = new Repo { AccountExistsReturn = false };
        var controller = new MeterReadingsController(new DummyService(), repo);

        var result = await controller.GetByAccountId(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetByAccountId_ReturnsReadings()
    {
        var repo = new Repo
        {
            AccountExistsReturn = true,
            Readings = new[] { new MeterReading { AccountId = 1 } }
        };
        var controller = new MeterReadingsController(new DummyService(), repo);

        var result = await controller.GetByAccountId(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var readings = Assert.IsAssignableFrom<IEnumerable<MeterReading>>(ok.Value);
        Assert.Single(readings);
    }
}
