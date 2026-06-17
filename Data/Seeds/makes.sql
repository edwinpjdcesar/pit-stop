--	Check for pre-existing values and reset auto incrementing Id fields.
IF (OBJECT_ID(N'dbo.Make') IS NOT NULL AND IDENT_CURRENT('Make.Id') IS NOT NULL)
BEGIN
	DELETE FROM Make
	DBCC CHECKIDENT (Make, RESEED, 0)
END

DECLARE @tempMake TABLE (
	Code NVARCHAR(256),
	Description NVARCHAR(256)
);

INSERT INTO @tempMake (Code, Description) VALUES
('ACURA', 'Acura'),
('ALFA', 'Alfa Romeo'),
('AMC', 'AMC'),
('ASTON', 'Aston Martin'),
('AUDI', 'Audi'),
('AVANTI', 'Avanti'),
('BENTL', 'Bentley'),
('BMW', 'BMW'),
('BUICK', 'Buick'),
('CAD', 'Cadillac'),
('CHEV', 'Chevrolet'),
('CHRY', 'Chrysler'),
('DAEW', 'Daewoo'),
('DAIHAT', 'Daihatsu'),
('DATSUN', 'Datsun'),
('DELOREAN', 'DeLorean'),
('DODGE', 'Dodge'),
('EAGLE', 'Eagle'),
('FER', 'Ferrari'),
('FIAT', 'FIAT'),
('FISK', 'Fisker'),
('FORD', 'Ford'),
('FREIGHT', 'Freightliner'),
('GEO', 'Geo'),
('GMC', 'GMC'),
('HONDA', 'Honda'),
('AMGEN', 'HUMMER'),
('HYUND', 'Hyundai'),
('INFIN', 'Infiniti'),
('ISU', 'Isuzu'),
('JAG', 'Jaguar'),
('JEEP', 'Jeep'),
('KIA', 'Kia'),
('LAM', 'Lamborghini'),
('LAN', 'Lancia'),
('ROV', 'Land Rover'),
('LEXUS', 'Lexus'),
('LINC', 'Lincoln'),
('LOTUS', 'Lotus'),
('MAS', 'Maserati'),
('MAYBACH', 'Maybach'),
('MAZDA', 'Mazda'),
('MCLAREN', 'McLaren'),
('MB', 'Mercedes-Benz'),
('MERC', 'Mercury'),
('MERKUR', 'Merkur'),
('MINI', 'MINI'),
('MIT', 'Mitsubishi'),
('NISSAN', 'Nissan'),
('OLDS', 'Oldsmobile'),
('PEUG', 'Peugeot'),
('PLYM', 'Plymouth'),
('PONT', 'Pontiac'),
('POR', 'Porsche'),
('RAM', 'RAM'),
('REN', 'Renault'),
('RR', 'Rolls-Royce'),
('SAAB', 'Saab'),
('SATURN', 'Saturn'),
('SCION', 'Scion'),
('SMART', 'smart'),
('SRT', 'SRT'),
('STERL', 'Sterling'),
('SUB', 'Subaru'),
('SUZUKI', 'Suzuki'),
('TESLA', 'Tesla'),
('TOYOTA', 'Toyota'),
('TRI', 'Triumph'),
('VOLKS', 'Volkswagen'),
('VOLVO', 'Volvo'),
('YUGO', 'Yugo');

--	Inject any values that are missing from temp table.
INSERT INTO Make (Code, Name)
SELECT *
FROM @tempMake;
