namespace IMS.Models.Team;

public enum GetUserTeamsReturnStatus
{
    Success = 0, // 成功
    Error = 1 // 后端错误
}

public class GetUserTeamsReturnModel
{
    public GetUserTeamsReturnModel(GetUserTeamsReturnStatus s)
    {
        Code = s;
        Teams = new List<TeamItemModel>();
    }

    public GetUserTeamsReturnModel(List<TeamItemModel> l)
    {
        Code = GetUserTeamsReturnStatus.Success;
        Teams = l;
    }

    public GetUserTeamsReturnStatus Code { get; set; }
    public List<TeamItemModel> Teams { get; set; }
}