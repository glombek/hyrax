using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hyrax.Core.Models.Implement
{
    public class Author : IAuthor
    {
        public Author(string name) => Name = name;

        public string Name { get; set; }

        public static implicit operator Author(string author) => new Author(author);
    }
}
