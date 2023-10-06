create table users
(
    uid        int auto_increment primary key,
    username   varchar(20) unique not null,
    password   varchar(64) not null,
    created_at timestamp   default CURRENT_TIMESTAMP null,
    updated_at timestamp   default CURRENT_TIMESTAMP null on update CURRENT_TIMESTAMP,
    status     enum ('Normal', 'Banned', 'Deleted') default 'Normal'          not null
);