-- SQLite

CREATE TABLE EMPLOYEES(
    Id_employee INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    UserName VARCHAR (50) NOT NULL UNIQUE,
    Password VARCHAR (50) NOT NULL,
    Email VARCHAR (100) NOT NULL,
    Status INT DEFAULT 1,                      
    CreateTime TIMESTAMP DEFAULT (datetime('now', 'localtime')), 
    UpdateTime TIMESTAMP
);


CREATE TABLE ROLE(
    Id_role INTEGER PRIMARY KEY,
    Role_name VARCHAR (50) NOT NULL,
    Description TEXT,
    CreateTime TIMESTAMP DEFAULT (datetime('now', 'localtime')), 
    UpdateTime TIMESTAMP,
    Id_employee INTEGER,
    FOREIGN KEY (Id_employee) REFERENCES EMPLOYEES(Id_employee)
);

CREATE TABLE MENU(
    Id_menu INTEGER PRIMARY KEY,
    Menu_name VARCHAR (50) NOT NULL,
    Path VARCHAR (200),
    CreateTime TIMESTAMP DEFAULT (datetime ('now','localtime')),
    Id_employee INTEGER,
    FOREIGN KEY (Id_employee) REFERENCES EMPLOYEES(Id_employee)
);

--DROP TABLE MENU;

CREATE TABLE EMPLOYEE_ROLE(
    Id_employee INTEGER,
    Id_role INTEGER,
    PRIMARY KEY (Id_employee, Id_role), 
    FOREIGN KEY (Id_employee) REFERENCES EMPLOYEES(Id_employee),
    FOREIGN KEY (Id_role) REFERENCES ROLE(Id_role)
);

CREATE TABLE ROLE_MENU(
    Id_role INTEGER,
    Id_menu INTEGER,
    PRIMARY KEY (Id_role, Id_menu),
    FOREIGN KEY (Id_role) REFERENCES ROLE(Id_role),
    FOREIGN KEY (Id_menu) REFERENCES MENU(Id_menu)
);

-- I N S E R T   V A L U E S --

-- Employees
INSERT INTO EMPLOYEES( Name, UserName, Password, Email)
VALUES('TEST','Test','Test123','test@test.com');

INSERT INTO EMPLOYEES( Name, UserName, Password, Email)
VALUES('TEST1','Test1','Test456','test1@test1.com');

INSERT INTO EMPLOYEES( Name, UserName, Password, Email)
VALUES('Vanessa Nava','Vn_nv','vane123','test@test.com');

INSERT INTO EMPLOYEES( Name, UserName, Password, Email)
VALUES('Melissa Nava','Ml_nv','meli456','test@test.com');

-- Role
INSERT INTO ROLE(Id_role, Role_name, Description,Id_employee)
VALUES(1,'Admin','Administration',1);

INSERT INTO ROLE(Id_role, Role_name, Description,Id_employee)
VALUES(2,'General','General work',2);

INSERT INTO ROLE(Id_role,Role_name, Description,Id_employee)
VALUES(3,'Admin','Administration',3);

INSERT INTO ROLE(Id_role,Role_name, Description,Id_employee)
VALUES(4,'General','General work',4);

INSERT INTO ROLE(Id_role,Role_name, Description,Id_employee)
VALUES(5,'Admin','Administration',4);

INSERT INTO ROLE(Id_role,Role_name, Description,Id_employee)
VALUES(6,'General','General work',3);

-- Menu 
INSERT INTO MENU(Id_menu,Menu_name, Id_employee)
VALUES(1,'Admin Menu',1);

INSERT INTO MENU(Id_menu, Menu_name, Id_employee)
VALUES(2,'General Menu',2);

INSERT INTO MENU(Id_menu, Menu_name, Id_employee)
VALUES(3,'Admin Menu',3);

INSERT INTO MENU(Id_menu, Menu_name, Id_employee)
VALUES(4,'General Menu',4);

INSERT INTO MENU(Id_menu, Menu_name, Id_employee)
VALUES(5,'Admin Menu',4);

INSERT INTO MENU(Id_menu, Menu_name, Id_employee)
VALUES(6,'General Menu',3);


-- Employee Role
INSERT INTO EMPLOYEE_ROLE(Id_employee,Id_role)
VALUES (1,1);

INSERT INTO EMPLOYEE_ROLE(Id_employee,Id_role)
VALUES (2,2);

INSERT INTO EMPLOYEE_ROLE(Id_employee,Id_role)
VALUES (3,1);
UPDATE EMPLOYEE_ROLE SET Id_role=3 WHERE Id = 3;

INSERT INTO EMPLOYEE_ROLE(Id_employee,Id_role)
VALUES (4,4);

INSERT INTO EMPLOYEE_ROLE(Id_employee,Id_role)
VALUES (5,4);
UPDATE EMPLOYEE_ROLE SET Id_role=5 WHERE Id_employee = 4;
UPDATE EMPLOYEE_ROLE SET Id_employee=4 WHERE Id_role = 4;

INSERT INTO EMPLOYEE_ROLE(Id_employee,Id_role)
VALUES (3,6);

-- Role Menu
INSERT INTO ROLE_MENU(Id_role,Id_menu)
VALUES (1,1);

INSERT INTO ROLE_MENU(Id_role,Id_menu)
VALUES (2,2);

INSERT INTO ROLE_MENU(Id_role,Id_menu)
VALUES (3,3);

INSERT INTO ROLE_MENU(Id_role,Id_menu)
VALUES (4,4);

INSERT INTO ROLE_MENU(Id_role,Id_menu)
VALUES (5,5);

INSERT INTO ROLE_MENU(Id_role,Id_menu)
VALUES (6,6);

-- C O N S U L T A S --

--
SELECT DISTINCT m.Menu_name FROM EMPLOYEES e 
JOIN EMPLOYEE_ROLE er ON e.Id_employee = er.Id_employee
JOIN ROLE_MENU rm ON er.Id_role = rm.Id_role
JOIN MENU m ON rm.Id_menu = m.Id_menu
WHERE e.Id_employee = 1;

-- Nombre de todos los Administradores
SELECT DISTINCT e.Name FROM EMPLOYEES e
JOIN ROLE r ON e.Id_employee = r.Id_employee
WHERE r.Role_name = 'Admin';

--Todos los empleados que tienen acceso al Menu general
SELECT DISTINCT e.Id_employee,e.Name, r.Role_name FROM EMPLOYEES e
JOIN ROLE r ON e.Id_employee = r.Id_employee
JOIN MENU m ON r.Id_role = rm.Id_role
JOIN ROLE_MENU rm ON m.Id_menu = rm.Id_menu
WHERE m.Menu_name = 'General Menu';

