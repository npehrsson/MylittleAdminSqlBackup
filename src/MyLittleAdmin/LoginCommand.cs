using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLittleAdmin
{
    public class LoginCommand
    {
        private readonly DbConnectionInfo _connectionInfo;

        public LoginCommand(DbConnectionInfo connectionInfo)
        {
            if (connectionInfo == null) throw new ArgumentNullException("connectionInfo");
            _connectionInfo = connectionInfo;
        }

        public async Task Execute(HttpClient client)
        {
            var list = new Dictionary<string, string>()
            {
                {"Login", _connectionInfo.UserName},
                {"Password", _connectionInfo.Password},
                {"ServerName", _connectionInfo.Host},
                {"InitialCatalog", _connectionInfo.DatabaseName}
            };

            await client.PostAsync(new Uri(_connectionInfo.Uri, "silentlogon.aspx"), new FormUrlEncodedContent(list)).ConfigureAwait(false);
        }
    }
}