﻿using ExcelDataReader;
using HumanResources.Domain.Entities;
using HumanResources.Infrastructure.DbContext;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HumanResources.Web.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelService _excelService;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
            _excelService = new ExcelService();
        }
        public async Task<IActionResult> Index()
        {
            // Generate the dynamic table name, e.g., "Employee_47"
            int tableNumber = 47;
            string tableName = $"Employee_{tableNumber}";

            // Create the SQL query to fetch data from the dynamic table
            string sqlQuery = $"SELECT * FROM {tableName}";

            List<dynamic> employees = new List<dynamic>();

            // Open a connection to the database
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlQuery;
                _context.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        var employee = new
                        {
                            Id = result["Id"],
                        };
                        employees.Add(employee);
                    }
                }
            }

            return View(employees);
        }

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file)
        {
            // Register the code pages encoding provider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (file != null && file.Length > 0)
            {
                var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\uploads\\";

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        bool isHeaderSkipped = false;
                        Attendance previousAttendance = null;

                        while (reader.Read())
                        {
                            if (!isHeaderSkipped)
                            {
                                isHeaderSkipped = true;
                                continue;
                            }

                            // Extract DateTime and split into date and time
                            var dateTimeValue = DateTime.Parse(reader.GetValue(1).ToString());
                            var dateOnly = DateOnly.FromDateTime(dateTimeValue);
                            var timeOnly = dateTimeValue.TimeOfDay;

                            if (previousAttendance != null && previousAttendance.AttendanceDate == dateOnly)
                            {
                                // Same date, update CheckOutTime
                                previousAttendance.CheckOutTime = timeOnly;
                                _context.Update(previousAttendance);
                            }
                            else
                            {
                                var employeeExists = _context.EmployeeTbl.FirstOrDefault(
                                    e => e.Id == int.Parse(reader.GetValue(0).ToString()));


                                // If employee exists, insert the attendance record
                                if (employeeExists !=null)
                                {

                                    // New attendance record for a different date
                                    previousAttendance = new Attendance
                                    {
                                        // Check if the EmployeeId exists in EmployeeTbl

                                        EmployeeId = int.Parse(reader.GetValue(0).ToString()),

                                        CheckInTime = timeOnly,
                                        AttendanceDate = dateOnly
                                    };
                                    _context.Add(previousAttendance);
                                }

                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            ViewBag.Message = "success";
            return View();
        }
    }
}