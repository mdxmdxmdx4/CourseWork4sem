CREATE OR REPLACE PROCEDURE CANCEL_FLIGHT (
    p_flight_num NUMBER
) AS
    v_status NUMBER;
BEGIN
    -- Получаем статус рейса
    SELECT status INTO v_status FROM flight WHERE flight_number = p_flight_num;
    -- Проверяем статус рейса и выбрасываем исключение при несоответствии
    IF v_status <> 1 THEN
        RAISE_APPLICATION_ERROR(-20009, 'Flight is not on "scheduled" status, it cannot be cancelled');
    END IF;
    -- Обновляем статус рейса и билетов
    UPDATE flight SET status = 2 WHERE flight_number = p_flight_num;
    UPDATE ticket SET status_num = 101 WHERE flight_number = p_flight_num AND status_num <> 101;
    
    COMMIT;
END;



----------------------------------------------------------------------------
CREATE OR REPLACE PROCEDURE CHECK_FLIGHTS AS
BEGIN
    UPDATE Flight
    SET status = 
        CASE 
            WHEN departure_datetime > SYSDATE AND SYSDATE < arrive_datetime THEN 5 -- "Executing"
            WHEN SYSDATE >= arrive_datetime THEN 3 -- "Completed"
        END
    WHERE status = 1; -- check only for scheduled flights
END;


-----------------------------------------------------------------------
create or replace procedure ADD_PLANE(
p_plane_model nvarchar2,
p_pacles_capacity number,
max_luggage_weigth number
)
as
begin
insert into Plane(PLANE_MODEL, PLACES_CAPACITY, MAX_LUGGAGE_WEIGTH)
values (p_plane_model,p_pacles_capacity,max_luggage_weigth);
commit;
end;




---------------------------------------------------------------
create or replace procedure ADD_EMPLOYEE(
p_empl_fname nvarchar2,
p_empl_position number,
p_salary number,
p_start_work_date date
)
as
begin
insert into Personel(employee_fname,employee_position, employee_mnth_salary,employee_start_work) 
values(p_empl_fname,p_empl_position,p_salary,p_start_work_date);
commit;
end;

--------------------------------------------------------------
CREATE OR REPLACE PROCEDURE REMOVE_EMPLOYEE(
  p_empl_id NUMBER
) AS
  v_exists NUMBER;
BEGIN
  SELECT COUNT(*) INTO v_exists FROM Personel WHERE employee_id = p_empl_id;

  IF v_exists > 0 THEN
    DELETE FROM Personel WHERE employee_id = p_empl_id;
    COMMIT;
  ELSE
    RAISE_APPLICATION_ERROR(-20001, 'No such employee');
  END IF;
END;


-----------------------------------------------------------
create or replace procedure ADD_AIRPORT(
p_airport_name nvarchar2,
p_location_country nvarchar2,
p_location_city nvarchar2
)
as
begin
insert into AIRPORT(airport_name, location_country, location_city)
values(p_airport_name,p_location_country,p_location_city);
commit;
end;


----------------------------------------------------
create or replace procedure ADD_FLIGHT(
p_plane_id number,
p_dep_airport_id number,
p_dest_airport_id number,
p_dep_datetime date,
p_dest_datetime date
)
as
av_seats NUMBER;
BEGIN
select places_capacity 
into av_seats
from plane
where plane_id = p_plane_id;
insert into Flight(plane_id, departure_airport_id, destination_airport_id, departure_datetime, arrive_datetime, available_seats,status)
values(p_plane_id, p_dep_airport_id, p_dest_airport_id, p_dep_datetime, p_dest_datetime, av_seats, 1);
commit;
END;




-------------------------------------------------------------------------
create or replace procedure ADD_CUSTOMER(
p_cust_fullname nvarchar2,
p_birth_date date,
p_passport_series nvarchar2
)
as
begin
insert into customer(full_name, date_of_birth, passport_series)
values(p_cust_fullname, p_birth_date, p_passport_series);
commit;
end;




--------------------------------------------------------------------------
CREATE OR REPLACE PROCEDURE IMPORT_TABLES_XML AS
  v_file UTL_FILE.FILE_TYPE;
  v_line VARCHAR2(32766);
  v_airport_name nvarchar2(40);
  v_location_country nvarchar2(40);
  v_location_city nvarchar2(40);
