delete
from web.userinfo
where web.userinfo.name = 123;
delete
from web.user
where web.user.username = 123;

insert into teaminfo(name, description, JoinCode) VALUE ('team1', 'team1', '123456789');
insert into teammember(tid, uid, role) VALUE (1, 47, 'Creator');