namespace Auth.Jwt.Domain;

public interface IEntity<TKey>
{
    TKey Id { get; }
}

public abstract class Entity<TKey> : IEntity<TKey>
{
    public TKey Id { get; private set; }
}

public class Entity : Entity<long> { }
