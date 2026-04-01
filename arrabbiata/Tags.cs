namespace arrabbiata;

public class  Tag
{
    public Guid Id { get; private set; }
    public string? Name { get; private set; }

    public Tag(Guid id, string? name)
        => (Id, Name) = (id, name);
}