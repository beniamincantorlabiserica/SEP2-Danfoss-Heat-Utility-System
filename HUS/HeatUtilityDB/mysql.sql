--CREATE TABLE ProductionUnits(
--    produnit VARCHAR(20) NOT NULL 
--);

CREATE TABLE BusinessResultsArchive(
    id INT NOT NULL PRIMARY KEY,
    electicity_price INT NOT NULL, 
    time_period INT NOT NULL,
    total_cost FLOAT NOT NULL,
);

CREATE TABLE ProductionUnitAccountabilityArchive(
    id INT NOT NULL PRIMARY KEY, 
    demand INT NOT NULL, 
    electicity_price INT NOT NULL,
    hour_end TIME NOT NULL,
    hour_start TIME NOT NULL,
    time_period INT NOT NULL,
    produnit SET NOT NULL,
);