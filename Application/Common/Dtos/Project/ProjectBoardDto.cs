namespace ProjectPlanner.Application.Common.Dtos.Project;

public record ProjectBoardDto(
    int ProjectId,
    IEnumerable<BoardColumnDto> Columns);
