USE [fmsD4PL]
GO
IF  COL_LENGTH('dbo.TRITPurchaseOrder', 'PREALERT') IS NULL
BEGIN
	ALTER TABLE dbo.TRITPurchaseOrder
	ADD [PREALERT] bit
END