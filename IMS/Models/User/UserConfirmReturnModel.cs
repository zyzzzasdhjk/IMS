namespace IMS.Models.User;

public enum UserConfirmReturnStatus
{
    Success = 0, // 成功
    NonExistent = 1, // 不存在
    Error = 2, // 超时
    Completed = 3 // 已完成
}

public class UserConfirmReturnModel
{
    public UserConfirmReturnModel(UserConfirmReturnStatus s)
    {
        switch (s)
        {
            case UserConfirmReturnStatus.Success:
                Code = UserConfirmReturnStatus.Success;
                Message = "验证成功";
                break;
            case UserConfirmReturnStatus.NonExistent:
                Code = UserConfirmReturnStatus.NonExistent;
                Message = "验证码错误！";
                break;
            case UserConfirmReturnStatus.Error:
                Code = UserConfirmReturnStatus.Error;
                Message = "验证码错误";
                break;
            case UserConfirmReturnStatus.Completed:
                Code = UserConfirmReturnStatus.Completed;
                Message = "验证已完成";
                break;
        }
    }

    public UserConfirmReturnStatus Code { get; set; } = UserConfirmReturnStatus.Success;
    public string? Message { get; set; }
}