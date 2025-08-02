using MeterReadingsApi.Shared;
using System.Net.Http.Json;

namespace MeterReadingsBlazorClient.Pages
{
    public partial class Home
    {
        private IList<AccountDto>? accounts;
        private IList<MeterReadingDto>? meterReadings;

        protected override async Task OnInitializedAsync()
        {
            accounts = await Http.GetFromJsonAsync<IList<AccountDto>>("accounts");
        }

        private async Task OnAccountSelected(AccountDto account)
        {
            var response = await Http.GetAsync($"accounts/{account.AccountId}/meter-readings");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                meterReadings = Array.Empty<MeterReadingDto>();
            }
            else
            {
                response.EnsureSuccessStatusCode();
                meterReadings = await response.Content.ReadFromJsonAsync<IList<MeterReadingDto>>();
            }
        }
    }
}