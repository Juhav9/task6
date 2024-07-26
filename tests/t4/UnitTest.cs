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

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> fixture) : base(new string[] { "Courses.db" }, testOutputHelper)
    {
        _client = fixture.CreateClient();
        WriteLine(fixture.HostUrl);
    }

    [Theory]
    [JsonFileData("testdata.json", "delete", typeof(Tuple<string, string>), typeof(IEnumerable<int>))]
    public async void Checkpoint04_DELETE(Tuple<string, string> data, IEnumerable<int> expected)
    {
        // Try to delete a course that should be able to be deleted. Then verify that the course is deleted.

        // Arrange

        // Act
        var response = await _client.DeleteAsync($"{data.Item1}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(204, (int)response.StatusCode);

        // Act 2
        var responseGet = await _client.GetFromJsonAsync<IEnumerable<object>>($"{data.Item2}");
        Assert.NotNull(responseGet);
        var actualCourseIds = responseGet.Select(r => ((JsonElement)r).GetProperty("courseId").GetInt32());
        Assert.Equal(expected.OrderBy(i => i), actualCourseIds.OrderBy(i => i));
    }

    [Theory]
    [JsonFileData("testdata.json", "codes", typeof(Tuple<string, string>), typeof(int))]
    public async void Checkpoint04_NotDeleted(Tuple<string, string> data, int expected)
    {
        // Try to delete a course that should not be able to be deleted. Then verify that the course is still there.

        // Arrange
        var responseGetInitial = await _client.GetFromJsonAsync<IEnumerable<object>>($"{data.Item2}");
        Assert.NotNull(responseGetInitial);
        var initialCourseIds = responseGetInitial.Select(r => ((JsonElement)r).GetProperty("courseId").GetInt32());

        // Act
        var response = await _client.DeleteAsync($"{data.Item1}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(expected, (int)response.StatusCode);

        // Act 2
        var responseGetAfter = await _client.GetFromJsonAsync<IEnumerable<object>>($"{data.Item2}");
        Assert.NotNull(responseGetAfter);
        var afterCourseIds = responseGetInitial.Select(r => ((JsonElement)r).GetProperty("courseId").GetInt32());
        Assert.Equal(initialCourseIds, afterCourseIds);
    }
}