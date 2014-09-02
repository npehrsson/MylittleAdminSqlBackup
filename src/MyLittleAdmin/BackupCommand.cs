using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MyLittleAdmin
{
    public class BackupCommand {
        private readonly DbConnectionInfo _connectionInfo;

        public BackupCommand(DbConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
            SleepIntervalUntilFetchDatabase = new TimeSpan(0,0,0,5);
        }

        public TimeSpan SleepIntervalUntilFetchDatabase { get; set; }

        public async Task<bool> Execute(HttpClient httpClient, Stream stream)
        {
            var uri = new Uri(_connectionInfo.Uri, "tools/backupwh.aspx");

            var pageContent = await httpClient.GetStringAsync(uri).ConfigureAwait(false);

            var viewStateRegex = new Regex(@"id=""__VIEWSTATE""\s*value=""([^""]+)""");
            var viewStateMatch = viewStateRegex.Match(pageContent);

            if (!viewStateMatch.Success)
                return false;

            var viewState = viewStateMatch.Groups[1].Value;

            var eventValidationRegex = new Regex(@"id=""__EVENTVALIDATION""\s*value=""([^""]+)""");
            var eventValidationMatch = eventValidationRegex.Match(pageContent);

            if (!eventValidationMatch.Success)
                return false;

            var eventValidation = eventValidationMatch.Groups[1].Value;

            var postData = new Dictionary<string, string>()
            {
                {"__VIEWSTATE", viewState},
                {"__EVENTVALIDATION", eventValidation},
                {"fDatabase$cControl", _connectionInfo.DatabaseName},
                {"fSetName$cControl", string.Empty},
                {"fSetDescription$cControl", string.Empty},
                {"btnSubmit", "Backup"}
            };

            var response = await httpClient.PostAsync(uri, new FormUrlEncodedContent(postData)).ConfigureAwait(false);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var backupUrlRegex = new Regex(@"""([^""]+/dbbackup/[^""]+)");
            var backupUrlMatch = backupUrlRegex.Match(data);

            if (!backupUrlMatch.Success)
                return false;

            Thread.Sleep(SleepIntervalUntilFetchDatabase.Milliseconds);

            var fileResponse = await httpClient.GetAsync(backupUrlMatch.Groups[1].Value).ConfigureAwait(false);

            await fileResponse.Content.CopyToAsync(stream).ConfigureAwait(false);
            
            return true;
        }
    }
}