using APBD12.Dtos;
using APBD12.Data;
using APBD12.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD12.Services;

public class TripService : ITripService
{
    private readonly S30515Context _context;
    public TripService(S30515Context context)
    {
        _context = context;
    }

    public TripResponse GetTrips(int page, int pageSize)
    {
        var query = _context.Trips
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips).ThenInclude(ct => ct.IdClientNavigation)
            .OrderByDescending(t => t.DateFrom)
            .Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries
                    .Select(c => new CountryDto { Name = c.Name })
                    .ToList(),
                Clients = t.ClientTrips
                    .Where(ct => ct.IdClientNavigation != null)
                    .Select(ct => new ClientDto
                    {
                        FirstName = ct.IdClientNavigation!.FirstName,
                        LastName = ct.IdClientNavigation.LastName
                    }).ToList()
            });

        var total = query.Count();
        var allPages = (int)Math.Ceiling(total / (double)pageSize);

        return new TripResponse
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = allPages,
            Trips = query.Skip((page - 1) * pageSize).Take(pageSize).ToList()
        };
    }

    public async Task<(bool Success, string Message)> AssignClientToTripAsync(int idTrip, AssignClientDto dto, CancellationToken ct)
    {
        var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel, ct);
        if (existingClient != null)
        {
            var alreadyRegistered = await _context.ClientTrips.AnyAsync(ct =>
                ct.IdClient == existingClient.IdClient && ct.IdTrip == idTrip, ct);
            if (alreadyRegistered) return (false, "Client already registered for this trip.");
            return (false, "Client with this PESEL already exists.");
        }

        var trip = await _context.Trips.FindAsync(new object[] { idTrip }, ct);
        if (trip == null) return (false, "Trip not found.");
        if (trip.DateFrom <= DateTime.Now) return (false, "Cannot register for a past trip.");

        var client = existingClient ?? new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };
        if (existingClient == null)
            _context.Clients.Add(client);

        // Save changes to get the IdClient for a new client
        if (existingClient == null)
            await _context.SaveChangesAsync(ct);

        var clientTrip = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.UtcNow,
            PaymentDate = dto.PaymentDate
        };
        _context.ClientTrips.Add(clientTrip);

        await _context.SaveChangesAsync(ct);
        return (true, null);
    }
}
