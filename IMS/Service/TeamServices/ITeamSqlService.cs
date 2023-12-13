using IMS.Models;
using IMS.Models.Team;

namespace IMS.Service.TeamServices;

public interface ITeamSqlService
{
    /// <summary>
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="joinCode"></param>
    /// <returns></returns>
    public UserCreateTeamResponseStatus UserCreateTeam(int uid, string name, string description, string joinCode);

    /// <summary>
    ///     用户的身份信息的修改
    /// </summary>
    /// <param name="commandUid">命令发出者id</param>
    /// <param name="uid">被指定的用户id</param>
    /// <param name="tid">团队id</param>
    /// <param name="role">要将目标用户更改为何种身份</param>
    /// <returns></returns>
    public UserAppointResponseStatus UserAppoint(int commandUid, int uid, int tid, string role);

    /// <summary>
    ///     根据加入码加入团队
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="joinCode"></param>
    /// <returns></returns>
    public ResponseModel JoinTeam(int uid, string joinCode);

    /// <summary>
    ///     根据tid加入团队
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ResponseModel JoinTeam(int uid, int tid);

    /// <summary>
    ///     用户退出团队
    /// </summary>
    /// <param name="uid">用户id</param>
    /// <param name="tid">团队id</param>
    /// <returns></returns>
    public ResponseModel ExitTeam(int uid, int tid);

    /// <summary>
    ///     获取团队信息
    /// </summary>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ResponseModel GetTeamInfo(int tid);

    // public ReturnMessageModel UpdateTeam(TeamInfoModel t);

    /// <summary>
    ///     获取用户的全部团队
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public ResponseModel GetUserTeams(int uid);

    /// <summary>
    ///     团队创建者删除掉团队
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="tid"></param>
    public ResponseModel DeleteTeam(int uid, int tid);

    /// <summary>
    ///     查询该团队的全部成员
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="tid"></param>
    /// <returns></returns>
    public ResponseModel GetTeamMembers(int uid, int tid);
}