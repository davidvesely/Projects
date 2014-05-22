namespace TaskAsync
{
    /// <summary>
    /// Sample class providing example contact information.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Gets or sets a unique identifier for this <see cref="Contact"/> instance.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact.
        /// </summary>
        /// <value>
        /// The name of the contact.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the age of the contact.
        /// </summary>
        /// <value>
        /// The age of the contact.
        /// </value>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the avatar associated with the contact.
        /// </summary>
        /// <value>
        /// The avatar of the contact.
        /// </value>
        public string Avatar { get; set; }
    }
}
