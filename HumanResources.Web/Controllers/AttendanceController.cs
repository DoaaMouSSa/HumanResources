﻿using AspNetCore.Reporting;
using ExcelDataReader;
using HumanResources.Application.AttendanceServices;
using HumanResources.Application.Dtos;
using HumanResources.Application.WeekServices;
using HumanResources.Domain.Entities;
using HumanResources.Infrastructure.DbContext;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;
using static HumanResources.Application.Dtos.AttendanceDto;
using static HumanResources.Domain.Enums.Enums;

namespace HumanResources.Web.Controllers
{

    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExcelService _excelService;
        private readonly IWeekService _weekService;
        private readonly IAttendanceService _attendanceService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AttendanceController(ApplicationDbContext context
            , IWeekService weekService,
IWebHostEnvironment webHostEnvironment,
IAttendanceService attendanceService)
        {
            _context = context;
            _weekService = weekService;
            _attendanceService = attendanceService;
            _excelService = new ExcelService();
            _webHostEnvironment = webHostEnvironment;
            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);
        }

        public IActionResult Create()
        {

            return View();
        }
        int employeeCodeGeneral = 0;
        Employee employee;
        Attendance attendance = null;
        int? weekId;
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
            DateTime dateTime = DateTime.Now;
            Week week = new Week()
            {
                CreatedDateTime = dateTime,
                CreatedDate = DateOnly.FromDateTime(dateTime),
                Date = DateOnly.FromDateTime(dateTime).ToString("dd MMMM yyyy"),
            };
            _context.Add(week);
            _context.SaveChanges();
            weekId= week.Id;
            using var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            while (reader.Read())
            {
                int employeeCode = Convert.ToInt32(reader.GetValue(0)?.ToString());
                DateTime dateTimeValue = DateTime.Parse(reader.GetValue(1)?.ToString());

                if (await IsEmployeeExist(employeeCode))
                {
                    employeeCodeGeneral = employeeCode;
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
            decimal totalHours = 0;
            long totalMinutes = 0;
            decimal totalDelaysHours = attendance.DelaysHours ?? 0; // Get total delays (if any)
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
                attendance.TotalWorkingHoursBeforeDelays = totalHours + .5m;

            }else if(totalMinutes >= 50)
            {
                attendance.TotalWorkingHoursBeforeDelays = totalHours + 1;

            }
            else
            {
                attendance.TotalWorkingHoursBeforeDelays = totalHours;
            }
            
            decimal? totalWorkingHours= (attendance.TotalWorkingHoursBeforeDelays)-totalDelaysHours;
            decimal? overTimeHours=0;
            if(totalWorkingHours.HasValue && (totalWorkingHours.Value ==48 ||totalWorkingHours.Value <48))
            {
                attendance.TotalWorkingHours = totalWorkingHours;

            }
            else if (totalWorkingHours.HasValue && (totalWorkingHours.Value > 48))
            {
                const decimal WORKING_HOURS= 48;
                overTimeHours = totalWorkingHours - WORKING_HOURS;
                attendance.OverTimeHours = overTimeHours;
                attendance.TotalWorkingHours = WORKING_HOURS;

            }
            attendance.CalculatedSalary = attendance.TotalWorkingHours * attendance.hourSalary;
            attendance.SalaryBeforeAdditon = (int)attendance.CalculatedSalary;
            const decimal OVERTIMEHOURSALARY = 1.5m;
            attendance.OverTimeHourSalary = attendance.hourSalary * OVERTIMEHOURSALARY;
            attendance.OverTimeSalary = attendance.OverTimeHourSalary * attendance.OverTimeHours;
            attendance.NetSalary =(int?) (attendance.SalaryBeforeAdditon + attendance.OverTimeSalary);

            if (attendance.NetSalary % 5==0)
            {
                //تقريب المرتب
            }
            // Save the delay information
            await _context.SaveChangesAsync();
        }


        private  async Task UpdateAttendanceDetailCheckOut(AttendanceDetails detail, TimeSpan checkOutTime)
        {
            detail.CheckOutTime = checkOutTime;
            if (detail.CheckInTime.HasValue)
            {
                // Define the excluded time range
                var excludedStart = new TimeSpan(12, 0, 0); // 12:00 PM
                var excludedEnd = new TimeSpan(13, 0, 0);  // 1:00 PM

                // Calculate the raw working hours
                var checkInTime = detail.CheckInTime.Value;
                var workingHours = checkOutTime - checkInTime;

                // Adjust for the excluded period
                if (checkInTime < excludedEnd && checkOutTime > excludedStart)
                {
                    // Calculate the overlap duration with the excluded period
                    var overlapStart = checkInTime > excludedStart ? checkInTime : excludedStart;
                    var overlapEnd = checkOutTime < excludedEnd ? checkOutTime : excludedEnd;
                    var excludedDuration = overlapEnd - overlapStart;

                    // Subtract the excluded duration from the total working hours
                    workingHours -= excludedDuration;
                }

                // Assign the adjusted working hours
                detail.WorkingHoursAday = workingHours;

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
                OverTimeSalary = (grossSalary / weeklyWorkHours) * (3/2),
                daySalary = grossSalary / workDaysPerWeek,
                DelaysHours = 0,
                OverTimeHours=0,
                DelaysTime = TimeSpan.Zero,
                WeekId = weekId,
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
                
            }

            await _context.SaveChangesAsync();
        }

        private TimeSpan? CalculateDelay(TimeSpan expectedCheckInTime, TimeSpan actualCheckInTime)
        {
            // Define the break period
            var breakStartTime = new TimeSpan(12, 0, 0); // 12:00 PM
            var breakEndTime = new TimeSpan(13, 0, 0); // 1:00 PM

            // If the actual check-in time is during the break period, treat it as if it were at the break end
            if (actualCheckInTime >= breakStartTime && actualCheckInTime < breakEndTime)
            {
                return breakStartTime - expectedCheckInTime;
            }

            // Calculate delay only if the adjusted actual check-in time is greater than the expected check-in time
            return actualCheckInTime > expectedCheckInTime
                ? actualCheckInTime - expectedCheckInTime
                : null;
        }


        private async Task UpdateAttendanceDelays(Attendance attendance, TimeSpan? delay)
        {
            // Sum all Delay values that are not null
            TimeSpan totalDelay = TimeSpan.Zero; // Start with a zero TimeSpan

            for (int i = 0; i < attendance.AttendanceDetails.Count; i++)
            {
                // Check if Delay has a value before adding
                if (attendance.AttendanceDetails[i].Delay.HasValue)
                {
                    totalDelay = totalDelay.Add(attendance.AttendanceDetails[i].Delay.Value);
                }
            }
            attendance.DelaysTime=totalDelay;
            if (totalDelay.Minutes >= 20 && totalDelay.Minutes <= 49)
            {
                attendance.DelaysHours = attendance.DelaysTime.Value.Hours + .5m; // Round to 1 hour
            }
            else if (totalDelay.Minutes >= 50 && totalDelay.Minutes <= 59)
            {
                attendance.DelaysHours = attendance.DelaysTime.Value.Hours + 1; // Half an hour
            }
            else
            {
                attendance.DelaysHours = attendance.DelaysTime.Value.Hours + 0; // 0 an hour
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
            attendance.TotalWorkingHours = attendance.TotalWorkingHoursBeforeDelays-attendance.DelaysHours;
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
            decimal totalDelayMinutes = (decimal)totalDelaysTime.TotalMinutes;

            // Convert delay minutes into hours with the correct rounding logic
            decimal delayHours = 0;
            if (totalDelayMinutes > 0)
            {
                delayHours = Math.Floor(totalDelayMinutes / 60); // Full hours
                decimal remainingMinutes = totalDelayMinutes % 60;

                // Apply the rounding rules for remaining minutes
                if (remainingMinutes > 30)
                {
                    delayHours += 1; // Round up to the next hour
                }
                else if (remainingMinutes > 0)
                {
                    delayHours += .5m; // Round up to half an hour
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


        public async Task<IActionResult> Index()
        {
            IEnumerable<Attendance> data = _context.AttendanceTbl.Include("Employee").AsEnumerable();
            IEnumerable<WeekDto> weeks = await _weekService.GetAll();
            ViewData["WeekLst"] = new SelectList(weeks, "Id", "Date");
            return View(data);
        }
        public IActionResult Print()
        {
            var attendanceDT = new DataTable();
            attendanceDT = GetAttendanceForReport();
            string mimetype = "";
            int extension = 1;
            var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\attendanceReport.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("prm", "تقرير المرتبات");
            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("attendanceDataSet", attendanceDT);
            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);
            return File(result.MainStream, "application/pdf");
        }

        public IActionResult Details(int id)
        {
            Attendance data = _context.AttendanceTbl.Where(att => att.Id == id).Include("AttendanceDetails").Include("Employee").FirstOrDefault();
            return View(data);
        }
        public DataTable GetAttendanceForReport()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            dt.Columns.Add("Salary");
            DataRow row;
            List<AttendanceDtoForReport> data = _attendanceService.GetForReport();
            for(int i = 0; i < data.Count(); i++)
            {
                row = dt.NewRow();
                row["Id"] = data[i].Id;
                row["Name"] = data[i].Name;
                row["Salary"] = data[i].Salary;
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
