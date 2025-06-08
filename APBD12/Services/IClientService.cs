namespace APBD12.Services;

public interface IClientService
{
    Task<(bool Success, string Message)> DeleteClientAsync(int idClient);
    
}