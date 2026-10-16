namespace RaceDayAPI.DTOs
{
    public record RegisterDto(string Name, string Surname, string Email, string Password, int RoleId);
    public record LoginDto(string Email, string Password);
    public record AuthResponseDto(string Token, int UserId, string Email, string Role);

    public record UserProfileDto(int UserId, string Name, string Surname, string Email, string Role);
    public record UpdateProfileDto(string Name, string Surname);

    public record CategoryDto(int CategoryId, string CategoryName, string? Description);
    public record CreateCategoryDto(string CategoryName, string Description);

    public record EventDto(int EventId, int OrganiserId, int CategoryId, string Title, string? Description, DateTime EventDate, string Location, string? CategoryName, string? OrganiserName);
    public record CreateEventDto(string Title, string Description, int CategoryId, DateTime EventDate, string Location);

    public record EnrolmentDto(int EnrolmentId, int EventId, int ParticipantId, DateTime EnrolmentDate, string Status, string? ParticipantName, string? EventTitle);

    public record ResultDto(int ResultId, int EnrolmentId, int? FinishTimeSeconds, int? Position, string Status, string? ParticipantName);
    public record CreateResultDto(int EnrolmentId, int FinishTimeSeconds, int Position, string Status);
}