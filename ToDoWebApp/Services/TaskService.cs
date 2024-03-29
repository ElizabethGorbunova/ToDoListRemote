﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDoWebApp.Entities;
using ToDoWebApp.Enums;
using ToDoWebApp.ModelsDto;

namespace ToDoWebApp.Services;

public class TaskService : ITaskService
{
    private readonly TaskDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<TaskService> _logger;
    public TaskService(TaskDbContext dbContext, IMapper mapper, ILogger<TaskService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }
    

    public IEnumerable<MyTaskDtoOut> GetAllTasks()
    {
        var allTasks = _dbContext
            .Tasks
            .Include(t => t.User)
            .ToList();
        if (allTasks == null)
        {
            return null;
        }

        var allTasksDto = _mapper.Map<List<MyTaskDtoOut>>(allTasks);
        return allTasksDto;

    }

  

    public MyTaskDtoOut GetTaskById(int id)
    {
        var task = _dbContext
            .Tasks
            .Include(t=>t.User)
            .FirstOrDefault(t => t.MyTaskId == id);
        if (task == null)
        {
            return null;
        }
        var myTaskDto = _mapper.Map<MyTaskDtoOut>(task);
        var GroupNameFromDB = _dbContext.Groups.FirstOrDefault(g => g.GroupId == task.GroupId);
        myTaskDto.GroupName = GroupNameFromDB.GroupName;
        return myTaskDto;
    }


    public MyTaskDtoOut AddNewTask(MyTaskDtoIn task, int userId)         
    {
        var myTaskMapped = _mapper.Map<MyTask>(task);
        myTaskMapped.UserId = userId;
        

        _dbContext.Add(myTaskMapped);
        _dbContext.SaveChanges();

        var taskDtoOut = _mapper.Map<MyTaskDtoOut>(myTaskMapped);
        var GroupNameFromDB = _dbContext.Groups.FirstOrDefault(g => g.GroupId == task.GroupId);
        taskDtoOut.GroupName = GroupNameFromDB.GroupName;
        return taskDtoOut;


    }


    public EditTaskResult<MyTaskDtoOut> EditTask(MyTaskDtoIn taskDtoIn, int id)   
    {
        var taskToEdit = _dbContext.Tasks.FirstOrDefault(t => t.MyTaskId == id);

        if(taskToEdit == null)
        {
            return new EditTaskResult<MyTaskDtoOut> { IsSuccess = false, Model = null };
        }

        taskToEdit.Status = taskDtoIn.Status;
        taskToEdit.Category = taskDtoIn.Category;
        taskToEdit.Name =taskDtoIn.Name;
        taskToEdit.Date = taskDtoIn.Date;
        taskToEdit.GroupId= taskDtoIn.GroupId;
        _dbContext.SaveChanges();

        var myTaskDtoOut = _mapper.Map<MyTaskDtoOut>(taskDtoIn);

        var GroupNameFromDB = taskToEdit.Group.GroupName;
        myTaskDtoOut.GroupName = GroupNameFromDB;
        myTaskDtoOut.UserId = taskToEdit.UserId;

        return new EditTaskResult<MyTaskDtoOut> { IsSuccess = true, Model = myTaskDtoOut};
    }

    public void DeleteTask(int id)
    {
        
        var taskToDelete = _dbContext.Tasks.FirstOrDefault(t => t.MyTaskId == id);
        _dbContext.Tasks.Remove(taskToDelete);
        _dbContext.SaveChanges();

        _logger.LogWarning($"Task {id} was deleted");

    }
}


