using System.Text.Json.Nodes;
using IMS.Models;
using IMS.Models.Team;

namespace IMS.Service.TeamServices;

public interface ITeamSqlService
{
    /// <summary>
    /// 新增成员
    /// </summary>
    /// <returns></returns>
    public bool AddMember(int uid,int tid);
    
    /// <summary>
    /// 删除成员
    /// </summary>
    /// <returns></returns>
    public bool DeleteMember(int uid,int tid);
    
    /// <summary>
    /// 获取所有成员
    /// </summary>
    /// <returns></returns>
    public List<Dictionary<string, object>> GetAllMembers(int tid);

    /// <summary>
    /// 增加管理员
    /// </summary>
    /// <returns></returns>
    public bool AddAdministrator(int uid,int tid);
    
    /// <summary>
    /// 删除管理员
    /// </summary>
    /// <returns></returns>
    public bool DeleteAdministrator(int uid,int tid);
    
    /// <summary>
    /// 创建团队
    /// </summary>
    /// <param name="json">团队的基本信息</param>
    /// <returns></returns>
    public ReturnMessageModel CreateTeam(TeamInfo t);
    
    /// <summary>
    /// 更新团队信息，根据传入的json中的key来更新
    /// </summary>
    /// <param name="json">需要更新的信息</param>
    /// <returns></returns>
    public ReturnMessageModel UpdateTeam(TeamInfo t);

    /// <summary>
    /// 更新团队人数
    /// </summary>
    /// <param name="tid"></param>
    /// <param name="newNum"></param>
    /// <returns></returns>
    public ReturnMessageModel UpdateTeamPeopleMaxNum(int tid, int newNum);
    
    /// <summary>
    /// 删除团队
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ReturnMessageModel DeleteTeam(int tid);
    
    /// <summary>
    /// 对团队进行封禁,时间以小时计算
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ReturnMessageModel BanTeam(int tid,int time);
}