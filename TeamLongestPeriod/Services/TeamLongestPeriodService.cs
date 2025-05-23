using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
                await this.ResetInMemoryDatabase(); //Forgets the data from prevoiously uploaded files
                await this.ReadInputFile(file);
                await this.CalculateCoworkingPeriods();
            }
            catch
            {
                throw;
            }

            return await this._context.Output.ToListAsync();
        }

        private async Task CalculateCoworkingPeriods()
        {
            var pairDurations = new Dictionary<(int, int), int>();

            foreach (var group in this._context.Employees.AsEnumerable().GroupBy(x => x.ProjectID))
            {
                var employees = group.ToList();

                for (int i = 0; i < employees.Count; i++)
                {
                    for (int j = i + 1; j < employees.Count; j++)
                    {
                        Employee employee1 = null;
                        Employee employee2 = null;

                        if (employees[i].EmployeeID < employees[j].EmployeeID)
                        {
                            employee1 = employees[i];
                            employee2 = employees[j];
                        }
                        else
                        {
                            employee1 = employees[j];
                            employee2 = employees[i];
                        }

                        int overlappingDays = this.GetOverlapInDays(employee1.DateFrom, employee1.DateTo, employee2.DateFrom, employee2.DateTo);

                        if (overlappingDays > 0)
                        {
                            await this.SaveData(employee1, employee2, overlappingDays);
                        }
                    }
                }
            }
        }

        private int GetOverlapInDays(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            var overlapStart = start1 > start2 ? start1 : start2;
            var overlapEnd = end1 < end2 ? end1 : end2;

            return overlapEnd >= overlapStart ? (overlapEnd - overlapStart).Days + 1 : 0;
        }

        private async Task SaveData(Employee employee1, Employee employee2, int overlappingDays)
        {
            this.ValidateInputData(employee1.EmployeeID, employee2.EmployeeID);

            if (this._context.Output.Any(x => x.EmployeeOneID == employee1.EmployeeID && x.EmployeeTwoID == employee2.EmployeeID))
            {
                var output = this._context.Output.Where(x => x.EmployeeOneID == employee1.EmployeeID && x.EmployeeTwoID == employee2.EmployeeID).Single();

                output.Days += overlappingDays;
            }
            else
            {
                await this._context.Output.AddAsync(new Output
                {
                    EmployeeOneID = employee1.EmployeeID,
                    EmployeeTwoID = employee2.EmployeeID,
                    Days = overlappingDays,
                    ProjectID = employee1.ProjectID
                });
            }

            await this._context.SaveChangesAsync();
        }

        private void ValidateInputData(int employeeOneID, int employeeTwoID)
        {
            if(employeeOneID == employeeTwoID)
            {
                throw new ValidationException($"The data in the file is corrupt! Check the time spans for employee {employeeOneID}!");
            }
        }

        private async Task ReadInputFile(IFormFile file)
        {
            using(var stream = new StreamReader(file.OpenReadStream()))
            {
                while(!stream.EndOfStream)
                {
                    string[] values = stream.ReadLine().Split(',');

                    await this._context.Employees.AddAsync(new Employee
                    {
                        EmployeeID = int.TryParse(values[0], out int employeeID) ? employeeID : throw new Exception("CSV Validation Exception EmployeeID"),
                        ProjectID = int.TryParse(values[1], out int projectID) ? projectID : throw new Exception("CSV Validation Exception ProjectID"),
                        DateFrom = DateTime.TryParse(values[2], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFrom) ? dateFrom : throw new Exception("CSV Validation Exception DateFrom"),
                        DateTo = DateTime.TryParse(values[3], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTo) ? dateTo : values[3].Trim().ToUpper() == "NULL" ? DateTime.Today : throw new Exception($"CSV Validation Exception DateTo {values[3]}, {values[3].ToUpper()}")
                    });
                }
            }

            await this._context.SaveChangesAsync();
        }

        private async Task ResetInMemoryDatabase()
        {
            this._context.Employees.RemoveRange(this._context.Employees);
            this._context.Output.RemoveRange(this._context.Output);

            await this._context.SaveChangesAsync();
        }
    }
}
