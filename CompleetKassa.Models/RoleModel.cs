using System.Collections.Generic;

namespace CompleetKassa.Models
{
    public class RoleModel
    {
        public RoleModel()
        {
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Allow { get; set; }
        public List<ResourceModel> Resources { get; set; }
    }
}
