主要是采用了webapi的形式，利用后端接口来传递数据。绿色的接口是不需要鉴权，红色的需要。鉴权是将后端传过来的authorization放在header里面来进行鉴权。



> 访问方式：例如在user下的Register的访问方法为：/user/Register，方法为post

- message 函数执行的信息（说明错误信息）

- status 返回代码（这两个是所有的返回值都带有的）

返回值没有写的话代表只有上面这两个返回值



### User

这一部分的借口主要是是要实现用户登录注册相关，以及查询用户信息的功能 



#### **Register**

用户注册函数，注册成功会向用户填入的邮箱中发送一份包含验证码的邮件以激活账号，该验证码的有效时间为30分钟。未激活的账号无法登录

请求参数

- Username 要求在4位以上，16位以下，支持英语和数字

- Password 要求8位以上，20位以下，要求至少有数字，字母和符号中的两种

- Email 用户验证自己信息的邮箱，邮箱只允许使用一次

返回值

- uid 用户注册后得到的uid（如果注册成功的话）



#### Login

用户登录函数

请求参数

- Username 用户的用户名

- Password 密码

返回值

- uid 用户的uid

- authenticationCode 身份授权码，需要鉴权的接口所需的

```JSON
{
    "status": 0,
    "message": "登录成功",
    "data": {
        "uid": 47,
        "authenticationCode": "D342B5824858E4BAC7FCEC2416266503AB7ED7559066173950C9E6DE40B0F44C"
    }
}
```



#### Confirm

用户验证自己所设的验证邮箱

请求参数

- Uid 用户的uid

- CheckCode 验证码

返回值



#### ResendEmail

重新发送验证邮件，重发验证邮件会使得之前的验证邮件中的验证码失效

请求参数

- Uid 用户的uid



#### ResetEmail

用户修改自己的验证邮箱

请求参数

- Username 用户名

- Email 新的邮箱



#### ResetPassword

用户发出修改密码的请求，然后会发送验证邮件

请求参数

- Uid



#### ResetPasswordConfirm

用户修改密码

请求参数

- Uid

- CheckCode 验证码

- Password 新密码



#### GetUserInfo

获取用户信息

请求参数：

- Uid

返回值

```JSON
{
    "status": 0,
    "message": "获取用户信息成功",
    "data": {
        "name": "123",
        "gender": "Unknown",
        "birthday": "",
        "description": "",
        "createdAt": "2023-11-08"
    }
}
```



#### UpdateUserInfo

更新用户的个人信息

请求参数

- Uid

- Name

- Gender

- Birthday

- Description

返回值



### Team

#### CreateTeam

创建团队

请求参数

- Uid

- Name ： 不允许为空，20字以下

- Description ： 对团队的描述，可以为空140字以下

- JoinCode ： 用户可以自定义团队的加入码，要么为空，要么为9位数的数字和大写字母组成加入码（由后端判断是否重复）

返回值



#### GetUserTeams

查询用户所在的全部团队

请求参数

- Uid

返回值

```JSON
{
  "code": 0,
  "teams": [   注意，这里是一个列表
      {
          "tid": 1,
          "name": "team1",
          "description": "team1",
          "number": 1
      },
      {
          "tid": 9,
          "name": "医生",
          "description": "test",
          "number": 1
      }
  ]
  }
}
```



#### JoinTeam

用户加入团队

请求参数

- Uid

- Tid 团队id

- JoinCode  加入码，这两个参数2选1就行了，如果两个都不为空的话，会优先选择Tid



#### UserAppoint

用户修改团队中其他成员的身份，团队创建者拥有最高权限，可以任命和撤销管理员

管理员可以修改团队的信息，可以发布和审核任务，可以踢出普通成员

普通成员可以成为任务的执行者

请求参数

- CommandUid 任务发出者的Uid

- Uid 被任命者的Uid

- Tid 所在的团队的id

- Role 要把这个人的身份更改为什么呢



#### ExitTeam

用户退出团队

请求参数

- Uid

- Tid



#### GetTeamInfo

获取团队的基本信息

请求参数

- Tid

返回值

```JSON
{
    "status": 0,
    "message": "ok",
    "data": {
        "tid": 0,
        "name": "yyy",
        "description": "yyy",
        "joinCOde": null,
        "createTime": "2023-11-10",
        "number": 1
    }
}
```



#### GetTeamMembers

获取团队中的全部成员

请求参数

- Tid

返回值

```JSON
{
    "status": 0,
    "message": "ok",
    "data": [
        {
            "name": "123",
            "uid": 47,
            "role": "Creator"
        }
    ]
}
```



### Task



#### GetTaskInfo

获取项目的基本信息

请求值

- Tid



#### CreateTask

创建一个项目

请求值

- CommandUid 命令发出者的Uid

- Uid 被指定成为项目负责人的Uid

- Content 项目说明

- Title 项目名字

- Tid  项目所属的团队

返回值





