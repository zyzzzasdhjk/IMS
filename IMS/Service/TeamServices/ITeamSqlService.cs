namespace IMS.Service.TeamServices;

public interface ITeamSqlService
{
    /// <summary>
    /// 新增成员
    /// </summary>
    /// <returns></returns>
    public bool AddMember();
    
    /// <summary>
    /// 删除成员
    /// </summary>
    /// <returns></returns>
    public bool DeleteMember();
    
    /// <summary>
    /// 获取所有成员
    /// </summary>
    /// <returns></returns>
    public string GetAllMembers(int tid);
    
    /// <summary>
    /// 获取成员
    /// </summary>
    /// <returns></returns>
    public bool GetMember();

    /// <summary>
    /// 增加管理员
    /// </summary>
    /// <returns></returns>
    public bool AddAdministrator();
    
    /// <summary>
    /// 删除管理员
    /// </summary>
    /// <returns></returns>
    public bool DeleteAdministrator();
    
    /// <summary>
    /// 获取所有管理员
    /// </summary>
    /// <returns></returns>
    public bool GetAllAdministrators();
}