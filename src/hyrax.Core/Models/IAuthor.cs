namespace hyrax.Core.Models
{
    public interface IAuthor:IEquatable<IAuthor>
    {
        /// <summary>
        /// A URL-safe string that uniquely identifies the author
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Display name of the author
        /// </summary>
        string Name { get; }
    }
}
