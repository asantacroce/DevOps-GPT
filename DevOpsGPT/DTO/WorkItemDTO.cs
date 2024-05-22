namespace DevOpsGPT.Services.DTO;
public record WorkItemDTO(
    string Id,
    string Title,
    string State,
    string Uri,
    string DateCreated,
    string WorkItemType
    );