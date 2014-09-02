using System;
using System.Net.Http;
using NUnit.Framework;

namespace MyLittleAdmin.Tests {
    [TestFixture]
    public class LoginCommandTests
    {
        private const string BaseUri = "";

        [Test]
        public async void Execute_Works() {
            var connectionInfo = new DbConnectionInfo() {
                Uri = new Uri(BaseUri),
                Host = "",
                DatabaseName = "",
                UserName = "",
                Password = ""
            };
            var loginCommand = new LoginCommand(connectionInfo);

            var httpClient = new HttpClient();
            await loginCommand.Execute(httpClient);
        }
    }
}