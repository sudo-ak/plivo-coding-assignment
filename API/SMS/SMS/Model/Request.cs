using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Model
{
    public class Request
    {
        [Required(ErrorMessage = "from is missing")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "from is invalid")]
        public string From { get; set; }

        [Required(ErrorMessage = "to is missing")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "to is invalid")]
        public string To { get; set; }

        [Required(ErrorMessage = "text is missing")]
        [StringLength(120, MinimumLength = 1, ErrorMessage = "text is invalid")]
        public string Text { get; set; }
    }
}
