using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Election.Dto
{
  
        public class DashboardReportDto
        {
            public List<InfoDto> Infos { get; set; }
            public List<List<InfoDto>> InfoLists { get; set; }
        
    }
}
