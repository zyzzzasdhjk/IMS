USE web;

INSERT INTO User(Username,Password) VALUE ('123456','3BswQFduU+x68TRa4lI4vDWV5vWGFyC7RpCLR3dn5Qw=|ZxdbfnSxuvS6zxF/Dt3pjg==');
Insert INTO UserInfo(uid, name, gender, birthday, description) VALUE (1,'张三','Female','1990-01-01','genshin impact open');
Insert INTO TeamInfo(name, people_max_num, status) VALUE ('原神',20,'Normal');
Insert INTO TeamMember(tid, uid, role) VALUE (1,1,'Creator');