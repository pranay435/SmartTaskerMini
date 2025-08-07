using Microsoft.AspNetCore.Mvc;
using SmartTaskerMini.Core.Application;

namespace SmartTaskerMini.WebApp.Controllers;

public class ReportsController : Controller
{
    private readonly ReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(ReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DailyReport(DateTime date)
    {
        try
        {
            var report = await _reportService.GenerateDailyReportAsync(date);
            _logger.LogInformation("Daily report generated for {Date}", date.ToString("yyyy-MM-dd"));
            
            ViewBag.ReportDate = date.ToString("MMMM dd, yyyy");
            ViewBag.Report = report;
            
            return View("DailyReport");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating daily report for {Date}", date);
            ViewBag.Error = "Error generating report: " + ex.Message;
            return View("Index");
        }
    }
}