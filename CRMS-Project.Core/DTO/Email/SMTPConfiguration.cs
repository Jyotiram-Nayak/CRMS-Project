using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Email
{
    public class SMTPConfiguration
    {
        public string SenderEmail { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SenderName { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSSL { get; set; }
        public bool IsBodyHTML { get; set; }
    }
}
