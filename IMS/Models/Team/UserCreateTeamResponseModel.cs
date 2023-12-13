namespace IMS.Models.Team;

public enum UserCreateTeamResponseStatus
{
    Success = 0,
    NameIllegality = 1, // 团队名不合格
    DescriptionIllegality = 2, // 团队描述不合格
    JoinCodeIllegality = 3, // 加入码不合格
    JoinCodeRepeat = 4, // 加入码重复
    UnknownError = 5 // 未知错误
}

public class UserCreateTeamResponseModel
{
    public UserCreateTeamResponseModel(UserCreateTeamResponseStatus s)
    {
        switch (s)
        {
            case UserCreateTeamResponseStatus.Success:
                Code = UserCreateTeamResponseStatus.Success;
                Message = "创建成功";
                break;
            case UserCreateTeamResponseStatus.NameIllegality:
                Code = UserCreateTeamResponseStatus.NameIllegality;
                Message = "团队名不合格";
                break;
            case UserCreateTeamResponseStatus.DescriptionIllegality:
                Code = UserCreateTeamResponseStatus.DescriptionIllegality;
                Message = "团队描述不合格";
                break;
            case UserCreateTeamResponseStatus.JoinCodeIllegality:
                Code = UserCreateTeamResponseStatus.JoinCodeIllegality;
                Message = "加入码不合格";
                break;
            case UserCreateTeamResponseStatus.JoinCodeRepeat:
                Code = UserCreateTeamResponseStatus.JoinCodeRepeat;
                Message = "加入码重复";
                break;
            case UserCreateTeamResponseStatus.UnknownError:
                Code = UserCreateTeamResponseStatus.UnknownError;
                Message = "未知错误";
                break;
        }
        /*算一个数的所有因数*/
    }

    public UserCreateTeamResponseStatus Code { get; set; }
    public string? Message { get; set; }
}