using APBD12.Data;
using Microsoft.EntityFrameworkCore;

namespace APBD12.Services;

public class ClientService : IClientService
{
    private readonly S30515Context _context;
    public ClientService(S30515Context context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> DeleteClientAsync(int idClient)
    {
        var hasTrips = await _context.ClientTrips.AnyAsync(ct => ct.IdClient == idClient);
        if (hasTrips)
            return (false, "Client has assigned trips, cannot delete.");

        var client = await _context.Clients.FindAsync(idClient);
        if (client == null)
            return (false, "Client not found.");

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}