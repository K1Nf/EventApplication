using System.ComponentModel.DataAnnotations;
namespace EventApplication.Models.RequestFolder
{
    public record class UserRequest([Required] string UserName, [Required] string FirstName,
        [Required] string LastName, string? SecondName, [Required] string PhoneNumber,
        [Required] string Email, [Required] string Password, [Required] string City,
        [Required] string Country, string? Information, [Required] DateOnly BirthDate, string? ImageLink);
}