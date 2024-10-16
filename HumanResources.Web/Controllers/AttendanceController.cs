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
            AttendancDetails attendancDetails = new AttendancDetails();
            //int EmployeeId = 0;
            //int Month = 0;
            //int Year = 0;
            //int DaysPresent = 0;
            // Step 1: Get attendance counts and update NetSalary
            var attendanceCounts = await _context.AttendanceTbl
                .GroupBy(a => new
                {
                    a.EmployeeId,              // تجميع حسب كود الموظف
                    Month = a.AttendanceDate.Month,  // تجميع حسب الشهر
                    Year = a.AttendanceDate.Year    // تجميع حسب السنة
                })
                .Select(g =>new
                {
                    EmployeeId = g.Key.EmployeeId,
                    Month = g.Key.Month,            // الشهر
                    Year = g.Key.Year,            // السنة
                    DaysPresent = g.Count(),       // عدد أيام الحضور في هذا الشهر
                })
                .Join(_context.EmployeeTbl,            // الانضمام مع جدول الموظفين
                      attendance => attendance.EmployeeId,  // شرط الربط باستخدام EmployeeId
                      employee => employee.Id,
                      (attendance, employee) => new
                      {
                          EmployeeCode = attendance.EmployeeId, // كود الموظف
                          EmpName = employee.Name,           // اسم الموظف
                          DaysPresent = attendance.DaysPresent,
                          GrossSalary = employee.GrossSalary,
                          netSalary = employee.NetSalary,// عدد أيام الحضور
                          Month = attendance.Month,             // الشهر
                          Year = attendance.Year
                      })
                .OrderBy(a => a.Year).ThenBy(a => a.Month)  // ترتيب النتائج
                .ToListAsync();

            // تحديث صافي المرتب بناءً على عدد الأيام
            foreach (var attendance in attendanceCounts)
            {
                var employee = await _context.EmployeeTbl.FindAsync(attendance.EmployeeCode);
                if (employee != null && employee.GrossSalary.HasValue)
                {
                    // حساب صافي المرتب
                    employee.NetSalary = (employee.GrossSalary.Value / 30) * attendance.DaysPresent;
                }
            }

            // احفظ التغييرات في قاعدة البيانات
            await _context.SaveChangesAsync();


            ViewBag.test = attendanceCounts;
            ViewBag.Message = "success";
            return View();
        }
    }
}