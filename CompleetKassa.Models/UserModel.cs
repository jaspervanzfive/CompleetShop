using System.Collections.Generic;

namespace CompleetKassa.Models
{
    public class UserModel
    {
        public UserModel()
        {
        }

        public int ID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public List<RoleModel> Roles { get; set; }
    }
}
