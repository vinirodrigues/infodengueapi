-- Dump do Banco de Dados InfoDengue


CREATE DATABASE InfoDengueDB;
GO

USE InfoDengueDB;
GO


CREATE TABLE Solicitantes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    CPF NVARCHAR(11) NOT NULL UNIQUE
);


CREATE TABLE Relatorios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DataSolicitacao DATETIME NOT NULL,
    Arbovirose NVARCHAR(50) NOT NULL,
    SemanaInicio INT NOT NULL,
    SemanaTermino INT NOT NULL,
    CodigoIBGE INT NOT NULL,
    Municipio NVARCHAR(100) NOT NULL,
    SolicitanteId INT NOT NULL,
    FOREIGN KEY (SolicitanteId) REFERENCES Solicitantes(Id)
);

-- Inserir dados iniciais
INSERT INTO Solicitantes (Nome, CPF) VALUES ('João Silva', '12345678901');
INSERT INTO Solicitantes (Nome, CPF) VALUES ('Maria Oliveira', '98765432100');

INSERT INTO Relatorios (DataSolicitacao, Arbovirose, SemanaInicio, SemanaTermino, CodigoIBGE, Municipio, SolicitanteId) 
VALUES ('2024-12-01', 'Dengue', 45, 48, 3304557, 'Rio de Janeiro', 1);

INSERT INTO Relatorios (DataSolicitacao, Arbovirose, SemanaInicio, SemanaTermino, CodigoIBGE, Municipio, SolicitanteId) 
VALUES ('2024-12-02', 'Chikungunya', 45, 48, 3550308, 'São Paulo', 2);
