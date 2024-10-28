using ExcelDataReader;
using HumanResources.Domain.Entities;
using HumanResources.Infrastructure.DbContext;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                        string lastDate = "";
                        var monthlyAttendance = new Dictionary<(int Month, int Year, int EmployeeId), Attendance>();

                        while (reader.Read())
                        {
                            // Get EmployeeCode and DateTime from Excel
                            var employeeCode =Convert.ToInt16(reader.GetValue(0).ToString());
                            var employee = await _context.EmployeeTbl.FirstOrDefaultAsync(e => e.Id == employeeCode);

                            if (employee == null)
                            {
                                // Skip if employee code is not found
                                continue;
                            }

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

                                    attendance.WorkingHours = attendance.AttendanceDetails
                                        .Sum(ad => ad.WorkingHoursAday?.Ticks ?? 0);

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

                        // Final save for last month's attendance
                        foreach (var attendance in monthlyAttendance.Values)
                        {
                            attendance.WorkingDays = attendance.AttendanceDetails
                                .Select(ad => ad.AttendanceDate)
                                .Distinct()
                                .Count();

                            attendance.WorkingHours = attendance.AttendanceDetails
                                .Sum(ad => ad.WorkingHoursAday?.Ticks ?? 0);
                            //decimal HourSalary = 0;
                            decimal netSalary=0, daySalary = 0;
                            SalaryFormula ? salaryFormula = attendance.Employee.SalaryFormula.Value;
                            if(salaryFormula == SalaryFormula.كامل)
                            {
                                daySalary = 0;
                                daySalary = Math.Floor(attendance.Employee.GrossSalary / 30);
                                netSalary = Convert.ToDecimal(daySalary * attendance.WorkingDays);
                            }
                            else if (salaryFormula == SalaryFormula.تقليدى)
                            {
                                daySalary = 0;
                                daySalary = Math.Floor(attendance.Employee.GrossSalary / 26);
                                netSalary = Convert.ToDecimal(daySalary * attendance.WorkingDays);

                            }
                            attendance.daySalary = daySalary;
                            attendance.NetSalary = netSalary;
                            _context.Update(attendance);
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }

            return View();
        }

        //int workingDaysCount = 0;
        //[HttpPost]
        //public async Task<IActionResult> Create(IFormFile file)
        //{
        //    // Register the code pages encoding provider
        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //    if (file != null && file.Length > 0)
        //    {
        //        var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\uploads\\";
        //        if (!Directory.Exists(uploadsFolder))
        //        {
        //            Directory.CreateDirectory(uploadsFolder);
        //        }

        //        var filePath = Path.Combine(uploadsFolder, file.FileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
        //        {
        //            using (var reader = ExcelReaderFactory.CreateReader(stream))
        //            {
        //                AttendanceDetails previousAttendance = null;
        //                string lastDate = "";
        //                int employeeId;
        //                var monthlyAttendance = new Dictionary<(int Month, int Year, int EmployeeId), Attendance>();

        //                while (reader.Read())
        //                {
        //                    // Get EmployeeId and DateTime from Excel
        //                    employeeId = Convert.ToInt16(reader.GetValue(0));
        //                    var dateTimeValue = DateTime.Parse(reader.GetValue(1).ToString());
        //                    var dateOnly = DateOnly.FromDateTime(dateTimeValue);
        //                    var currentMonthYear = $"{dateOnly.Month}-{dateOnly.Year}";
        //                    var timeOnly = dateTimeValue.TimeOfDay;

        //                    if (lastDate == "") lastDate = currentMonthYear;
        //                    int result = string.Compare(currentMonthYear, lastDate);

        //                    // Check if month has changed, update Attendance if so
        //                    if (result != 0)
        //                    {
        //                        var lastDateParts = lastDate.Split('-');
        //                        int lastMonth = int.Parse(lastDateParts[0]);
        //                        int lastYear = int.Parse(lastDateParts[1]);

        //                        if (monthlyAttendance.TryGetValue((lastMonth, lastYear, employeeId), out var attendance))
        //                        {
        //                            attendance.WorkingDays = attendance.AttendanceDetails
        //                                .Select(ad => ad.AttendanceDate)
        //                                .Distinct()
        //                                .Count();

        //                            attendance.WorkingHours = attendance.AttendanceDetails
        //                                .Sum(ad => ad.WorkingHoursAday?.Ticks ?? 0);

        //                            _context.Update(attendance);
        //                        }
        //                    }

        //                    // Check if previousAttendance exists with same date to update CheckOutTime
        //                    if (previousAttendance != null && previousAttendance.AttendanceDate == dateOnly)
        //                    {
        //                        previousAttendance.CheckOutTime = timeOnly;
        //                        if (previousAttendance.CheckInTime != null)
        //                        {
        //                            previousAttendance.WorkingHoursAday = previousAttendance.CheckOutTime - previousAttendance.CheckInTime;
        //                        }
        //                        _context.Update(previousAttendance);
        //                    }
        //                    else
        //                    {
        //                        // Get or create Attendance record for month and employee
        //                        if (!monthlyAttendance.TryGetValue((dateOnly.Month, dateOnly.Year, employeeId), out var attendance))
        //                        {
        //                            attendance = new Attendance
        //                            {
        //                                EmployeeId = employeeId,
        //                                Month = dateOnly.Month,
        //                                Year = dateOnly.Year
        //                            };
        //                            _context.Add(attendance);
        //                            await _context.SaveChangesAsync();
        //                            monthlyAttendance[(dateOnly.Month, dateOnly.Year, employeeId)] = attendance;
        //                        }

        //                        // Create new AttendanceDetails entry for the day
        //                        previousAttendance = new AttendanceDetails
        //                        {
        //                            AttendanceId = attendance.Id,
        //                            CheckInTime = timeOnly,
        //                            AttendanceDate = dateOnly,
        //                        };

        //                        _context.Add(previousAttendance);
        //                        await _context.SaveChangesAsync();

        //                        // Link AttendanceDetails to Attendance
        //                        attendance.AttendanceDetails.Add(previousAttendance);
        //                    }
        //                    lastDate = currentMonthYear;
        //                }

        //                // Final save for last month's attendance
        //                foreach (var attendance in monthlyAttendance.Values)
        //                {
        //                    attendance.WorkingDays = attendance.AttendanceDetails
        //                        .Select(ad => ad.AttendanceDate)
        //                        .Distinct()
        //                        .Count();

        //                    attendance.WorkingHours = attendance.AttendanceDetails
        //                        .Sum(ad => ad.WorkingHoursAday?.Ticks ?? 0);

        //                    _context.Update(attendance);
        //                }

        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //    }

        //    return View();
        //}

        //public async Task<IActionResult> Create(IFormFile file)
        //{
        //    // Register the code pages encoding provider
        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        //    if (file != null && file.Length > 0)
        //    {
        //        var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\uploads\\";

        //        if (!Directory.Exists(uploadsFolder))
        //        {
        //            Directory.CreateDirectory(uploadsFolder);
        //        }

        //        var filePath = Path.Combine(uploadsFolder, file.FileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
        //        {
        //            using (var reader = ExcelReaderFactory.CreateReader(stream))
        //            {
        //                AttendanceDetails previousAttendance = null;
        //                decimal workingHoursAMonth = 0;
        //                int month=0;
        //                int employeeId;
        //                //int? attendanceId=0;
        //                DateOnly dateOnly;
        //                string lastDate="";
        //                string currentDate;

        //                while (reader.Read())
        //                {

        //                    // Extract DateTime and split into date and time
        //                    employeeId = Convert.ToInt16(reader.GetValue(0));
        //                    var dateTimeValue = DateTime.Parse(reader.GetValue(1).ToString());
        //                    dateOnly = DateOnly.FromDateTime(dateTimeValue);
        //                    currentDate = dateOnly.Month.ToString();
        //                    currentDate += "-"+ dateOnly.Year.ToString();
        //                    if (lastDate == "") { lastDate = currentDate; }
        //                    var timeOnly = dateTimeValue.TimeOfDay;
        //                    int result = string.Compare(currentDate, lastDate);
        //                    workingDaysCount++;
        //                    if (result != 0)
        //                    {
        //                        int currentMonth= DateOnly.Parse(lastDate).Month;
        //                        int currentYear= DateOnly.Parse(lastDate).Year;
        //                        test(currentMonth, currentYear, employeeId);
        //                    }
        //                    // Extract DateTime and split into date and time
        //                    if (previousAttendance != null && previousAttendance.AttendanceDate == dateOnly)
        //                    {
        //                        previousAttendance.CheckOutTime = timeOnly;
        //                        // Calculate working hours if CheckInTime and CheckOutTime are available
        //                        if (previousAttendance.CheckInTime != null && previousAttendance.CheckOutTime != null)
        //                        {
        //                            previousAttendance.WorkingHoursAday = previousAttendance.CheckOutTime - previousAttendance.CheckInTime;

        //                        }
        //                        _context.Update(previousAttendance);
        //                    }
        //                    else
        //                    {
        //                            previousAttendance = new AttendanceDetails
        //                            {
        //                                CheckInTime = timeOnly,
        //                                AttendanceDate = dateOnly,
        //                            };

        //                            _context.Add(previousAttendance);
        //                            _context.SaveChanges();

        //                    }
        //                    lastDate = dateOnly.Month.ToString();
        //                    lastDate += "-" + dateOnly.Year.ToString();
        //                }
        //            }

        //        }

        //    }
        //    return View();

        //}
        //private void test(int currentMonth, int currentYear, int employeeId)
        //{
        //    Attendance attendance = new Attendance();
        //    attendance.Month = currentMonth;
        //    attendance.Year = currentYear;
        //    attendance.WorkingDays = workingDaysCount;
        //    _context.Add(attendance);
        //    _context.SaveChanges();
        //    workingDaysCount = 0;
        //}
        //private int? test(int currentMonth,int currentYear,int employeeId)
        //{
        //    Attendance attendance = new Attendance();
        //    attendance.Month = currentMonth;
        //    attendance.Year = currentYear;
        //    attendance.EmployeeId = employeeId;
        //   // List<AttendanceDetails> attendanceDetails = _context.AttendanceDetailsTbl
        //   //   .Where(ad => ad.AttendanceDate.ToString().Contains(currentMonth.ToString())
        //   // && ad.AttendanceDate.ToString().Contains(currentYear.ToString())).ToList();
        //   // TimeSpan totalWorkingHours = new TimeSpan(attendanceDetails.Sum(a => a.WorkingHoursAday));
        //    _context.Add(attendance);
        //    _context.SaveChanges();
        //    int? attendanceId = attendance.Id;
        //    return attendanceId;
        //}
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
