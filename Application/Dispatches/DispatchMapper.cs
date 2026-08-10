using Domain;
namespace Application.Dispatches;

public static class DispatchMapper
{
    public static Dispatch ToDomain(CreateDispatchRequest request, Guid shipperId)
    {
        Stop pickupStop = Stop.Create(1, request.PickupStop.Address,
            request.PickupStop.LocationName, request.PickupStop.ContactName,
            request.PickupStop.ContactPhone, request.PickupStop.ContactEmail);

        Stop dropoffStop = Stop.Create(2, request.DropoffStop.Address,
            request.DropoffStop.LocationName, request.DropoffStop.ContactName,
            request.DropoffStop.ContactPhone, request.DropoffStop.ContactEmail);

        IEnumerable<(string Vin, int Year, string Make, string Model, string Color)> vehicleInputs =
            request.Vehicles.Select(v => (v.Vin, v.Year, v.Make, v.Model, v.Color));

        return Dispatch.Create(shipperId, request.CarrierId,
            request.Price, request.PickupDate, request.DropoffDate,
            request.Description, pickupStop, dropoffStop, vehicleInputs);
    }

    public static CreateDispatchResponse ToResponse(Dispatch dispatch)
    {
        return new CreateDispatchResponse(
            dispatch.DispatchId,
            dispatch.ShipperId,
            dispatch.CarrierId,
            dispatch.DispatchStatus.ToString(),
            dispatch.Price,
            dispatch.PickupDate,
            dispatch.DropoffDate,
            dispatch.Description,
            dispatch.IsSigned,
            ToStopResponse(dispatch.PickupStop),
            ToStopResponse(dispatch.DropoffStop),
            dispatch.Vehicles.Select(ToVehicleResponse),
            dispatch.CreatedAt);
    }

    private static StopResponse ToStopResponse(Stop stop)
    {
        return new StopResponse(
            stop.StopId,
            stop.StopNumber.ToString(),
            stop.Address,
            stop.LocationName,
            stop.ContactName,
            stop.ContactPhone,
            stop.ContactEmail);
    }

    private static VehicleResponse ToVehicleResponse(Vehicle vehicle)
    {
        return new VehicleResponse(
            vehicle.VehicleId,
            vehicle.VehicleStatus.ToString(),
            vehicle.Vin,
            vehicle.Year,
            vehicle.Make,
            vehicle.Model,
            vehicle.Color,
            ToStopResponse(vehicle.PickupStop),
            ToStopResponse(vehicle.DropoffStop));
    }
}


