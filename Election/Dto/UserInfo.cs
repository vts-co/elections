using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Election.Dto
{

    public class UserInfo
    {
        public int UserId { get; set; }
        public Guid PersonId { get; set; }
        public int RoleId { get; set; }
        public Guid? CategoryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Picture { get; set; }


    }
}
