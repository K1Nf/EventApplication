using EventApplication.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventApplication.ViewModels
{
    
    public class AccEventVM
    {
        public required User User { get; set; }

        public required List<Event> Events { get; set; }
    }
}
