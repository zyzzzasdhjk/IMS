use web;

-- 用户账号表，单独与用户信息
create table if not exists web.User 
(
    uid        int auto_increment primary key,
    username   varchar(20) unique not null,
    password   varchar(69) not null comment '密码', 
    email      varchar(50) not null comment '邮箱',
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    updated_at DATETIME   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    status     enum ('Normal', 'Banned', 'Deleted', 'UnConfirmed') default 'Normal' not null
);

-- 用户信息表
CREATE TABLE IF NOT EXISTS web.UserInfo (
    uid INT PRIMARY KEY,
    name VARCHAR(20) NOT NULL,
    gender ENUM('男', '女', '其他', '未知') NOT NULL DEFAULT '未知',
    birthday DATE,
    description TEXT,
    update_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    foreign key (uid) references User(uid)
);

CREATE TRIGGER InsertUserInfo AFTER INSERT ON web.User
FOR EACH ROW
BEGIN
    INSERT INTO UserInfo(uid, name) VALUE (NEW.uid, NEW.username);
END;

-- 团队信息表
CREATE TABLE IF NOT EXISTS TeamInfo(
    tid INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(20) NOT NULL ,
    description TEXT, -- 描述
    PeopleNumber int default 1, -- 人数,创建的时候只有创建者一个人
    JoinCode VARCHAR(9), -- 加入码
    status ENUM('Normal','Banned','Deleted') default 'Normal' , -- 团队的状态
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    updated_at DATETIME   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP
); 

-- 团队成员表
CREATE TABLE IF NOT EXISTS TeamMember(
    tid INT,
    uid INT,
    role ENUM('Creator','Member','Admin','Deleted') default 'Member', -- 团队成员的角色 创建者，成员，管理员，被删除
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    updated_at DATETIME   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    primary key(tid,uid),
    foreign key(tid) references TeamInfo(tid),
    foreign key (uid) references User(uid)
);

-- 团队角色视图
CREATE VIEW TeamMemberView AS
    SELECT T.tid, U.uid ,U.name, role, T.created_at, updated_at
    FROM TeamMember AS T LEFT JOIN UserInfo AS U ON T.uid = U.uid;

-- 用户团队视图
CREATE VIEW UserTeamsView AS
    SELECT TM.uid, TM.tid, TI.name,TI.description,TI.status,TI.PeopleNumber
    FROM TeamMember AS TM LEFT JOIN TeamInfo TI on TM.tid = TI.tid;

-- 用户创建团队
CREATE PROCEDURE UserCreateTeam(IN n varchar(20), IN d text,IN j varchar(9),IN u int,OUT msg INT)
BEGIN
    IF EXISTS(SELECT * FROM TeamInfo WHERE JoinCode = j) THEN -- 判断是否存在相同的加入码
        SET msg = 1; -- 代表加入码重复
    ELSE
        IF j = '' THEN
            INSERT INTO TeamInfo (name, description) VALUES (n, d);
        ELSE
            INSERT INTO TeamInfo (name, description, JoinCode) VALUES (n, d,j);
        END IF;
        INSERT INTO TeamMember (tid, uid, role) VALUES (LAST_INSERT_ID(), u, 'Creator');
        SET msg = 0; 
    END IF;
END;

-- 用户删除团队，需要鉴定权限
CREATE PROCEDURE UserDeleteTeam(IN u int,IN t int,OUT msg INT)
BEGIN
    IF EXISTS(SELECT * FROM TeamMember WHERE uid = u AND tid = t AND role = 'Creator') THEN
        UPDATE TeamInfo SET status = 'Deleted' WHERE tid = t;
        UPDATE TeamMember SET  role = 'Deleted' WHERE tid = t; -- 删除掉团队中全部用户的信息
        SET msg = 0;
    ELSE
        SET msg = 1;
    END IF;
END;

-- 用户查询团队成员
CREATE PROCEDURE UserGetTeamMembers(IN u int,IN t int,OUT msg INT)
BEGIN
    IF EXISTS(SELECT * FROM TeamMember WHERE uid = u AND tid = t) THEN
        SELECT role FROM TeamMember WHERE uid = u AND tid = t;
        SET msg = 0;
    ELSE
        SET msg = 1;
    END IF;
END;


-- 任务表
CREATE TABLE TaskInfo(
    taskId int primary key ,
    name varchar(20),
    description text,
    status enum('Incomplete','Complete','Timeout','Abandon') default 'Incomplete',
    -- proportion int, -- 分值，用于计算总的进度和当前进度
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    end_at DATETIME not null, -- 预计的完成时间,如果是null的话，就代表是没有结束时间限制
    updated_at DATETIME   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP
);

-- 团队-任务表
CREATE TABLE IF NOT EXISTS TeamTasks(
    tid int,
    taskId int,
    foreign key (tid) references TeamInfo(tid),
    foreign key (taskId) references TaskInfo(taskId)
);

-- 任务-子任务表
CREATE TABLE IF NOT EXISTS TaskSubtasks(
    taskId int,
    subtaskId int,
    foreign key (subtaskId) references TaskInfo(taskId),
    foreign key (taskId) references TaskInfo(taskId)
);

-- 任务-成员表
CREATE TABLE IF NOT EXISTS TaskMembers(
    taskId int,
    uid int,
    role enum('Admin','Member','Deleted') default 'Member',
    created_at DATETIME default CURRENT_TIMESTAMP null,
    updated_at DATETIME default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    primary key(taskId,uid),
    foreign key (taskId) references TaskInfo(taskId),
    foreign key (uid) references User(uid)
);

-- 创建团队 参数为名字，描述和结束时间
CREATE PROCEDURE CreateTask(IN ti int,IN n varchar(20),IN d text,IN t DATETIME)
BEGIN
    INSERT INTO TaskInfo (name, description, end_at) VALUES (n, d, t);
    INSERT INTO TeamTasks (tid, taskId) VALUES (ti, LAST_INSERT_ID());
END;

-- 删除团队 删除团队的时候删除全部的团队成员
CREATE TRIGGER DeleteTask BEFORE DELETE ON TeamInfo
FOR EACH ROW
BEGIN
    UPDATE TaskMembers SET TaskMembers.role = 'Deleted' WHERE tid = OLD.tid;
END;

