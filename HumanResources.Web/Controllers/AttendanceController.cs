using ExcelDataReader;
using HumanResources.Domain.Entities;
using HumanResources.Infrastructure.DbContext;
using HumanResources.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
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
                        decimal workingHoursAMonth = 0;
                        int month=0;
                        int employeeId;
                        //int? attendanceId=0;
                        DateOnly dateOnly;
                        string lastDate="";
                        string currentDate;
                      
                        while (reader.Read())
                        {

                            // Extract DateTime and split into date and time
                            employeeId = Convert.ToInt16(reader.GetValue(0));
                            var dateTimeValue = DateTime.Parse(reader.GetValue(1).ToString());
                            dateOnly = DateOnly.FromDateTime(dateTimeValue);
                            currentDate = dateOnly.Month.ToString();
                            currentDate += "-"+ dateOnly.Year.ToString();
                            if (lastDate == "") { lastDate = currentDate; }
                            var timeOnly = dateTimeValue.TimeOfDay;
                            int result = string.Compare(currentDate, lastDate);
                            if (result != 0)
                            { 
                                int currentMonth= DateOnly.Parse(lastDate).Month;
                                int currentYear= DateOnly.Parse(lastDate).Year;
                                test(currentMonth, currentYear, employeeId);
                            }
                            // Extract DateTime and split into date and time
                            if (previousAttendance != null && previousAttendance.AttendanceDate == dateOnly)
                            {
                                previousAttendance.CheckOutTime = timeOnly;
                                // Calculate working hours if CheckInTime and CheckOutTime are available
                                if (previousAttendance.CheckInTime != null && previousAttendance.CheckOutTime != null)
                                {
                                    previousAttendance.WorkingHoursAday = previousAttendance.CheckOutTime - previousAttendance.CheckInTime;
                                    
                                }
                                _context.Update(previousAttendance);
                            }
                            else
                            {
                                    previousAttendance = new AttendanceDetails
                                    {
                                        CheckInTime = timeOnly,
                                        AttendanceDate = dateOnly,
                                    };
                                    
                                    _context.Add(previousAttendance);
                                    _context.SaveChanges();
                                
                            }
                            lastDate = dateOnly.Month.ToString();
                            lastDate += "-" + dateOnly.Year.ToString();
                        }
                    }

                }

            }
            return View();

        }
        private void test(int currentMonth, int currentYear, int employeeId)
        {
            Attendance attendance = new Attendance();
            attendance.Month = currentMonth;
            attendance.Year = currentYear;
            if(_context.EmployeeTbl.FirstOrDefault(e=>e.Id == employeeId) != null)
            {
                attendance.EmployeeId = employeeId;

            }
            // List<AttendanceDetails> attendanceDetails = _context.AttendanceDetailsTbl
            //   .Where(ad => ad.AttendanceDate.ToString().Contains(currentMonth.ToString())
            // && ad.AttendanceDate.ToString().Contains(currentYear.ToString())).ToList();
            // TimeSpan totalWorkingHours = new TimeSpan(attendanceDetails.Sum(a => a.WorkingHoursAday));
            _context.Add(attendance);
            _context.SaveChanges();
        }
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
            IEnumerable<Attendance> data = _context.AttendanceTbl.AsEnumerable();
            return View(data);
        }
       
    }
}
