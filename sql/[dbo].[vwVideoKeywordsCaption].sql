-- ==========================================
-- Create View template for Windows Azure SQL Database
-- ==========================================
IF object_id(N'[dbo].[vwVideoKeywordsCaption]', 'V') IS NOT NULL
	DROP VIEW [dbo].[vwVideoKeywordsCaption]
GO

CREATE VIEW [dbo].[vwVideoKeywordsCaption] AS

--Search 2: Based on input and caption from API
SELECT v.Id, v.Name, v.Keywords, v.RawUrl, c.Text, c.thumbnailIndex, va.thumbnailUrl FROM [Video] v
JOIN [Caption] c
ON v.Id = c.videoId
JOIN [VisionAnalysis] va
ON v.Id = va.videoId AND c.thumbnailIndex = va.thumbnailIndex

