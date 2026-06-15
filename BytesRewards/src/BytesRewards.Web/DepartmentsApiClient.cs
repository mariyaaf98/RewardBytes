using System.Net.Http.Json;

namespace BytesRewards.Web;

public class DepartmentsApiClient(HttpClient httpClient)
{
    public async Task<List<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        var departments = await httpClient.GetFromJsonAsync<List<DepartmentDto>>(
            "/departments", cancellationToken);

        return departments ?? [];
    }

    public async Task<Guid?> CreateDepartmentAsync(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/departments", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken);
    }

    // NOTE: Update and Delete endpoints are not yet available on the backend.
    // These methods are ready to be wired once PUT /departments/{id}
    // and DELETE /departments/{id} are implemented.

    public async Task UpdateDepartmentAsync(
        Guid id,
        CreateDepartmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/departments/{id}", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteDepartmentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"/departments/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

public sealed class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
