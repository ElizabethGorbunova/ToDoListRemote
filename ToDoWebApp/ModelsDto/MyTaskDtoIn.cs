﻿using ToDoWebApp.Enums;
using System.ComponentModel.DataAnnotations;
using ToDoWebApp.Entities;

namespace ToDoWebApp.ModelsDto
{
    public class MyTaskDtoIn
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
        public Category Category { get; set; }

        public int GroupId { get; set; }
        

    }
}
