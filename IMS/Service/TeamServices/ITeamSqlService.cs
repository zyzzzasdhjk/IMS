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
    /// 
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="joinCode"></param>
    /// <returns></returns>
    public UserCreateTeamResponseStatus UserCreateTeam(int uid, string name, string description, string joinCode);
    
    /// <summary>
    /// 用户的身份信息的修改
    /// </summary>
    /// <param name="commandUid"></param>
    /// <param name="uid"></param>
    /// <param name="tid"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public UserAppointResponseStatus UserAppoint(int commandUid, int uid,int tid,string role);
    
    /// <summary>
    /// 根据加入码加入团队
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="joinCode"></param>
    /// <returns></returns>
    public JoinTeamResponseModel JoinTeam(int uid,string joinCode);
    
    /// <summary>
    /// 根据tid加入团队
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="tid"></param>
    /// <returns></returns>
    public JoinTeamResponseModel JoinTeam(int uid, int tid);
    
    /// <summary>
    /// 获取团队信息
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public TeamInfo GetTeamInfo(int tid);
    
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
    /// <param name="t">团队的基本信息</param>
    /// <returns></returns>
    public ReturnMessageModel CreateTeam(TeamInfo t);
    
    /// <summary>
    /// 更新团队信息，根据传入的json中的key来更新
    /// </summary>
    /// <param name="t">需要更新的信息</param>
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
    /// <param name="time"></param>
    /// <returns></returns>
    public ReturnMessageModel BanTeam(int tid,int time);

    /// <summary>
    /// 获取用户的全部团队
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public GetUserTeamsReturnModel GetUserTeams(int uid);
}