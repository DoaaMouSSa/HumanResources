﻿using ExcelDataReader;
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
           // await RecalculateMonthlyWorkingHours((int)detail.Attendance.EmployeeCode, detail.Attendance.Year.Value, detail.Attendance.Month.Value);

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
           // long totalDelaysMinutes = (long)(attendance.DelaysTime?.TotalMinutes ?? 0); // Get total delay minutes

            foreach (var detail in detailsForMonth)
            {
                if (detail.WorkingHoursAday.HasValue)
                {
                    totalHours += detail.WorkingHoursAday.Value.Hours;
                    totalMinutes += detail.WorkingHoursAday.Value.Minutes;
                }
            }
            if(totalMinutes >= 20 && totalMinutes <= 49)
            {
                attendance.TotalWorkingHoursBeforeDelays = totalHours+.5;

            }else if(totalMinutes >= 50)
            {
                attendance.TotalWorkingHoursBeforeDelays = totalHours + 1;

            }
            else
            {
                attendance.TotalWorkingHoursBeforeDelays = totalHours;
            }

            double? totalWorkingHours= (attendance.TotalWorkingHoursBeforeDelays)-totalDelaysHours;
            double? overTimeHours=0;
            if(totalWorkingHours.HasValue && (totalWorkingHours.Value ==48 ||totalWorkingHours.Value <48))
            {
                attendance.TotalWorkingHours = totalWorkingHours;

            }
            else if (totalWorkingHours.HasValue && (totalWorkingHours.Value > 48))
            {
                const double WORKING_HOURS= 48;
                overTimeHours = totalWorkingHours - WORKING_HOURS;
                attendance.OverTimeHours = overTimeHours;
                attendance.TotalWorkingHours = WORKING_HOURS;

            }
            // Save the delay information
            await _context.SaveChangesAsync();
        }


        private  async Task UpdateAttendanceDetailCheckOut(AttendanceDetails detail, TimeSpan checkOutTime)
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
                // Update the Check-Out time and recalculate monthly working hours
                UpdateAttendanceDetailCheckOut(existingDetail, time);

                // Call RecalculateMonthlyWorkingHours only for updates
                await RecalculateMonthlyWorkingHours((int)attendance.EmployeeCode, attendance.Year.Value, attendance.Month.Value);
                await CalculateWorkingDays((int)attendance.EmployeeCode, attendance.Year.Value, attendance.Month.Value);
            }
            else
            {
                // Add new detail and calculate delay if applicable
                var delay = CalculateDelay((TimeSpan)employee.CheckInTime, time);
                AddNewAttendanceDetail(attendance, date, time, delay);

                // Update cumulative delays in the Attendance record only for new details
                await UpdateAttendanceDelays(attendance, delay);

                // No need to call RecalculateMonthlyWorkingHours here
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

                // Add to DelaysHours based on delay calculation logic
                double delayMinutes = attendance.DelaysTime.Value.Minutes;

                if (delayMinutes >= 20 && delayMinutes <= 49)
                {
                    attendance.DelaysHours = attendance.DelaysTime.Value.Hours + .5; // Round to 1 hour
                }
                else if (delayMinutes >= 50 && delayMinutes <= 59)
                {
                    attendance.DelaysHours = attendance.DelaysTime.Value.Hours + 1; // Half an hour
                }
                else
                {
                    attendance.DelaysHours = attendance.DelaysTime.Value.Hours + 0; // 0 an hour
                }
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
                .Where(d => d.Delay.HasValue) // Only consider details with a delay
                .Select(d => d.Delay.Value)
                .Aggregate(TimeSpan.Zero, (sum, delay) => sum.Add(delay));

            // Update the Attendance record with total delay time
            attendance.DelaysTime = totalDelaysTime;

            // Calculate total delay hours
            double totalDelayMinutes = totalDelaysTime.TotalMinutes;

            // Convert delay minutes into hours with the correct rounding logic
            double delayHours = 0;
            if (totalDelayMinutes > 0)
            {
                delayHours = Math.Floor(totalDelayMinutes / 60); // Full hours
                double remainingMinutes = totalDelayMinutes % 60;

                // Apply the rounding rules for remaining minutes
                if (remainingMinutes > 30)
                {
                    delayHours += 1; // Round up to the next hour
                }
                else if (remainingMinutes > 0)
                {
                    delayHours += 0.5; // Round up to half an hour
                }
            }

            // Update the DelaysHours property
            attendance.DelaysHours = delayHours;

            // Save changes to update the Attendance record
            await _context.SaveChangesAsync();

            Console.WriteLine($"Total Delay Time: {totalDelaysTime}, Total Delay Hours: {delayHours}");
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
