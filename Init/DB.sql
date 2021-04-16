/*CREATE DATABASE poscfg; 
GO
USE poscfg;
GO

CREATE LOGIN adminposcfg WITH PASSWORD = 'Reso2020!!';
CREATE USER adminposcfg for login adminposcfg;
GO
exec sp_addrolemember 'db_owner', 'adminposcfg';*/

CREATE Table [Terminals]
(
[ID] int IDENTITY(1,1),
[TerminalID] varchar(20) NOT NULL UNIQUE,
[SerialNumber] varchar(50),
[Suspend] varchar(5),
ParmConnectTime int,
Custom1 varchar(30),
Custom2 varchar(30),
Custom3 varchar(30),
Custom4 varchar(30),
Custom5 varchar(30),
Custom6 varchar(30),
Custom7 varchar(30),
Custom8 varchar(30),
Custom9 varchar(30),
Custom10 varchar(30),
Custom11 varchar(30),
Custom12 varchar(30),
Custom13 varchar(30),
Custom14 varchar(30),
Custom15 varchar(30),
Custom16 varchar(30),
Custom17 varchar(30),
Custom18 varchar(30),
Custom19 varchar(30),
Custom20 varchar(30),
TranConnectTime DateTIME
)
GO

--Insertar Datos de ejemplo


INSERT INTO TERMINALS 
(TerminalID,SerialNumber,Suspend,ParmConnectTime,Custom1,Custom2,Custom3,
Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10,Custom11,Custom12,Custom13
,Custom14,Custom15,Custom16,Custom17,Custom18,Custom19,Custom20,TranConnectTime)
values('30404537','17296CT26874812','0',1637,'EL Perro','Juan 888',
'RUT: 030211650015','30404537|02066300001','30404537|20825629','30404537|99999999',
'30404537|44399','30404537|0','30404537|0','30404537|0','30404537|0','30404537|0',
'30404537|0','30404537|0','30404537|0','','','','','30404537|0',getdate()),
('3040453123','17296CT26874812','0',1637,'EL Maragato','Juan 88831',
'RUT: 030211650015','30404537|02066300001','30404537|20825629','30404537|99999999',
'30404537|44399','30404537|0','30404537|0','30404537|0','30404537|0','30404537|0',
'30404537|0','30404537|0','30404537|0','','','','','30404537|0',getdate()),
('3040412','17296CT26874812','0',1637,'EL Rochense','Pedro 8890',
'RUT: 030211650015','30404537|02066300001','30404537|20825629','30404537|99999999',
'30404537|44399','30404537|0','30404537|0','30404537|0','30404537|0','30404537|0',
'30404537|0','30404537|0','30404537|0','','','','','30404537|0',getdate()),
('30404133','17296CT26874812','0',1637,'EL Pajaro','Miguel 9090',
'RUT: 030211650015','30404537|02066300001','30404537|20825629','30404537|99999999',
'30404537|44399','30404537|0','30404537|0','30404537|0','30404537|0','30404537|0',
'30404537|0','30404537|0','30404537|0','','','','','30404537|0',getdate())


-- TABLA SYSTEM

CREATE Table [System]
(
[ID] int IDENTITY(1,1),
[TerminalID] varchar(20),
[TerminalChecksum] int,
[ControlGroup] int,
[ControlChecksum] int,
ParameterGroup int,
ParameterReload int,
ParameterVersion int,
ProgramID int,
ProgramReload int,
ProgramVersion int,
Paquete varchar(30),
ConnectGroup int,
ParmConnChecksum int,
TranConnChecksum1 int,
TranConnChecksum2 int
)
GO

--Insertar Datos en table System de ejemplo

INSERT INTO SYSTEMPOSs
(TerminalID,TerminalChecksum,ControlGroup,ControlChecksum,ParameterGroup,ParameterReload,ParameterVersion,
ProgramID,ProgramReload,ProgramVersion,Paquete,ConnectGroup,ParmConnChecksum,TranConnChecksum1,TranConnChecksum2)
values('30404537',0,41,1,1,1,1,1,121,0,'P20180524+PPA2',15000101,0,0,0)


-- Tabla TerminalStatus

