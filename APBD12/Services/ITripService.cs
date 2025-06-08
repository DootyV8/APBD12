using APBD12.Dtos;

namespace APBD12.Services;

public interface ITripService
{
    TripResponse GetTrips(int page, int pageSize);
    Task<(bool Success, string Message)> AssignClientToTripAsync(int idTrip, AssignClientDto dto, CancellationToken ct);
}