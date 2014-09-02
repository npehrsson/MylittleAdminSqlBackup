using System;
using System.Globalization;
using System.IO;
using System.Net.Http;

namespace MyLittleAdmin {
    public class BackupManager
    {
        private readonly DbConnectionInfo _connectionInfo;

        public BackupManager(DbConnectionInfo connectionInfo)
        {
            if (connectionInfo == null) throw new ArgumentNullException("connectionInfo");
            _connectionInfo = connectionInfo;
        }

        public void Backup(Stream stream)
        {
            var httpClient = new HttpClient();
            var loginCommand = new LoginCommand(_connectionInfo);

            var loginTask =  loginCommand.Execute(httpClient);
            loginTask.Wait();

            var backupCommand = new BackupCommand(_connectionInfo);     
            var backupTask = backupCommand.Execute(httpClient, stream);
            backupTask.Wait();

            if (!backupTask.Result)
                throw new Exception("could not backup the database");

            if (stream.Length == 0 || stream.Length / 1000 < 1000)
                throw new Exception("could not backup the database, backup to small");
        }

        public void Backup(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            if(fileInfo.Exists)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "file {0} already exists", fileInfo.FullName));

            using (var fileStream = fileInfo.Create())
                Backup(fileStream);
        }
    }
}