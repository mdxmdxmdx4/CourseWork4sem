CREATE PROFILE CLIENT_PROFILE limit
password_life_time unlimited
sessions_per_user 1000
FAILED_LOGIN_ATTEMPTS 5
PASSWORD_LOCK_TIME 1
PASSWORD_REUSE_TIME 10
PASSWORD_GRACE_TIME default
connect_time 180
idle_time 35;

create role CLIENT_ROLE;
grant create session to CLIENT_ROLE;



create user MDAIR_CLIENT identified by 7777777
profile CLIENT_PROFILE
account unlock;




CREATE PROFILE MANAGER_PROFILE limit
password_life_time 90
sessions_per_user 3
FAILED_LOGIN_ATTEMPTS 3
PASSWORD_LOCK_TIME 2
PASSWORD_REUSE_TIME 10
PASSWORD_GRACE_TIME default
connect_time 240
idle_time 45;


create role MANAGER_ROLE;
grant create session to MANAGER_ROLE;

create user MDAIR_MANAGER identified by qazwsx909
PROFILE MANAGER_PROFILE
account unlock;