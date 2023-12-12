USE web;

INSERT INTO User(Username,Password) VALUE ('123456','3BswQFduU+x68TRa4lI4vDWV5vWGFyC7RpCLR3dn5Qw=|ZxdbfnSxuvS6zxF/Dt3pjg==');
Insert INTO UserInfo(uid, name, gender, birthday, description) VALUE (1,'张三','Female','1990-01-01','genshin impact open');
Insert INTO TeamInfo(name) VALUE ('Normal');
Insert INTO TeamMember(tid, uid, role) VALUE (1,1,'Creator');

INSERT INTO web.TaskInfo(web.taskinfo.name, web.taskinfo.description, web.taskinfo.status, web.taskinfo.end_at) value 
    ('测试任务','测试任务描述', 'Incomplete', '2024-01-01 00:00:00');
INSERT INTO web.TaskMembers(taskId, uid, role) value
    (1, 1, 'Admin');
INSERT INTO web.TaskMembers(taskId, uid, role) value
    (1, 2, 'Member');