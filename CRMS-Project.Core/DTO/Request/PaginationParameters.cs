using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.DTO.Request
{
    public class PaginationParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? FilterOn { get; set; } 
        public string? FilterQuery { get; set; }
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;
    }
}
