using System.Text;
using IntegrationTests.Factories;
using Xunit;

namespace IntegrationTests.IntegrationTests;

public class DiffIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DiffIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task TestSaveLeftAndRightAndGetDiff_Equal()
    {
        string id = "1";

        // Save left data
        var leftContent = new StringContent("{\"data\": \"AAAAAA==\"}", Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/v1/diff/{id}/left", leftContent);
        response.EnsureSuccessStatusCode();

        // Save right data
        var rightContent = new StringContent("{\"data\": \"AAAAAA==\"}", Encoding.UTF8, "application/json");
        response = await _client.PutAsync($"/v1/diff/{id}/right", rightContent);
        response.EnsureSuccessStatusCode();

        // Get diff
        response = await _client.GetAsync($"/v1/diff/{id}");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"diffResultType\":\"Equals\"", responseString);
    }

    [Fact]
    public async Task TestSaveLeftAndRightAndGetDiff_SizeDoNotMatch()
    {
        string id = "2";

        // Save left data
        var leftContent = new StringContent("{\"data\": \"AAA=\"}", Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/v1/diff/{id}/left", leftContent);
        response.EnsureSuccessStatusCode();

        // Save right data
        var rightContent = new StringContent("{\"data\": \"AAAAAA==\"}", Encoding.UTF8, "application/json");
        response = await _client.PutAsync($"/v1/diff/{id}/right", rightContent);
        response.EnsureSuccessStatusCode();

        // Get diff
        response = await _client.GetAsync($"/v1/diff/{id}");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"diffResultType\":\"SizeDoNotMatch\"", responseString);
    }

    [Fact]
    public async Task TestSaveLeftAndRightAndGetDiff_ContentDoNotMatch()
    {
        string id = "3";

        // Save left data
        var leftContent = new StringContent("{\"data\": \"AAAAAA==\"}", Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/v1/diff/{id}/left", leftContent);
        response.EnsureSuccessStatusCode();

        // Save right data
        var rightContent = new StringContent("{\"data\": \"AQABAQ==\"}", Encoding.UTF8, "application/json");
        response = await _client.PutAsync($"/v1/diff/{id}/right", rightContent);
        response.EnsureSuccessStatusCode();

        // Get diff
        response = await _client.GetAsync($"/v1/diff/{id}");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"diffResultType\":\"ContentDoNotMatch\"", responseString);
        Assert.Contains("\"offset\":0", responseString);
        Assert.Contains("\"length\":1", responseString);
        Assert.Contains("\"offset\":2", responseString);
        Assert.Contains("\"length\":2", responseString);
    }
}