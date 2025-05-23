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

		[BindProperty]
		public List<Output> Output { get; set; }

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
				var errorMessage = "Plese, select file or verify the file is not empty!"; // move to constants.cs

                this._logger.LogError(errorMessage);

                return base.BadRequest(errorMessage);
            }

            try
            {
                var result = await this._service.ProcessUploadedFile(input);

				if (result.Any()) 
				{
					this.Output = result.OrderByDescending(x => x.Days).ToList();
				}
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex.Message);
                return base.BadRequest(ex.Message);
            }

            this._logger.Log(LogLevel.Information, "File processed!");

			return base.Page();
		}
    }
}
