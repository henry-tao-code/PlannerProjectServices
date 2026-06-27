namespace ProjectPlanner.Application.Common.Dto.Comment;

public class CreateCommentDto
{
    public int AuthorId { get; set; }
    public string Body { get; set; } = null!;
}