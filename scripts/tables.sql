create table Airport(
airport_id number GENERATED BY DEFAULT AS IDENTITY (START WITH 100 INCREMENT BY 1) primary key,
airport_name nvarchar2(60) not null,
location_country nvarchar2(40) not null,
location_city nvarchar2(40) not null
);

create table Plane(
plane_id number GENERATED BY DEFAULT AS IDENTITY (START WITH 1000 INCREMENT BY 1) primary key,
plane_model nvarchar2(20) not null,
places_capacity number(3) not null,
max_luggage_weigth number(4) not null
);

create table Position(
position_id number generated by default as identity(start with 1 maxvalue 100 increment by 1) primary key,
position_title nvarchar2(50) not null
);


create table Personel(
employee_id number generated by default as identity(start with 1 increment by 1) primary key,
employee_fname nvarchar2(50) not null,
employee_position number not null,
employee_mnth_salary number(10,2) not null,
employee_start_work date not null,
constraint fk_personel_position
foreign key (employee_position)
references Position (position_id)
);

select * from Personel;

create table Status(
status_id number generated by default as identity(start with 1 increment by 1) primary key,
    status_description nvarchar2(30)
);


create table Flight(
flight_number number GENERATED BY DEFAULT AS IDENTITY (START WITH 100 INCREMENT BY 1) primary key,
plane_id number references Plane(plane_id),
departure_airport_id number references Airport(airport_id) not null,
destination_airport_id number references Airport(airport_id) not null,
departure_datetime date not null,
arrive_datetime date not null,
available_seats number(3) not null,
status number references Status(status_id) not null 
);


create table Completed_flights(
flight_number number primary key,
plane_id number references Plane(plane_id),
departure_airport_id number references Airport(airport_id) not null,
destination_airport_id number references Airport(airport_id) not null,
departure_datetime date not null,
arrive_datetime date not null,
occupied_places_num number(3) not null,
result_luggage_weigth number(4) not null,
status references Status(status_id) not null 
);

create table Luggage(
luggage_id number GENERATED BY DEFAULT AS IDENTITY (START WITH 100 INCREMENT BY 1) primary key,
weigth number(3) not null
);
create table Ticket(
ticket_num number GENERATED BY DEFAULT AS IDENTITY (START WITH 100 INCREMENT BY 1) primary key,
flight_number number references Flight(flight_number) not null,
place_number number(3) not null,
price number(7,2) not null,
status_num number references Ticket_status(status_num) not null,
customer_id number references Customer(customer_id) not null,
luggage_id number references Luggage(luggage_id)
);


create table Ticket_status(
status_num number GENERATED BY DEFAULT AS IDENTITY (START WITH 100 INCREMENT BY 1) primary key,
status_description nvarchar2(20) not null
)


CREATE TABLE Customer ( 
  customer_id NUMBER GENERATED BY DEFAULT AS IDENTITY(START WITH 100 INCREMENT BY 1) PRIMARY KEY,
  full_name NVARCHAR2(100) NOT NULL,
  date_of_birth DATE NOT NULL,
  passport_series NVARCHAR2(10) not null unique
);