CREATE TABLE poscfg.dbo.TerminalsStatus (
	TerminalID nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Status int NULL,
	ID int IDENTITY(1,1) NOT NULL,
	CONSTRAINT TerminalsStatus_PK PRIMARY KEY (ID)
) GO;


-- poscfg.dbo.TerminalsStatus foreign keys

ALTER TABLE poscfg.dbo.TerminalsStatus ADD CONSTRAINT TerminalsStatus_FK FOREIGN KEY (TerminalID) REFERENCES poscfg.dbo.Terminals(TerminalID) GO;

--DATOS
INSERT INTO poscfg.dbo.TerminalsStatus (TerminalID,Status) VALUES 
('12',1)
,('16',1)
,('111',1)
,('3',1)
,('475747546474',1)
,('55555',1)
,('44444',1)
,('33333',1)
,('2211',1)
,('2222',1)
;
INSERT INTO poscfg.dbo.TerminalsStatus (TerminalID,Status) VALUES 
('1111',1)
;


-- JPOS
/*
CREATE DATABASE jpos_db;


CREATE TABLE jpos_db.sysconfig (
	id varchar(64) NOT NULL,
	value varchar(128) NOT NULL,
	readPerm varchar(64),
	writePerm varchar(64)

)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_0900_ai_ci;

*/

-- jpos_db.sysconfig definition

INSERT INTO jpos_db.sysconfig (id,value,readPerm,writePerm) VALUES 
('12','Handy',NULL,NULL)
,('52','handy',NULL,NULL)
,('55','handy',NULL,NULL)
,('ptf.57','handy',NULL,NULL)
,('ca.57','handy',NULL,NULL)
,('ptf.56','handy',NULL,NULL)
,('ca.56','handy',NULL,NULL)
,('ptf.52','handy',NULL,NULL)
,('ptf.53','handy',NULL,NULL)
,('ptf.54','handy',NULL,NULL)
;
INSERT INTO jpos_db.sysconfig (id,value,readPerm,writePerm) VALUES 
('ca.54','handy',NULL,NULL)
,('pft.54','handy',NULL,NULL)
,('pft.57','handy',NULL,NULL)
,('ca.12','chau',NULL,NULL)
,('pft.52','HANDY',NULL,NULL)
,('ca.52','HANDY',NULL,NULL)
,('pft.16','chau',NULL,NULL)
,('ca.16','chau',NULL,NULL)
,('pft.12','chau',NULL,NULL)
;

/*
CREAR TABLA CARGAMASIVAAUX
*/
CREATE TABLE poscfg.dbo.CargaMasivaAux (
	id int IDENTITY(0,1) NOT NULL,
	TerminalID nvarchar NULL,
	SerialNumber nvarchar NULL,
	Custom1 nvarchar NULL,
	Custom2 nvarchar NULL,
	Custom3 nvarchar NULL,
	Custom4 nvarchar NULL,
	Custom5 nvarchar NULL,
	Custom6 nvarchar NULL,
	Custom7 nvarchar NULL,
	Custom8 nvarchar NULL,
	Custom9 nvarchar NULL,
	Custom10 nvarchar NULL,
	Custom11 nvarchar NULL,
	Custom12 nvarchar NULL,
	Custom13 nvarchar NULL,
	Custom14 nvarchar NULL,
	Custom15 nvarchar NULL,
	Custom16 nvarchar NULL,
	Custom19 nvarchar NULL,
	TerminalChecksum int NOT NULL,
	ControlGroup int NOT NULL,
	ControlCheckSum int NOT NULL,
	ParameterGroup int NOT NULL,
	ParameterReload int NOT NULL,
	ParameterVersion int NOT NULL,
	ProgramID int NOT NULL,
	ProgramReload int NOT NULL,
	ProgramVersion int NOT NULL,
	Paquete nvarchar(100) NULL,
	ConnectGroup int NOT NULL,
	ParmConnChecksum int NOT NULL,
	id_jpos nvarchar(100) NOT NULL,
	value nvarchar(100) NOT NULL,
	CONSTRAINT CargaMasivaAux_PK PRIMARY KEY (id)
) GO
