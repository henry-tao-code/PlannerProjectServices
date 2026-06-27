namespace ProjectPlanner.Application.Common.Dto.Project;

public record ProjectBoardDto(
    int ProjectId,
    IEnumerable<BoardColumnDto> Columns);
