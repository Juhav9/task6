using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using UnivEnrollerApi.Data;
using Savonia.xUnit.Helpers;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;
using Savonia.xUnit.Helpers.Infrastructure;
using System.Threading.Tasks;

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
    [JsonFileData("testdata.json", "put", typeof(Tuple<int, int, int, DateTime, bool>), typeof(int))]
    public async void Checkpoint05_PUT(Tuple<int, int, int, DateTime, bool> data, int expected)
    {
        // Arrange
        var grade = new JsonObject
        {
            { "studentId", data.Item1 },
            { "courseId", data.Item2 },
            { "grade", data.Item3 },
            { "gradingDate", data.Item4 },
            { "override", data.Item5 }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/grade", grade);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expected, (int)response.StatusCode);

        // Act 2
        var e = await GetGrading(data);

        // Assert 2
        Assert.NotNull(e);
        Assert.Equal(data.Item3, e.Value.GetProperty("grade").GetInt32());
        Assert.Equal(data.Item4.Date, e.Value.GetProperty("gradingDate").GetDateTime().Date);
    }

    [Theory]
    [JsonFileData("testdata.json", "fail", typeof(Tuple<int, int, int, DateTime, bool>), typeof(Tuple<int, string?>))]
    public async void Checkpoint05_ShouldFailToPut(Tuple<int, int, int, DateTime, bool> data, Tuple<int, string?> expected)
    {
        // Arrange
        var grade = new JsonObject
        {
            { "studentId", data.Item1 },
            { "courseId", data.Item2 },
            { "grade", data.Item3 },
            { "gradingDate", data.Item4 },
            { "override", data.Item5 }
        };
        var initialGrading = await GetGrading(data);

        // Act
        var response = await _client.PutAsJsonAsync($"/grade", grade);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expected.Item1, (int)response.StatusCode);
        if (null != expected.Item2)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(expected.Item2, content);
        }
        if (null != initialGrading)
        {
            var e = await GetGrading(data);
            Assert.NotNull(e);
            Assert.Equal(initialGrading.Value.GetProperty("grade").GetRawText(), e.Value.GetProperty("grade").GetRawText());
        }
    }

    private async Task<JsonElement?> GetGrading(Tuple<int, int, int, DateTime, bool> data)
    {
        var responseGet = await _client.GetFromJsonAsync<IEnumerable<object>>($"/student/{data.Item1}/courses");
        Assert.NotNull(responseGet);
        object? obj = responseGet.FirstOrDefault(re => ((JsonElement)re).GetProperty("courseId").GetInt32() == data.Item2);
        if (null == obj)
        {
            return null;
        }
        else
        {
            return (JsonElement)obj;
        }
    }
}