-- ==========================================
-- Create View template for Windows Azure SQL Database
-- ==========================================
IF object_id(N'[dbo].[vwVideoKeywords]', 'V') IS NOT NULL
	DROP VIEW [dbo].[vwVideoKeywords]
GO

CREATE VIEW [dbo].[vwVideoKeywords] AS

SELECT v.Id, v.Name, v.Keywords, v.RawUrl
FROM [Video] v
