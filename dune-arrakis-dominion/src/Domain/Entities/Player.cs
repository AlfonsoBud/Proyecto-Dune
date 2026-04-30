namespace Domain.Entities
{
    using System;

    public class Player
    {
        public Guid Id { get; set; }
        public string Alias { get; set; }

        public Player(string alias)
        {
            Id = Guid.NewGuid();
            Alias = alias;
        }
    }
}