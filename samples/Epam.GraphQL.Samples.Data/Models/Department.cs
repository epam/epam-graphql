using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Samples.Data.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public Department Parent { get; set; }

        public ICollection<Department> Children { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public int CreatedById { get; set; }

        public int ModifiedById { get; set; }

        public User CreatedBy { get; set; }

        public User ModifiedBy { get; set; }
    }
}