BEGIN
  -- Открываем файл для чтения
  v_file := UTL_FILE.FOPEN('DATA_DIR2', 'exported_data.xml', 'r', 32766);

  -- Читаем строки из файла
  LOOP
    BEGIN
      UTL_FILE.GET_LINE(v_file, v_line);
      IF v_line = '<Airport>' THEN
        -- Начало записи аэропорта
        v_airport_name := NULL;
        v_location_country := NULL;
        v_location_city := NULL;
      ELSIF v_line LIKE '<name>%' THEN
        -- Чтение названия аэропорта
        v_airport_name := SUBSTR(v_line, 7, LENGTH(v_line) - 13);
      ELSIF v_line LIKE '<country>%' THEN
        -- Чтение страны аэропорта
        v_location_country := SUBSTR(v_line, 10, LENGTH(v_line) - 19);
      ELSIF v_line LIKE '<city>%' THEN
        -- Чтение города аэропорта
        v_location_city := SUBSTR(v_line, 7, LENGTH(v_line) - 13);
      ELSIF v_line = '</Airport>' THEN
        -- Конец записи аэропорта, сохраняем в базу данных
        INSERT INTO Airport (airport_name, location_country, location_city)
        VALUES (v_airport_name, v_location_country, v_location_city);
      END IF;
    EXCEPTION
      WHEN NO_DATA_FOUND THEN
        -- Конец файла, выходим из цикла
        EXIT;
    END;
  END LOOP;

  UTL_FILE.FCLOSE(v_file);
END;



-------------------------------------------------------
CREATE OR REPLACE PROCEDURE EXPORT_AIRPORT_XML
AS 
  v_clob CLOB;
  v_file UTL_FILE.FILE_TYPE;
BEGIN
  SELECT DBMS_XMLGEN.GETXML('SELECT * FROM AIRPORT') INTO v_clob FROM DUAL;
  v_file := UTL_FILE.FOPEN('DATA_DIR2', 'AIRPORT.xml', 'w');
  UTL_FILE.PUT(v_file, v_clob);
  UTL_FILE.FCLOSE(v_file);
END;




---------------------------------------------------------------
CREATE OR REPLACE PROCEDURE insert_flights AS
  v_plane_id NUMBER;
  v_dep_airport_id NUMBER;
  v_dest_airport_id NUMBER;
  v_dep_datetime DATE;
  v_arr_datetime DATE;
  v_available_seats NUMBER;
  v_status_id NUMBER := 1;
BEGIN
  FOR dep_airport IN (SELECT airport_id, location_city FROM airport) LOOP
    FOR dest_airport IN (SELECT airport_id, location_city FROM airport) LOOP
      IF dep_airport.location_city != dest_airport.location_city THEN
        SELECT plane_id INTO v_plane_id FROM plane ORDER BY DBMS_RANDOM.VALUE FETCH FIRST 1 ROWS ONLY;
        v_dep_airport_id := dep_airport.airport_id;
        v_dest_airport_id := dest_airport.airport_id;
        v_dep_datetime := TO_DATE('10.08.2027 00:00:00', 'DD.MM.YYYY HH24:MI:SS') + DBMS_RANDOM.VALUE(0, 90);
        v_arr_datetime := v_dep_datetime + NUMTODSINTERVAL(DBMS_RANDOM.VALUE(180, 960), 'MINUTE');
        SELECT places_capacity into v_available_seats FROM plane WHERE plane_id = v_plane_id;
INSERT INTO flight(plane_id, departure_airport_id, destination_airport_id, departure_datetime, arrive_datetime, available_seats, status)
VALUES(v_plane_id, v_dep_airport_id, v_dest_airport_id, TO_DATE(v_dep_datetime, 'DD.MM.YYYY HH24:MI:SS'), 
TO_DATE(v_arr_datetime, 'DD.MM.YYYY HH24:MI:SS'), v_available_seats, v_status_id);
      END IF;
    END LOOP;
  END LOOP;
  COMMIT;
END;



------------------------------------------------------------
CREATE OR REPLACE PROCEDURE CheckUserByPassportSeries(
    pPassportSeries IN NVARCHAR2,
    pResult OUT NUMBER
)
AS
    vUserCount NUMBER;
BEGIN
    -- Проверяем, есть ли пользователь с указанной серией паспорта
    SELECT COUNT(*) INTO vUserCount
    FROM Customer
    WHERE passport_series = pPassportSeries;
    IF vUserCount > 0 THEN
        -- Пользователь с указанной серией паспорта существует
        -- Можно продолжить проверку пароля или выполнить другие действия
        pResult := 1;
    ELSE
        -- Пользователь с указанной серией паспорта не существует
        pResult := 0;
    END IF;
