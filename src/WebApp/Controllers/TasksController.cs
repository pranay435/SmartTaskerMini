using Microsoft.AspNetCore.Mvc;
using SmartTaskerMini.Core.Application;
using SmartTaskerMini.Core.Domain;
using SmartTaskerMini.WebApp.Models;

namespace SmartTaskerMini.WebApp.Controllers;

public class TasksController : Controller
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    public async Task<IActionResult> Index()
    {
        var tasks = await _taskService.ListAsync();
        return View(tasks);
    }

    public IActionResult Create()
    {
        return View(new TaskViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _taskService.AddAsync(model.Title, model.DueUtc, model.Priority);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id)
    {
        await _taskService.MarkDoneAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
            return NotFound();

        var model = new TaskViewModel
        {
            Id = task.Id,
            Title = task.Title,
            DueUtc = task.DueUtc,
            Priority = task.Priority
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TaskViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var task = await _taskService.GetTaskByIdAsync(model.Id);
        if (task == null)
            return NotFound();

        task.Title = model.Title;
        task.DueUtc = model.DueUtc;
        task.Priority = model.Priority;

        await _taskService.UpdateTaskAsync(task);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _taskService.DeleteTaskAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> History()
    {
        var history = await _taskService.GetCompletedTasksAsync();
        return View(history);
    }
}