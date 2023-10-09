use web;

-- 用户账号表，单独与用户信息
create table if not exists User 
(
    uid        int auto_increment primary key,
    username   varchar(20) unique not null,
    password   varchar(69) not null,
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    updated_at DATETIME   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    status     enum ('Normal', 'Banned', 'Deleted') default 'Normal' not null
);

-- 用户信息表
CREATE TABLE IF NOT EXISTS UserInfo (
    uid INT PRIMARY KEY,
    name VARCHAR(20) NOT NULL,
    gender ENUM('Male', 'Female', 'Other') NOT NULL,
    birthday DATE,
    description TEXT,
    update_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    foreign key (uid) references User(uid)
);

-- 团队信息表
CREATE TABLE IF NOT EXISTS TeamInfo(
    tid INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(20) NOT NULL ,
    people_max_num TINYINT,  -- 团队最大人数上限
    status ENUM('Normal','Banned','Deleted'), -- 团队的状态
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    updated_at DATETIME   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP
); 

-- 团队成员表
CREATE TABLE IF NOT EXISTS TeamMember(
    tid INT,
    uid INT,
    role ENUM('Creator','Member','Admin','Deleted'), -- 团队成员的角色 创建者，成员，管理员，被删除
    created_at DATETIME   default CURRENT_TIMESTAMP null,
    updated_at DATETIME   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    primary key(tid,uid),
    foreign key(tid) references TeamInfo(tid),
    foreign key (uid) references User(uid)
);

-- 团队角色视图
CREATE VIEW TeamMemberView AS
    SELECT T.tid, U.name, role, T.created_at, updated_at
    FROM TeamMember AS T LEFT JOIN UserInfo AS U ON T.uid = U.uid;