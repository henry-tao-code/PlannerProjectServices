using Domain.Enums;
using ProjectPlanner.Application.Common.Dto.Attachment;
using ProjectPlanner.Application.Common.Dto.Comment;
using ProjectPlanner.Application.Common.Dto.History;

namespace ProjectPlanner.Application.Common.Dto.Issue;

public class IssueDetailDto
{
    public int Id { get; set; }
    public string IssueKey { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public IssueType IssueType { get; set; }
    public IssueStatus Status { get; set; }
    public IssuePriority Priority { get; set; }

    public int ProjectId { get; set; }
    public int? SprintId { get; set; }

    public int? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }

    public int? ReporterId { get; set; }
    public string? ReporterName { get; set; }

    public int? ParentIssueId { get; set; }

    public int StoryPoints { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public IssueTimeTrackingDto TimeTracking { get; set; } = new();

    public List<string> Labels { get; set; } = [];

    public List<CommentDto> Comments { get; set; } = [];
    public List<HistoryDto> History { get; set; } = [];
    public List<AttachmentDto> Attachments { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}