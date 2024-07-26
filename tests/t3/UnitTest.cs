using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using UnivEnrollerApi.Data;
using UnivEnrollerApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Savonia.xUnit.Helpers;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Numerics;
using Savonia.xUnit.Helpers.Infrastructure;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly HttpClient _client;
    private readonly UnivEnrollerContext _db;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture) : base(new string[] { "Courses.db" }, testOutputHelper)
    {
        _client = fixture.CreateClient();
        WriteLine(fixture.HostUrl);

        string connectionstring = "Data Source=Courses.db";

        var optionsBuilder = new DbContextOptionsBuilder<UnivEnrollerContext>();
        optionsBuilder.UseSqlite(connectionstring);

        _db = new UnivEnrollerContext(optionsBuilder.Options);
    }

    [Fact]
    public async void Checkpoint03_PUT()
    {
        // Arrange
        Random r = new Random();
        int skip = r.Next(0, _db.Courses.Count());
        var data = await _db.Courses.Skip(skip).Take(1).FirstAsync();
        var id = data.Id;
        string name = $"{data.Name}-{r.Next()}";
        var course = new JsonObject
        {
            { "name", name }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/course/{id}", course);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(204, (int)response.StatusCode);

        // Act 2
        var responseGet = await _client.GetFromJsonAsync<object>($"/course/{id}");
        Assert.NotNull(responseGet);

        JsonElement e = (JsonElement)responseGet;
        Assert.Equal(id, e.GetProperty("id").GetInt32());
        Assert.Equal(name, e.GetProperty("name").GetString());
    }

    [Theory]
    [JsonFileData("testdata.json", typeof(string), typeof(int))]
    public async void Checkpoint03_NotFound(string data, int expected)
    {
        // Arrange
        string name = $"some value";
        var course = new JsonObject
        {
            { "name", name }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{data}", course);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expected, (int)response.StatusCode);
    }

    [Fact]
    public async void Checkpoint03_Invalid()
    {
        // Arrange
        Random r = new Random();
        int skip = r.Next(0, _db.Courses.Count());
        var data = await _db.Courses.Skip(skip).Take(1).FirstAsync();
        var id = data.Id;
        string name = "";
        var course = new JsonObject
        {
            { "name", name }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/course/{id}", course);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(400, (int)response.StatusCode);
    }

}