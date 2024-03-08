using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FonTech.Domain.DTO;

public class UserDTO
{
    [DisplayName("Адресс почты")]
    [MaxLength(156, ErrorMessage = "Длина Email адреса не должна превышать {1} символов")]
    [Required(ErrorMessage = Messages.REQUIRED)]
    [EmailAddress(ErrorMessage = "Некорректный формат Email адреса")]
    public string? Email { get; init; }

    [DisplayName("Номер телефона")]
    [Phone(ErrorMessage = "Некорректный формат номера телефона")]
    [Required(ErrorMessage = Messages.REQUIRED)]
    public string? Phone { get; init; }

    [DisplayName("Возраст")]
    [MaxLength(4, ErrorMessage = "Длина возраста не должна превышать {1} символа")]
    [Required(ErrorMessage = Messages.REQUIRED)]
    [Range(0, 1900, ErrorMessage = "Поле возраста должно быть в диапазоне от 0 до 1900")]
    public int? Age { get; init; }

    [DisplayName("Роль")]
    [Required(ErrorMessage = Messages.REQUIRED)]
    public Roles? Role { get; init; }
}

public static class Messages
{
    public const string REQUIRED = "Поле [{0}] обязательно для заполнения";
}

public enum Roles
{
    Admin = 1,
    Moderator = 2,
    User = 3,
    Guest = 4
}
