using System.ComponentModel.DataAnnotations;
namespace EventApplication.Models.RequestFolder
{
    public record class EventRequest(
        [Required(ErrorMessage = "Укажите название мероприятия")] [property:  Display(Name = "Название мероприятия")]
        string Name,

        [Required(ErrorMessage = "Укажите место проведения мероприятия")] [property:  Display(Name = "Место проведения")] 
        string Place,

        [Required(ErrorMessage = "Укажите дату мероприятия")] [property:  Display(Name = "Дата проведения")] 
        DateOnly Date,

        [Required(ErrorMessage = "Укажите время начала мероприятия")] [property:  Display(Name = "Время создания")] 
        TimeOnly TimeStart, 

        [ Required(ErrorMessage = "Укажите время окончания мероприятия")] [property:  Display(Name = "Время окончания")] 
        TimeOnly TimeEnd,

        [Required(ErrorMessage = "Укажите максимальное количество гостей")] [property:  Display(Name = "Максимальное количество гостей")] 
        int Max_Ghost,

        [Required(ErrorMessage = "Добавьте описание мероприятия")] [property:  Display(Name = "Описание мероприятия")]
        string Description,

        [Required] [property:  Display(Name = "Создано")]
        DateTimeOffset Created,

        [Required] [property : Display(Name = "Статус мероприятия")] 
        string Status,
        Guid UserId, 
        
        [Required(ErrorMessage = "Добавьте хотя бы 1 тэг")] 
        List<int> TagesIds, 


        string? Dop_Information
        );

}