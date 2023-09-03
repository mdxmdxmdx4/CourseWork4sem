CREATE OR REPLACE TRIGGER update_available_seats
BEFORE INSERT ON Ticket
FOR EACH ROW
DECLARE
  v_available_seats number;
BEGIN
  SELECT available_seats
  INTO v_available_seats
  FROM Flight
  WHERE flight_number = :NEW.flight_number;
  
  IF v_available_seats <= 0 THEN
    RAISE_APPLICATION_ERROR(-20001, 'No available seats on this flight.');
  END IF;
  UPDATE Flight
  SET available_seats = (SELECT places_capacity FROM Plane WHERE plane_id = 
  (SELECT plane_id FROM Flight WHERE flight_number = :NEW.flight_number)) - 
  (SELECT COUNT(*) FROM Ticket WHERE flight_number = :NEW.flight_number and status_num = 100) - 1
  WHERE flight_number = :NEW.flight_number;
END;    



CREATE OR REPLACE TRIGGER increase_available_seats
AFTER UPDATE OF status_num ON Ticket
FOR EACH ROW
BEGIN
  IF :NEW.status_num = 101 AND :OLD.status_num != 101 THEN -- добавляем проверку на изменение статуса
    UPDATE Flight
    SET available_seats = LEAST(available_seats + 1, 
                                 (SELECT places_capacity FROM Plane WHERE plane_id = 
                                  (SELECT plane_id FROM Flight WHERE flight_number = :NEW.flight_number)))
    WHERE flight_number = :NEW.flight_number;
  END IF;
END;




CREATE OR REPLACE TRIGGER lug_weigth_check
BEFORE INSERT ON Ticket
FOR EACH ROW
DECLARE
v_total_luggage_weight NUMBER;
v_max_luggage_weight NUMBER;
v_plane_id NUMBER;
BEGIN

SELECT NVL(SUM(l.weigth), 0)
INTO v_total_luggage_weight
FROM Ticket t JOIN Luggage l ON t.luggage_id = l.luggage_id
WHERE t.flight_number = :NEW.flight_number and t.status_num <> 101;

dbms_output.put_line('Luggage weighs ' || TO_CHAR(v_total_luggage_weight));

SELECT max_luggage_weigth, plane_id
INTO v_max_luggage_weight, v_plane_id
FROM Plane
WHERE plane_id = (SELECT plane_id FROM Flight WHERE flight_number = :NEW.flight_number);

IF v_total_luggage_weight + NVL(:NEW.luggage_id, 0) > v_max_luggage_weight THEN
RAISE_APPLICATION_ERROR(-20002, 'Total luggage weight is exceeding the limit.');
END IF;
end;






create or replace trigger Flight_status_check
after update of status on Flight
for each row
declare
  v_total_weight NUMBER;
  oc_seats NUMBER;
begin
SELECT COUNT(*)
into oc_seats
FROM Ticket
WHERE flight_number = :new.flight_number and status_num = 100;
  if :new.status in (2, 3) then
   SELECT SUM(l.weigth)
   into v_total_weight
FROM Luggage l
JOIN Ticket t ON l.luggage_id = t.luggage_id
WHERE t.flight_number = :new.flight_number;
 dbms_output.put_line(oc_seats);
  dbms_output.put_line(v_total_weight);
    insert into Completed_flights(flight_number, plane_id, departure_airport_id, destination_airport_id, 
                                   departure_datetime, arrive_datetime, occupied_places_num, result_luggage_weigth, status)
  values (:new.flight_number, :new.plane_id, :new.departure_airport_id, :new.destination_airport_id, 
           :new.departure_datetime, :new.arrive_datetime, oc_seats, v_total_weight, :new.status);
  end if;
end;

update Flight set status = 2 where flight_number = 87694;
select * from Completed_flights;
delete from Completed_flights;