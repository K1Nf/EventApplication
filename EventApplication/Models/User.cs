using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.HttpResults;
using EventApplication.Models.RequestFolder;

namespace EventApplication.Models
{
    public class User
    {

        public User()
        {
            
        }
        public User(string userName, string firstName, string lastName, string secondName, 
                    string phoneNumber, string email, string password, string city, 
                    string country, string information, DateOnly birthDate, string imageLink)
        {
            BirthDate = birthDate;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            Password = password;
            City = city;
            Country = country;
            Rating = 0;
            Count_Rating = 0;
            DateRegistration = DateTimeOffset.UtcNow;
            Image_link = imageLink;
            Information = information;
            SecondName = secondName;
            Id = Guid.NewGuid();
        }


        
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }



        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }



        [Display(Name = "Отчество")]
        public string SecondName { get; set; } = null!;



        [Required(ErrorMessage = "Введите фамилию")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }



        [Required(ErrorMessage = "Укажите Логин")]
        [Display(Name = "Логин")]
        public string UserName { get; set; }



        [Display(Name = "Ссылка на фото-аватар")]
        public string? Image_link { get; set; } = string.Empty;



        [Required(ErrorMessage = "Укажите телефон")]
        [Display(Name = "Телефон")]
        [StringLength(11)]
        public string? PhoneNumber { get; set; }



        [Required(ErrorMessage = "Укажите электронную почту")]
        [Display(Name = "Электронная почта")]
        [EmailAddress]
        public string? Email { get; set; }



        [Required(ErrorMessage = "Укажите город")]
        [Display(Name = "Город")]
        public string City { get; set; }



        [Required]
        [Display(Name = "Рейтинг")]
        public double Rating { get; set; } 



        [Required]
        public int Count_Rating { get; set; }



        [Required(ErrorMessage = "Расскажите о себе всем")]
        [Display(Name = "Информация о пользователе")]
        public string? Information { get; set; } = string.Empty;



        [PasswordPropertyText]
        [Required(ErrorMessage = "Придумайте пароль")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }



        [Required]
        [Display(Name = "Страна")]
        public string Country { get; set; }



        [Display(Name = "Дата регистрации")]
        public DateTimeOffset DateRegistration { get; set; } = DateTimeOffset.UtcNow;



        [Required(ErrorMessage = "Укажите дату рождения")]
        [Display(Name = "Дата рождения")]
        public DateOnly BirthDate { get; set; }


        [NotMapped]
        [Display(Name = "Тэги")]
        public int TagId { get; set; }
        [NotMapped]
        public List<Tag> Tags { get; set; } = [];



        [NotMapped]
        [Required(ErrorMessage = "Добавьте хотя бы 1 тэг pls")]
        public List<int> TagsIds { get; set; }



        public static Result<User> CreateUser(UserRequest userRequest)
        {
            if (string.IsNullOrWhiteSpace(userRequest.FirstName) || string.IsNullOrWhiteSpace(userRequest.LastName) ||
                string.IsNullOrWhiteSpace(userRequest.Country) || string.IsNullOrWhiteSpace(userRequest.PhoneNumber) ||
                string.IsNullOrWhiteSpace(userRequest.City) || string.IsNullOrWhiteSpace(userRequest.Email) ||
                string.IsNullOrWhiteSpace(userRequest.BirthDate.ToString()))
            {
                return Result.Failure<User>("Неверно заданы параметры"); // Result.Failure("Неверно заданы параметры");
            }

            var user = new User(userRequest.UserName, userRequest.FirstName, userRequest.LastName,
                                userRequest.SecondName!, userRequest.PhoneNumber, userRequest.Email,
                                userRequest.Password, userRequest.City, userRequest.Country,
                                userRequest.Information!, userRequest.BirthDate, userRequest.ImageLink!);

            return Result.Success(user);
        }

        public Result ChangeRating(int mark)
        {
            Count_Rating++;
            Rating = (mark + Rating) / Count_Rating;

            return Result.Success();
        }
    }
}
