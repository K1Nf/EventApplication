using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using NuGet.Packaging.Signing;
using Microsoft.AspNetCore.Mvc;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using EventApplication.Models.RequestFolder;

namespace EventApplication.Models
{
    [PrimaryKey("Id")]
    public class Event
    {
        private Event(string _name, string _place, DateOnly _date, TimeOnly _timestart, TimeOnly _timeend, int _max_ghost, string _description, Guid _userId)
        {
            Name = _name;
            Place = _place;
            Date = _date;
            TimeStart = _timestart;
            TimeEnd = _timeend;
            Max_Ghost = _max_ghost;
            Description = _description;
            Created = DateTimeOffset.UtcNow;
            Status = Statuses[0];
            UserId = _userId;
            Id = Guid.NewGuid();
        }

        public Event()
        {
            
        }



        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }



        [Required(ErrorMessage = "Укажите место проведения мероприятия")]
        [Display(Name = "Название проведения")]
        public string Name { get; set; }



        [Required(ErrorMessage = "Укажите место проведения мероприятия")]
        [Display(Name = "Место проведения")]
        public string Place { get; set; }



        [Required(ErrorMessage = "Укажите дату мероприятия")]
        [Display(Name = "Дата проведения")]
        public DateOnly Date { get; set; }



        [Required(ErrorMessage = "Укажите время начала мероприятия")]
        [Display(Name = "Время создания")]
        public TimeOnly TimeStart { get; set; }



        [Required(ErrorMessage = "Укажите время окончания мероприятия")]
        [Display(Name = "Время окончания")]
        public TimeOnly TimeEnd { get; set; }



        [Required(ErrorMessage = "Укажите максимальное количество гостей")]
        [Display(Name = "Максимальное количество гостей")]
        public int Max_Ghost { get; set; }



        [Display(Name="Ход мероприятия")]
        public string Status { get; set; } = Statuses[0];



        [Required(ErrorMessage = "Добавьте описание мероприятия")]
        [Display(Name = "Описание мероприятия")]
        public string? Description { get; set; } = string.Empty;



        [Display(Name = "Дополнительная информация о мероприятии")]
        public string? Dop_Information { get; set; } = string.Empty;


        [Display(Name = "Мероприятие создано ")]
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;



        [Required]
        [Display(Name = "Организатор мероприятия")]
        public Guid UserId { get; set; }


        
        [ValidateNever]
        [Display(Name = "Организатор мероприятия")]
        public User User { get; set; } = null!;



        [Required(ErrorMessage = "Добавьте тэги для мероприятия")]
        [Display(Name = "Тэги")]
        public int TagId { get; set; }



        public List<Tag> Tags { get; set; } = [];



        [NotMapped]
        [Required(ErrorMessage = "Добавьте хотя бы 1 тэг pls")]
        public List<int> TagesIds { get; set; } 



        public static Result<Event> CreateEvent(EventRequest eventRequest)
        {
            if (string.IsNullOrWhiteSpace(eventRequest.Name) || string.IsNullOrWhiteSpace(eventRequest.Place) ||
                 (eventRequest.TimeStart > eventRequest.TimeEnd) || string.IsNullOrWhiteSpace(eventRequest.Description))
                return Result.Failure<Event>("Неверно заданы параметры");


            // колхоз-заглушка


            //GUID первого пользователя
            Guid userId = Guid.Parse("680062c6-9456-47da-b091-f3ebbcea269a");              
            // конец колхоз-заглушки


            var @event = new Event(eventRequest.Name, eventRequest.Place, eventRequest.Date, eventRequest.TimeStart, eventRequest.TimeEnd,
                eventRequest.Max_Ghost, eventRequest.Description, userId);

            return Result.Success(@event);

        }



        private static readonly List<string> Statuses = ["Запланировано", "В процессе", "Завершено"];
        public static void ChangeStatus(Event ev)
        {
            if ((ev.Date == DateOnly.FromDateTime(DateTime.Today)) &&
                (ev.TimeStart.Hour <= DateTime.Now.Hour) &&
                (ev.TimeStart.Minute <= DateTime.Now.Minute))
            {
                Console.WriteLine("Событие началось...");
                ev.Status = Statuses[1];
            }

            if ((ev.Date < DateOnly.FromDateTime(DateTime.Today)) ||
                ((ev.TimeEnd.Hour <= DateTime.Now.Hour) &&
                (ev.TimeEnd.Minute <= DateTime.Now.Minute) &&
                (ev.Date <= DateOnly.FromDateTime(DateTime.Today))))
            {
                Console.WriteLine("Событие завершено...");
                ev.Status = Statuses[2];
            }
        }
    }
}
