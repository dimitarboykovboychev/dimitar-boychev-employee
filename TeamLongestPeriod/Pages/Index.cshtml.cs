using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TeamLongestPeriod.Models;
using TeamLongestPeriod.Services;

namespace TeamLongestPeriod.Pages
{
	public class IndexModel: PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly ITeamLongestPeriodService _service;

		public Output _output;

		public IndexModel(ILogger<IndexModel> logger, ITeamLongestPeriodService service)
		{
			this._logger = logger;
			this._service = service;
		}

		public void OnGet()
		{
			this._logger.Log(LogLevel.Information, "Get Index Page");
		}

		public async Task<IActionResult> OnPost(IFormFile input)
		{
            if (input == null || input.Length == 0)
            {
				var errorMessage = "Invalid file"; // move to constants.cs

                this._logger.LogError(errorMessage);

                return base.BadRequest(errorMessage);
            }

            try
            {
                var result = await this._service.ProcessUploadedFile(input);

				if (result.Any()) 
				{
					_output = result.First();
				}
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex.Message);
                return base.BadRequest(ex.Message);
            }

            this._logger.Log(LogLevel.Information, "Get Index Page");

            return base.RedirectToAction("OnGet"); //TODO
		}
    }
}
