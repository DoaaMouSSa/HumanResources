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
    [Authorize]

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
           using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (file == null || file.Length <= 0)
                    return BadRequest("File is not provided or empty.");

                // Save the uploaded file and process it
                string filePath = SaveUploadedFile(file);
                await ProcessExcelFile(filePath);

                await transaction.CommitAsync();
                TempData["SuccessMessage"] = "File processed successfully!";

                return RedirectToAction("Index", "Attendance");
            }
            catch (Exception ex)
            {
                // Rollback the transaction if there's an error
                await transaction.RollbackAsync();

                // Optionally, log the exception here for debugging purposes

                // Return a friendly error message or redirect with TempData
                TempData["ErrorMessage"] = "يرجى مراجعة ملف البصمة جيدا ثم حاول ثانية";
                return View();
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
                // Check if the first column value is null or empty
                if (reader.GetValue(0) == null || string.IsNullOrWhiteSpace(reader.GetValue(0).ToString()))
                    continue;

                // Check if the second column value is null or empty
                if (reader.GetValue(1) == null || string.IsNullOrWhiteSpace(reader.GetValue(1).ToString()))
                    continue;

                // Try parsing the employee code and date-time value
                if (int.TryParse(reader.GetValue(0).ToString(), out int employeeCode) &&
                    DateTime.TryParse(reader.GetValue(1).ToString(), out DateTime dateTimeValue))
                {
                    if (await IsEmployeeExist(employeeCode))
                    {
                        employeeCodeGeneral = employeeCode;
                        await AddOrUpdateAttendanceDetails(employeeCode, dateTimeValue);
                    }
                }
                else
                {
                    // Optionally log or handle invalid data rows
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
                .FirstOrDefaultAsync(a => a.EmployeeCode == employeeCode && a.WeekId==weekId);

            if (attendance == null || attendance.AttendanceDetails == null)
                return;

            var detailsForMonth = attendance.AttendanceDetails
                .ToList();

            if (!detailsForMonth.Any())
                return;

            // Aggregate total working hours and minutes manually, including delays subtraction
            decimal totalHours = 0;
            long totalMinutes = 0;
           // long totalDelaysMinutes = (long)(attendance.DelaysTime?.TotalMinutes ?? 0); // Get total delay minutes

            foreach (var detail in detailsForMonth)
            {
                if (detail.WorkingHoursAday.HasValue)
                {
                    totalHours += detail.WorkingHoursAday.Value.Hours;
                    totalMinutes += detail.WorkingHoursAday.Value.Minutes;
                }
            }
            //to get pure minutes
            if(totalMinutes > 59)
            {
                totalHours += (totalMinutes / 60);
                totalMinutes = (totalMinutes % 60);
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
            // attendance.TotalWorkingHours = attendance.TotalWorkingHoursBeforeDelays - attendance.DelaysHours;

            attendance.TotalWorkingHours = attendance.TotalWorkingHoursBeforeDelays;
            const int DEFAULTWORKINGHOURS= 48;
            //calculate salary
            if (attendance.TotalWorkingHours > DEFAULTWORKINGHOURS)
            {
                attendance.CalculatedSalary = (DEFAULTWORKINGHOURS * attendance.hourSalary);
                attendance.CalculatedSalaryAfterAdditonals = attendance.CalculatedSalary + attendance.Bonus -attendance.Loan- attendance.Deduction;
                attendance.OverTimeHours=attendance.TotalWorkingHours- DEFAULTWORKINGHOURS;
                attendance.OverTimeSalary = attendance.OverTimeHours * attendance.OverTimeHourSalary;
                attendance.NetSalary = Convert.ToInt16(attendance.CalculatedSalaryAfterAdditonals + attendance.OverTimeSalary);
            }
            else if(attendance.TotalWorkingHours < DEFAULTWORKINGHOURS || attendance.TotalWorkingHours == DEFAULTWORKINGHOURS)
            {
                attendance.CalculatedSalary = (attendance.TotalWorkingHours * attendance.hourSalary);
                attendance.CalculatedSalaryAfterAdditonals = attendance.CalculatedSalary + attendance.Bonus - attendance.Loan - attendance.Deduction;
                attendance.NetSalary = Convert.ToInt16(attendance.CalculatedSalaryAfterAdditonals);

            }
            else
            {
                attendance.CalculatedSalary = (attendance.TotalWorkingHours * attendance.hourSalary);
                attendance.CalculatedSalaryAfterAdditonals = attendance.CalculatedSalary + attendance.Bonus - attendance.Loan - attendance.Deduction;
                attendance.NetSalary = Convert.ToInt16(attendance.CalculatedSalaryAfterAdditonals);

            }
            // Modify the first digit of NetSalary
            // Adjust the last digit of NetSalary based on specific conditions
            int? baseValue = attendance.NetSalary / 10 * 10; // Get the first two digits as the base (e.g., 20X -> 200)
            int? lastDigit = attendance.NetSalary % 10;     // Get the last digit (unit place)

            if (lastDigit >= 3 && lastDigit <= 7)
            {
                attendance.NetSalary = baseValue + 5; // Set the last digit to 5
            }
            else if (lastDigit == 1 || lastDigit == 2)
            {
                attendance.NetSalary = baseValue + 0; // Set the last digit to 0
            }
            else if (lastDigit == 8 || lastDigit == 9)
            {
                attendance.NetSalary = baseValue + 10; // Set the last digit to 0
            }

            await _context.SaveChangesAsync();
        }

        private async Task UpdateAttendanceDetailCheckOut(AttendanceDetails detail, TimeSpan checkOutTime)
        {
            detail.CheckOutTime = checkOutTime;

            if (detail.CheckInTime.HasValue)
            {
                var checkInTime = detail.CheckInTime.Value;

                // Define the time intervals
                var defaultCheckInTime = new TimeSpan(8, 0, 0); // 12:00 PM
                var noon = new TimeSpan(12, 0, 0); // 12:00 PM
                var afterNoon = new TimeSpan(13, 0, 0); // 1:00 PM

                if(checkInTime < defaultCheckInTime)
                {
                    checkInTime = defaultCheckInTime;
                }
                // Initialize working hours
                TimeSpan morningWorkingHours = TimeSpan.Zero;
                TimeSpan afternoonWorkingHours = TimeSpan.Zero;

                // Calculate working hours before 12:00 PM
                if (checkInTime < noon)
                {
                    var endMorning = checkOutTime < noon ? checkOutTime : noon;
                    morningWorkingHours = endMorning - checkInTime;
                }

                // Calculate working hours after 1:00 PM
                if (checkOutTime > afterNoon)
                {
                    var startAfternoon = checkInTime > afterNoon ? checkInTime : afterNoon;
                    afternoonWorkingHours = checkOutTime - startAfternoon;
                }

                // Summing both intervals
                detail.WorkingHoursAday = morningWorkingHours + afternoonWorkingHours;


            }
        }

        private void AddNewAttendanceDetail(Attendance attendance, DateOnly date, TimeSpan checkInTime, TimeSpan? delay)
        {
            var newDetail = new AttendanceDetails
            {
                AttendanceDate = date,
                CheckInTime = checkInTime,
                AttendanceId = attendance.Id,
                Delay = delay,
                
            };

            _context.Add(newDetail);
        }
        private async Task<Attendance> GetOrCreateAttendanceAsync(int employeeCode, DateTime dateTimeValue, decimal grossSalary)
        {
            var attendance = await _context.AttendanceTbl
                .Include(a => a.AttendanceDetails)
                .FirstOrDefaultAsync(a => a.EmployeeCode == employeeCode &&a.WeekId==weekId);

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
            int employeeId = _context.EmployeeTbl.Where(e => e.IsDeleted == false && e.Code == employeeCode).AsNoTracking().FirstOrDefault().Id;
            Bonus bonus = _context.BonusTbl
    .Where(b => b.Done == false && b.EmployeeId == employeeId)
    .FirstOrDefault();
           List<Loan> loans = _context.LoanTbl
  .Where(b => b.Done == false && b.EmployeeId == employeeId)
  .ToList();
            List<Deduction> deductions = _context.DeductionTbl
  .Where(d => d.Done == false && d.EmployeeId == employeeId)
  .ToList();

            if (bonus!=null)
            {
                bonus.Done = true;
                bonus.DoneDate = DateOnly.FromDateTime(DateTime.Now);
                _context.BonusTbl.Update(bonus);
                _context.SaveChanges();
            }
            decimal loanPaid = 0;
            decimal loanLeft = 0;

            if (loans.Count() > 0)
            {
                foreach (Loan loan in loans)
                {
                    if (loan.numberofpayment == 1)
                    {
                        loan.paid = loan.loan_amount;
                        loan.left = 0;
                        loan.Done = true;
                        loan.DoneDate = DateOnly.FromDateTime(DateTime.Now);
                        _context.LoanTbl.Update(loan);
                        _context.SaveChanges();
                    }
                    else if (loan.numberofpayment > 1)
                    {
                        loan.paid = loan.paid + loan.payment_unit;
                        loan.left = loan.left - loan.payment_unit;
                        _context.LoanTbl.Update(loan);
                        _context.SaveChanges();
                        if (loan.left == 0)
                        {
                            loan.Done = true;
                            loan.DoneDate = DateOnly.FromDateTime(DateTime.Now);
                        }
                    }
                    loanPaid =loanPaid+ loan.payment_unit;
                    loanLeft = loanLeft + loan.left;
                }
            }
            decimal deductionAmount = 0;
            decimal deductionHours = 0;
            decimal deductionHoursAmount = 0;
            if (deductions.Count() > 0)
            {
                
                foreach(var deduction in deductions)
                {
                   
                    deductionAmount += deduction.amount;
                    deductionHours += deduction.hours;
                    deductionHoursAmount += (deduction.DeductionType==DeductionType.ساعات)? deduction.hours:0;
                    deduction.Done = true;
                    deduction.DoneDate = DateOnly.FromDateTime(DateTime.Now);
                    _context.DeductionTbl.Update(deduction);
                    _context.SaveChanges();
                }
              
            }
            return new Attendance
            {
                EmployeeCode = employeeCode,
                Year = year,
                Month = month,
                hourSalary = grossSalary / weeklyWorkHours,
                OverTimeHourSalary = (grossSalary / weeklyWorkHours) * 1.5m,
                daySalary = grossSalary / workDaysPerWeek,
                Bonus = (bonus != null)?bonus.amount : 0, // If bonus is null, set Bonus_Amount to 0
                Loan = loanPaid, // If bonus is null, set Bonus_Amount to 0
                Loanleft = loanLeft, // If bonus is null, set Bonus_Amount to 0
                Deduction= deductionAmount,
                DeductionHours= deductionHours,
                DeductionHoursAmount= deductionHoursAmount,
                DelaysHours = 0,
                OverTimeHours=0,
                DelaysTime = TimeSpan.Zero,
                WeekId = weekId,
                TotalWorkingHours = 0,
                SalaryBeforeAdditon = 0,
                
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
            }
            else
            {
                // Add new detail and calculate delay if applicable
                var delay = CalculateDelay((TimeSpan)employee.CheckInTime, time);
                AddNewAttendanceDetail(attendance, date, time, delay);

                // Update cumulative delays in the Attendance record only for new details

                await UpdateAttendanceDelays(attendance, delay);
                
            }
            await CalculateWorkingDays((int)attendance.EmployeeCode, attendance.Year.Value, attendance.Month.Value);

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
                .FirstOrDefaultAsync(a => a.EmployeeCode == employeeCode &&a.WeekId==weekId);

            if (attendance == null || attendance.AttendanceDetails == null)
            {
                throw new Exception($"Attendance record not found for Employee with code {employeeCode} in {year}-{month}.");
            }

            // Calculate the total number of unique working days
            var workingDays = attendance.AttendanceDetails.Count();

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
            ViewBag.WeekLst = new SelectList(await _context.WeekTbl.OrderByDescending(w=>w.Id).ToListAsync(), "Id", "CreatedDateTime");
            int? lastWeekId = _context.WeekTbl.Max(a => a.Id);

            var data = _context.AttendanceTbl.Where(a => a.WeekId == lastWeekId).Include(a => a.Employee)
                .Include(a => a.AttendanceDetails).ToList();
            return View(data);
        }
        [HttpGet]         
        public async Task<IActionResult> GetAttendanceByWeek(int weekId)
        {
            var data = await _context.AttendanceTbl
                .Include(a => a.Employee)
                .Include(a => a.AttendanceDetails)
                .Where(a => a.WeekId == weekId)
                .ToListAsync();

            if (data == null || !data.Any())
            {
                // Handle no data scenario
                return Content("<p>No attendance data available for this week.</p>");
            }

            return PartialView("_AttendanceTable", data);
        }


        //public IActionResult Print(int weekId)
        //{
        //    string weekDate = _context.WeekTbl.Where(w => w.Id == weekId).FirstOrDefault().CreatedDate.ToString();
        //    var attendanceDT = new DataTable();
        //    attendanceDT = GetAttendanceForReport(weekId);
        //    string mimetype = "";
        //    int extension = 1;
        //    var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "attendanceReport.rdlc");
        //    Dictionary<string, string> parameters = new Dictionary<string, string>();
        //    parameters.Add("date", weekDate);
        //    LocalReport localReport = new LocalReport(path);
        //    localReport.AddDataSource("attendanceDataSet", attendanceDT);
        //    var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);
        //    return File(result.MainStream, "application/pdf");
        //}
        public IActionResult Print(int weekId)
        {
            // Retrieve the week's created date as a string
            string weekDate = _context.WeekTbl
                .Where(w => w.Id == weekId)
                .Select(w => w.CreatedDate.ToString())
                .FirstOrDefault();

            // Get the attendance data for the report
            var attendanceDT = GetAttendanceForReport(weekId);

            // Define the RDLC report path
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "Reports", "attendanceReport.rdlc");

            // Prepare report parameters
            Dictionary<string, string> parameters = new Dictionary<string, string>
    {
        { "date", weekDate }
    };

            // Load and configure the local report
            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("attendanceDataSet", attendanceDT);

            // Generate the report as a PDF
            string mimetype = "application/pdf";
            int extension = 1;
            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

            // Return the PDF file as a downloadable response
            return File(result.MainStream, mimetype, "AttendanceReport.pdf");
        }

        public IActionResult Details(int id)
        {
            Attendance data = _context.AttendanceTbl.Where(att => att.Id == id).Include("AttendanceDetails").Include("Employee").FirstOrDefault();
            return View(data);
        }
        public DataTable GetAttendanceForReport(int weekId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");
            dt.Columns.Add("Salary");
            dt.Columns.Add("attendanceDays");
            dt.Columns.Add("daySalary");
            dt.Columns.Add("hourSalary");
            dt.Columns.Add("delays");
            dt.Columns.Add("overtime");
            dt.Columns.Add("hoursWeek");
            dt.Columns.Add("discount");
            dt.Columns.Add("totalHour");
            dt.Columns.Add("netSalary");
            DataRow row;
            List<AttendanceDtoForReport> data = _attendanceService.GetForReport(weekId);
            for(int i = 0; i < data.Count(); i++)
            {
                row = dt.NewRow();
                row["Id"] = data[i].Id;
                row["Name"] = data[i].Name;
                row["Salary"] = data[i].Salary;
                row["attendanceDays"] = data[i].attendanceDays;
                row["daySalary"] = data[i].daySalary;
                row["hourSalary"] = data[i].hourSalary;
                row["delays"] = data[i].delays;
                row["overtime"] = data[i].overtime;
                row["hoursWeek"] = data[i].hoursWeek;
                row["discount"] = data[i].discount;
                row["totalHour"] = data[i].totalHour;
                row["netSalary"] = data[i].netSalary;
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
