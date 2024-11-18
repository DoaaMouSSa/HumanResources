using ExcelDataReader;
using HumanResources.Domain.Entities;
using HumanResources.Infrastructure.DbContext;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file)
        {
            try
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
                            AttendanceDetails previousAttendance = null;
                            var empCodeGeneral = 0;
                            string lastDate = "";
                            var monthlyAttendance = new Dictionary<(int Month, int Year, int EmployeeId), Attendance>();
                            int? attendanceId = 0;
                            while (reader.Read())
                            {
                                // Get EmployeeCode and DateTime from Excel
                                var employeeCode = Convert.ToInt16(reader.GetValue(0).ToString());
                               
                                if (empCodeGeneral != employeeCode && empCodeGeneral !=0)
                                {
                                    // Final save for last month's attendance
                                    //foreach (var attendan in monthlyAttendance.Values)
                                    //{
                                        //تجميع ايام الحضور
                                        Attendance attendance=_context.AttendanceTbl.Where(a=>a.Id==attendanceId)
                                            .FirstOrDefault();
                                        attendance.WorkingDays = attendance.AttendanceDetails
                                            .Select(ad => ad.AttendanceDate)
                                            .Distinct()
                                            .Count();
                                    //تجميع ساعات العمل
                                    // Calculate total working hours and assign it to WorkingHours
                                    //attendance.WorkingHoursTime = CalculateTotalWorkingHours(attendance.AttendanceDetails);


                                    long? workingHours = 0;
                                        //if (attendance.WorkingHoursTime != null)
                                        //{
                                            attendance.WorkingHours = (long?)CalculateTotalWorkingHours(attendance.AttendanceDetails).TotalHours;
                                        workingHours = attendance.WorkingHours;
                                        //}

                                        decimal netSalary = 0, daySalary = 0, hourSalary = 0;

                                        hourSalary = Math.Round(attendance.Employee.GrossSalary / 48, 2);
                                        daySalary = Math.Round(attendance.Employee.GrossSalary / 6, 2);

                                        if(attendance.WorkingHours < 48 || attendance.WorkingHours==48)
                                    {
                                        netSalary = Convert.ToDecimal(hourSalary * ((decimal)workingHours));

                                    }else if(attendance.WorkingHours > 48)
                                    {
                                        netSalary = Convert.ToDecimal(hourSalary * 48);

                                        long? overTimeToGetNetSalary = attendance.WorkingHours - 48;
                                        attendance.OverTimeHours = overTimeToGetNetSalary;
                                        decimal overTimeHourSalary = hourSalary * (decimal)1.5;
                                        attendance.OverTimeHourSalary = overTimeHourSalary;
                                        decimal overTimeSalary = (decimal)overTimeToGetNetSalary * overTimeHourSalary;
                                        attendance.OverTimeSalary = overTimeSalary;
                                        netSalary += overTimeSalary;


                                    }
                                    else
                                    {
                                        netSalary = Convert.ToDecimal(hourSalary * ((decimal)workingHours));

                                    }

                                    attendance.hourSalary = hourSalary;
                                        attendance.daySalary = daySalary;
                                        attendance.NetSalary = netSalary;
                                        _context.Update(attendance);
                                    //}

                                    await _context.SaveChangesAsync();
                                }
                                var employee = await _context.EmployeeTbl.FirstOrDefaultAsync(e => e.Id == employeeCode);

                                if (employee == null)
                                {
                                    // Skip if employee code is not found
                                    continue;
                                }
                                empCodeGeneral = employee.Id;
                                int employeeId = employee.Id;
                                var dateTimeValue = DateTime.Parse(reader.GetValue(1).ToString());
                                var dateOnly = DateOnly.FromDateTime(dateTimeValue);
                                var currentMonthYear = $"{dateOnly.Month}-{dateOnly.Year}";
                                var timeOnly = dateTimeValue.TimeOfDay;

                                if (lastDate == "") lastDate = currentMonthYear;
                                int result = string.Compare(currentMonthYear, lastDate);

                                // Check if month has changed, update Attendance if so
                                if (result != 0)
                                {
                                    var lastDateParts = lastDate.Split('-');
                                    int lastMonth = int.Parse(lastDateParts[0]);
                                    int lastYear = int.Parse(lastDateParts[1]);

                                    if (monthlyAttendance.TryGetValue((lastMonth, lastYear, employeeId), out var attendance))
                                    {
                                        attendance.WorkingDays = attendance.AttendanceDetails
                                            .Select(ad => ad.AttendanceDate)
                                            .Distinct()
                                            .Count();
                                        //تجميع ساعات العمل
                                        // Calculate total working hours and assign it to WorkingHours
                                        attendance.WorkingHoursTime = CalculateTotalWorkingHours(attendance.AttendanceDetails);



                                        if (attendance.WorkingHoursTime != null)
                                        {
                                            attendance.WorkingHours = (long?)attendance.WorkingHoursTime.Value.TotalHours;

                                        }

                                        _context.Update(attendance);
                                    }
                                }

                                // Check if previousAttendance exists with same date to update CheckOutTime
                                if (previousAttendance != null && previousAttendance.AttendanceDate == dateOnly && previousAttendance.Attendance.EmployeeId == employeeId)
                                {
                                    previousAttendance.CheckOutTime = timeOnly;
                                    if (previousAttendance.CheckInTime != null)
                                    {
                                        previousAttendance.WorkingHoursAday = previousAttendance.CheckOutTime - previousAttendance.CheckInTime;
                                    }
                                    _context.Update(previousAttendance);
                                }
                                else
                                {
                                    // Get or create Attendance record for month and employee
                                    if (!monthlyAttendance.TryGetValue((dateOnly.Month, dateOnly.Year, employeeId), out var attendance))
                                    {
                                        attendance = new Attendance
                                        {
                                            EmployeeId = employeeId,
                                            Month = dateOnly.Month,
                                            Year = dateOnly.Year
                                        };
                                        _context.Add(attendance);

                                        await _context.SaveChangesAsync();
                                        attendanceId = attendance.Id;
                                        monthlyAttendance[(dateOnly.Month, dateOnly.Year, employeeId)] = attendance;
                                    }

                                    // Create new AttendanceDetails entry for the day
                                    previousAttendance = new AttendanceDetails
                                    {
                                        AttendanceId = attendance.Id,
                                        CheckInTime = timeOnly,
                                        AttendanceDate = dateOnly,
                                    };

                                    _context.Add(previousAttendance);
                                    await _context.SaveChangesAsync();

                                    // Link AttendanceDetails to Attendance
                                    attendance.AttendanceDetails.Add(previousAttendance);
                                }
                                lastDate = currentMonthYear;
                            }
                            
                        }
                    }
                }


                return RedirectToAction("Index", "Attendance");
            }catch(Exception ex)
            {
                return View();
            }
        }
        public TimeSpan CalculateTotalWorkingHours(ICollection<AttendanceDetails> attendanceDetails)
        {
            var uniqueAttendanceDetails = attendanceDetails.Distinct().ToList();

            int totalHours = 0;
            int totalMinutes = 0;
            int totalSeconds = 0;

            // Loop through each attendance detail and sum the hours, minutes, and seconds
            foreach (var detail in uniqueAttendanceDetails)
            {
                if (detail.WorkingHoursAday.HasValue)
                {
                    totalHours += detail.WorkingHoursAday.Value.Hours;
                    totalMinutes += detail.WorkingHoursAday.Value.Minutes;
                    totalSeconds += detail.WorkingHoursAday.Value.Seconds;
                }
            }

            // Normalize totalSeconds and totalMinutes
            totalMinutes += totalSeconds / 60;
            totalSeconds = totalSeconds % 60;

            totalHours += totalMinutes / 60;
            totalMinutes = totalMinutes % 60;

            // Return the total time as a TimeSpan
            return new TimeSpan(totalHours, totalMinutes, totalSeconds);
        }


        public IActionResult Index()
        {
            IEnumerable<Attendance> data = _context.AttendanceTbl.Include("Employee").AsEnumerable();
            return View(data);
        }
        public IActionResult Details(int id)
        {
            Attendance data = _context.AttendanceTbl.Where(att=>att.Id==id).Include("AttendanceDetails").Include("Employee").FirstOrDefault();
            return View(data);
        }

    }
}
