using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Election.Dto
{
    public class CandidateDto
    {
        public string CandidateName { get; set; }
        public double Percentage { get; set; }
        public int TotalVoters { get; set; }
    }
}