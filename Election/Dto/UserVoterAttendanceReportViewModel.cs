using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Election.Dto
{
   
        public class UserVoterAttendanceReportViewModel
        {
            public string UserName { get; set; } // من جدول Users
            public int AttendedCount { get; set; } // عدد الناخبين الذين سجلهم
        }

    
}