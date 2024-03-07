using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hyrax.Core.Models.Implement
{
    public class Author : IAuthor
    {
        public Author(string username, string name)
        {
            Name = name;
            Username = username;
        }

        public string Username { get; }
        public string Name { get; }

        public static implicit operator Author(string author) => new(author, author);

        #region Equals
        public bool Equals(IAuthor? other)
        {
            if (other is null)
            {
                return false;
            }

            return Username == other.Username;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is IAuthor author)
            {
                return Equals(author);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }
        #endregion
    }
}
