-- ==========================================
-- Create View template for Windows Azure SQL Database
-- ==========================================
IF object_id(N'[dbo].[vwVideoKeywordsCaptionTags]', 'V') IS NOT NULL
	DROP VIEW [dbo].[vwVideoKeywordsCaptionTags]
GO

CREATE VIEW [dbo].[vwVideoKeywordsCaptionTags] AS

--Search 3: Based on input, caption and tags from API
SELECT newid() as 'Id', v.Name, v.Keywords, v.RawUrl, d.tags, va.thumbnailUrl, c.Text 
FROM [Description] d 
JOIN [Video] v
ON v.Id = d.videoId
JOIN [VisionAnalysis] va
ON v.Id = va.videoId AND d.thumbnailIndex = va.thumbnailIndex 
JOIN [Caption] c
ON v.Id = c.videoId AND c.thumbnailIndex = va.thumbnailIndex
