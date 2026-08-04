namespace ProjectPlanner.Application.Common.Dtos.Comment;

public class CreateCommentDto
{
    public int AuthorId { get; set; }
    public string Body { get; set; } = null!;
}