END;





----------------------------------------------------------------
CREATE OR REPLACE PROCEDURE GetUserInfoByPassportSeries(
    pPassportSeries IN NVARCHAR2,
    pCustomerId OUT NUMBER,
    pFullName OUT NVARCHAR2
)
AS
BEGIN
    SELECT customer_id, full_name
    INTO pCustomerId, pFullName
    FROM Customer
    WHERE passport_series = pPassportSeries;
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        pCustomerId := NULL;
        pFullName := NULL;
END;

DECLARE
    vCustomerId NUMBER;
    vFullName NVARCHAR2(100);
BEGIN
    GetUserInfoByPassportSeries('AB7658412', vCustomerId, vFullName);

    -- Используйте vCustomerId и vFullName для дальнейших действий
    IF vCustomerId IS NOT NULL THEN
        DBMS_OUTPUT.PUT_LINE('User ID: ' || vCustomerId);
        DBMS_OUTPUT.PUT_LINE('User full name: ' || vFullName);
    ELSE
        DBMS_OUTPUT.PUT_LINE('User not found');
    END IF;
END;


------------
--SHOWING---
------------

CREATE OR REPLACE PROCEDURE SHOW_PEROSNEL(
p_personel_list OUT SYS_REFCURSOR
)
AS
BEGIN
open p_personel_list for 
select pr.employee_id, pr.employee_fname, ps.position_title, pr.employee_mnth_salary, employee_start_work
from system.Personel pr inner join system.Position ps on pr.employee_position = ps.position_id ;
END;

--2
CREATE OR REPLACE PROCEDURE SHOW_PLANE(
p_plane_list OUT SYS_REFCURSOR
)
AS
BEGIN
open p_plane_list for 
select * from Plane;
END;

--3
CREATE OR REPLACE PROCEDURE SHOW_CUSTOMER(
p_customer_list OUT SYS_REFCURSOR
)
AS
BEGIN
open p_customer_list for 
select * from Customer;
END;

--4
CREATE OR REPLACE PROCEDURE SHOW_COMPLETED_FLIGHTS(
p_compl_flight_list OUT SYS_REFCURSOR
)
AS
BEGIN
open p_compl_flight_list for 
select f.flight_number, plane.plane_model, a1.location_city as "DEPARTURE_CITY", a1.airport_name AS "DEPARTURE_AIRPORT" ,
a2.location_city as "DESTINATION_CITY", a2.airport_name as "DESTINATION_AIRPORT",
TO_CHAR(f.departure_datetime, 'DD.MM.YY HH24:MI:SS') AS departure_datetime, 
TO_CHAR(f.arrive_datetime, 'DD.MM.YY HH24:MI:SS') AS arrive_datetime,
f.occupied_places_num, s.status_description
from completed_flights f join Plane on plane.plane_id = f.plane_id
join Airport a1 on f.departure_airport_id = a1.airport_id
join Airport a2 on f.destination_airport_id = a2.airport_id
join Status s on f.status = s.status_id;
END;


--5
CREATE OR REPLACE PROCEDURE SHOW_NEAREST_FLIGHTS(
p_nearest_flights_list OUT SYS_REFCURSOR
)
AS
BEGIN
open p_nearest_flights_list for 
select f.flight_number, plane.plane_model, a1.location_city as "DEPARTURE_CITY", a1.airport_name AS "DEPARTURE_AIRPORT" ,
a2.location_city as "DESTINATION_CITY", a2.airport_name as "DESTINATION_AIRPORT",
TO_CHAR(f.departure_datetime, 'DD.MM.YY HH24:MI:SS') AS departure_datetime, 
TO_CHAR(f.arrive_datetime, 'DD.MM.YY HH24:MI:SS') AS arrive_datetime,
f.available_seats, s.status_description
from Flight f join Plane on plane.plane_id = f.plane_id
join Airport a1 on f.departure_airport_id = a1.airport_id
join Airport a2 on f.destination_airport_id = a2.airport_id
join Status s on f.status = s.status_id
order by departure_datetime asc
fetch first 20 rows only;
END;

--6
CREATE OR REPLACE PROCEDURE SHOW_AIRPORTS(
p_airports_list OUT SYS_REFCURSOR
)
AS
BEGIN
open p_airports_list for 
select * from airport;
END;

