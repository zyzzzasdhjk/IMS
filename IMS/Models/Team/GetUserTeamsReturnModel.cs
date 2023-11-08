namespace IMS.Models.Team;

public enum GetUserTeamsReturnStatus
{
    Success = 0, // 成功
    Error = 1 // 后端错误
}

public class GetUserTeamsReturnModel
{
    public GetUserTeamsReturnStatus Code { get; set; } = GetUserTeamsReturnStatus.Success;
    public List<TeamItemModel> Teams { get; set; } = new List<TeamItemModel>();
}