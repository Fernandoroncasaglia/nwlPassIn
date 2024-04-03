using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Checkin.DoCheckIn;

public class DoAttendeeCheckInUseCase
{
    private readonly PassInDbContext _dbContext;
    public DoAttendeeCheckInUseCase()
    {
        _dbContext = new PassInDbContext();
    }
    public ResponseRegisteredJson Execute(Guid attendeeId)
    {
        Validate(attendeeId);

        var entity = new CheckIn
        {
            Attendee_id = attendeeId,
            Created_at = DateTime.UtcNow,
        };

        _dbContext.CheckIns.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Id,
        };
    }

    private void Validate(Guid attendeeId)
    {
        var existAttendee = _dbContext.Attendees.Any(attendee => attendee.Id == attendeeId);
        if (existAttendee == false)
        {
            throw new NotFoundException("The Attendee with this Id was not found.");
        }

        var existCheckIn = _dbContext.CheckIns.Any(ch => ch.Attendee_id == attendeeId);

        if (existCheckIn)
        {
            throw new ConflictException("Attendee can not do checking twice in the same event.");
        }
    }
}