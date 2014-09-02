using System;

namespace MyLittleAdmin
{
    public class DbConnectionInfo
    {
        public Uri Uri { get; set; }
        public string Host { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}