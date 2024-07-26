using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Savonia.xUnit.Helpers;
using Xunit;
using Xunit.Abstractions;
using System.Text.Json;
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
    [JsonFileData("testdata.json", "courses", typeof(string), typeof(IEnumerable<int>))]
    public async void Checkpoint02_Courses(string data, IEnumerable<int> expected)
    {
        // Arrange

        // Act
        var response = await _client.GetFromJsonAsync<IEnumerable<object>>($"{data}");

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        var actual = response.Select(r => ((JsonElement)r).GetProperty("courseId").GetInt32());
        Assert.Equal(expected.OrderBy(i => i), actual.OrderBy(i => i));
    }

    [Theory]
    [JsonFileData("testdata.json", "grades", typeof(Tuple<string, int>), typeof(Tuple<int, DateTime>))]
    public async void Checkpoint02_Grade(Tuple<string, int> data, Tuple<int, DateTime> expected)
    {
        // Arrange

        // Act
        var response = await _client.GetFromJsonAsync<IEnumerable<object>>($"{data.Item1}");

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Random r = new Random();
        JsonElement e = (JsonElement)response.Skip(r.Next(0, response.Count())).Take(1).First();
        Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => e.GetProperty("university"));
        JsonElement val;
        Assert.True(e.TryGetProperty("id", out val));
        Assert.True(e.TryGetProperty("course", out val));
        Assert.True(e.TryGetProperty("courseId", out val));

        object? obj = response.FirstOrDefault(re => ((JsonElement)re).GetProperty("id").GetInt32() == data.Item2);
        Assert.NotNull(obj);
        e = (JsonElement)obj;
        Assert.Equal(expected.Item1, e.GetProperty("grade").GetInt32());
        Assert.Equal(expected.Item2, e.GetProperty("gradingDate").GetDateTime().Date);
    }

    [Theory]
    [JsonFileData("testdata.json", "notfound", typeof(string), typeof(int))]
    public async void Checkpoint02_NotFound(string data, int expected)
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{data}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expected, (int)response.StatusCode);
    }
}