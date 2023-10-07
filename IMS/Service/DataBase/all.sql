use web;

-- 用户账号表，单独与用户信息
create table if not exists user 
(
    uid        int auto_increment primary key,
    username   varchar(20) unique not null,
    password   varchar(64) not null,
    created_at timestamp   default CURRENT_TIMESTAMP null,
    updated_at timestamp   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    status     enum ('Normal', 'Banned', 'Deleted') default 'Normal' not null
);

-- 用户信息表
CREATE TABLE IF NOT EXISTS userInfo (
    uid INT PRIMARY KEY,
    name VARCHAR(20) NOT NULL,
    gender ENUM('Male', 'Female', 'Other') NOT NULL,
    birthday DATE,
    description TEXT,
    update_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    foreign key (uid) references user(uid)
);

-- 团队信息表
CREATE TABLE IF NOT EXISTS team_info(
    tid INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(20) NOT NULL ,
    people_max_num TINYINT,  -- 团队最大人数上限
    status ENUM('Normal','Banned','Deleted'), -- 团队的状态
    created_at timestamp   default CURRENT_TIMESTAMP null,
    updated_at timestamp   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP
); 

-- 团队成员表
CREATE TABLE IF NOT EXISTS team_member(
    tid INT,
    uid INT,
    
    foreign key(tid) references team_info(tid),
    foreign key (uid) references user(uid)
);