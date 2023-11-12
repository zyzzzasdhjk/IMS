use web;

-- 用户账号表，单独与用户信息
create table if not exists User 
(
    uid        int auto_increment primary key,
    username   varchar(20) unique not null,
    password   varchar(69) not null,
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    updated_at DATETIME   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    status     enum ('Normal', 'Banned', 'Deleted', 'UnConfirmed') default 'Normal' not null
);

-- 用户信息表
CREATE TABLE IF NOT EXISTS UserInfo (
    uid INT PRIMARY KEY,
    name VARCHAR(20) NOT NULL,
    gender ENUM('Male', 'Female', 'Other', 'Unknown') NOT NULL,
    birthday DATE,
    description TEXT,
    email VARCHAR(20) not null comment '用户的邮箱',
    update_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    foreign key (uid) references User(uid)
);

CREATE TRIGGER InsertUserInfo AFTER INSERT ON User
FOR EACH ROW
BEGIN
    INSERT INTO UserInfo(uid, name, gender) VALUE (NEW.uid, NEW.username, 'Unknown');
END ;

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

-- 任务表
CREATE TABLE TaskInfo(
    taskId int primary key ,
    name varchar(20),
    description text,
    status enum('Incomplete','Complete','Timeout','Abandon'),
    proportion int, -- 分值，用于计算总的进度和当前进度
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    end_at DATETIME, -- 预计的完成时间
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
)