namespace ProjectPlanner.Application.Common.Dto.Comment;

public class UpdateCommentDto
{
    public string Body { get; set; } = null!;
    public uint RowVersion { get; set; }
}