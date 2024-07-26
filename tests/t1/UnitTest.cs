using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Savonia.xUnit.Helpers;
using Xunit;
using Xunit.Abstractions;
using System.Text.Json;
using System.Text.Json.Nodes;
using Savonia.xUnit.Helpers.Infrastructure;

namespace tests;

public class UnitTest : AppTestBase, IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly HttpClient _client;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture) : base(new string[] { "Courses.db" }, testOutputHelper)
    {
        _client = fixture.CreateClient();
        WriteLine(fixture.HostUrl);
    }

    [Theory]
    [JsonFileData("testdata.json", "get", typeof(string), typeof(IEnumerable<string>))]
    public async void Checkpoint01_GET(string data, IEnumerable<string> expected)
    {
        // Arrange

        // Act
        var response = await _client.GetFromJsonAsync<IEnumerable<object>>($"{data}");

        // Assert
        Assert.NotNull(response);
        var actual = response.Select(r => ((JsonElement)r).GetProperty("name").GetString());
        Assert.Equal(expected.OrderBy(i => i), actual.OrderBy(i => i));
    }

    [Fact]
    public async void Checkpoint01_POST()
    {
        // Arrange
        Random r = new Random();
        int univId = 1;
        string name = $"newly created university {r.Next()}";
        var univ = new JsonObject
        {
            { "id", univId },
            { "name", name }
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/university", univ);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var location = response.Headers.Location;

        // Act 2
        var responseGet = await _client.GetFromJsonAsync<object>(location);
        Assert.NotNull(responseGet);

        JsonElement e = (JsonElement)responseGet;
        Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() =>e.GetProperty("course"));
        Assert.Equal(name, e.GetProperty("name").GetString());
        Assert.NotEqual(univId, e.GetProperty("id").GetInt32());
    }

    [Theory]
    [JsonFileData("testdata.json", "delete", typeof(string), typeof(int))]
    public async void Checkpoint01_DELETE(string data, int expected)
    {
        // Arrange

        // Act
        var response = await _client.DeleteAsync($"{data}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expected, (int)response.StatusCode);

        // Act 2
        if (response.IsSuccessStatusCode)
        {
            var responseAfterDelete = await _client.DeleteAsync($"{data}");
            Assert.NotNull(responseAfterDelete);
            Assert.Equal(404, (int)responseAfterDelete.StatusCode);
        }
    }

    [Fact]
    public async void Checkpoint01_GET_make_some_coffee()
    {
        // Arrange
        var data = "/university/make-some-coffee";
        var expected = 418;

        // Act
        var response = await _client.GetAsync($"{data}");

        // Assert
        Assert.NotNull(response);
        var actual = (int)response.StatusCode;
        Assert.Equal(expected, actual);
    }
}