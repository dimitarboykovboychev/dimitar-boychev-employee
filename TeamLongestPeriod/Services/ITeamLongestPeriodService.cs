using TeamLongestPeriod.Models;

namespace TeamLongestPeriod.Services
{
    public interface ITeamLongestPeriodService
    {
        Task<List<Output>> ProcessUploadedFile(IFormFile file);
    }
}
