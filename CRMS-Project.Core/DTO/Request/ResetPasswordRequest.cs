using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Request
{
    public class ResetPasswordRequest
    {
        public string Uid { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
