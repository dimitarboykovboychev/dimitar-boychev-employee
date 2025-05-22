using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TeamLongestPeriod.Models;

namespace TeamLongestPeriod.Services
{
    public class TeamLongestPeriodService: ITeamLongestPeriodService
    {
        private readonly TeamLongestPeriodDbContext _context;

        public TeamLongestPeriodService(TeamLongestPeriodDbContext context)
        {
            this._context = context;
        }

        public async Task<List<Output>> ProcessUploadedFile(IFormFile file)
        {
            try
            {
                using(var stream = new StreamReader(file.OpenReadStream()))
                {
                    while(!stream.EndOfStream)
                    {
                        string[] values = stream.ReadLine().Split(',');

                        await _context.Employees.AddAsync(new Employee
                        {
                            EmployeeID = int.TryParse(values[0], out int employeeID) ? employeeID : throw new Exception("CSV Validation Exception EmployeeID"),
                            ProjectID = int.TryParse(values[1], out int projectID) ? projectID : throw new Exception("CSV Validation Exception ProjectID"),
                            DateFrom = DateTime.TryParse(values[2], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFrom) ? dateFrom : throw new Exception("CSV Validation Exception DateFrom"),
                            DateTo = DateTime.TryParse(values[3], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTo) ? dateTo : values[3].Trim().ToUpper() == "NULL" ? DateTime.Today : throw new Exception($"CSV Validation Exception DateTo {values[3]}, {values[3].ToUpper()}")
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

            return await _context.Output.ToListAsync();
        }
    }
}
