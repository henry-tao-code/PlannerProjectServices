public class IssueLabel
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public Issue Issue { get; set; } = null!;
    public string Value { get; private set; } = null!;
    private IssueLabel() { }
    public IssueLabel(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Label cannot be empty.");

        Value = value.Trim().ToLowerInvariant();
    }
}