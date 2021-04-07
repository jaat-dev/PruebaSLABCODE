namespace Projects.Api.Options
{
    public class MailSettings
    {
        public string From { get; set; }

        public string Smtp { get; set; }

        public int Port { get; set; }

        public string Password { get; set; }
    }
}
