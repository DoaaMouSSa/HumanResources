using ExcelDataReader;
using HumanResources.Domain.Entities;
using HumanResources.Infrastructure.DbContext;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;
using static HumanResources.Domain.Enums.Enums;

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

        public IActionResult Create()
        {

            return View();
        }
        int employeeCodeGeneral = 0;
        Employee employee;
        Attendance attendance = null;
        Week week = null;
        AttendanceDetails attendanceDetails = null;
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file)
        {
            try
            {
                if (file == null || file.Length <= 0)
                    return BadRequest("File is not provided or empty.");

                string filePath = SaveUploadedFile(file);
                await ProcessExcelFile(filePath);

                return RedirectToAction("Index", "Attendance");
            }
            catch (Exception ex)
            {
                //message of worng excel sheet
                throw new Exception($"Error processing file: {ex.Message}");
            }
        }
        private string SaveUploadedFile(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return filePath;
        }
        private async Task ProcessExcelFile(string filePath)
        {
            using var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            while (reader.Read())
            {
                int employeeCode = Convert.ToInt32(reader.GetValue(0)?.ToString());
                DateTime dateTimeValue = DateTime.Parse(reader.GetValue(1)?.ToString());

                if (await IsEmployeeExist(employeeCode))
                {
                    await AddOrUpdateAttendanceDetails(employeeCode, dateTimeValue);
                }
            }
        }
        private async Task AddOrUpdateAttendanceDetails(int employeeCode, DateTime dateTimeValue)
        {
            var dateOnly = DateOnly.FromDateTime(dateTimeValue);
            var timeOnly = dateTimeValue.TimeOfDay;

            var employee = await GetEmployeeAsync(employeeCode);
            var attendance = await GetOrCreateAttendanceAsync(employeeCode, dateTimeValue, employee.GrossSalary);

            await AddOrUpdateAttendanceDetailAsync(attendance, employee, dateOnly, timeOnly);

            // Recalculate working hours for the employee on the specific date
            await RecalculateMonthlyWorkingHours(employeeCode, dateOnly.Year, dateOnly.Month);

            // **Calculate and update the working days**
            await CalculateWorkingDays(employeeCode, dateOnly.Year, dateOnly.Month);
        }

        private async Task RecalculateMonthlyWorkingHours(int employeeCode, int year, int month)
        {
            var attendance = await _context.AttendanceTbl
                .Include(a => a.AttendanceDetails)
                .FirstOrDefaultAsync(a => a.EmployeeCode == employeeCode && a.Year == year && a.Month == month);

            if (attendance == null || attendance.AttendanceDetails == null)
                return;

            var detailsForMonth = attendance.AttendanceDetails
                .Where(detail => detail.AttendanceDate.Value.Year == year && detail.AttendanceDate.Value.Month == month)
                .ToList();

            if (!detailsForMonth.Any())
                return;

            // Aggregate total working hours and minutes manually, including delays subtraction
            double totalHours = 0;
            long totalMinutes = 0;
            double totalDelaysHours = attendance.DelaysHours ?? 0; // Get total delays (if any)
            long totalDelaysMinutes = (long)(attendance.DelaysTime?.TotalMinutes ?? 0); // Get total delay minutes

            foreach (var detail in detailsForMonth)
            {
                if (detail.WorkingHoursAday.HasValue)
                {
                    totalHours += detail.WorkingHoursAday.Value.Hours;
                    totalMinutes += detail.WorkingHoursAday.Value.Minutes;
                }
            }

            // Convert total minutes to hours
            totalHours += totalMinutes / 60;
            totalMinutes = totalMinutes % 60;
            attendance.TotalWorkingHoursBeforeDelays = totalHours;

            // Subtract delays from total hours and minutes
            totalHours -= totalDelaysHours;
            totalMinutes -= totalDelaysMinutes;

            // Adjust if total minutes go below 0
            if (totalMinutes < 0)
            {
                totalHours -= 1;
                totalMinutes += 60;
            }

            // If the remaining minutes are 41 or more, convert them into 1 hour
            if (totalMinutes >= 41)
            {
                totalHours += 1;
                totalMinutes -= 60; // Subtract the 60 minutes converted to 1 hour
            }

            // Calculate DelaysHours based on DelaysTime
            if (attendance.DelaysTime.HasValue)
            {
                // Get the total delay in minutes
                double delayMinutes = attendance.DelaysTime.Value.TotalMinutes;

                // Convert delay to hours based on the rules
                if (delayMinutes >= 41)
                {
                    attendance.DelaysHours = attendance.DelaysHours+=1; // Round to the nearest hour
                }
                else if (delayMinutes >= 21 && delayMinutes < 41)
                {
                    attendance.DelaysHours = attendance.DelaysHours += .5;  // Half an hour
                }
                else
                {
                    attendance.DelaysHours = attendance.DelaysHours += 0; // No delay
                }
            }

            // Save the results to the Attendance table
            attendance.TotalWorkingHours = totalHours; // Total hours (decimal value)

            // Save the delay information
            await _context.SaveChangesAsync();
        }


        private void UpdateAttendanceDetailCheckOut(AttendanceDetails detail, TimeSpan checkOutTime)
        {
            detail.CheckOutTime = checkOutTime;
            if (detail.CheckInTime.HasValue)
            {
                // Calculate the working hours for the day
                detail.WorkingHoursAday = checkOutTime - detail.CheckInTime.Value;
            }
        }
        private void AddNewAttendanceDetail(Attendance attendance, DateOnly date, TimeSpan checkInTime, TimeSpan? delay)
        {
            var newDetail = new AttendanceDetails
            {
                AttendanceDate = date,
                CheckInTime = checkInTime,
                AttendanceId = attendance.Id,
                Delay = delay
            };

            _context.Add(newDetail);
        }
        private async Task<Attendance> GetOrCreateAttendanceAsync(int employeeCode, DateTime dateTimeValue, decimal grossSalary)
        {
            var attendance = await _context.AttendanceTbl
                .Include(a => a.AttendanceDetails)
                .FirstOrDefaultAsync(a => a.EmployeeCode == employeeCode && a.Year == dateTimeValue.Year && a.Month == dateTimeValue.Month);

            if (attendance == null)
            {
                var newAttendance = CreateNewAttendance(employeeCode, dateTimeValue.Year, dateTimeValue.Month, grossSalary);
                _context.Add(newAttendance);
                await _context.SaveChangesAsync(); // Ensure ID is generated
                return newAttendance;
            }
            return attendance;
        }

        private async Task<Employee> GetEmployeeAsync(int employeeCode)
        {
            var employee = await _context.EmployeeTbl.FirstOrDefaultAsync(e => e.Code == employeeCode);
            if (employee == null)
            {
                throw new Exception($"Employee with code {employeeCode} not found.");
            }
            return employee;
        }


        private Attendance CreateNewAttendance(int employeeCode, int year, int month, decimal grossSalary)
        {
            const int weeklyWorkHours = 48;
            const int workDaysPerWeek = 6;

            return new Attendance
            {
                EmployeeCode = employeeCode,
                Year = year,
                Month = month,
                hourSalary = grossSalary / weeklyWorkHours,
                daySalary = grossSalary / workDaysPerWeek,
                DelaysHours = 0,
                DelaysTime = TimeSpan.Zero,
                //WorkingHoursTime = TimeSpan.Zero,
                TotalWorkingHours = 0
            };
        }

        private async Task AddOrUpdateAttendanceDetailAsync(Attendance attendance, Employee employee, DateOnly date, TimeSpan time)
        {
            var existingDetail = attendance.AttendanceDetails.FirstOrDefault(d => d.AttendanceDate == date);

            if (existingDetail != null)
            {
                // Check if it's a Check-Out and update only the working hours
                UpdateAttendanceDetailCheckOut(existingDetail, time);
            }
            else
            {
                // It's a new Check-In, calculate delay if applicable
                var delay = CalculateDelay((TimeSpan)employee.CheckInTime, time);
                AddNewAttendanceDetail(attendance, date, time, delay);
                UpdateAttendanceDelays(attendance, delay);
            }

            await _context.SaveChangesAsync();
        }


        private TimeSpan? CalculateDelay(TimeSpan expectedCheckInTime, TimeSpan actualCheckInTime)
        {
            return actualCheckInTime > expectedCheckInTime
                ? actualCheckInTime - expectedCheckInTime
                : null;
        }

        private async Task UpdateAttendanceDelays(Attendance attendance, TimeSpan? delay)
        {
            if (delay.HasValue)
            {
                // Update the cumulative delay in the Attendance record
                attendance.DelaysTime = (attendance.DelaysTime ?? TimeSpan.Zero) + delay.Value;
                attendance.DelaysHours = (attendance.DelaysHours ?? 0) + (long)delay.Value.TotalHours;
            }
        }
        private async Task CalculateWorkingHoursForEmployee(int employeeCode, int attendanceId)
        {
            // Retrieve the attendance record with all related AttendanceDetails
            var attendance = await _context.AttendanceTbl
                .Include(a => a.AttendanceDetails)
                .FirstOrDefaultAsync(a => a.Id == attendanceId && a.EmployeeCode == employeeCode);

            if (attendance == null)
            {
                throw new Exception($"Attendance record not found for Employee with code {employeeCode} and Attendance ID {attendanceId}.");
            }

            // Initialize variables for total working hours and working hours per day
            TimeSpan totalWorkingHours = TimeSpan.Zero;

            // Loop through each AttendanceDetail to calculate daily working hours
            foreach (var detail in attendance.AttendanceDetails)
            {
                // Skip if CheckInTime or CheckOutTime is null
                if (detail.CheckInTime.HasValue && detail.CheckOutTime.HasValue)
                {
                    // Calculate daily working hours (CheckOutTime - CheckInTime)
                    TimeSpan workingHours = detail.CheckOutTime.Value - detail.CheckInTime.Value;

                    // Accumulate the working hours for the entire month
                    totalWorkingHours = totalWorkingHours.Add(workingHours);

                    // Optionally, log or store the working hours for each day (if needed)
                    Console.WriteLine($"Date: {detail.AttendanceDate}, Working Hours: {workingHours.TotalHours} hours");
                }
            }

            // Update the Attendance record with the total working hours
            //attendance.WorkingHoursTime = totalWorkingHours;
            attendance.TotalWorkingHours = (long)totalWorkingHours.TotalHours;

            // Save the changes
            await _context.SaveChangesAsync();

            Console.WriteLine($"Total Working Hours for Attendance ID {attendanceId}: {totalWorkingHours.TotalHours} hours");
        }

        private async Task CalculateCumulativeDelays(Attendance attendance)
        {
            if (attendance.AttendanceDetails == null || !attendance.AttendanceDetails.Any())
            {
                return;
            }

            // Aggregate delays from AttendanceDetails
            var totalDelaysTime = attendance.AttendanceDetails
                .Where(d => d.Delay.HasValue)
                .Select(d => d.Delay.Value)
                .Aggregate(TimeSpan.Zero, (sum, delay) => sum.Add(delay));

            var totalDelaysHours = (long)totalDelaysTime.TotalHours;

            // Update the Attendance record
            attendance.DelaysTime = totalDelaysTime;

            // Save changes to update the Attendance record
            await _context.SaveChangesAsync();
        }

        private async Task CalculateWorkingDays(int employeeCode, int year, int month)
        {
            // Retrieve the attendance record for the employee in the specified year and month
            var attendance = await _context.AttendanceTbl
                .Include(a => a.AttendanceDetails)
                .FirstOrDefaultAsync(a => a.EmployeeCode == employeeCode && a.Year == year && a.Month == month);

            if (attendance == null || attendance.AttendanceDetails == null)
            {
                throw new Exception($"Attendance record not found for Employee with code {employeeCode} in {year}-{month}.");
            }

            // Calculate the total number of unique working days
            var workingDays = attendance.AttendanceDetails
                .Where(detail => detail.CheckInTime.HasValue) // Count only days with CheckInTime
                .Select(detail => detail.AttendanceDate) // Get the dates
                .Distinct() // Ensure each date is counted only once
                .Count();

            // Update the Attendance record with the calculated working days
            attendance.WorkingDays = workingDays;

            // Save changes to update the database
            await _context.SaveChangesAsync();

            Console.WriteLine($"Working Days for Employee {employeeCode} in {year}-{month}: {workingDays}");
        }


        private async Task<bool> IsEmployeeExist(int employeeCode)
        {
            employee = await _context.EmployeeTbl.FirstOrDefaultAsync(e => e.Code == employeeCode);
            return employee != null;
        }
        private DateTime GetStartOfWeek(DateTime date)
        {
            // Get the start of the week (Monday)
            var diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0) diff += 7; // Ensure it handles Sunday as the last day of the week
            var startOfWeek = date.AddDays(-diff).Date;
            return startOfWeek;
        }


        public IActionResult Index()
        {
            IEnumerable<Attendance> data = _context.AttendanceTbl.Include("Employee").AsEnumerable();
            return View(data);
        }
        public IActionResult Details(int id)
        {
            Attendance data = _context.AttendanceTbl.Where(att => att.Id == id).Include("AttendanceDetails").Include("Employee").FirstOrDefault();
            return View(data);
        }

    }
}
